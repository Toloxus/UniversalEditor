using System;
using UniversalEditor.Accessors;
using UniversalEditor.Compression;
using UniversalEditor.IO;
using UniversalEditor.ObjectModels.Multimedia.Audio.Project;

namespace UniversalEditor.DataFormats.Multimedia.Audio.Project.LMMS
{
	public class LMMSCompressedDataFormat : DataFormat
	{
		protected override DataFormatReference MakeReferenceInternal()
		{
			DataFormatReference dfr = base.MakeReferenceInternal();
			dfr.Capabilities.Add(typeof(AudioProjectObjectModel), DataFormatCapabilities.All);
			return dfr;
		}
		protected override void LoadInternal(ref ObjectModel objectModel)
		{
			Reader br = base.Accessor.Reader;
			byte[] input = br.ReadToEnd();
			byte[] output = CompressionModules.Gzip.Decompress(input);
			LMMSProjectDataFormat mmp = new LMMSProjectDataFormat();
            MemoryAccessor file = new MemoryAccessor(output);
			Document doc = new Document(objectModel, mmp, file);

			file.Open();
			doc.Load();
			file.Close();
		}
		protected override void SaveInternal(ObjectModel objectModel)
		{
			throw new NotImplementedException();
		}
	}
}