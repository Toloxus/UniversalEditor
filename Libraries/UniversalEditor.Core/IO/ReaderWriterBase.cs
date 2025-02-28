//
//  ReaderWriterBase.cs - common methods for implementing Reader and Writer
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

namespace UniversalEditor.IO
{
	public abstract class ReaderWriterBase
	{
		private Endianness mvarEndianness = Endianness.LittleEndian;
		public Endianness Endianness { get { return mvarEndianness; } set { mvarEndianness = value; } }

		public void SwapEndianness()
		{
			if (mvarEndianness == Endianness.LittleEndian)
			{
				mvarEndianness = Endianness.BigEndian;
			}
			else
			{
				mvarEndianness = Endianness.LittleEndian;
			}
		}

		private bool mvarReverse = false;
		public bool Reverse { get { return mvarReverse; } }

		private Accessor mvarAccessor = null;
		public Accessor Accessor { get { return mvarAccessor; } }

		public Transformation.TransformationCollection Transformations { get; } = new Transformation.TransformationCollection();

		protected NewLineSequence _ActualNewLineSequenceForAutomatic = NewLineSequence.Default;
		public NewLineSequence NewLineSequence { get; set; } = NewLineSequence.Automatic;

		private string GetNewLineSequence(NewLineSequence newLineSequence)
		{
			if (newLineSequence == NewLineSequence.Automatic)
			{
				return GetNewLineSequence(_ActualNewLineSequenceForAutomatic);
			}

			string newline = System.Environment.NewLine;
			switch (newLineSequence)
			{
				case IO.NewLineSequence.CarriageReturn:
				{
					newline = "\r";
					break;
				}
				case IO.NewLineSequence.LineFeed:
				{
					newline = "\n";
					break;
				}
				case IO.NewLineSequence.CarriageReturnLineFeed:
				{
					newline = "\r\n";
					break;
				}
				case IO.NewLineSequence.LineFeedCarriageReturn:
				{
					newline = "\n\r";
					break;
				}
			}
			return newline;
		}
		public string GetNewLineSequence()
		{
			return GetNewLineSequence(NewLineSequence);
		}

		public ReaderWriterBase(Accessor accessor)
		{
			this.mvarAccessor = accessor;
			this.mvarEndianness = Endianness.LittleEndian;
			this.mvarReverse = false;
		}

		public long CalculateAlignment(long currentPosition, long alignTo, long extraPadding = 0)
		{
			long paddingCount = ((alignTo - (currentPosition % alignTo)) % alignTo);
			paddingCount += extraPadding;
			return paddingCount;
		}
	}
}
