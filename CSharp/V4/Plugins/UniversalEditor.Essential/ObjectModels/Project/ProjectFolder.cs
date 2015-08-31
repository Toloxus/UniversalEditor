﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniversalEditor.ObjectModels.Project
{
	public class ProjectFolder : ICloneable
	{
		public class ProjectFolderCollection
			: System.Collections.ObjectModel.Collection<ProjectFolder>
		{
			private ProjectFolder _parent = null;
			public ProjectFolderCollection(ProjectFolder parent = null)
			{
				_parent = parent;
			}

			public ProjectFolder Add(string Name)
			{
				ProjectFolder folder = new ProjectFolder();
				folder.Name = Name;
				Add(folder);
				return folder;
			}

			protected override void InsertItem(int index, ProjectFolder item)
			{
				base.InsertItem(index, item);
				item.Parent = _parent;
			}
			protected override void RemoveItem(int index)
			{
				this[index].Parent = null;
				base.RemoveItem(index);
			}
			protected override void ClearItems()
			{
				foreach (ProjectFolder folder in this)
				{
					folder.Parent = null;
				}
				base.ClearItems();
			}
		}

		public ProjectFolder()
		{
			mvarFolders = new ProjectFolder.ProjectFolderCollection(this);
			mvarFiles = new ProjectFile.ProjectFileCollection(this);
		}

		private string mvarName = String.Empty;
		public string Name { get { return mvarName; } set { mvarName = value; } }

		private ProjectFolder mvarParent = null;
		public ProjectFolder Parent { get { return mvarParent; } private set { mvarParent = value; } }

		private ProjectFolder.ProjectFolderCollection mvarFolders = null;
		public ProjectFolder.ProjectFolderCollection Folders { get { return mvarFolders; } }

		private ProjectFile.ProjectFileCollection mvarFiles = null;
		public ProjectFile.ProjectFileCollection Files { get { return mvarFiles; } }

		public object Clone()
		{
			ProjectFolder clone = new ProjectFolder();
			foreach (ProjectFile file in mvarFiles)
			{
				clone.Files.Add(file.Clone() as ProjectFile);
			}
			foreach (ProjectFolder folder in mvarFolders)
			{
				clone.Folders.Add(folder.Clone() as ProjectFolder);
			}
			clone.Name = (mvarName.Clone() as string);
			return clone;
		}
	}
}