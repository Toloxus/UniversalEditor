using System;
namespace UniversalEditor.DataFormats.Multimedia.Audio.Waveform.MonkeyAudio
{
	public class APLDataFormat : DataFormat
	{
		protected override DataFormatReference MakeReferenceInternal()
		{
			DataFormatReference dfr = base.MakeReferenceInternal();
			return dfr;
		}

		protected override void LoadInternal(ref ObjectModel objectModel)
		{
			throw new NotImplementedException();
		}
		protected override void SaveInternal(ObjectModel objectModel)
		{
			throw new NotImplementedException();
		}
	}
}