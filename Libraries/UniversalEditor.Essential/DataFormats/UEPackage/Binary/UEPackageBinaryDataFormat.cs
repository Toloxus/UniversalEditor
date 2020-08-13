﻿//
//  UEPackageBinaryDataFormat.cs
//
//  Author:
//       Michael Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2020 Mike Becker's Software
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
using System.Collections.Generic;
using UniversalEditor.DataFormats.Package.OpenDocument;
using UniversalEditor.ObjectModels.Package;
using UniversalEditor.ObjectModels.UEPackage;

namespace UniversalEditor.DataFormats.UEPackage.Binary
{
	public class UEPackageBinaryDataFormat : OpenDocumentDataFormat
	{
		private static DataFormatReference _dfr = null;
		protected override DataFormatReference MakeReferenceInternal()
		{
			if (_dfr == null)
			{
				_dfr = base.MakeReferenceInternal();
				_dfr.Capabilities.Clear();
				_dfr.Capabilities.Add(typeof(UEPackageObjectModel), DataFormatCapabilities.All);
			}
			return _dfr;
		}

		protected override void BeforeLoadInternal(Stack<ObjectModel> objectModels)
		{
			base.BeforeLoadInternal(objectModels);
			objectModels.Push(new PackageObjectModel());
		}
		protected override void AfterLoadInternal(Stack<ObjectModel> objectModels)
		{
			base.AfterLoadInternal(objectModels);

			PackageObjectModel package = (objectModels.Pop() as PackageObjectModel);
			UEPackageObjectModel ue = (objectModels.Pop() as UEPackageObjectModel);
		}

		protected override void BeforeSaveInternal(Stack<ObjectModel> objectModels)
		{
			UEPackageObjectModel ue = (objectModels.Pop() as UEPackageObjectModel);
			if (ue == null) throw new ObjectModelNotSupportedException();

			PackageObjectModel package = new PackageObjectModel();

			objectModels.Push(package);

			base.BeforeSaveInternal(objectModels);
		}
	}
}
