//
//  ModelMorph.cs - the abstract base class from which all 3D model morphs derive
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

using System;
using System.Collections.ObjectModel;

namespace UniversalEditor.ObjectModels.Multimedia3D.Model.Morphing
{
	/// <summary>
	/// The abstract base class from which all 3D model morphs derive.
	/// </summary>
	public abstract class ModelMorph : ICloneable
	{
		public class ModelMorphCollection : Collection<ModelMorph>
		{
		}

		/// <summary>
		/// Gets or sets the name of the morph.
		/// </summary>
		/// <value>The name of the morph.</value>
		public string Name { get; set; } = string.Empty;

		public abstract object Clone();
	}
}
