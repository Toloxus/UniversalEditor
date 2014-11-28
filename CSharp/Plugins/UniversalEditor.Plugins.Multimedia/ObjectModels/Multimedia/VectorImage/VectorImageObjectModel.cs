﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniversalEditor.ObjectModels.Multimedia.VectorImage
{
	public class VectorImageObjectModel : ObjectModel
	{
		private static ObjectModelReference _omr = null;
		public override ObjectModelReference MakeReference()
		{
			if (_omr == null)
			{
				_omr = base.MakeReference();
				_omr.Title = "Vector image";
				_omr.Path = new string[] { "Multimedia", "Picture" };
			}
			return _omr;
		}

		public override void Clear()
		{
		}

		public override void CopyTo(ObjectModel where)
		{
		}
	}
}
