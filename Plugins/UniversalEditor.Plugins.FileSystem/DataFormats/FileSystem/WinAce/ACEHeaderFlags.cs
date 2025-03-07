//
//  ACEHeaderFlags.cs - indicates attributes for a WinACE archive
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

namespace UniversalEditor.DataFormats.FileSystem.WinAce
{
	/// <summary>
	/// Indicates attributes for a WinACE archive.
	/// </summary>
	[Flags()]
	public enum ACEHeaderFlags : ushort
	{
		AddSize = 1,
		HasComment = 2,
		SPBefore = 4096,
		SPAfter = 8192,
		HasPassword = 16384,
		ACE_LIM256 = 1024,
		MultiVolume = 2048,
		HasAuthenticityVerification = 4096,
		ACE_RECOV = 8192,
		ACE_LOCK = 16384,
		Solid = 32768
	}
}
