﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniversalEditor.DataFormats.FileSystem.Apple.HFS.Internal.CatalogRecords
{
	internal class HFSCatalogFileRecord : HFSCatalogRecord
	{
		/// <summary>
		/// File flags.
		/// </summary>
		public HFSFileFlags flags;
		/// <summary>
		/// The file type. This field should always contain 0.
		/// </summary>
		public sbyte fileType;
		/// <summary>
		/// The file's Finder information.
		/// </summary>
		public HFSFInfo finderUserInformation;
		/// <summary>
		/// The file ID.
		/// </summary>
		public int fileID;
		/// <summary>
		/// The first allocation block of the data fork.
		/// </summary>
		public short dataForkFirstAllocationBlock;
		/// <summary>
		/// The logical EOF of the data fork.
		/// </summary>
		public int dataForkLogicalEOF;
		/// <summary>
		/// The physical EOF of the data fork.
		/// </summary>
		public int dataForkPhysicalEOF;
		/// <summary>
		/// The first allocation block of the resource fork.
		/// </summary>
		public short resourceForkFirstAllocationBlock;
		/// <summary>
		/// The logical EOF of the resource fork.
		/// </summary>
		public int resourceForkLogicalEOF;
		/// <summary>
		/// The physical EOF of the resource fork.
		/// </summary>
		public int resourceForkPhysicalEOF;
		/// <summary>
		/// The date and time this file was created.
		/// </summary>
		public int creationTimestamp;
		/// <summary>
		/// The date and time this file was last modified.
		/// </summary>
		public int modificationTimestamp;
		/// <summary>
		/// HFS+ only
		/// </summary>
		public int attributeModificationTimestamp;
		/// <summary>
		/// HFS+ only
		/// </summary>
		public int lastAccessTimestamp;
		/// <summary>
		/// The date and time this file was last backed up.
		/// </summary>
		public int lastBackupTimestamp;
		/// <summary>
		/// HFS+ only
		/// </summary>
		public HFSPlusPermissions permissions;
		/// <summary>
		/// Additional information used by the Finder.
		/// </summary>
		public HFSFXInfo finderAdditionalInformation;
		/// <summary>
		/// HFS+ only
		/// </summary>
		public HFSPlusTextEncoding textEncoding;
		/// <summary>
		/// The file clump size.
		/// </summary>
		public short fileClumpSize;
		/// <summary>
		/// The first extent record of the file's data fork.
		/// </summary>
		public HFSExtentDescriptor[] firstDataForkExtentRecord;
		/// <summary>
		/// The first extent record of the file's resource fork.
		/// </summary>
		public HFSExtentDescriptor[] firstResourceForkExtentRecord;
		/// <summary>
		/// Reserved.
		/// </summary>
		public int reserved2;
		
		/// <summary>
		/// HFS+ only
		/// </summary>
		public HFSPlusForkData dataFork;
		/// <summary>
		/// HFS+ only
		/// </summary>
		public HFSPlusForkData resourceFork;
	}
}