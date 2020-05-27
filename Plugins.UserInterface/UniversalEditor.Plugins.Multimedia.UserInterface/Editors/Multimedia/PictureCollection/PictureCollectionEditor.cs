﻿//
//  PictureCollectionEditor.cs - provides a UWT-based Editor for PictureCollectionObjectModel
//
//  Author:
//       Mike Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2019-2020 Mike Becker
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using MBS.Framework.UserInterface;
using MBS.Framework.UserInterface.Controls;
using MBS.Framework.UserInterface.Dialogs;
using MBS.Framework.UserInterface.Layouts;
using UniversalEditor.Accessors;
using UniversalEditor.DataFormats.Multimedia.Picture.Microsoft.Bitmap;
using UniversalEditor.ObjectModels.Multimedia.Picture;
using UniversalEditor.ObjectModels.Multimedia.Picture.Collection;
using UniversalEditor.UserInterface;

namespace UniversalEditor.Plugins.Multimedia.UserInterface.Editors.Multimedia.PictureCollection
{
	/// <summary>
	/// Provides a UWT-based Editor for <see cref="PictureCollectionObjectModel"/>.
	/// </summary>
	[ContainerLayout("~/Editors/Multimedia/PictureCollection/PictureCollectionEditor.glade")]
	public class PictureCollectionEditor : Editor
	{
		private Button cmdSave;
		private Button cmdSaveAll;
		private NumericTextBox txtFrameIndex;

		private static EditorReference _er = null;
		public override EditorReference MakeReference()
		{
			if (_er == null)
			{
				_er = base.MakeReference();
				_er.SupportedObjectModels.Add(typeof(PictureCollectionObjectModel));
			}
			return _er;
		}

		[EventHandler(nameof(txtFrameIndex), "Changed")]
		private void txtFrameIndex_Changed(object sender, EventArgs e)
		{
			PictureCollectionObjectModel coll = (ObjectModel as PictureCollectionObjectModel);
			if (coll == null) return;

			cmdSave.Enabled = (SelectedFrameIndex >= 0 && SelectedFrameIndex < coll.Pictures.Count);

			if (txtFrameIndex.Value >= 0 && txtFrameIndex.Value < coll.Pictures.Count)
				picFrame.Image = coll.Pictures[(int)txtFrameIndex.Value].ToImage();
		}

		protected override void OnCreated(EventArgs e)
		{
			base.OnCreated(e);

			OnObjectModelChanged(e);
		}

		protected override void OnObjectModelChanged(EventArgs e)
		{
			base.OnObjectModelChanged(e);

			if (!IsCreated) return;

			cmdSave.Enabled = false;
			cmdSaveAll.Enabled = false;

			DocumentExplorer.Nodes.Clear();

			PictureCollectionObjectModel coll = (ObjectModel as PictureCollectionObjectModel);
			if (coll == null) return;

			EditorDocumentExplorerNode nodeFrames = DocumentExplorer.Nodes.Add("Frames");
			for (int i = 0; i < coll.Pictures.Count; i++)
			{
				nodeFrames.Nodes.Add(String.Format("Frame {0}", (i + 1).ToString()));
				nodeFrames.Nodes[i].SetExtraData("index", i);
			}

			txtFrameIndex.Maximum = coll.Pictures.Count - 1;
			cmdSave.Enabled = coll.Pictures.Count > 0;
			cmdSaveAll.Enabled = coll.Pictures.Count > 0;

			if (coll.Pictures.Count > 0)
				picFrame.Image = coll.Pictures[0].ToImage();
		}

		protected override void OnDocumentExplorerSelectionChanged(EditorDocumentExplorerSelectionChangedEventArgs e)
		{
			base.OnDocumentExplorerSelectionChanged(e);

			if (e.Node == null) return;

			int index = e.Node.GetExtraData<int>("index");
			SelectedFrameIndex = index;
		}

		public int SelectedFrameIndex
		{
			get { return (int)txtFrameIndex.Value; }
			set { txtFrameIndex.Value = value; Refresh(); }
		}

		private PictureFrame picFrame = null;

		[EventHandler(nameof(cmdSave), "Click")]
		void CmdSave_Click(object sender, EventArgs e)
		{
			PictureCollectionObjectModel coll = ObjectModel as PictureCollectionObjectModel;
			if (SelectedFrameIndex < 0 || SelectedFrameIndex >= coll.Pictures.Count)
				return;

			FileDialog dlg = new FileDialog();
			dlg.Mode = FileDialogMode.Save;
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				PictureObjectModel pic = coll.Pictures[SelectedFrameIndex];
				BitmapDataFormat bmp = new BitmapDataFormat();

				FileAccessor fa = new FileAccessor(dlg.SelectedFileNames[dlg.SelectedFileNames.Count - 1]);
				fa.AllowWrite = true;
				fa.ForceOverwrite = true;
				fa.Open();
				Document.Save(pic, bmp, fa, true);
			}
		}
		[EventHandler(nameof(cmdSaveAll), "Click")]
		void CmdSaveAll_Click(object sender, EventArgs e)
		{
			FileDialog dlg = new FileDialog();
			dlg.Mode = FileDialogMode.SelectFolder;
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				PictureCollectionObjectModel coll = ObjectModel as PictureCollectionObjectModel;

				string directoryName = dlg.SelectedFileNames[dlg.SelectedFileNames.Count - 1];
				if (!System.IO.Directory.Exists(directoryName))
				{
					System.IO.Directory.CreateDirectory(directoryName);
				}

				for (int i = 0; i < coll.Pictures.Count; i++)
				{
					PictureObjectModel pic = coll.Pictures[i];
					BitmapDataFormat bmp = new BitmapDataFormat();

					FileAccessor fa = new FileAccessor(directoryName + System.IO.Path.DirectorySeparatorChar.ToString() + i.ToString().PadLeft(8, '0') + ".bmp");
					fa.AllowWrite = true;
					fa.ForceOverwrite = true;
					fa.Open();
					Document.Save(pic, bmp, fa, true);
				}
			}
		}


		public override void UpdateSelections()
		{
		}

		protected override EditorSelection CreateSelectionInternal(object content)
		{
			return null;
		}


	}
}
