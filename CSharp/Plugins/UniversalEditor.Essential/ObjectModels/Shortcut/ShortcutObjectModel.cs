﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UniversalEditor.ObjectModels.Shortcut
{
	public class ShortcutObjectModel : ObjectModel
	{
		private static ObjectModelReference _omr = null;
		public override ObjectModelReference MakeReference()
		{
			if (_omr == null)
			{
				_omr = base.MakeReference();
				_omr.Title = "Shortcut";
			}
			return _omr;
		}

		public override void Clear()
		{
		}

		public override void CopyTo(ObjectModel where)
		{
		}

		private string mvarComment = String.Empty;
		/// <summary>
		/// Tooltip for the entry, for example "View sites on the Internet". The value should not
		/// be redundant with the shortcut title.
		/// </summary>
		public string Comment { get { return mvarComment; } set { mvarComment = value; } }

		private string mvarIconFileName = String.Empty;
		/// <summary>
		/// File name or known icon name of the icon to display in the file manager, menus, etc.
		/// </summary>
		public string IconFileName { get { return mvarIconFileName; } set { mvarIconFileName = value; } }

		private string mvarExecutableFileName = String.Empty;
		public string ExecutableFileName { get { return mvarExecutableFileName; } set { mvarExecutableFileName = value; } }

		private System.Collections.Specialized.StringCollection mvarExecutableArguments = new System.Collections.Specialized.StringCollection();
		public System.Collections.Specialized.StringCollection ExecutableArguments { get { return mvarExecutableArguments; } }

		private string mvarWorkingDirectory = String.Empty;
		/// <summary>
		/// The directory in which to run the program, if different than the program's location.
		/// </summary>
		public string WorkingDirectory { get { return mvarWorkingDirectory; } set { mvarWorkingDirectory = value; } }

		private bool mvarRunInTerminal = false;
		/// <summary>
		/// Whether the program runs in a terminal window.
		/// </summary>
		public bool RunInTerminal { get { return mvarRunInTerminal; } set { mvarRunInTerminal = value; } }
	}
}
