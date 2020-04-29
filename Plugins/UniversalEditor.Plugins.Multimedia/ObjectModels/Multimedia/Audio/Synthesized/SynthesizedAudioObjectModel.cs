//
//  SynthesizedAudioObjectModel.cs - provides an ObjectModel for manipulating synthesized audio files (e.g. MIDI, VSQ, etc.)
//
//  Author:
//       Michael Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2011-2020 Mike Becker's Software
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using UniversalEditor.ObjectModels.Multimedia.Audio.Voicebank;

namespace UniversalEditor.ObjectModels.Multimedia.Audio.Synthesized
{
	/// <summary>
	/// Provides an <see cref="ObjectModel" /> for manipulating synthesized audio files (e.g. MIDI, VSQ, etc.).
	/// </summary>
	public class SynthesizedAudioObjectModel : AudioObjectModel
	{
		private static ObjectModelReference _omr = null;
		protected override ObjectModelReference MakeReferenceInternal()
		{
			if (_omr == null)
			{
				_omr = base.MakeReferenceInternal();
				_omr.Title = "Synthesized audio sequence";
				_omr.Path = new string[] { "Multimedia", "Audio", "Synthesized Audio" };
			}
			return _omr;
		}

		public SynthesizedAudioObjectModel()
		{
			// HACK: since we don't have a good way to specify defaults for blank templates.
			// FIXME: THIS MAY NOT GET CLEARED WHEN OPENING A NEW FILE SINCE WE ASSUME THAT A NEW OBJECT MODEL IS EMPTY!!!
			Tracks.Add(new SynthesizedAudioTrack());
		}

		public short ChannelCount { get; set; } = 2;

		public string Name { get; set; } = string.Empty;

		public double Tempo { get; set; } = 120.0;

		public SynthesizedAudioTrack.SynthesizedAudioTrackCollection Tracks { get; } = new SynthesizedAudioTrack.SynthesizedAudioTrackCollection();
		public VoicebankObjectModel.VoicebankObjectModelCollection Voices { get; } = new VoicebankObjectModel.VoicebankObjectModelCollection();

		public override void CopyTo(ObjectModel destination)
		{
			SynthesizedAudioObjectModel clone = destination as SynthesizedAudioObjectModel;
			clone.Name = (this.Name.Clone() as string);
			clone.Tempo = this.Tempo;
			foreach (SynthesizedAudioTrack track in this.Tracks)
			{
				clone.Tracks.Add(track.Clone() as SynthesizedAudioTrack);
			}
		}
		public override void Clear()
		{
			this.Name = string.Empty;
			this.Tempo = 120.0;
			this.Tracks.Clear();
		}
	}
}