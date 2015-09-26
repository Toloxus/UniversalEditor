﻿using System;
using System.Collections.Generic;
using UniversalEditor.Accessors;
using UniversalEditor.ObjectModels.FileSystem;

namespace UniversalEditor.DataFormats.FileSystem.CHD
{
	public class CHDDataFormat : DataFormat
	{
		private static DataFormatReference _dfr = null;
		protected override DataFormatReference MakeReferenceInternal()
		{
			if (_dfr == null)
			{
				_dfr = base.MakeReferenceInternal();
				_dfr.Capabilities.Add(typeof(FileSystemObjectModel), DataFormatCapabilities.All);
			}
			return _dfr;
		}

		private uint mvarFormatVersion = 1;
		public uint FormatVersion { get { return mvarFormatVersion; } set { mvarFormatVersion = value; } }

		private CHDFlags mvarFlags = CHDFlags.None;
		public CHDFlags Flags { get { return mvarFlags; } set { mvarFlags = value; } }

		private CHDCompressionType mvarCompressionType = CHDCompressionType.None;
		public CHDCompressionType CompressionType { get { return mvarCompressionType; } set { mvarCompressionType = value; } }

		private uint mvarHunkSize = 0;
		/// <summary>
		/// Number of 512-byte sectors per hunk.
		/// </summary>
		public uint HunkSize { get { return mvarHunkSize; } set { mvarHunkSize = value; } }

		private long m_RawMapOffset = 0;

		/// <summary>
		/// The type of hunk.
		/// </summary>
		internal const byte	V34_MAP_ENTRY_FLAG_TYPE_MASK	=	0x0f;
		/// <summary>
		/// No CRC is present.
		/// </summary>
		internal const byte V34_MAP_ENTRY_FLAG_NO_CRC = 0x10;

		protected override void LoadInternal(ref ObjectModel objectModel)
		{
			FileSystemObjectModel fsom = (objectModel as FileSystemObjectModel);
			if (fsom == null) throw new ObjectModelNotSupportedException();

			IO.Reader br = base.Accessor.Reader;
			br.Accessor.Position = 0;
			br.Endianness = IO.Endianness.BigEndian;
			string signature = br.ReadFixedLengthString(8);
			if (signature != "MComprHD") throw new InvalidDataFormatException("File does not begin with \"MComprHD\"");

			uint headerLength = br.ReadUInt32();						// length of header (including tag and length fields)
			mvarFormatVersion = br.ReadUInt32();						// drive format version
			mvarFlags = (CHDFlags)br.ReadUInt32();						// flags (see below)
			mvarCompressionType = (CHDCompressionType)br.ReadUInt32();	// compression type

			uint totalHunks = 0;

			if (mvarFormatVersion == 1 || mvarFormatVersion == 2)
			{
				mvarHunkSize = br.ReadUInt32();
			}
			if (mvarFormatVersion < 5)
			{
				totalHunks = br.ReadUInt32();
			}
			if (mvarFormatVersion <= 2)
			{
				uint cylinders = br.ReadUInt32();		// number of cylinders on hard disk
				uint heads = br.ReadUInt32();			// number of heads on hard disk
				uint sectors = br.ReadUInt32();			// number of sectors on hard disk
			}
			else
			{
				ulong logicalBytes = br.ReadUInt64();
				ulong metaOffset = br.ReadUInt64();
			}

			if (mvarFormatVersion <= 3)
			{
				byte[] md5 = br.ReadBytes(16);			// MD5 checksum of raw data
				byte[] parentmd5 = br.ReadBytes(16);	// MD5 checksum of parent file
			}
			if (mvarFormatVersion >= 3)
			{
				mvarHunkSize = br.ReadUInt32();
			}
			if (mvarFormatVersion >= 5)
			{
				uint unitbytes = br.ReadUInt32();			// number of bytes per unit within each hunk
				byte[] rawsha1 = br.ReadBytes(20);			// raw data SHA1
				byte[] sha1 = br.ReadBytes(20);				// combined raw+meta SHA1
				byte[] parentsha1 = br.ReadBytes(20);		// combined raw+meta SHA1 of parent
			}
			else if (mvarFormatVersion >= 3)
			{
				byte[] sha1 = br.ReadBytes(20);				// combined raw+meta SHA1
				byte[] parentsha1 = br.ReadBytes(20);		// combined raw+meta SHA1 of parent
				if (mvarFormatVersion == 4)
				{
					byte[] rawsha1 = br.ReadBytes(20);			// raw data SHA1
				}
			}

			if (m_RawMapOffset == 0) m_RawMapOffset = br.Accessor.Position;

			for (uint i = 0; i < totalHunks; i++)
			{
				File file = new File();
				file.Name = "HUNK" + (i + 1).ToString().PadLeft(4, '0');
				file.Size = mvarHunkSize;
				file.Source = new CHDHunkFileSource(br, i, m_RawMapOffset, mvarHunkSize);
				fsom.Files.Add(file);
			}

		}

		Compression.CompressionModule[] compressionModules = new Compression.CompressionModule[]
		{
			new Compression.Modules.Deflate.DeflateCompressionModule()
		};

		protected override void SaveInternal(ObjectModel objectModel)
		{
			FileSystemObjectModel fsom = (objectModel as FileSystemObjectModel);
			if (fsom == null) throw new ObjectModelNotSupportedException();

			IO.Writer bw = base.Accessor.Writer;
			bw.Endianness = IO.Endianness.BigEndian;

			bw.WriteFixedLengthString("MComprHD");

			uint headerLength = 16;
			
			// length of header (including tag and length fields)
			bw.WriteUInt32(headerLength);
			// drive format version
			bw.WriteUInt32(mvarFormatVersion);
			bw.WriteUInt32((uint)mvarFlags);
			bw.WriteUInt32((uint)mvarCompressionType);

			uint totalHunks = 0;

			if (mvarFormatVersion == 1 || mvarFormatVersion == 2)
			{
				bw.WriteUInt32(mvarHunkSize);
			}
			if (mvarFormatVersion < 5)
			{
				bw.WriteUInt32(totalHunks);
			}
			if (mvarFormatVersion <= 2)
			{
				// number of cylinders on hard disk
				uint cylinders = 0;
				bw.WriteUInt32(cylinders);
				// number of heads on hard disk
				uint heads = 0;
				bw.WriteUInt32(heads);
				// number of sectors on hard disk
				uint sectors = 0;
				bw.WriteUInt32(sectors);
			}
			else
			{
				ulong logicalBytes = 0;
				bw.WriteUInt64(logicalBytes);
				ulong metaOffset = 0;
				bw.WriteUInt64(metaOffset);
			}

			if (mvarFormatVersion <= 3)
			{ 
				// MD5 checksum of raw data
				byte[] md5 = new byte[16];
				bw.WriteBytes(md5);

				// MD5 checksum of parent file
				byte[] parentmd5 = new byte[16];
				bw.WriteBytes(parentmd5);
			}
			if (mvarFormatVersion >= 3)
			{
				bw.WriteUInt32(mvarHunkSize);
			}
			if (mvarFormatVersion >= 5)
			{
				uint unitbytes = 0;			// number of bytes per unit within each hunk
				bw.WriteUInt32(unitbytes);
				byte[] rawsha1 = new byte[20];			// raw data SHA1
				bw.WriteBytes(rawsha1);

				byte[] sha1 = new byte[20];				// combined raw+meta SHA1
				bw.WriteBytes(sha1);

				byte[] parentsha1 = new byte[20];		// combined raw+meta SHA1 of parent
				bw.WriteBytes(parentsha1);
			}
			else if (mvarFormatVersion >= 3)
			{
				byte[] sha1 = new byte[20];				// combined raw+meta SHA1
				bw.WriteBytes(sha1);

				byte[] parentsha1 = new byte[20];		// combined raw+meta SHA1 of parent
				bw.WriteBytes(parentsha1);

				if (mvarFormatVersion == 4)
				{
					byte[] rawsha1 = new byte[20];			// raw data SHA1
					bw.WriteBytes(rawsha1);
				}
			}
		}
	}
}