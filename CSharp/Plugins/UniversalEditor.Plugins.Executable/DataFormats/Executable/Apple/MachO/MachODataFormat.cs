﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniversalEditor.IO;
using UniversalEditor.ObjectModels.Executable;

namespace UniversalEditor.DataFormats.Executable.Apple.MachO
{
	public class MachODataFormat : DataFormat
	{
		private static DataFormatReference _dfr = null;
		protected override DataFormatReference MakeReferenceInternal()
		{
			if (_dfr == null)
			{
				_dfr = base.MakeReferenceInternal();
				_dfr.Capabilities.Add(typeof(ExecutableObjectModel), DataFormatCapabilities.All);
				_dfr.Sources.Add("https://developer.apple.com/library/mac/documentation/DeveloperTools/Conceptual/MachORuntime/index.html#//apple_ref/c/tag/mach_header");
			}
			return _dfr;
		}

		private MachOCpuType mvarCpuType = MachOCpuType.X86;
		/// <summary>
		/// The architecture you intend to use the file on.
		/// </summary>
		public MachOCpuType CpuType { get { return mvarCpuType; } set { mvarCpuType = value; } }

		private MachOCpuSubType mvarCpuSubType = MachOCpuSubType.PowerPC_All;
		public MachOCpuSubType CpuSubType { get { return mvarCpuSubType; } set { mvarCpuSubType = value; } }

		private MachOFileType mvarFileType = MachOFileType.Executable;
		public MachOFileType FileType { get { return mvarFileType; } set { mvarFileType = value; } }

		private MachOFlags mvarFlags = MachOFlags.None;
		public MachOFlags Flags { get { return mvarFlags; } set { mvarFlags = value; } }

		protected override void LoadInternal(ref ObjectModel objectModel)
		{
			ExecutableObjectModel exe = (objectModel as ExecutableObjectModel);
			if (exe == null) throw new ObjectModelNotSupportedException();

			Reader reader = base.Accessor.Reader;
			
			// An integer containing a value identifying this file as a 32-bit Mach-O file. Use the
			// constant MH_MAGIC if the file is intended for use on a CPU with the same endianness
			// as the computer on which the compiler is running. The constant MH_CIGAM can be used
			// when the byte ordering scheme of the target machine is the reverse of the host CPU.
			MachOMagic magic = (MachOMagic)reader.ReadUInt32();

			// set up endianness
			switch (magic)
			{
				case MachOMagic.MachOBigEndian:
				{
					reader.Endianness = Endianness.BigEndian;
					break;
				}
				case MachOMagic.MachOLittleEndian:
				{
					reader.Endianness = Endianness.LittleEndian;
					break;
				}
			}

			// parse the format
			switch (magic)
			{
				case MachOMagic.MachOBigEndian:
				case MachOMagic.MachOLittleEndian:
				{
					mvarCpuType = (MachOCpuType)reader.ReadInt32();
					mvarCpuSubType = (MachOCpuSubType)reader.ReadInt32();
					mvarFileType = (MachOFileType)reader.ReadInt32();

					uint loadCommandCount = reader.ReadUInt32();
					uint loadCommandAreaSize = reader.ReadUInt32();

					mvarFlags = (MachOFlags)reader.ReadInt32();
					break;
				}
				default:
				{
					throw new InvalidDataFormatException("The executable format 0x" + ((uint)magic).ToString("X") + " is not supported");
				}
			}
		}

		protected override void SaveInternal(ObjectModel objectModel)
		{
			ExecutableObjectModel exe = (objectModel as ExecutableObjectModel);
			if (exe == null) throw new ObjectModelNotSupportedException();

			Writer writer = base.Accessor.Writer;

			MachOMagic magic = MachOMagic.MachOBigEndian;

			writer.WriteUInt32((uint)magic);

			switch (magic)
			{
				case MachOMagic.MachOBigEndian:
				{
					writer.Endianness = Endianness.BigEndian;
					break;
				}
				case MachOMagic.MachOLittleEndian:
				{
					writer.Endianness = Endianness.LittleEndian;
					break;
				}
			}


			switch (magic)
			{
				case MachOMagic.MachOBigEndian:
				case MachOMagic.MachOLittleEndian:
				{
					writer.WriteInt32((int)mvarCpuType);
					writer.WriteInt32((int)mvarCpuSubType);
					writer.WriteInt32((int)mvarFileType);

					uint loadCommandCount = 0;
					uint loadCommandSize = 0;
					writer.WriteUInt32(loadCommandCount);
					writer.WriteUInt32(loadCommandSize);

					writer.WriteInt32((int)mvarFlags);
					break;
				}
				default:
				{
					throw new InvalidDataFormatException("The executable format 0x" + ((uint)magic).ToString("X") + " is not supported");
				}
			}
		}
	}
}