using System;
using UniversalEditor.IO;
using UniversalEditor.Plugins.Multimedia.ObjectModels.Audio.Waveform;
using UniversalEditor.Plugins.Multimedia.ObjectModels.Video;
namespace UniversalEditor.Plugins.Multimedia.DataFormats.Video.ROQ
{
	public class ROQDataFormat : DataFormat
	{
		public override DataFormatReference MakeReference()
		{
			DataFormatReference dfr = base.MakeReference();
			dfr.Filters.Add("id software RoQ video", new byte?[][] { new byte?[] { new byte?(132), new byte?(16), new byte?(255), new byte?(255), new byte?(255), new byte?(255), new byte?(30), new byte?(0) } }, new string[] { "*.roq" });
			dfr.Capabilities.Add(typeof(VideoObjectModel), DataFormatCapabilities.All);
			dfr.Sources.Add("http://multimedia.cx/mirror/idroq.txt");
			return dfr;
		}

		protected override void LoadInternal(ref ObjectModel objectModel)
		{
			BinaryReader br = base.Stream.BinaryReader;
			byte[] signature = br.ReadBytes(8u);
			VideoObjectModel vom = objectModel as VideoObjectModel;
			WaveformAudioObjectModel wave = new WaveformAudioObjectModel();
			AudioTrack audio = new AudioTrack();
			VideoTrack video = new VideoTrack();
			video.FrameRate = 30;
			wave.Header.BitsPerSample = 16;
			wave.Header.SampleRate = 22050;
			while (!br.EndOfStream)
			{
				ROQChunk chunk = new ROQChunk();
				chunk.ID = br.ReadInt16();
				int size = br.ReadInt32();
				chunk.Argument = br.ReadInt16();
				chunk.Data = br.ReadBytes(size);
				short iD = chunk.ID;
				switch (iD)
				{
					case 4097:
					{
						BinaryReader brch = new BinaryReader(chunk.Data);
						video.Width = (int)brch.ReadInt16();
						video.Height = (int)brch.ReadInt16();
						video.BlockDimension = (int)brch.ReadInt16();
						video.SubBlockDimension = (int)brch.ReadInt16();
						break;
					}
					case 4098:
					{
						break;
					}
					default:
					{
						if (iD != 4113)
						{
							switch (iD)
							{
							}
						}
						break;
					}
				}
			}
		}
		protected override void SaveInternal(ObjectModel objectModel)
		{
			throw new NotImplementedException();
		}
	}
}