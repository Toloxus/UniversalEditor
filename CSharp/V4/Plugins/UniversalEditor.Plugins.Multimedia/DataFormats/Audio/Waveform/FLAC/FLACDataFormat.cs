using System;
using UniversalEditor.IO;
using UniversalEditor.Plugins.Multimedia.DataFormats.Audio.Waveform.FLAC.Internal;
using UniversalEditor.Plugins.Multimedia.ObjectModels.Audio.Waveform;
namespace UniversalEditor.Plugins.Multimedia.DataFormats.Audio.Waveform.FLAC
{
	public class FLACDataFormat : DataFormat
	{
		public override DataFormatReference MakeReference()
		{
			DataFormatReference dfr = base.MakeReference();
			dfr.Filters.Add("Free Lossless Audio Codec", new byte?[][] { new byte?[] { new byte?(102), new byte?(76), new byte?(97), new byte?(67) } }, new string[] { "*.flac" });
			dfr.Capabilities.Add(typeof(WaveformAudioObjectModel), DataFormatCapabilities.All);
			return dfr;
		}

		private FLACMetadataBlock.FLACMetadataBlockCollection mvarMetadataBlocks = new FLACMetadataBlock.FLACMetadataBlockCollection();
		public FLACMetadataBlock.FLACMetadataBlockCollection MetadataBlocks { get { return this.mvarMetadataBlocks; } }

		protected override void LoadInternal(ref ObjectModel objectModel)
		{
			throw new NotImplementedException();
		}
		private void SaveMetadataBlockHeader(BinaryWriter bw, FLACMetadataBlockHeader header)
		{
			byte flagAndType = 0;
			if (header.IsLastMetadataBlock)
			{
				flagAndType |= 1;
			}
			flagAndType |= (byte)header.BlockType;
			bw.Write(flagAndType);
			bw.WriteInt24(header.ContentLength);
		}
		private void SaveMetadataBlockStreamInfo(BinaryWriter bw, FLACMetadataBlockStreamInfo block)
		{
			this.SaveMetadataBlockHeader(bw, new FLACMetadataBlockHeader
			{
				IsLastMetadataBlock = this.mvarMetadataBlocks.Count == 0, 
				BlockType = FLACMetadataBlockType.StreamInfo, 
				ContentLength = 0
			});
		}
		protected override void SaveInternal(ObjectModel objectModel)
		{
			BinaryWriter bw = base.Stream.BinaryWriter;
			bw.WriteFixedLengthString("fLaC");
			FLACMetadataBlockStreamInfo streamInfo = new FLACMetadataBlockStreamInfo();
			this.SaveMetadataBlockStreamInfo(bw, streamInfo);
			bw.Flush();
		}
	}
}