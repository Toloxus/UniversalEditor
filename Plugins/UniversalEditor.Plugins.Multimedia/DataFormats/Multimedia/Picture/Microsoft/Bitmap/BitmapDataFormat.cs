﻿//
//  BitmapDataFormat.cs - provides a DataFormat for manipulating images in Windows and OS/2 bitmap format
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

using System.Collections.Generic;
using UniversalEditor.ObjectModels.Multimedia.Picture;

using MBS.Framework.Drawing;

namespace UniversalEditor.DataFormats.Multimedia.Picture.Microsoft.Bitmap
{
	/// <summary>
	/// Provides a <see cref="DataFormat" /> for manipulating images in Windows and OS/2 bitmap format.
	/// Supports bitmap formats that are not natively supported by Windows.
	/// </summary>
	public class BitmapDataFormat : DataFormat
	{
		public const int BITMAP_HEADER_SIZE = (2 + 6 * 4 + 2 * 2 + 6 * 4);

		public const int BITMAP_PALETTE_ENTRY_SIZE_24BIT = 3;
		public const int BITMAP_PALETTE_ENTRY_SIZE_32BIT = 4;

		/// <summary>
		/// The number of bits-per-pixel. The biBitCount member of the BITMAPINFOHEADER structure determines the
		/// number of bits that define each pixel and the maximum number of colors in the bitmap.
		/// </summary>
		public BitmapBitsPerPixel PixelDepth { get; set; } = BitmapBitsPerPixel.DeepColor;

		protected override DataFormatReference MakeReferenceInternal()
		{
			DataFormatReference dfr = base.MakeReferenceInternal();
			dfr.Capabilities.Add(typeof(PictureObjectModel), DataFormatCapabilities.All);
			dfr.ExportOptions.Add(new CustomOptionChoice(nameof(PixelDepth), "Pixel _depth", true, new CustomOptionFieldChoice[]
			{
				new CustomOptionFieldChoice("16 colors", BitmapBitsPerPixel.Color16),
				new CustomOptionFieldChoice("256 colors", BitmapBitsPerPixel.Color256),
				new CustomOptionFieldChoice("Deep color (-bit R8G8B8)", BitmapBitsPerPixel.DeepColor),
				new CustomOptionFieldChoice("High color (16-bit R5G5B5)", BitmapBitsPerPixel.HighColor),
				new CustomOptionFieldChoice("Monochrome", BitmapBitsPerPixel.Monochrome),
				new CustomOptionFieldChoice("True color (24-bit R8G8B8)", BitmapBitsPerPixel.TrueColor)
			}));
			return dfr;
		}

		/// <summary>
		/// The horizontal resolution, in pixels-per-meter, of the target device for the bitmap. An application
		/// can use this value to select a bitmap from a resource group that best matches the characteristics of
		/// the current device.
		/// </summary>
		/// <value>The horizontal resolution, in pixels-per-meter, of the target device for the bitmap.</value>
		public int HorizontalResolution { get; set; } = 0;
		/// <summary>
		/// The vertical resolution, in pixels-per-meter, of the target device for the bitmap. An application
		/// can use this value to select a bitmap from a resource group that best matches the characteristics of
		/// the current device.
		/// </summary>
		/// <value>The vertical resolution, in pixels-per-meter, of the target device for the bitmap.</value>
		public int VerticalResolution { get; set; } = 0;

		protected override void LoadInternal(ref ObjectModel objectModel)
		{
			PictureObjectModel pic = (objectModel as PictureObjectModel);
			if (pic == null) throw new ObjectModelNotSupportedException();

			IO.Reader br = base.Accessor.Reader;

			string signature = br.ReadFixedLengthString(2);
			switch (signature)
			{
				case "BM":
				case "BA":
				case "CI":
				case "CP":
				case "IC":
				case "PT":
				{
					break;
				}
				default:
				{
					throw new InvalidDataFormatException();
				}
			}

			int fileSize = br.ReadInt32();                  // 4522
			short reserved1 = br.ReadInt16();
			short reserved2 = br.ReadInt16();
			int offset = br.ReadInt32();                    // 122

			BitmapInfoHeader header = BitmapInfoHeader.Load(br);
			HorizontalResolution = header.PelsPerMeterX;
			VerticalResolution = header.PelsPerMeterY;
			PixelDepth = header.PixelDepth;                 // TrueColor

			pic.Width = header.Width;
			pic.Height = header.Height;

			byte bitRead = 0;
			int bitsRead = 0;

			List<Color> palette = new List<Color>();

			// there is a palette
			// To read the palette, we can simply read in a block of bytes
			// since our array elements are guaranteed to be contiguous and in row-major order.
			int paletteSize = offset - header.HeaderSize;                                           // 122 - 108 = 14
			int bitmapPaletteEntrySize = 0;

			switch (PixelDepth)
			{
				case BitmapBitsPerPixel.Color256:
				{
					bitmapPaletteEntrySize = 4;
					break;
				}
				case BitmapBitsPerPixel.TrueColor:
				{
					bitmapPaletteEntrySize = BITMAP_PALETTE_ENTRY_SIZE_24BIT;
					break;
				}
				case BitmapBitsPerPixel.DeepColor:
				{
					bitmapPaletteEntrySize = BITMAP_PALETTE_ENTRY_SIZE_32BIT;
					break;
				}
			}
			int numPaletteEntries = paletteSize / bitmapPaletteEntrySize;                        // 14 / 4 = 3

			if (header.UsedColorIndexCount > 0)
				numPaletteEntries = header.UsedColorIndexCount; // use this, it's more accurate if available

			for (int i = 0; i < numPaletteEntries; i++)
			{
				byte b = 0, g = 0, r = 0, a = 255;
				switch (PixelDepth)
				{
					case BitmapBitsPerPixel.TrueColor:
					{
						b = br.ReadByte();
						g = br.ReadByte();
						r = br.ReadByte();
						break;
					}
					case BitmapBitsPerPixel.Color256:
					{
						b = br.ReadByte();
						g = br.ReadByte();
						r = br.ReadByte();
						a = br.ReadByte();
						a = 255;
						break;
					}
					case BitmapBitsPerPixel.DeepColor:
					{
						b = br.ReadByte();
						g = br.ReadByte();
						r = br.ReadByte();
						a = br.ReadByte();
						break;
					}
				}
				palette.Add(Color.FromRGBAByte(r, g, b, a));
			}

			if (PixelDepth == BitmapBitsPerPixel.TrueColor)
			{
			}
			br.Accessor.Position = offset;

			// starts from bottom-left corner, goes BGR
			for (int y = header.Height - 1; y >= 0; y--)
			{
				for (int x = 0; x < header.Width; x++)
				{
					byte r = 0, g = 0, b = 0, a = 255;

					switch (PixelDepth)
					{
						case BitmapBitsPerPixel.Monochrome:
						{
							// TODO: Figure out how to read this bitmap
							if (bitsRead == 0) bitRead = br.ReadByte();

							b = (byte)(bitRead << (bitsRead));
							g = (byte)(bitRead << (bitsRead + 1));
							r = (byte)(bitRead << (bitsRead + 2));

							bitsRead += 3;
							if (bitsRead == 8)
							{
								bitsRead = 0;
							}
							break;
						}
						case BitmapBitsPerPixel.Color16:
						{
							break;
						}
						case BitmapBitsPerPixel.Color256:
						{
							byte rgb = br.ReadByte();
							r = (byte)(palette[rgb].R * 255);
							g = (byte)(palette[rgb].G * 255);
							b = (byte)(palette[rgb].B * 255);
							break;
						}
						case BitmapBitsPerPixel.HighColor:
						{
							// X1R5G5B5
							if (header.Compression == BitmapCompression.Bitfields)
							{
								// R5G6B5
								short value = br.ReadInt16();
								b = (byte)(8 * value.GetBits(0, 5));
								g = (byte)(8 * value.GetBits(6, 6));
								r = (byte)(8 * value.GetBits(11, 5));
							}
							else
							{
								// X1R5G5B5
								short value = br.ReadInt16();
								int x1 = value.GetBits(16, 1);
								b = (byte)(8 * value.GetBits(0, 5));
								g = (byte)(8 * value.GetBits(5, 5));
								r = (byte)(8 * value.GetBits(10, 5));
							}
							break;
						}
						case BitmapBitsPerPixel.TrueColor:
						{
							// The 24-bit pixel (24bpp) format supports 16,777,216 distinct colors and stores 1 pixel value per 3 bytes. Each pixel value defines the red, green and blue samples of the pixel (8.8.8.0.0 in RGBAX notation). Specifically, in the order: blue, green and red (8 bits per each sample).
							b = br.ReadByte(); // (2,2) B 204
							g = br.ReadByte(); // (2,2) G 72
							r = br.ReadByte(); // (2,2) R 63
							break;
						}
						case BitmapBitsPerPixel.DeepColor:
						{
							if (header.Compression == BitmapCompression.Bitfields)
							{
								// this is really black magic going on here. these aren't bitfields at all..

								// a = br.ReadByte(); // (2,2) R 63
								b = br.ReadByte(); // (2,2) B 204
								g = br.ReadByte(); // (2,2) B 204
								r = br.ReadByte(); // (2,2) G 72
								a = br.ReadByte();
								a = 255;
							}
							else
							{
								b = br.ReadByte(); // (2,2) R 63
								g = br.ReadByte(); // (2,2) B 204
								r = br.ReadByte(); // (2,2) B 204
								a = br.ReadByte(); // (2,2) G 72
								a = 255;
							}
							break;
						}
					}

					Color color = Color.FromRGBAByte(r, g, b, a);
					pic.SetPixel(color, x, y);
				}
				br.Align(4);
			}
		}
		protected override void SaveInternal(ObjectModel objectModel)
		{
			PictureObjectModel pic = (objectModel as PictureObjectModel);
			IO.Writer bw = base.Accessor.Writer;

			string signature = "BM";
			bw.WriteFixedLengthString(signature);

			int bpp = 4;
			switch (PixelDepth)
			{
				case BitmapBitsPerPixel.TrueColor:
				{
					bpp = 3;
					break;
				}
			}

			int imageSize = (pic.Width * pic.Height * bpp) + pic.Height;
			int fileSize = 54 + imageSize;
			bw.WriteInt32(fileSize);

			short reserved1 = 0;
			short reserved2 = 0;
			bw.WriteInt16(reserved1);
			bw.WriteInt16(reserved2);

			int offset = 54;
			bw.WriteInt32(offset);

			BitmapInfoHeader header = new BitmapInfoHeader();
			header.Width = pic.Width;
			header.Height = pic.Height;
			header.Planes = 1;
			header.PixelDepth = PixelDepth;
			header.Compression = BitmapCompression.None;
			header.ImageSize = imageSize;
			header.PelsPerMeterX = HorizontalResolution;
			header.PelsPerMeterY = VerticalResolution;
			header.UsedColorIndexCount = 0;
			header.RequiredColorIndexCount = 0;
			BitmapInfoHeader.Save(bw, header);

			for (int y = pic.Height - 1; y >= 0; y--)
			{
				for (int x = 0; x < pic.Width; x++)
				{
					Color color = pic.GetPixel(x, y);
					byte r = (byte)(color.R * 255), g = (byte)(color.G * 255), b = (byte)(color.B * 255), a = (byte)(color.A * 255);

					/*
					if (bitsPerPixel == 1)
					{
						// TODO: Figure out how to read this bitmap
						if (bitsRead == 0) bitRead = br.ReadByte();

						b = (byte)(bitRead << (bitsRead));
						g = (byte)(bitRead << (bitsRead + 1));
						r = (byte)(bitRead << (bitsRead + 2));

						bitsRead += 3;
						if (bitsRead == 8)
						{
							bitsRead = 0;
						}
					}
					else */

					switch (PixelDepth)
					{
						case BitmapBitsPerPixel.TrueColor:
						{
							bw.WriteByte(b);
							bw.WriteByte(g);
							bw.WriteByte(r);
							break;
						}
						case BitmapBitsPerPixel.DeepColor:
						{
							bw.WriteByte(b);
							bw.WriteByte(g);
							bw.WriteByte(r);
							bw.WriteByte(a);
							break;
						}
					}
				}
			}
		}
	}
}
