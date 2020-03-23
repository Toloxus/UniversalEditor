﻿using System;
using System.Collections.Generic;
using System.Text;

namespace UniversalEditor.Compression.Modules.Deflate.Internal
{
	internal class Deflater
	{
		private FastEncoder encoder;
		public Deflater(bool doGZip)
		{
			this.encoder = new FastEncoder(doGZip);
		}
		public int Finish(byte[] output)
		{
			return this.encoder.Finish(output);
		}
		public int GetDeflateOutput(byte[] output)
		{
			return this.encoder.GetCompressedOutput(output);
		}
		public bool NeedsInput()
		{
			return this.encoder.NeedsInput();
		}
		public void SetInput(byte[] input, int startIndex, int count)
		{
			this.encoder.SetInput(input, startIndex, count);
		}
	}
}
