﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UniversalEditor.Plugins.Multimedia.ObjectModels.Picture;

namespace UniversalEditor.Plugins.Multimedia.DataFormats.Picture.PortableNetworkGraphics
{
    public class PNGDataFormat : DataFormat
    {
        public override DataFormatReference MakeReference()
        {
            DataFormatReference dfr = base.MakeReference();
            dfr.Capabilities.Add(typeof(PictureObjectModel), DataFormatCapabilities.All);
            dfr.Filters.Add("Portable Network Graphics", new byte?[][] { new byte?[] { (byte)0x89, (byte)'P', (byte)'N', (byte)'G', (byte)0x0D, (byte)0x0A, (byte)0x1A, (byte)0x0A } }, new string[] { "*.png" });
            return dfr;
        }

        protected override void LoadInternal(ref ObjectModel objectModel)
        {
            IO.BinaryReader br = base.Stream.BinaryReader;
            PictureObjectModel pic = (objectModel as PictureObjectModel);

            byte[] signature = br.ReadBytes(8);
            br.Endianness = IO.Endianness.BigEndian;

            PNGChunk.PNGChunkCollection chunks = new PNGChunk.PNGChunkCollection();

            while (!br.EndOfStream)
            {
                int chunkLength = br.ReadInt32();
                string chunkType = br.ReadFixedLengthString(4);
                byte[] chunkData = br.ReadBytes(chunkLength);
                int chunkCRC = br.ReadInt32();
                chunks.Add(chunkType, chunkData);
            }

            IO.BinaryReader brIHDR = new IO.BinaryReader(chunks["IHDR"].Data);
            brIHDR.Endianness = IO.Endianness.BigEndian;
            pic.Width = brIHDR.ReadInt32();
            pic.Height = brIHDR.ReadInt32();
            byte bitDepth = brIHDR.ReadByte();
            byte colorType = brIHDR.ReadByte();
            byte compressionMethod = brIHDR.ReadByte();
            byte filterMethod = brIHDR.ReadByte();
            byte interlaceMethod = brIHDR.ReadByte();

            byte[] imageData = chunks["IDAT"].Data;

            switch (compressionMethod)
            {
                case 0: // DEFLATE compression
                {
                    byte[] uncompressedImageData = Compression.CompressionStream.Decompress(Compression.CompressionMethod.Zlib, imageData);

                    for (int x = 0; x < pic.Width; x++)
                    {
                        for (int y = 0; y < pic.Height; y++)
						{
							byte r = 0, g = 0, b = 0;
                            int index = (x * (y + pic.Width)) + 3;
							if ((index - 2) < uncompressedImageData.Length) r = uncompressedImageData[index - 2];
							if ((index - 1) < uncompressedImageData.Length) r = uncompressedImageData[index - 1];
							if ((index) < uncompressedImageData.Length) r = uncompressedImageData[index];
                            System.Drawing.Color color = System.Drawing.Color.FromArgb(r,g,b);
                            pic.SetPixel(color, x, y);
                        }
                    }
                    break;
                }
            }
        }

        protected override void SaveInternal(ObjectModel objectModel)
        {
            IO.BinaryWriter bw = base.Stream.BinaryWriter;
            PictureObjectModel pic = (objectModel as PictureObjectModel);

            byte[] signature = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
            bw.Write(signature);

            bw.Flush();
        }
    }
}