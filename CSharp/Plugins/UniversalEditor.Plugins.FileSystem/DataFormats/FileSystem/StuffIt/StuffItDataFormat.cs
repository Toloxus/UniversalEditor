﻿/*
 * Created by SharpDevelop.
 * User: Mike Becker
 * Date: 6/15/2013
 * Time: 2:09 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using UniversalEditor.IO;
using UniversalEditor.ObjectModels.FileSystem;

namespace UniversalEditor.DataFormats.FileSystem.StuffIt
{
	/// <summary>
	/// Description of StuffItDataFormat.
	/// </summary>
	public class StuffItDataFormat : DataFormat
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
		
		// TODO: need to research the StuffIt data format
		
		protected override void LoadInternal(ref ObjectModel objectModel)
		{
			FileSystemObjectModel fsom = (objectModel as FileSystemObjectModel);
			if (fsom == null) throw new ObjectModelNotSupportedException();
			
			IO.Reader br = base.Accessor.Reader;
			
			br.Endianness = IO.Endianness.BigEndian;
			
			string signature = br.ReadFixedLengthString(80);
			if (signature != "StuffIt (c)1997-2001 Aladdin Systems, Inc., http://www.aladdinsys.com/StuffIt/\r\n") throw new InvalidDataFormatException("File does not begin with \"StuffIt (c)1997-2001 Aladdin Systems, Inc., http://www.aladdinsys.com/StuffIt/\", 0x0D, 0x0A");
			
			ushort u0 = br.ReadUInt16();
			ushort u1 = br.ReadUInt16();
			ushort u2 = br.ReadUInt16();
			ushort u3 = br.ReadUInt16();
			ushort u4 = br.ReadUInt16();
			ushort u5 = br.ReadUInt16();
			ushort u6 = br.ReadUInt16();
			ushort u7 = br.ReadUInt16();
			ushort u8 = br.ReadUInt16();
			ushort u9 = br.ReadUInt16();
			ushort u10 = br.ReadUInt16();
			ushort u11 = br.ReadUInt16();
			ushort u12 = br.ReadUInt16();
			ushort u13 = br.ReadUInt16();
			ushort u14 = br.ReadUInt16();
			
			long timestamp = br.ReadInt64();
			
			ushort u15 = br.ReadUInt16();
			ushort u16 = br.ReadUInt16();
			ushort u17 = br.ReadUInt16();
			ushort u18 = br.ReadUInt16();
			ushort u19 = br.ReadUInt16();
			ushort u20 = br.ReadUInt16();
			ushort u21 = br.ReadUInt16();
			ushort u22 = br.ReadUInt16();
			ushort u23 = br.ReadUInt16();
			ushort u24 = br.ReadUInt16();
			ushort u25 = br.ReadUInt16();
			ushort u26 = br.ReadUInt16();
			ushort u27 = br.ReadUInt16();
			ushort u28 = br.ReadUInt16();
			
			// for each file
			ushort fileNameLength = br.ReadUInt16();
			string fileName = br.ReadNullTerminatedString();
			byte nul = br.ReadByte();
			
			uint u29 = br.ReadUInt32();
			ushort u30 = br.ReadUInt16();
			ushort u31 = br.ReadUInt16();
			ushort u32 = br.ReadUInt16();
			
			br.Accessor.Seek(24, SeekOrigin.Current);
		}
		protected override void SaveInternal(ObjectModel objectModel)
		{
			FileSystemObjectModel fsom = (objectModel as FileSystemObjectModel);
			if (fsom == null) throw new ObjectModelNotSupportedException();
			
			IO.Writer bw = base.Accessor.Writer;
			
			bw.WriteFixedLengthString("StuffIt (c)1997-2001 Aladdin Systems, Inc., http://www.aladdinsys.com/StuffIt/\r\n");
			
		}
	}
}