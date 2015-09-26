﻿/*
 * Created by SharpDevelop.
 * User: Mike Becker
 * Date: 5/12/2013
 * Time: 4:46 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UniversalEditor.ObjectModels.Multimedia.Picture;

namespace UniversalEditor.DataFormats.Multimedia.Picture.LEAD
{
	/// <summary>
	/// Description of CMPDataFormat.
	/// </summary>
	public class CMPDataFormat : DataFormat
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
			string LEAD = br.ReadFixedLengthString(4);
			if (LEAD != "LEAD") throw new InvalidDataFormatException("File does not begin with \"LEAD\"");
			
			ushort unknown1 = br.ReadUInt16();
			ushort unknown2 = br.ReadUInt16();
			ushort unknown3 = br.ReadUInt16();
			ushort unknown4 = br.ReadUInt16();
			ushort unknown5 = br.ReadUInt16();
			ushort unknown6 = br.ReadUInt16();
			
			pic.Width = br.ReadUInt16();
			pic.Height = br.ReadUInt16();
			
			ushort unknown9 = br.ReadUInt16();
			ushort unknown10 = br.ReadUInt16();
			
			// starts the compressed data
			byte[] compressedData = br.ReadToEnd();
			byte[] decompressedData = null; // UniversalEditor.Compression.CompressionStream.Decompress(UniversalEditor.Compression.CompressionMethod.Deflate, compressedData);
		}
		protected override void SaveInternal(ObjectModel objectModel)
		{
			throw new NotImplementedException();
		}
	}
}