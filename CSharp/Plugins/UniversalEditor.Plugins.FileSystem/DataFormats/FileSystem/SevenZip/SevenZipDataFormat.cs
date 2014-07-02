﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniversalEditor.ObjectModels.FileSystem;

namespace UniversalEditor.DataFormats.FileSystem.SevenZip
{
    public class SevenZipDataFormat : DataFormat
    {
        private static DataFormatReference _dfr = null;
        public override DataFormatReference MakeReference()
        {
            if (_dfr == null)
            {
                _dfr = base.MakeReference();
                _dfr.Capabilities.Add(typeof(FileSystemObjectModel), DataFormatCapabilities.All);
                _dfr.Filters.Add("7-ZIP archive", new byte?[][] { new byte?[] { (byte)'7', (byte)'z', 0xBC, 0xAF, 0x27, 0x1C } }, new string[] { "*.7z" });
                _dfr.ContentTypes.Add("application/x-7z-compressed");
            }
            return _dfr;
        }

        protected override void LoadInternal(ref ObjectModel objectModel)
        {
            FileSystemObjectModel fsom = (objectModel as FileSystemObjectModel);
            if (fsom == null) return;

            IO.Reader br = base.Accessor.Reader;
            br.Accessor.Position = 0;

            string signature1 = br.ReadFixedLengthString(2); // 7z
            if (signature1 != "7z") throw new InvalidDataFormatException("File does not begin with 7z");

            int signature2 = br.ReadInt32();
            if (signature2 != 0x1C27AFBC) throw new InvalidDataFormatException("File does not begin with LE: 0x1C27AFBC");

			byte formatVersionMajor = br.ReadByte();
			byte formatVersionMinor = br.ReadByte();

			uint startHeaderCRC = br.ReadUInt32();

			ulong nextHeaderOffset = br.ReadUInt64();
			ulong nextHeaderSize = br.ReadUInt64();
			uint nextHeaderCRC = br.ReadUInt32();

			br.Accessor.Seek((long)nextHeaderOffset, IO.SeekOrigin.Current);

			while (true)
			{
				SevenZipBlockType blockID = (SevenZipBlockType)ReadNumber(br);
				switch (blockID)
				{
					case SevenZipBlockType.EncodedHeader:
					{
						SevenZipHeader header = ReadEncodedHeader(br);
						break;
					}
					case SevenZipBlockType.PackInfo:
					{
						SevenZipPackInfo packInfo = ReadPackInfo(br);
						break;
					}
				}
			}

            short u1a = br.ReadInt16();					// 1024		1024
            int u1b = br.ReadInt16();					// 25464	-6047
            short u2a = br.ReadInt16();					// 24321	7104
            short compressedSize = br.ReadInt16();		// 58		240 - this is not compressed size

            int u3 = br.ReadInt32();

            short u4a = br.ReadInt16();
            short u4b = br.ReadInt16();

            int u5 = br.ReadInt32();
            short u6 = br.ReadInt16();
            short u7 = br.ReadInt16();
            short u8 = br.ReadInt16();

			// byte[] compressedData = br.ReadBytes(compressedSize);

			int u9 = br.ReadInt32();
			int u10 = br.ReadInt32();
			int u11 = br.ReadInt32();
			int u12 = br.ReadInt32();
			
			short decompressedSize = br.ReadInt16();

			short u14 = br.ReadInt16();
			short u15 = br.ReadInt16();
			short u16 = br.ReadInt16();
			short u17 = br.ReadInt16();
			short u18 = br.ReadInt16();
			short u19 = br.ReadInt16();

			short fileNameLength = br.ReadInt16();
			fileNameLength--;
			string fileName = br.ReadFixedLengthString(fileNameLength, IO.Encoding.UTF16LittleEndian);
			fileName = fileName.TrimNull();

			short u20 = br.ReadInt16();
			short u21 = br.ReadInt16();
			int u22 = br.ReadInt32();
			int u23 = br.ReadInt32();
			short u24 = br.ReadInt16();
			short u25 = br.ReadInt16();
			short u26 = br.ReadInt16();
			int u27 = br.ReadInt32();
        }

		private SevenZipHeader ReadEncodedHeader(IO.Reader br)
		{
			SevenZipHeader header = new SevenZipHeader();
			while (true)
			{
				SevenZipBlockType blockID = (SevenZipBlockType)ReadNumber(br);
				if (blockID == SevenZipBlockType.End) break;

				switch (blockID)
				{
					case SevenZipBlockType.PackInfo:
					{
						SevenZipPackInfo packInfo = ReadPackInfo(br);
						break;
					}
					case SevenZipBlockType.UnpackInfo:
					{
						SevenZipUnpackInfo unpackInfo = ReadUnpackInfo(br);
						break;
					}
					case SevenZipBlockType.Size:
					{

						break;
					}
				}
			}
			return header;
		}

		private SevenZipUnpackInfo ReadUnpackInfo(IO.Reader br)
		{
			while (true)
			{
				SevenZipBlockType blockID = (SevenZipBlockType)ReadNumber(br);
				switch (blockID)
				{
					case SevenZipBlockType.Folder:
					{
						ulong folderCount = ReadNumber(br);
						byte external = br.ReadByte();
						switch (external)
						{
							case 0:
							{
								for (ulong i = 0; i < folderCount; i++)
								{
									SevenZipFolder folder = ReadFolder(br);
								}
								break;
							}
							case 1:
							{
								ulong dataStreamIndex = ReadNumber(br);
								break;
							}
						}
						break;
					}
				}
			}
		}

		private SevenZipFolder ReadFolder(IO.Reader br)
		{
			ulong coderCount = ReadNumber(br);
			for (ulong i = 0; i < coderCount; i++)
			{
				SevenZipCoder coder = ReadCodersInfo(br);
			}
			NumBindPairs
			BindPairsInfo[NumBindPairs]
			{
			  InIndex;
			  OutIndex;
			}
			PackedIndices
		}

		private SevenZipCoder ReadCodersInfo(IO.Reader br)
		{
			SevenZipCoder retval = new SevenZipCoder();
			byte idSizeAndFlags = br.ReadByte();
			byte idSize = (byte)idSizeAndFlags.GetBits(0, 4);
			retval.Flags = (SevenZipCoderFlags)idSizeAndFlags.GetBits(4, 4);
			retval.CodecID = br.ReadFixedLengthString(idSize);
			if ((retval.Flags & SevenZipCoderFlags.IsComplex) == SevenZipCoderFlags.IsComplex)
			{
				ulong inputStreamCount = ReadNumber(br);
				ulong outputStreamCount = ReadNumber(br);
			}
			if ((retval.Flags & SevenZipCoderFlags.HasAttributes) == SevenZipCoderFlags.HasAttributes)
			{
				ulong propertiesSize = ReadNumber(br);
				byte[] properties = br.ReadBytes(propertiesSize);
			}
			return retval;
		}

		private SevenZipPackInfo ReadPackInfo(IO.Reader br)
		{
			SevenZipPackInfo packInfo = new SevenZipPackInfo();
			packInfo.packPos = ReadNumber(br);
			packInfo.packStreamCount = ReadNumber(br);
			packInfo.packSizes = new ulong[packInfo.packStreamCount];
			packInfo.packStreamDigests = new uint[packInfo.packStreamCount];

			while (true)
			{
				SevenZipBlockType blockType = (SevenZipBlockType)ReadNumber(br);
				if (blockType == SevenZipBlockType.End) break;

				switch (blockType)
				{
					case SevenZipBlockType.Size:
					{
						for (ulong i = 0; i < packInfo.packStreamCount; i++)
						{
							packInfo.packSizes[i] = ReadNumber(br);
						}
						break;
					}
					case SevenZipBlockType.CRC:
					{
						for (ulong i = 0; i < packInfo.packStreamCount; i++)
						{
							packInfo.packStreamDigests[i] = br.ReadUInt32();
						}
						break;
					}
					default:
					{
						break;
					}
				}
			}

			return packInfo;
		}

		private ulong ReadNumber(IO.Reader br)
		{
			if (br.EndOfStream) throw new System.IO.EndOfStreamException();
			
			byte firstByte = br.ReadByte();
			byte mask = 0x80;
			ulong value = 0;
			for (int i = 0; i < 8; i++)
			{
				if ((firstByte & mask) == 0)
				{
					ulong highPart = ((ulong)firstByte & ((ulong)mask - 1));
					value += (highPart << (i * 8));
					return value;
				}
				if (br.EndOfStream) throw new System.IO.EndOfStreamException();
				value |= ((ulong)br.ReadByte() << (8 * i));
				mask >>= 1;
			}
			return value;
		}

        protected override void SaveInternal(ObjectModel objectModel)
        {
            throw new NotImplementedException();
        }
    }
}
