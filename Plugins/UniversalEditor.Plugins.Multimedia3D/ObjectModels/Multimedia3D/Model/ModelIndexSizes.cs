//
//  ModelIndexSizes.cs - indicates the index sizes for a 3D model
//
//  Author:
//       Michael Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2013-2020 Mike Becker's Software
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

namespace UniversalEditor.ObjectModels.Multimedia3D.Model
{
	/// <summary>
	/// Indicates the index sizes for a 3D model.
	/// </summary>
	public class ModelIndexSizes
	{
		public byte Vertex { get; set; } = 0;
		public byte Texture { get; set; } = 0;
		public byte Material { get; set; } = 0;
		public byte Bone { get; set; } = 0;
		public byte Morph { get; set; } = 0;
		public byte RigidBody { get; set; } = 0;

		public void Clear()
		{
			Vertex = 0;
			Texture = 0;
			Material = 0;
			Bone = 0;
			Morph = 0;
			RigidBody = 0;
		}
	}
}
