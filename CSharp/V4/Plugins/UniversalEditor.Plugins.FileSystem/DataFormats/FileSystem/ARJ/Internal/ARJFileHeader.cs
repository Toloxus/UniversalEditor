﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniversalEditor.DataFormats.FileSystem.ARJ.Internal
{
	internal struct ARJFileHeader
	{
		/// <summary>
		/// Size of header including extra data.
		/// </summary>
		public byte HeaderSize;
		/// <summary>
		/// Archiver version number.
		/// </summary>
		public byte VersionNumber;
		/// <summary>
		/// Minimum version needed to extract.
		/// </summary>
		public byte MinimumRequiredVersion;
		/// <summary>
		/// Host operating system.
		/// </summary>
		public ARJHostOperatingSystem HostOperatingSystem;
		public ARJInternalFlags InternalFlags;
		public ARJCompressionMethod CompressionMethod;
		public ARJFileType FileType;
		public byte Reserved;
		public int Timestamp;
		public int CompressedSize;
		public int OriginalSize;
		public int OriginalCRC32;
		/// <summary>
		/// Filespec position in filename.
		/// </summary>
		public short FileSpecPosition;
		public short FileAttributes;
		public short HostData;
		public int Unknown1;
		public int Unknown2;
		public int Unknown3;
		public int Unknown4;
		public string FileName;
		public byte[] CompressedData;
	}
}