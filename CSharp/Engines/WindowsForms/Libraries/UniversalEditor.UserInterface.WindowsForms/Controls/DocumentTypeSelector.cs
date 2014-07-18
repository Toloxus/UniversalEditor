﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UniversalEditor.UserInterface.WindowsForms.Controls
{
	[DefaultEvent("SelectionChanged")]
    public partial class DocumentTypeSelector : UserControl
    {
        public event EventHandler SelectionFinalized;
		public event EventHandler SelectionChanged;

        public DocumentTypeSelector()
        {
            InitializeComponent();

            IconMethods.PopulateSystemIcons(ref imlLargeIcons);
            IconMethods.PopulateSystemIcons(ref imlSmallIcons);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            RefreshList();
        }

        private bool mvarIncludeObjectModelsWithoutEditor = false;
        /// <summary>
        /// Determines whether to include object models in the list if the object models do not have an associated editor.
        /// </summary>
        [Description("Determines whether to include object models in the list if the object models do not have an associated editor."), DefaultValue(false)]
        public bool IncludeObjectModelsWithoutEditor { get { return mvarIncludeObjectModelsWithoutEditor; } set { mvarIncludeObjectModelsWithoutEditor = value; } }

        private void RefreshList()
        {
            switch (mvarObjectType)
            {
                case DocumentTypeSelectorObjectTypes.DataFormat:
                {
                    foreach (DataFormatReference dfr in UniversalEditor.Common.Reflection.GetAvailableDataFormats())
                    {
                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = dfr.Title;
                        lvi.Tag = dfr;
                        lvDataFormats.Items.Add(lvi);
                    }
                    break;
                }
                case DocumentTypeSelectorObjectTypes.ObjectModel:
                {
                    tvObjectModels.Nodes.Clear();

                    ObjectModelReference[] omrs = UniversalEditor.Common.Reflection.GetAvailableObjectModels();
                    foreach (ObjectModelReference omr in omrs)
                    {
                        if (!mvarIncludeObjectModelsWithoutEditor && Common.Reflection.GetAvailableEditors(omr).Length == 0) continue;

                        if (omr.Path.Length > 0)
                        {
                            if (txtSearch.Text.Length != 0 && !omr.Path[omr.Path.Length - 1].ToLower().Contains(txtSearch.Text.ToLower())) continue;
                        }

                        TreeNode tnParent = null;
                        foreach (string pathItem in omr.Path)
                        {
                            if (tnParent == null)
                            {
                                if (tvObjectModels.Nodes.ContainsKey(pathItem))
                                {
                                    tnParent = tvObjectModels.Nodes[pathItem];
                                }
                                else
                                {
                                    tnParent = tvObjectModels.Nodes.Add(pathItem, pathItem);
                                }
                            }
                            else
                            {
                                if (tnParent.Nodes.ContainsKey(pathItem))
                                {
                                    tnParent = tnParent.Nodes[pathItem];
                                }
                                else
                                {
                                    tnParent = tnParent.Nodes.Add(pathItem, pathItem);
                                }
                            }

                            if (Array.IndexOf(omr.Path, pathItem) < omr.Path.Length - 1)
                            {
                                tnParent.ImageKey = "generic-folder-closed";
                                tnParent.SelectedImageKey = "generic-folder-closed";
                            }
                        }

                        if (tnParent != null)
                        {
                            if (txtSearch.Text.Length != 0 && omr.Path[omr.Path.Length - 1].ToLower().Contains(txtSearch.Text.ToLower()))
                            {
                                tnParent.EnsureVisible();
                                tvObjectModels.SelectedNode = tnParent;
                                tvObjectModels_AfterSelect(null, new TreeViewEventArgs(tvObjectModels.SelectedNode));
                            }
                            tnParent.Tag = omr;
                        }
                    }
                    break;
                }
                case DocumentTypeSelectorObjectTypes.Both:
                {
                    tvObjectModels.Nodes.Clear();

                    ObjectModelReference[] omrs = UniversalEditor.Common.Reflection.GetAvailableObjectModels();
                    foreach (ObjectModelReference omr in omrs)
                    {
                        if (!mvarIncludeObjectModelsWithoutEditor && Common.Reflection.GetAvailableEditors(omr).Length == 0) continue;

                        TreeNode tnParent = null;
                        foreach (string pathItem in omr.Path)
                        {
                            if (tnParent == null)
                            {
                                if (tvObjectModels.Nodes.ContainsKey(pathItem))
                                {
                                    tnParent = tvObjectModels.Nodes[pathItem];
                                }
                                else
                                {
                                    tnParent = tvObjectModels.Nodes.Add(pathItem, pathItem);
                                }
                            }
                            else
                            {
                                if (tnParent.Nodes.ContainsKey(pathItem))
                                {
                                    tnParent = tnParent.Nodes[pathItem];
                                }
                                else
                                {
                                    tnParent = tnParent.Nodes.Add(pathItem, pathItem);
                                }
                            }
                        }

                        tnParent.Tag = omr;
                        DataFormatReference[] dfrs = UniversalEditor.Common.Reflection.GetAvailableDataFormats(omr);
                        foreach (DataFormatReference dfr in dfrs)
                        {
                            TreeNode tnDataFormat = new TreeNode();
                            tnDataFormat.Text = dfr.Title;
                            tnDataFormat.Tag = dfr;
                            tnParent.Nodes.Add(tnDataFormat);
                        }
                    }
                    break;
                }
            }
        }

        private DocumentTypeSelectorObjectTypes mvarObjectType = DocumentTypeSelectorObjectTypes.ObjectModel;
        public DocumentTypeSelectorObjectTypes ObjectType
        {
            get { return mvarObjectType; }
            set
            {
                mvarObjectType = value;
                switch (mvarObjectType)
                {
					case DocumentTypeSelectorObjectTypes.DataFormat:
					{
						tvObjectModels.Visible = false;
						tvObjectModels.Enabled = false;
						lvDataFormats.Visible = true;
						lvDataFormats.Enabled = true;
						break;
					}
					case DocumentTypeSelectorObjectTypes.ObjectModel:
					{
						tvObjectModels.Visible = true;
						tvObjectModels.Enabled = true;
						lvDataFormats.Visible = false;
						lvDataFormats.Enabled = false;
						break;
					}
					case DocumentTypeSelectorObjectTypes.Both:
					{
						tvObjectModels.Visible = true;
						tvObjectModels.Enabled = true;
						lvDataFormats.Visible = false;
						lvDataFormats.Enabled = false;
						break;
					}
                }
            }
        }

		protected void OnSelectionChanged(EventArgs e)
		{
			if (lvDataFormats.Visible && lvDataFormats.SelectedItems.Count == 1)
			{
				mvarSelectedObject = (lvDataFormats.SelectedItems[0].Tag as DataFormatReference);
			}
			else if (tvObjectModels.Visible && tvObjectModels.SelectedNode != null)
			{
				if (tvObjectModels.SelectedNode.Tag is DataFormatReference)
				{
					mvarSelectedObject = (tvObjectModels.SelectedNode.Tag as DataFormatReference);
				}
				else if (tvObjectModels.SelectedNode.Tag is ObjectModelReference)
				{
					mvarSelectedObject = (tvObjectModels.SelectedNode.Tag as ObjectModelReference);
				}
				else
				{
					mvarSelectedObject = null;
				}
			}

			if (SelectionChanged != null) SelectionChanged(this, e);

		}

        protected void OnSelectionFinalized(EventArgs e)
        {
            OnSelectionChanged(e);
            if (SelectionFinalized != null) SelectionFinalized(this, e);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            RefreshList();
		}

		private void tvObjectModels_AfterSelect(object sender, TreeViewEventArgs e)
		{
			OnSelectionChanged(EventArgs.Empty);
		}

		private void lvDataFormats_SelectedIndexChanged(object sender, EventArgs e)
		{
			OnSelectionChanged(EventArgs.Empty);
		}

		private object mvarSelectedObject = null;
		/// <summary>
		/// The <see cref="UniversalEditor.DataFormat" /> or <see cref="UniversalEditor.ObjectModel" /> that is currently selected.
		/// </summary>
		public object SelectedObject { get { return mvarSelectedObject; } }


        private void tvObjectModels_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null) return;
            if (e.Node.ImageKey == "generic-folder-open")
            {
                e.Node.ImageKey = "generic-folder-closed";
                e.Node.SelectedImageKey = "generic-folder-closed";
            }
        }
        private void tvObjectModels_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null) return;
            if (e.Node.ImageKey == "generic-folder-closed")
            {
                e.Node.ImageKey = "generic-folder-open";
                e.Node.SelectedImageKey = "generic-folder-open";
            }
        }

        private void tvObjectModels_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (tvObjectModels.SelectedNode != null)
            {
                OnSelectionFinalized(e);
            }
        }
	}
    
    [Flags()]
    public enum DocumentTypeSelectorObjectTypes
    {
        None = 0,
        ObjectModel = 1,
        DataFormat = 2,
        Both = 3
    }
}