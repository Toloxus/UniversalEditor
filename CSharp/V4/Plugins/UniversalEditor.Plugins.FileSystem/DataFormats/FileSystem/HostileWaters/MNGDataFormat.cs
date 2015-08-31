﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniversalEditor.ObjectModels.FileSystem;

namespace UniversalEditor.DataFormats.FileSystem.HostileWaters
{
    public class MNGDataFormat : DataFormat
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

        protected override void LoadInternal(ref ObjectModel objectModel)
        {
            FileSystemObjectModel fsom = (objectModel as FileSystemObjectModel);
            if (fsom == null) return;

            IO.Reader br = base.Accessor.Reader;
            uint fileCount = br.ReadUInt32();
            for (uint i = 0; i < fileCount; i++)
            {
                string fileName = br.ReadNullTerminatedString();
                uint compressedSize = br.ReadUInt32();
                uint decompressedSize = br.ReadUInt32();
                uint offset = br.ReadUInt32();

                File file = new File();
                file.Name = fileName;
                file.Size = decompressedSize;
                file.Properties.Add("reader", br);
                file.Properties.Add("offset", offset);

                file.Properties.Add("CompressedSize", compressedSize);
                file.Properties.Add("DecompressedSize", decompressedSize);
                file.DataRequest += file_DataRequest;
                fsom.Files.Add(file);
            }
        }

        private void file_DataRequest(object sender, DataRequestEventArgs e)
        {
            File file = (sender as File);
            IO.Reader br = (IO.Reader)file.Properties["reader"];
            uint length = (uint)file.Properties["CompressedSize"];
            uint offset = (uint)file.Properties["offset"];
            uint decompressedLength = (uint)file.Properties["DecompressedSize"];

            br.Accessor.Position = offset;
            byte[] compressedData = br.ReadBytes(length);
            byte[] decompressedData = UniversalEditor.Compression.CompressionModules.Zlib.Decompress(compressedData);
            if (decompressedData.Length != decompressedLength) throw new InvalidOperationException("Decompressed file size mismatch");

            e.Data = decompressedData;
        }

        protected override void SaveInternal(ObjectModel objectModel)
        {
            FileSystemObjectModel fsom = (objectModel as FileSystemObjectModel);
            if (fsom == null) return;

            IO.Writer bw = base.Accessor.Writer;
            bw.WriteUInt32((uint)fsom.Files.Count);

            uint offset = 0;
            foreach (File file in fsom.Files)
            {
                offset += (uint)(file.Name.Length + 13);
            }
            foreach (File file in fsom.Files)
            {
                bw.WriteNullTerminatedString(file.Name);
                
                byte[] decompressedData = file.GetData();
                byte[] compressedData = UniversalEditor.Compression.CompressionModules.Zlib.Compress(decompressedData);
                bw.WriteUInt32((uint)compressedData.Length);
                bw.WriteUInt32((uint)decompressedData.Length);
                bw.WriteUInt32(offset);
            }
            foreach (File file in fsom.Files)
            {
                bw.WriteBytes(file.GetData());
            }
            bw.Flush();
        }
    }
}