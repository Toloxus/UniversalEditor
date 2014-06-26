using System;

namespace UniversalEditor.UserInterface
{
	public class Command
	{
		public class CommandCollection
			: System.Collections.ObjectModel.Collection<Command>
		{
			public Command this[string ID]
			{
				get
				{
					foreach (Command command in this)
					{
						if (command.ID == ID) return command;
					}
					return null;
				}
			}
		}
		
		private bool mvarEnableTearoff = false;
		public bool EnableTearoff { get { return mvarEnableTearoff; } set { mvarEnableTearoff = value; } }
		
		private string mvarID = String.Empty;
		/// <summary>
		/// The ID of the command, used to reference it in <see cref="CommandReferenceCommandItem"/>.
		/// </summary>
		public string ID { get { return mvarID; } set { mvarID = value; } }
		
		private string mvarTitle = String.Empty;
		/// <summary>
		/// The title of the command (including mnemonic prefix, if applicable).
		/// </summary>
		public string Title { get { return mvarTitle; } set { mvarTitle = value; } }

		private string mvarDefaultCommandID = String.Empty;
		public string DefaultCommandID { get { return mvarDefaultCommandID; } set { mvarDefaultCommandID = value; } }

		private CommandShortcutKey mvarShortcutKey = new CommandShortcutKey();
		public CommandShortcutKey ShortcutKey { get { return mvarShortcutKey; } }
		
		private StockCommandType mvarStockCommandType = StockCommandType.None;
		/// <summary>
		/// A <see cref="StockCommandType"/> that represents a predefined, platform-themed command.
		/// </summary>
		public StockCommandType StockCommandType { get { return mvarStockCommandType; } set { mvarStockCommandType = value; } }
		
		private string mvarImageFileName = String.Empty;
		/// <summary>
		/// The file name of the image to be displayed on the command.
		/// </summary>
		public string ImageFileName { get { return mvarImageFileName; } set { mvarImageFileName = value; } }
		
		
		private CommandItem.CommandItemCollection mvarItems = new CommandItem.CommandItemCollection();
		/// <summary>
		/// The child <see cref="CommandItem"/>s that are contained within this <see cref="Command"/>.
		/// </summary>
		public CommandItem.CommandItemCollection Items { get { return mvarItems; } }
		
		/// <summary>
		/// The event that is fired when the command is executed.
		/// </summary>
		public event EventHandler Executed;
		
		/// <summary>
		/// Executes this <see cref="Command"/>.
		/// </summary>
		public void Execute()
		{
			if (Executed != null) Executed(this, EventArgs.Empty);
		}
	}
}
