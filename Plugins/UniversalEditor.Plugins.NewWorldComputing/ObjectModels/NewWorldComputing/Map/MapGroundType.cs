﻿//
//  MapGroundType.cs - indicates the type of ground for a ground tile placed on a map
//
//  Author:
//       Michael Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2011-2020 Mike Becker's Software
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

namespace UniversalEditor.ObjectModels.NewWorldComputing.Map
{
	/// <summary>
	/// Indicates the type of ground for a ground tile placed on a map.
	/// </summary>
	[Flags()]
	public enum MapGroundType
	{
		Unknown = 0x0000,
		Desert = 0x0001,
		Snow = 0x0002,
		Swamp = 0x0004,
		Wasteland = 0x0008,
		Beach = 0x0010,
		Lava = 0x0020,
		Dirt = 0x0040,
		Grass = 0x0080,
		Water = 0x0100
	}
}
