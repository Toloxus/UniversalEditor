﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UniversalEditor.IO;
using UniversalEditor.ObjectModels.FileSystem;

namespace UniversalEditor.DataFormats.FileSystem.FArC
{
    public partial class FARCDataFormat : DataFormat
    {
        // this was mentioned on 
        // http://forum.xentax.com/viewtopic.php?f=10&t=9639
        // could this mean anything? -v-
        // PS : for each files same header (0x10 bytes - maybe key for decrypt??) - 6D4A249C8529DE62C8E3893931C9E0BC 
        // The files from Project Diva F are the same way, except they have a different header -- 69173ED8F50714439F6240AA7466C37A

        // EDAT v4
        // key
        // 6D4BF3D7245DB294B6C3F9E32AA57E79
        // kgen key
        // D1DF87B5C1471B360ACE21315A339C06

        // (it's not keys for FARC)

        // I guess it's like AES / XOR because all Sony FS use this.
        // EBOOT - AES
        // PSARC - AES
        // PGD - AES + XOR

        private struct FileEntry
        {
            public string name;
            public int offset;
            public int compressedSize;
            public int decompressedSize;

            // for writing
            public byte[] compressedData;
        }

        private DataFormatReference _dfr = null;
        public override DataFormatReference MakeReference()
        {
            if (_dfr == null)
            {
                _dfr = base.MakeReference();
                _dfr.Capabilities.Add(typeof(FileSystemObjectModel), DataFormatCapabilities.All);
                _dfr.Filters.Add("FArC archive", new byte?[][] { new byte?[] { (byte)'F', (byte)'A', (byte)'r', (byte)'C' }, new byte?[] { (byte)'F', (byte)'A', (byte)'r', (byte)'c' }, new byte?[] { (byte)'F', (byte)'A', (byte)'R', (byte)'C' } }, new string[] { "*.farc" });
            }
            return _dfr;
        }

        private bool mvarEncrypted = false;
        public bool Encrypted { get { return mvarEncrypted; } set { mvarEncrypted = value; } }

        protected override void LoadInternal(ref ObjectModel objectModel)
        {
            FileSystemObjectModel fsom = (objectModel as FileSystemObjectModel);
            if (fsom == null) return;

            Reader reader = base.Accessor.Reader;
            reader.Endianness = IO.Endianness.BigEndian;
            string FArC = reader.ReadFixedLengthString(4);
            if (!(FArC == "FArC" || FArC  == "FArc" || FArC == "FARC")) throw new InvalidDataFormatException();

            int directorySize = reader.ReadInt32();
            int dummy = reader.ReadInt32();

            if (FArC == "FArC")
            {
                while (reader.Accessor.Position < directorySize)
                {
                    FileEntry entry = new FileEntry();
                    entry.name = reader.ReadNullTerminatedString();
                    entry.offset = reader.ReadInt32();
                    entry.compressedSize = reader.ReadInt32();
                    entry.decompressedSize = reader.ReadInt32();

                    File file = fsom.AddFile(entry.name);
                    if (entry.decompressedSize == 0)
                    {
                        file.Size = entry.compressedSize;
                        entry.decompressedSize = entry.compressedSize;
                    }
                    else
                    {
                        file.Size = entry.decompressedSize;
                    }

                    file.Properties.Add("reader", reader);
                    file.Properties.Add("FileEntry", entry);
                    file.DataRequest += file_DataRequest;
                }
            }
            else if (FArC == "FArc")
            {
                while (reader.Accessor.Position - 12 < directorySize - 4)
                {
                    FileEntry entry = new FileEntry();
                    entry.name = reader.ReadNullTerminatedString();
                    entry.offset = reader.ReadInt32();
                    entry.compressedSize = reader.ReadInt32();

                    File file = fsom.AddFile(entry.name);

                    file.Size = entry.compressedSize;
                    entry.decompressedSize = entry.compressedSize;

                    file.Properties.Add("reader", reader);
                    file.Properties.Add("FileEntry", entry);
                    file.DataRequest += file_DataRequest;
                }
            }
            else if (FArC == "FARC")
            {
                uint flag0 = reader.ReadUInt32();
                uint flag1 = reader.ReadUInt32();
                uint flag2 = reader.ReadUInt32();
                uint flag3 = reader.ReadUInt32();

                while (reader.Accessor.Position < directorySize + 8)
                {
                    FileEntry entry = new FileEntry();
                    entry.name = reader.ReadNullTerminatedString();
                    entry.offset = reader.ReadInt32();
                    entry.compressedSize = reader.ReadInt32();

                    File file = fsom.AddFile(entry.name);

                    file.Size = entry.compressedSize;
                    entry.decompressedSize = reader.ReadInt32();

                    file.Properties.Add("reader", reader);
                    file.Properties.Add("FileEntry", entry);
                    file.DataRequest += file_DataRequest;
                }
            }
            else
            {
                throw new InvalidDataFormatException("Unknown version " + FArC);
            }
        }

        private void file_DataRequest(object sender, DataRequestEventArgs e)
        {
            File file = (sender as File);
            Reader reader = (Reader)file.Properties["reader"];
            FileEntry entry = (FileEntry)file.Properties["FileEntry"];

            reader.Accessor.Position = entry.offset;

            byte[] decompressedData = new byte[entry.decompressedSize];
            if (entry.decompressedSize > 0)
            {
                byte[] compressedData = reader.ReadBytes(entry.compressedSize);
                if (entry.compressedSize != entry.decompressedSize)
                {
                    try
                    {
                        decompressedData = UniversalEditor.Compression.CompressionModule.FromKnownCompressionMethod(Compression.CompressionMethod.Gzip).Decompress(compressedData);
                    }
                    catch (Exception ex)
                    {
                        // data encrypted? we have to decrypt it
                        byte[][] keys = new byte[][]
                        {
                            // Virtua Fighter 5
                            new byte[] { 0x6D, 0x4A, 0x24, 0x9C, 0x85, 0x29, 0xDE, 0x62, 0xC8, 0xE3, 0x89, 0x39, 0x31, 0xC9, 0xE0, 0xBC },
                            // Project DIVA F
                            new byte[] { 0x69, 0x17, 0x3E, 0xD8, 0xF5, 0x07, 0x14, 0x43, 0x9F, 0x62, 0x40, 0xAA, 0x74, 0x66, 0xC3, 0x7A }
                        };

                        byte[] encryptedData = null;
                        for (int k = 0; k < keys.Length; k++)
                        {
                            encryptedData = compressedData;
                            try
                            {
                                encryptedData = Decrypt(compressedData, keys[k]);
                            }
                            catch (CryptographicException ex1)
                            {
                                continue;
                            }
                        }
                        if (encryptedData == null)
                        {
                            throw new InvalidDataFormatException("No valid encryption keys were available to process this file");
                        }

                        // FIXME:   Project DIVA F key seems to work (without throwing an invalid padding error) but is not in a known compression
                        //          method...
                        decompressedData = Compression.CompressionModule.FromKnownCompressionMethod(Compression.CompressionMethod.Gzip).Decompress(encryptedData);
                    }
                }
                else
                {
                    decompressedData = compressedData;
                }
            }
            else
            {
                decompressedData = reader.ReadBytes(entry.compressedSize);
            }
            e.Data = decompressedData;
        }

        private byte[] Decrypt(byte[] data, byte[] key)
        {
            byte[] input = data;
            System.Security.Cryptography.AesManaged aes = new System.Security.Cryptography.AesManaged();
            aes.Key = key;

            System.Security.Cryptography.ICryptoTransform xform = aes.CreateDecryptor();
            int blockCount = input.Length / xform.InputBlockSize;
            for (int i = 0; i < blockCount; i++)
            {
                if (i == blockCount - 1)
                {
                    byte[] output2 = xform.TransformFinalBlock(input, i * xform.InputBlockSize, xform.InputBlockSize);
                }
                else
                {
                    int l = xform.TransformBlock(input, i * xform.InputBlockSize, xform.InputBlockSize, input, i * xform.InputBlockSize);
                }
            }
            return input;
        }

        protected override void SaveInternal(ObjectModel objectModel)
        {
            FileSystemObjectModel fsom = (objectModel as FileSystemObjectModel);
            if (fsom == null) return;

            Writer writer = base.Accessor.Writer;
            writer.Endianness = IO.Endianness.BigEndian;
            int ioffset = 12, isize = 0;
            
            List<int> FileDecompressedDataLength = new List<int>();
            List<byte[]> FileCompressedData = new List<byte[]>();

            foreach (File file in fsom.Files)
            {
                ioffset += (file.Name.Length + 1) + 12;

                byte[] decompressedData = file.GetDataAsByteArray();
                byte[] compressedData = UniversalEditor.Compression.CompressionModule.FromKnownCompressionMethod(Compression.CompressionMethod.Gzip).Compress(decompressedData);
                FileCompressedData.Add(compressedData);

                isize += ioffset + compressedData.Length;
            }

            writer.WriteFixedLengthString("FArC");

            int filesize = isize; 
            writer.WriteInt32(filesize);
            int dummy = 0;
            writer.WriteInt32(dummy);

            // ioffset = 12;
            int i = 0;
            foreach (File file in fsom.Files)
            {
                writer.WriteNullTerminatedString(file.Name);
                writer.WriteInt32(ioffset);

                byte[] compressedData = FileCompressedData[i];
                writer.WriteInt32(compressedData.Length);
                writer.WriteInt32((int)file.Size);

                ioffset += compressedData.Length;
                i++;
            }
            foreach (byte[] data in FileCompressedData)
            {
                writer.WriteBytes(data);
            }
            writer.Flush();
        }
    }
}
