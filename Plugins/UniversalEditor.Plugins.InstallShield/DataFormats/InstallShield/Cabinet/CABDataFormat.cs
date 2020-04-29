﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniversalEditor.ObjectModels.FileSystem;

namespace UniversalEditor.DataFormats.InstallShield.Cabinet
{
    public class CABDataFormat : DataFormat
    {
        private static DataFormatReference _dfr = null;
        public override DataFormatReference MakeReference()
        {
            if (_dfr == null)
            {
                _dfr = base.MakeReference();
                _dfr.Capabilities.Add(typeof(FileSystemObjectModel), DataFormatCapabilities.All);
                _dfr.Filters.Add("InstallShield cabinet", new byte?[][] { new byte?[] { (byte)'I', (byte)'S', (byte)'c', (byte)'(' } }, new string[] { "*.cab" });
            }
            return _dfr;
        }

        private const int MAX_FILE_GROUP_COUNT = 71;
        private const int MAX_COMPONENT_COUNT = 71;

        private const int OFFSET_COUNT = 0x47;

        private const int CAB_SIGNATURE = 0x28635349;
        private const int MSCF_SIGNATURE = 0x4643534d;

        private const int COMMON_HEADER_SIZE = 20;
        private const int VOLUME_HEADER_SIZE_V5 = 40;
        private const int VOLUME_HEADER_SIZE_V6 = 64;

        protected override void LoadInternal(ref ObjectModel objectModel)
        {
            FileSystemObjectModel fsom = (objectModel as FileSystemObjectModel);
            if (fsom == null) return;

            IO.BinaryReader br = base.Stream.BinaryReader;
            br.BaseStream.Position = 0;
            br.Endianness = IO.Endianness.LittleEndian;

            string ISc_ = br.ReadFixedLengthString(4);
            if (ISc_ != "ISc(") throw new InvalidDataFormatException();

            uint version = br.ReadUInt32();
            int majorVersion = 0;
            if (version >> 24 == 1)
            {
                majorVersion = (int)((version >> 12) & 0xf);
            }
            else if (version >> 24 == 2 || version >> 24 == 4)
            {
                majorVersion = (int)(version & 0xffff);
                if (majorVersion != 0)
                {
                    majorVersion = (int)(majorVersion / 100);
                }
            }

            uint volumeInfo = br.ReadUInt32();
            uint cabinetDescriptorOffset = br.ReadUInt32();
            uint cabinetDescriptorSize = br.ReadUInt32();

            br.BaseStream.Seek(cabinetDescriptorOffset, System.IO.SeekOrigin.Begin);


            int unknown1 = br.ReadInt32();
            int unknown2 = br.ReadInt32();
            int unknown3 = br.ReadInt32();

            uint fileTableOffset = br.ReadUInt32();
            uint unknown4 = br.ReadUInt32();
            uint fileTableSize = br.ReadUInt32();
            uint fileTableSize2 = br.ReadUInt32();
            uint directoryCount = br.ReadUInt32();
            uint unknown5 = br.ReadUInt32();
            uint unknown6 = br.ReadUInt32();
            uint fileCount = br.ReadUInt32();
            uint fileTableOffset2 = br.ReadUInt32();

            if (br.BaseStream.Position != 0x30) throw new InvalidDataFormatException();
            if (fileTableSize != fileTableSize2) throw new InvalidDataFormatException("File table sizes do not match");

            uint unknown7 = br.ReadUInt32();
            uint unknown8 = br.ReadUInt32();
            uint unknown9 = br.ReadUInt32();
            ushort unknown10 = br.ReadUInt16();

            uint[] fileGroupOffsets = new uint[MAX_FILE_GROUP_COUNT];
            for (int i = 0; i < MAX_FILE_GROUP_COUNT; i++)
            {
                fileGroupOffsets[i] = br.ReadUInt32();
            }
            uint[] componentOffsets = new uint[MAX_COMPONENT_COUNT];
            for (int i = 0; i < MAX_COMPONENT_COUNT; i++)
            {
                componentOffsets[i] = br.ReadUInt32();
            }



        }

        protected override void SaveInternal(ObjectModel objectModel)
        {
            FileSystemObjectModel fsom = (objectModel as FileSystemObjectModel);
            if (fsom == null) return;

            IO.BinaryWriter bw = base.Stream.BinaryWriter;
            bw.WriteFixedLengthString("ISc(");

            int unknown1 = 0;
            int unknown2 = 0;
            uint fileTableOffset = 0;
            uint unknown3 = 0;
            uint fileTableSize = 0;
            uint fileTableSize2 = 0;
            uint directoryCount = 0;
            uint unknown4 = 0;
            uint unknown5 = 0;
            uint fileCount = 0;
            uint fileTableOffset2 = 0;
            uint unknown6 = 0;
            uint unknown7 = 0;
            uint unknown8 = 0;
            ushort unknown9 = 0;

            bw.Write(unknown1);
            bw.Write(unknown2);
            bw.Write(fileTableOffset);
            bw.Write(unknown3 );
            bw.Write(fileTableSize );
            bw.Write(fileTableSize2 );
            bw.Write(directoryCount );
            bw.Write(unknown4 );
            bw.Write(unknown5 );
            bw.Write(fileCount );
            bw.Write(fileTableOffset2);
            bw.Write(unknown6);
            bw.Write(unknown7);
            bw.Write(unknown8);
            bw.Write(unknown9);

            uint[] fileGroupOffsets = new uint[MAX_FILE_GROUP_COUNT];
            uint[] componentOffsets = new uint[MAX_COMPONENT_COUNT];

            for (int i = 0; i < MAX_FILE_GROUP_COUNT; i++)
            {
                bw.Write(fileGroupOffsets[i]);
            }
            for (int i = 0; i < MAX_COMPONENT_COUNT; i++)
            {
                bw.Write(componentOffsets[i]);
            }
            bw.Flush();
        }
    }
}