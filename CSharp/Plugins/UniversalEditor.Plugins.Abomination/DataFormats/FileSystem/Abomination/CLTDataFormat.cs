﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniversalEditor.IO;
using UniversalEditor.ObjectModels.FileSystem;

namespace UniversalEditor.DataFormats.FileSystem.Abomination
{
    public class CLTDataFormat : DataFormat
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
            if (fsom == null) throw new ObjectModelNotSupportedException();

            Reader reader = base.Accessor.Reader;
            string AWAD = reader.ReadFixedLengthString(4);
            if (AWAD != "AWAD") throw new InvalidDataFormatException("File does not begin with 'AWAD'");

            uint fileCount = reader.ReadUInt32();
            for (uint i = 0; i < fileCount; i++)
            {
                string fileName = reader.ReadFixedLengthString(260);
                uint length = reader.ReadUInt32();
                uint offset = reader.ReadUInt32();

                File file = fsom.AddFile(fileName);
                file.Size = length;
                file.Properties.Add("length", length);
                file.Properties.Add("offset", offset);
                file.Properties.Add("reader", reader);
                file.DataRequest += file_DataRequest;
            }
        }

        private void file_DataRequest(object sender, DataRequestEventArgs e)
        {
            File file = (sender as File);
            uint length = (uint)file.Properties["length"];
            uint offset = (uint)file.Properties["offset"];
            Reader reader = (Reader)file.Properties["reader"];
            reader.Seek(offset, SeekOrigin.Begin);
            byte[] data = reader.ReadBytes(length);
            e.Data = data;
        }

        protected override void SaveInternal(ObjectModel objectModel)
        {
            FileSystemObjectModel fsom = (objectModel as FileSystemObjectModel);
            if (fsom == null) throw new ObjectModelNotSupportedException();

            Writer writer = base.Accessor.Writer;
            writer.WriteFixedLengthString("AWAD");

            File[] files = fsom.GetAllFiles();
            writer.WriteUInt32((uint)files.Length);
            uint offset = (uint)(8 + ((260 + 4 + 4) * files.Length));

            foreach (File file in files)
            {
                writer.WriteFixedLengthString(file.Name, 260);
                writer.WriteUInt32((uint)file.Size);
                writer.WriteUInt32(offset);
            }
            foreach (File file in files)
            {
                writer.WriteBytes(file.GetDataAsByteArray());
            }
        }
    }
}
