//
//  PSFColumnInfo.cs - internal structure representing metadata information for a column in a PSF database
//
//  Author:
//       Mike Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2019-2020 Mike Becker
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

namespace UniversalEditor.Plugins.Sony.DataFormats.Database
{
	/// <summary>
	/// Internal structure representing metadata information for a column in a PSF database.
	/// </summary>
	public struct PSFColumnInfo
	{
		public ushort columnNameOffset;
		public PSFColumnDataType columnDataType;
		public uint dataLength;
		public uint dataMaxLength;
		public uint valueOffset;

		public PSFColumnInfo(ushort columnNameOffset, PSFColumnDataType columnDataType, uint dataLength, uint dataMaxLength, uint valueOffset) : this()
		{
			this.columnNameOffset = columnNameOffset;
			this.columnDataType = columnDataType;
			this.dataLength = dataLength;
			this.dataMaxLength = dataMaxLength;
			this.valueOffset = valueOffset;
		}
	}
}
