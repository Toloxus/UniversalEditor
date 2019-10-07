﻿//
//  PianoRollEditor.cs
//
//  Author:
//       Mike Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2019 Mike Becker
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
using UniversalEditor.UserInterface;

namespace UniversalEditor.Editors.Multimedia.Audio.Synthesized.PianoRoll
{
	public partial class PianoRollEditor
	{
		public override void UpdateSelections()
		{
			throw new NotImplementedException();
		}

		protected override EditorSelection CreateSelectionInternal(object content)
		{
			throw new NotImplementedException();
		}

		protected override void OnToolboxItemSelected(ToolboxItemEventArgs e)
		{
			base.OnToolboxItemSelected(e);

			switch (e.Item.Name)
			{
				case "ToolboxItem_Select":
				{
					this.PianoRoll.SelectionMode = PianoRollSelectionMode.Select;
					break;
				}
				case "ToolboxItem_Insert":
				{
					this.PianoRoll.SelectionMode = PianoRollSelectionMode.Insert;
					break;
				}
			}
		}
	}
}
