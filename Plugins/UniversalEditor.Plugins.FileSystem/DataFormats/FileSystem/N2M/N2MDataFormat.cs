//
//  N2MDataFormat.cs - provides a DataFormat for manipulating archives in N2M format
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

using System;

using UniversalEditor.ObjectModels.FileSystem;

namespace UniversalEditor.DataFormats.FileSystem.N2M
{
	/// <summary>
	/// Provides a <see cref="DataFormat" /> for manipulating archives in N2M format.
	/// </summary>
	public class N2MDataFormat : DataFormat
	{
		private static DataFormatReference _dfr;
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
			IO.Reader br = base.Accessor.Reader;

			string unknown = br.ReadFixedLengthString(10);
			int TSIZE = br.ReadInt32();
			do
			{
				int ZSIZE = br.ReadInt32();
				int SIZE = TSIZE;
				long pos = br.Accessor.Position;
				if (TSIZE >= 0x800000) SIZE = 0x800000;
				byte[] data = br.ReadBytes(ZSIZE);
				TSIZE -= SIZE;
				pos += ZSIZE;
				br.Accessor.Position = pos;
			}
			while (TSIZE != 0);
		}

		protected override void SaveInternal(ObjectModel objectModel)
		{
			throw new NotImplementedException();
		}
	}
}
