//
//  Animation.cs - represents an animation for a particular character model in a Concertroid Performance
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

namespace UniversalEditor.ObjectModels.Concertroid
{
	/// <summary>
	/// Represents an animation for a particular character model in a Concertroid <see cref="Concert.Performance" />.
	/// </summary>
	public class Animation
	{
		public class AnimationCollection
			: System.Collections.ObjectModel.Collection<Animation>
		{
		}
		/// <summary>
		/// Gets or sets the full path to the animation data file.
		/// </summary>
		/// <value>The full path to the animation data file.</value>
		public string FileName { get; set; } = String.Empty;
	}
}
