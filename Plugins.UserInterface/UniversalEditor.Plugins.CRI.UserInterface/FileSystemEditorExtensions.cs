//
//  FileSystemEditorExtensions.cs - provide extensions to the FileSystemEditor for CRI CPK archives
//
//  Author:
//       Mike Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2019-2020 Mike Becker's Software
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
using MBS.Framework;
using MBS.Framework.UserInterface;
using UniversalEditor.ObjectModels.FileSystem;
using UniversalEditor.Plugins.CRI.DataFormats.FileSystem.CPK;
using UniversalEditor.UserInterface;

namespace UniversalEditor.Plugins.CRI.UserInterface
{
	/// <summary>
	/// Provide extensions to the <see cref="Editors.FileSystem.FileSystemEditor" /> for CRI CPK archives.
	/// </summary>
	public class FileSystemEditorExtensions : EditorPlugin
	{
		public FileSystemEditorExtensions()
		{
			SupportedEditors.Add(typeof(Editors.FileSystem.FileSystemEditor));
		}

		protected override void InitializeInternal()
		{
			Context = new UIContext(new Guid("{7A5A7675-529E-46B1-A4FF-0786956DAE47}"), "CRI Extensions for FileSystemEditor");
		}

		private bool _initOnce = false;
		protected override void OnEditorCreated(EventArgs e)
		{
			base.OnEditorCreated(e);

			if (_initOnce)
				return;

			EditorReference er = Editor.MakeReference();

			if (Document == null)
				return;
			if (!(Document.DataFormat is CPKDataFormat))
				return;

			er.Commands["FileSystemContextMenu_Unselected"].Items.Add(new SeparatorCommandItem());
			er.Commands["FileSystemContextMenu_Unselected"].Items.Add(new CommandReferenceCommandItem("CRI_FileSystem_Extensions"));

			if (Context != null)
			{
				Context.AttachCommandEventHandler("CRI_FileSystem_Extensions_Export_Header", CRI_FileSystem_Extensions_Export_Header_Click);
				Context.AttachCommandEventHandler("CRI_FileSystem_Extensions_Export_TOC", CRI_FileSystem_Extensions_Export_TOC_Click);
				Context.AttachCommandEventHandler("CRI_FileSystem_Extensions_Export_ITOC", CRI_FileSystem_Extensions_Export_ITOC_Click);
				Context.AttachCommandEventHandler("CRI_FileSystem_Extensions_Export_GTOC", CRI_FileSystem_Extensions_Export_GTOC_Click);
				Context.AttachCommandEventHandler("CRI_FileSystem_Extensions_Export_ETOC", CRI_FileSystem_Extensions_Export_ETOC_Click);

				Context.AttachCommandEventHandler("CRI_FileSystem_Extensions_View_Info", CRI_FileSystem_Extensions_View_Info_Click);
				Context.AttachCommandEventHandler("CRI_FileSystem_Extensions_Export_CSV", CRI_FileSystem_Extensions_Export_CSV_Click);
				Context.AttachCommandEventHandler("CRI_FileSystem_Extensions_Export_All", CRI_FileSystem_Extensions_Export_All_Click);
			}
			else
			{
				((UIApplication)Application.Instance).AttachCommandEventHandler("CRI_FileSystem_Extensions_Export_Header", CRI_FileSystem_Extensions_Export_Header_Click);
				((UIApplication)Application.Instance).AttachCommandEventHandler("CRI_FileSystem_Extensions_Export_TOC", CRI_FileSystem_Extensions_Export_TOC_Click);
				((UIApplication)Application.Instance).AttachCommandEventHandler("CRI_FileSystem_Extensions_Export_ITOC", CRI_FileSystem_Extensions_Export_ITOC_Click);
				((UIApplication)Application.Instance).AttachCommandEventHandler("CRI_FileSystem_Extensions_Export_GTOC", CRI_FileSystem_Extensions_Export_GTOC_Click);
				((UIApplication)Application.Instance).AttachCommandEventHandler("CRI_FileSystem_Extensions_Export_ETOC", CRI_FileSystem_Extensions_Export_ETOC_Click);

				((UIApplication)Application.Instance).AttachCommandEventHandler("CRI_FileSystem_Extensions_View_Info", CRI_FileSystem_Extensions_View_Info_Click);
				((UIApplication)Application.Instance).AttachCommandEventHandler("CRI_FileSystem_Extensions_Export_CSV", CRI_FileSystem_Extensions_Export_CSV_Click);
				((UIApplication)Application.Instance).AttachCommandEventHandler("CRI_FileSystem_Extensions_Export_All", CRI_FileSystem_Extensions_Export_All_Click);
			}

			_initOnce = true;
		}

		private void CRI_FileSystem_Extensions_Export_All_Click(object sender, EventArgs e)
		{
			CPKDataFormat cpk = (Document.DataFormat as CPKDataFormat);
			if (cpk == null)
				return;

			MBS.Framework.UserInterface.Dialogs.FileDialog dlg = new MBS.Framework.UserInterface.Dialogs.FileDialog();
			dlg.Text = "Export all UTF tables";
			dlg.Mode = MBS.Framework.UserInterface.Dialogs.FileDialogMode.SelectFolder;
			dlg.FileNameFilters.Add("CRI Middleware UTF table", "*.utf");
			if (dlg.ShowDialog() == MBS.Framework.UserInterface.DialogResult.OK)
			{
				if (cpk.HeaderData != null)
				{
					System.IO.File.WriteAllBytes(System.IO.Path.Combine(new string[] { dlg.SelectedFileName, cpk.HeaderTable.Name + ".utf" }), cpk.HeaderData);
				}
				if (cpk.TocData != null)
				{
					System.IO.File.WriteAllBytes(System.IO.Path.Combine(new string[] { dlg.SelectedFileName, "CpkTocInfo.utf" }), cpk.TocData);
				}
				if (cpk.ETocData != null)
				{
					System.IO.File.WriteAllBytes(System.IO.Path.Combine(new string[] { dlg.SelectedFileName, "CpkETocInfo.utf" }), cpk.ETocData);
				}
				if (cpk.GTocData != null)
				{
					System.IO.File.WriteAllBytes(System.IO.Path.Combine(new string[] { dlg.SelectedFileName, "CpkGTocInfo.utf" }), cpk.GTocData);
				}
				if (cpk.ITocData != null)
				{
					System.IO.File.WriteAllBytes(System.IO.Path.Combine(new string[] { dlg.SelectedFileName, "CpkExtendId.utf" }), cpk.ITocData);
				}
			}
		}

		private void CRI_FileSystem_Extensions_Export_CSV_Click(object sender, EventArgs e)
		{
			CPKDataFormat cpk = (Document.DataFormat as CPKDataFormat);
			if (cpk == null)
				return;

			FileSystemObjectModel fsom = (Document.ObjectModel as FileSystemObjectModel);
			if (fsom == null)
				return;

			File[] files = fsom.GetAllFiles();

			MBS.Framework.UserInterface.Dialogs.FileDialog dlg = new MBS.Framework.UserInterface.Dialogs.FileDialog();
			dlg.Mode = MBS.Framework.UserInterface.Dialogs.FileDialogMode.Save;
			dlg.FileNameFilters.Add("Comma-separated values", "*.csv");
			dlg.Text = "Export CSV";
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				for (int i = 0; i < files.Length; i++)
				{
					// /pv_01_easy.dsc,0,32272,32272,100.00,,,2010/04/12 10:56:10,20480,/pv_01_easy.dsc
					sb.AppendLine(String.Format("/{0},{1},{2},{3},{4},,,{5},{6},/{0}", files[i].Name, files[i].GetAdditionalDetail("CRI.CPK.FileID"), files[i].Size, files[i].Size, "100.00", files[i].ModificationTimestamp.ToString(), 20480));
				}
				System.IO.File.WriteAllText(dlg.SelectedFileName, sb.ToString());
			}
		}

		private void CRI_FileSystem_Extensions_View_Info_Click(object sender, EventArgs e)
		{
			CPKDataFormat cpk = (Document.DataFormat as CPKDataFormat);
			if (cpk == null)
				return;

			CPKFileInfoDialog dlg = new CPKFileInfoDialog();
			dlg.Accessor = Document.Accessor;
			dlg.DataFormat = cpk;
			dlg.ShowDialog();
		}

		private void CRI_FileSystem_Extensions_Export_Header_Click(object sender, EventArgs e)
		{
			CPKDataFormat cpk = (Document.DataFormat as CPKDataFormat);
			if (cpk == null)
				return;

			if (cpk.HeaderData == null)
			{
				MBS.Framework.UserInterface.Dialogs.MessageDialog.ShowDialog("This archive does not contain a header (CPK ) table.", "Error", MBS.Framework.UserInterface.Dialogs.MessageDialogButtons.OK, MBS.Framework.UserInterface.Dialogs.MessageDialogIcon.Error);
				return;
			}

			MBS.Framework.UserInterface.Dialogs.FileDialog dlg = new MBS.Framework.UserInterface.Dialogs.FileDialog();
			dlg.Text = "Export header UTF";
			dlg.SelectedFileNames.Add("CpkHeader.utf");
			dlg.Mode = MBS.Framework.UserInterface.Dialogs.FileDialogMode.Save;
			dlg.FileNameFilters.Add("CRI Middleware UTF table", "*.utf");
			if (dlg.ShowDialog() == MBS.Framework.UserInterface.DialogResult.OK)
			{
				System.IO.File.WriteAllBytes(dlg.SelectedFileNames[dlg.SelectedFileNames.Count - 1], cpk.HeaderData);
			}
		}
		private void CRI_FileSystem_Extensions_Export_TOC_Click(object sender, EventArgs e)
		{
			CPKDataFormat cpk = (Document.DataFormat as CPKDataFormat);
			if (cpk == null)
				return;

			if (cpk.TocData == null)
			{
				MBS.Framework.UserInterface.Dialogs.MessageDialog.ShowDialog("This archive does not contain a file list TOC (TOC ) table.", "Error", MBS.Framework.UserInterface.Dialogs.MessageDialogButtons.OK, MBS.Framework.UserInterface.Dialogs.MessageDialogIcon.Error);
				return;
			}

			MBS.Framework.UserInterface.Dialogs.FileDialog dlg = new MBS.Framework.UserInterface.Dialogs.FileDialog();
			dlg.Text = "Export file list (TOC) UTF";
			dlg.SelectedFileNames.Add("CpkTocInfo.utf");
			dlg.Mode = MBS.Framework.UserInterface.Dialogs.FileDialogMode.Save;
			dlg.FileNameFilters.Add("CRI Middleware UTF table", "*.utf");
			if (dlg.ShowDialog() == MBS.Framework.UserInterface.DialogResult.OK)
			{
				System.IO.File.WriteAllBytes(dlg.SelectedFileNames[dlg.SelectedFileNames.Count - 1], cpk.TocData);
			}
		}
		private void CRI_FileSystem_Extensions_Export_ITOC_Click(object sender, EventArgs e)
		{
			CPKDataFormat cpk = (Document.DataFormat as CPKDataFormat);
			if (cpk == null)
				return;

			if (cpk.ITocData == null)
			{
				MBS.Framework.UserInterface.Dialogs.MessageDialog.ShowDialog("This archive does not contain an indexes TOC (ITOC) table.", "Error", MBS.Framework.UserInterface.Dialogs.MessageDialogButtons.OK, MBS.Framework.UserInterface.Dialogs.MessageDialogIcon.Error);
				return;
			}

			MBS.Framework.UserInterface.Dialogs.FileDialog dlg = new MBS.Framework.UserInterface.Dialogs.FileDialog();
			dlg.Text = "Export indexes (ITOC) UTF";
			dlg.SelectedFileNames.Add("CpkExtendId.utf");
			dlg.Mode = MBS.Framework.UserInterface.Dialogs.FileDialogMode.Save;
			dlg.FileNameFilters.Add("CRI Middleware UTF table", "*.utf");
			if (dlg.ShowDialog() == MBS.Framework.UserInterface.DialogResult.OK)
			{
				System.IO.File.WriteAllBytes(dlg.SelectedFileNames[dlg.SelectedFileNames.Count - 1], cpk.ITocData);
			}
		}
		private void CRI_FileSystem_Extensions_Export_GTOC_Click(object sender, EventArgs e)
		{
			CPKDataFormat cpk = (Document.DataFormat as CPKDataFormat);
			if (cpk == null)
				return;

			if (cpk.GTocData == null)
			{
				MBS.Framework.UserInterface.Dialogs.MessageDialog.ShowDialog("This archive does not contain a groups TOC (GTOC) table.", "Error", MBS.Framework.UserInterface.Dialogs.MessageDialogButtons.OK, MBS.Framework.UserInterface.Dialogs.MessageDialogIcon.Error);
				return;
			}

			MBS.Framework.UserInterface.Dialogs.FileDialog dlg = new MBS.Framework.UserInterface.Dialogs.FileDialog();
			dlg.Text = "Export groups (GTOC) UTF";
			dlg.Mode = MBS.Framework.UserInterface.Dialogs.FileDialogMode.Save;
			dlg.FileNameFilters.Add("CRI Middleware UTF table", "*.utf");
			if (dlg.ShowDialog() == MBS.Framework.UserInterface.DialogResult.OK)
			{
				System.IO.File.WriteAllBytes(dlg.SelectedFileNames[dlg.SelectedFileNames.Count - 1], cpk.GTocData);
			}
		}
		private void CRI_FileSystem_Extensions_Export_ETOC_Click(object sender, EventArgs e)
		{
			CPKDataFormat cpk = (Document.DataFormat as CPKDataFormat);
			if (cpk == null)
				return;

			if (cpk.ETocData == null)
			{
				MBS.Framework.UserInterface.Dialogs.MessageDialog.ShowDialog("This archive does not contain an end-of-file TOC (ETOC) table.", "Error", MBS.Framework.UserInterface.Dialogs.MessageDialogButtons.OK, MBS.Framework.UserInterface.Dialogs.MessageDialogIcon.Error);
				return;
			}

			MBS.Framework.UserInterface.Dialogs.FileDialog dlg = new MBS.Framework.UserInterface.Dialogs.FileDialog();
			dlg.Text = "Export end-of-file (ETOC) UTF";
			dlg.SelectedFileNames.Add("CpkEtocInfo.utf");
			dlg.Mode = MBS.Framework.UserInterface.Dialogs.FileDialogMode.Save;
			dlg.FileNameFilters.Add("CRI Middleware UTF table", "*.utf");
			if (dlg.ShowDialog() == MBS.Framework.UserInterface.DialogResult.OK)
			{
				System.IO.File.WriteAllBytes(dlg.SelectedFileNames[dlg.SelectedFileNames.Count - 1], cpk.ETocData);
			}
		}
	}
}
