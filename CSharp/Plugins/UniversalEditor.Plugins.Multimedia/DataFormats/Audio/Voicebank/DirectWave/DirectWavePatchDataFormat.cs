using System;
using UniversalEditor.IO;
using UniversalEditor.Plugins.Multimedia.ObjectModels.Audio.Voicebank;
namespace UniversalEditor.Plugins.Multimedia.DataFormats.Audio.Voicebank.DirectWave
{
	public class DirectWavePatchDataFormat : DataFormat
	{
		public override DataFormatReference MakeReference()
		{
			DataFormatReference dfr = base.MakeReference();
			dfr.Filters.Add("DirectWave patch", new byte?[][] { new byte?[] { new byte?(68), new byte?(119), new byte?(80), new byte?(114) } }, new string[] { "*.dwp" });
			dfr.Capabilities.Add(typeof(VoicebankObjectModel), DataFormatCapabilities.All);
			return dfr;
		}
		protected override void LoadInternal(ref ObjectModel objectModel)
		{
			VoicebankObjectModel vom = (objectModel as VoicebankObjectModel);
			BinaryReader br = base.Stream.BinaryReader;
			string DwPr = br.ReadFixedLengthString(4);
			int n0 = br.ReadInt32();
			int n = br.ReadInt32();
			vom.Name = br.ReadNullTerminatedString(32);
			vom.InstallationPath = br.ReadNullTerminatedString(260);
			int n2 = br.ReadInt32();
			int n3 = br.ReadInt32();
			int n4 = br.ReadInt32();
			int sampleCount = br.ReadInt32();
			byte[] data = br.ReadBytes(144u);
			for (int i = 0; i < sampleCount; i++)
			{
				VoicebankSample sample = new VoicebankSample();
				sample.Name = br.ReadNullTerminatedString(32);
				sample.FileName = br.ReadNullTerminatedString(260);
				sample.Data = br.ReadBytes(1406u);
				vom.Samples.Add(sample);
			}
		}
		protected override void SaveInternal(ObjectModel objectModel)
		{
			VoicebankObjectModel vom = objectModel as VoicebankObjectModel;
			BinaryWriter bw = base.Stream.BinaryWriter;
			bw.WriteFixedLengthString("DwPr");
			int n0 = 0;
			bw.Write(n0);
			int n = 0;
			bw.Write(n);
			bw.WriteNullTerminatedString(vom.Name, 32);
			bw.WriteNullTerminatedString(vom.InstallationPath, 264);
			bw.Flush();
		}
	}
}