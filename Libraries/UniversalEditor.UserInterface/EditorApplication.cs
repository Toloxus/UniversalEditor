﻿
using MBS.Framework.UserInterface;

namespace UniversalEditor.UserInterface
{
	public class EditorApplication : UIApplication, IHostApplication
	{
		/// <summary>
		/// Gets or sets the current window of the host application.
		/// </summary>
		public IHostApplicationWindow CurrentWindow { get { return UniversalEditor.UserInterface.Engine.CurrentEngine.LastWindow; } set { UniversalEditor.UserInterface.Engine.CurrentEngine.LastWindow = value; } }
		/// <summary>
		/// Gets or sets the output window of the host application, where other plugins can read from and write to.
		/// </summary>
		public HostApplicationOutputWindow OutputWindow { get; set; } = new HostApplicationOutputWindow();
		/// <summary>
		/// A collection of messages to display in the Error List panel.
		/// </summary>
		public HostApplicationMessage.HostApplicationMessageCollection Messages { get; } = new HostApplicationMessage.HostApplicationMessageCollection();
	}
}