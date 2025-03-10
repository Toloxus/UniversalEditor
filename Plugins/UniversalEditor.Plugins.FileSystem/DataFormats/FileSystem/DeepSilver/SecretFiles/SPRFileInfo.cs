//
//  SPRFileInfo.cs - internal structure describing file information for an SPR archive
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

namespace UniversalEditor.DataFormats.FileSystem.DeepSilver.SecretFiles
{
	/// <summary>
	/// Internal structure describing file information for an SPR archive.
	/// </summary>
	internal struct SPRFileInfo
	{
		/// <summary>
		/// Index into directory table, 0x1effffff for root
		/// </summary>
		public int parentDirectoryIndex;

		public int fileNamePrefixIndex;
		public int fileNameSuffixIndex;
		public int crc32;

		public DateTime timestamp;

		public uint decompressedLength;
		/// <summary>
		/// Compression header offset (0xffffffff if file is not compressed)
		/// </summary>
		public int compressionHeaderOffset;
		/// <summary>
		/// Compression header size (zero if file is not compressed)
		/// </summary>
		public uint compressionHeaderLength;

		public uint offset;
		public uint compressedLength;
	}
}
