﻿//
//  FileSystemEditor.Designer.cs - UWT Designer portions of FIleSystemEditor.cs
//
//  Author:
//       Michael Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2019 
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
using UniversalWidgetToolkit;
using UniversalWidgetToolkit.Controls;
using UniversalWidgetToolkit.Layouts;

namespace UniversalEditor.Editors.FileSystem
{
	partial class FileSystemEditor
	{
		private ListView tv = null;
		private DefaultTreeModel tmTreeView = null;

		private void InitializeComponent()
		{
			this.Layout = new BoxLayout(Orientation.Vertical);

			this.tmTreeView = new DefaultTreeModel(new Type[] { typeof(string), typeof(string), typeof(string), typeof(string) });

			this.tv = new ListView();
			this.tv.Mode = ListViewMode.Detail;
			this.tv.SelectionMode = SelectionMode.Multiple;
			this.tv.Model = this.tmTreeView;

			this.tv.Columns.Add(new ListViewColumnText(tmTreeView.Columns[0], "Name"));
			this.tv.Columns.Add(new ListViewColumnText(tmTreeView.Columns[1], "Size"));
			this.tv.Columns.Add(new ListViewColumnText(tmTreeView.Columns[2], "Type"));
			this.tv.Columns.Add(new ListViewColumnText(tmTreeView.Columns[3], "Date modified"));

			this.Controls.Add(this.tv, new BoxLayout.Constraints(true, true));
		}
	}
}