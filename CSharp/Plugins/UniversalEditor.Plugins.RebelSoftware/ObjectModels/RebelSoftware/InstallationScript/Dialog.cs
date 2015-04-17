﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniversalEditor.Plugins.RebelSoftware.ObjectModels.InstallationScript
{
	public abstract class Dialog : ICloneable
	{
		public class DialogCollection
			: System.Collections.ObjectModel.Collection<Dialog>
		{

		}

		public abstract object Clone();
	}
}
