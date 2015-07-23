﻿using System;
using System.Collections.Generic;
using System.Text;
using UniversalEditor.IO;
using UniversalEditor.ObjectModels.FileSystem;

namespace UniversalEditor.DataFormats.FileSystem.Gainax.LBX
{
	public class LBXDataFormat : DataFormat
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
			if (base.Accessor.Length < 6) throw new InvalidDataFormatException("File must be greater than 4 bytes");

			base.Accessor.Seek(-6, SeekOrigin.End);

			ushort fileCount = reader.ReadUInt16();
			uint directoryOffset = reader.ReadUInt32();
			if (directoryOffset > base.Accessor.Length) throw new InvalidDataFormatException("Directory offset goes past end of file");

			base.Accessor.Seek(directoryOffset, SeekOrigin.Begin);

			for (ushort i = 0; i < fileCount; i++)
			{
				string fileName = reader.ReadFixedLengthString(12).TrimNull();
				uint offset = reader.ReadUInt32();
				uint length = reader.ReadUInt32();

				File file = fsom.AddFile(fileName);
				file.Size = length;
				file.Properties.Add("reader", reader);
				file.Properties.Add("offset", offset);
				file.Properties.Add("length", length);
				file.DataRequest += file_DataRequest;
			}
		}

		private void file_DataRequest(object sender, DataRequestEventArgs e)
		{
			File file = (sender as File);
			Reader reader = (Reader)file.Properties["reader"];
			uint offset = (uint)file.Properties["offset"];
			uint length = (uint)file.Properties["length"];

			reader.Accessor.Seek(offset, SeekOrigin.Begin);
			e.Data = reader.ReadBytes(length);
		}

		protected override void SaveInternal(ObjectModel objectModel)
		{
			FileSystemObjectModel fsom = (objectModel as FileSystemObjectModel);
			if (fsom == null) throw new ObjectModelNotSupportedException();

			Writer writer = base.Accessor.Writer;

			File[] files = fsom.GetAllFiles();
			uint[] offsets = new uint[files.Length];
			uint[] lengths = new uint[files.Length];

			uint offset = 0;
			for (int i = 0; i < files.Length; i++)
			{
				byte[] data = files[i].GetData();
				writer.WriteBytes(data);

				lengths[i] = (uint)data.Length;
				offsets[i] = offset;
				offset += (uint)data.Length;
			}

			uint directoryOffset = (uint)base.Accessor.Position;
			for (int i = 0; i < files.Length; i++)
			{
				writer.WriteFixedLengthString(files[i].Name, 12);
				writer.WriteUInt32(offsets[i]);
				writer.WriteUInt32(lengths[i]);
			}

			writer.WriteUInt16((ushort)files.Length);
			writer.WriteUInt32(directoryOffset);

			writer.Flush();
		}
	}
}
