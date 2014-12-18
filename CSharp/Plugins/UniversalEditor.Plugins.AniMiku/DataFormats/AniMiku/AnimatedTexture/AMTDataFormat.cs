﻿using System;
using System.Collections.Generic;
using System.Text;

using UniversalEditor.ObjectModels.FileSystem;

namespace UniversalEditor.DataFormats.FileSystem.AniMiku.TexturePackage
{
	/// <summary>
	/// Implements the AniMiku Texture Package data format.
	/// </summary>
	public class AMTDataFormat : DataFormat
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
			int unknown = br.ReadInt32();
			int count = br.ReadInt32();
			for (int i = 0; i < count; i++)
			{
				int dataSize = br.ReadInt32();
				byte[] data = br.ReadBytes(dataSize);
				fsom.Files.Add(i.ToString().PadLeft(8, '0'), data);
			}
		}
		protected override void SaveInternal(ObjectModel objectModel)
		{
			FileSystemObjectModel fsom = (objectModel as FileSystemObjectModel);
			if (fsom == null) return;

			IO.Writer bw = base.Accessor.Writer;
			int unknown = 150;
			bw.WriteInt32(unknown);

			bw.WriteInt32(fsom.Files.Count);
			foreach (File file in fsom.Files)
			{
				byte[] data = file.GetDataAsByteArray();
				bw.WriteInt32(data.Length);
				bw.WriteBytes(data);
			}
		}
	}
}
