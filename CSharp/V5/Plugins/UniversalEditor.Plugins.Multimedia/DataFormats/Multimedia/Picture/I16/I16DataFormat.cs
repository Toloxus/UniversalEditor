﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniversalEditor.ObjectModels.Multimedia.Picture;

namespace UniversalEditor.DataFormats.Multimedia.Picture.I16
{
    public class I16DataFormat : DataFormat
    {
        private static DataFormatReference _dfr = null;
        protected override DataFormatReference MakeReferenceInternal()
        {
            if (_dfr == null)
            {
                _dfr = base.MakeReferenceInternal();
                _dfr.Capabilities.Add(typeof(PictureObjectModel), DataFormatCapabilities.All);
            }
            return _dfr;
        }

        protected override void LoadInternal(ref ObjectModel objectModel)
        {
            PictureObjectModel pic = (objectModel as PictureObjectModel);
            if (pic == null) return;

            IO.Reader br = base.Accessor.Reader;
            ushort u1 = br.ReadUInt16();
            ushort u2 = br.ReadUInt16();
            ushort u3 = br.ReadUInt16();
            ushort u4 = br.ReadUInt16();

            byte u = br.ReadByte();

            int size = (int)((double)br.Accessor.Length / 8);
            pic.Width = size;
            pic.Height = size;

            while (!br.EndOfStream)
            {
                byte r = br.ReadByte();
                byte g = br.ReadByte();
                byte b = br.ReadByte();
                byte a = br.ReadByte();

                Color color = Color.FromRGBA(a, r, g, b);
                pic.SetPixel(color);
            }
        }

        protected override void SaveInternal(ObjectModel objectModel)
        {
            throw new NotImplementedException();
        }
    }
}