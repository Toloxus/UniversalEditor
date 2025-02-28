//
//  TopicLinkDisplay31Opcode.cs - internal enum indicating opcode for topic link display on WinHelp 3.1 systems
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

namespace UniversalEditor.DataFormats.Help.Compiled.WinHelp.Internal
{
	/// <summary>
	/// Internal enum indicating opcode for topic link display on WinHelp 3.1 systems.
	/// </summary>
	internal enum TopicLinkDisplay31Opcode
	{
		None = 0x00,
		/// <summary>
		/// long vfldNumber	  0 = {vfld}   n = {vfld n}
		/// </summary>
		VFldNumber = 0x20,
		/// <summary>
		/// short dtypeNumber   0 = {dtype}  n = {dtype n}
		/// </summary>
		DTypeNumber = 0x21,
		/// <summary>
		/// short FontNumber	  index into Descriptor array of internal |FONT file
		/// </summary>
		FontNumber = 0x80,
		/// <summary>
		/// No firstlineindent / spacingabove on next paragraph
		/// </summary>
		LineBreak = 0x81,
		/// <summary>
		/// Next paragraph has same Paragraphinfo as this one
		/// </summary>
		EndOfParagraph = 0x82,
		/// <summary>
		/// Jump to next tab stop
		/// </summary>
		Tab = 0x83,
		PictureCenter = 0x86,
		PictureLeft = 0x87,
		PictureRight = 0x88,
		HotspotEnd = 0x89,

		NonBreakingSpace = 0x8B,
		NonBreakingHyphen = 0x8C,

		/// <summary>
		/// short Length; char MacroString[Length - 3];
		/// </summary>
		Macro = 0xC8,
		/// <summary>
		/// short Length; char MacroString[Length - 3];
		/// </summary>
		MacroWithoutFontChange = 0xCC,

		/// <summary>
		/// Popup jump
		/// TOPICOFFSET TopicOffset
		/// </summary>
		PopupJump0xE0 = 0xE0,
		/// <summary>
		/// Topic jump
		/// TOPICOFFSET TopicOffset
		/// </summary>
		TopicJump0xE1 = 0xE1,
		/// <summary>
		/// Popup jump
		/// TOPICOFFSET TopicOffset
		/// </summary>
		PopupJump0xE2 = 0xE2,
		/// <summary>
		/// Topic jump
		/// TOPICOFFSET TopicOffset
		/// </summary>
		PopupJump0xE3 = 0xE3,


		/// <summary>
		/// Popup jump without font change
		/// TOPICOFFSET TopicOffset
		/// </summary>
		PopupJumpWithoutFontChange = 0xE6,
		/// <summary>
		/// Topic jump without font change
		/// TOPICOFFSET TopicOffset
		/// </summary>
		TopicJumpWithoutFontChange = 0xE7,


		/// <summary>
		/// Popup jump into external file
		/// short SizeOfFollowingStruct
		/// struct
		/// {
		///		unsigned char Type		0, 1, 4 or 6
		///		TOPICOFFSET TopicOffset
		///		unsigned char WindowNumber	only if Type = 1
		///		STRINGZ NameOfExternalFile	only if Type = 4 or 6
		///		STRINGZ WindowName		only if Type = 6
		///	}
		/// </summary>
		PopupJumpIntoExternalFile = 0xEA,
		/// <summary>
		/// Popup jump into external file without font change
		/// short SizeOfFollowingStruct
		/// struct
		/// {
		///		unsigned char Type		0, 1, 4 or 6
		///		TOPICOFFSET TopicOffset
		///		unsigned char WindowNumber	only if Type = 1
		///		STRINGZ NameOfExternalFile	only if Type = 4 or 6
		///		STRINGZ WindowName		only if Type = 6
		///	}
		/// </summary>
		PopupJumpIntoExternalFileWithoutFontChange = 0xEB,
		/// <summary>
		/// Topic jump into external file / secondary window
		/// short SizeOfFollowingStruct
		/// struct
		/// {
		///		unsigned char Type		0, 1, 4 or 6
		///		TOPICOFFSET TopicOffset
		///		unsigned char WindowNumber	only if Type = 1
		///		STRINGZ NameOfExternalFile	only if Type = 4 or 6
		///		STRINGZ WindowName		only if Type = 6
		///	}
		/// </summary>
		TopicJumpIntoExternalFileSecondaryWindow = 0xEE,
		/// <summary>
		/// Topic jump into external file / secondary window without font change
		/// short SizeOfFollowingStruct
		/// struct
		/// {
		///		unsigned char Type		0, 1, 4 or 6
		///		TOPICOFFSET TopicOffset
		///		unsigned char WindowNumber	only if Type = 1
		///		STRINGZ NameOfExternalFile	only if Type = 4 or 6
		///		STRINGZ WindowName		only if Type = 6
		///	}
		/// </summary>
		TopicJumpIntoExternalFileSecondaryWindowWithoutFontChange = 0xEF,

		/// <summary>
		/// End of character formatting. Proceed with next Paragraphinfo if RecordType is 0x23, else you are done.
		/// </summary>
		EndOfCharacterFormatting = 0xFF
	}
}
