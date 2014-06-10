﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniversalEditor.DataFormats.Multimedia.Picture.Microsoft.Bitmap;
using UniversalEditor.IO;
using UniversalEditor.ObjectModels.Multimedia.Palette;
using UniversalEditor.ObjectModels.Multimedia.Picture;

namespace UniversalEditor.DataFormats.Multimedia.Picture.Cyberlore
{
    public class PICTDataFormat : DataFormat
    {
        private static DataFormatReference _dfr = null;
        public override DataFormatReference MakeReference()
        {
            if (_dfr == null)
            {
                _dfr = base.MakeReference();
                _dfr.Capabilities.Add(typeof(PictureObjectModel), DataFormatCapabilities.All);
                _dfr.Filters.Add("Cyberlore PICT image", new string[] { "*.pict" });
                _dfr.Sources.Add("http://wiki.xentax.com/index.php?title=GRF_Playboy_The_Mansion");
                _dfr.Sources.Add("http://forum.xentax.com/viewtopic.php?p=9778");
            }
            return _dfr;
        }

        private int mvarFormatVersion = 0;
        public int FormatVersion { get { return mvarFormatVersion; } set { mvarFormatVersion = value; } }

        private PaletteObjectModel mvarPalette = new PaletteObjectModel();
        public PaletteObjectModel Palette { get { return mvarPalette; } }

        protected override void LoadInternal(ref ObjectModel objectModel)
        {
            PictureObjectModel pic = (objectModel as PictureObjectModel);
            if (pic == null) throw new ObjectModelNotSupportedException();

            Reader reader = base.Accessor.Reader;
            mvarFormatVersion = reader.ReadInt32();
            if (mvarFormatVersion == 1)
            {
                // standard PICT format
                ushort colorCount = reader.ReadUInt16();
                uint unknown1 = reader.ReadUInt32();
                uint unknown2 = reader.ReadUInt32();
                ushort unknown3 = reader.ReadUInt16();
                ushort width = reader.ReadUInt16();
                ushort height = reader.ReadUInt16();
                ushort colorDepth = reader.ReadUInt16();
                for (ushort i = 0; i < colorCount; i++)
                {
                    byte blue = reader.ReadByte();
                    byte green = reader.ReadByte();
                    byte red = reader.ReadByte();
                    byte alpha = reader.ReadByte();
                    mvarPalette.Entries.Add(Color.FromRGBA(red, green, blue, alpha));
                }

                pic.Width = width;
                pic.Height = height;

                for (int y = height - 1; y >= 0; y--)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte paletteIndex = reader.ReadByte();
                        pic.SetPixel(mvarPalette.Entries[paletteIndex].Color, x, y);
                    }
                }
            }
            else if (mvarFormatVersion == 8)
            {
                // this is simply a Microsoft Bitmap file
                BitmapDataFormat bitmap = new BitmapDataFormat();
                ContinueLoading(ref objectModel, bitmap);
            }
        }

        protected override void SaveInternal(ObjectModel objectModel)
        {
            throw new NotImplementedException();
        }
    }
}