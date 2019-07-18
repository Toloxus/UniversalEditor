﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using UniversalEditor.Accessors;
using UniversalEditor.ObjectModels.FileSystem;
using UniversalEditor.ObjectModels.PropertyList;

using UniversalEditor.DataFormats.Markup.XML;
using UniversalEditor.ObjectModels.Markup;
using UniversalEditor.DataFormats.FileSystem.UXT;
using UniversalEditor.DataFormats.PropertyList.UniversalPropertyList;

using UniversalWidgetToolkit.Dialogs;
using UniversalWidgetToolkit.Drawing;
using UniversalEditor.UserInterface.Dialogs;
using UniversalWidgetToolkit;
using MBS.Framework.Drawing;

namespace UniversalEditor.UserInterface
{
	// TODO: Finish implementing ObjectModel Converters... will need to have something
	// like GetAvailableConvertersFrom(Type objectModel) to get a list of available
	// converters that can convert from the specified object model.

	public class Engine
	{
		private static Engine _TheEngine = new Engine();
		private SplashScreenWindow splasher = null;

		private void ShowSplashScreen()
		{
			// if (LocalConfiguration.SplashScreen.Enabled)
			// {
				splasher = new SplashScreenWindow();
				splasher.Show();
			// }
		}
		private void HideSplashScreen()
		{
			while (splasher == null)
			{
				System.Threading.Thread.Sleep(500);
			}
			splasher.Hide();
		}

		#region implemented abstract members of Engine
		protected void ShowCrashDialog (Exception ex)
		{
			Console.WriteLine (ex.ToString ());

			// Dialogs.CrashDialog dlg = new Dialogs.CrashDialog();
			// dlg.Exception = ex;
			// dlg.ShowDialog();
		}

		protected void BeforeInitialization ()
		{
			// Application.EnableVisualStyles();
			// Application.SetCompatibleTextRenderingDefault(false);

			// TODO: figure out why this is being done on BeforeInitialization and whether we could move it to after
			//       the configuration is initialized, so we can specify the user's favorite theme in a configuration file

			// AwesomeControls.Theming.BuiltinThemes. theme = new AwesomeControls.Theming.BuiltinThemes.VisualStudio2012Theme(AwesomeControls.Theming.BuiltinThemes.VisualStudio2012Theme.ColorMode.Dark);
			// theme.UseAllCapsMenus = false;
			// theme.SetStatusBarState(AwesomeControls.Theming.BuiltinThemes.VisualStudio2012Theme.StatusBarState.Initial);
			// AwesomeControls.Theming.BuiltinThemes.Office2003Theme theme = new AwesomeControls.Theming.BuiltinThemes.Office2003Theme();
			// AwesomeControls.Theming.BuiltinThemes.OfficeXPTheme theme = new AwesomeControls.Theming.BuiltinThemes.OfficeXPTheme();
			// AwesomeControls.Theming.BuiltinThemes.SlickTheme theme = new AwesomeControls.Theming.BuiltinThemes.SlickTheme();

			// Office 2000  =   {105843D0-2F26-4CB7-86AB-10A449815C19}
			// Office 2007	=	{4D86F538-E277-4E6F-9CAC-60F82D49A19D}
			// VS2012-Dark	=	{25134C94-B1EB-4C38-9B5B-A2E29FC57AE1}
			// VS2012-Light	=	{54CE64B1-2DE3-4147-B499-03F0934AFD37}
			// VS2012-Blue	=	{898A65FC-8D08-46F1-BB94-2BF666AC996E}

			// AwesomeControls.Theming.Theme theme = AwesomeControls.Theming.Theme.GetByID(new Guid("{54CE64B1-2DE3-4147-B499-03F0934AFD37}"));
			// AwesomeControls.Theming.Theme theme = AwesomeControls.Theming.Theme.GetByID(new Guid("{105843D0-2F26-4CB7-86AB-10A449815C19}"));
			// if (theme != null) AwesomeControls.Theming.Theme.CurrentTheme = theme;

			// AwesomeControls.Theming.Theme.CurrentTheme.Properties["UseAllCapsMenus"] = false;

			// Glue.ApplicationInformation.ApplicationID = new Guid("{b359fe9a-080a-43fc-ae38-00ba7ac1703e}");

			UniversalWidgetToolkit.Application.Initialize();
		}
		
		protected void MainLoop ()
		{
			switch (System.Environment.OSVersion.Platform)
			{
				case PlatformID.MacOSX:
				case PlatformID.Unix:
				{
					// Glue.ApplicationInformation.ApplicationDataPath = String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new string[] { "universal-editor", "plugins" });
					break;
				}
				case PlatformID.Win32NT:
				case PlatformID.Win32S:
				case PlatformID.Win32Windows:
				case PlatformID.WinCE:
				case PlatformID.Xbox:
				{
					// Glue.ApplicationInformation.ApplicationDataPath = String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new string[] { "Universal Editor", "Plugins" });
					break;
				}
			}

			// Glue.ApplicationEventEventArgs e = new Glue.ApplicationEventEventArgs(Glue.Common.Constants.EventNames.ApplicationStart);
			// Glue.Common.Methods.SendApplicationEvent(e);
			// if (e.CancelApplication) return;

			/*
			PieMenuManager.Title = "Universal Editor pre-alpha build";

			PieMenuItemGroup row1 = new PieMenuItemGroup();
			row1.Title = "Tools";
			row1.Items.Add("atlMFCTraceTool", "ATL/MFC Trace Tool");
			row1.Items.Add("textEditor", "Text Editor");
			PieMenuManager.Groups.Add(row1);
			*/

#if !DEBUG
			// Application.ThreadException += Application_ThreadException;
#endif

			UniversalWidgetToolkit.Application.Start();

			// Glue.Common.Methods.SendApplicationEvent(new Glue.ApplicationEventEventArgs(Glue.Common.Constants.EventNames.ApplicationStop));
		}

		/// <summary>
		/// Opens a new window, optionally loading the specified documents.
		/// </summary>
		/// <param name="documents">The document model(s) of the document(s) to load.</param>
		/// <returns>An <see cref="IHostApplicationWindow"/> representing the window that was created.</returns>
		protected IHostApplicationWindow OpenWindowInternal (params Document[] documents)
		{
			MainWindow mw = new MainWindow ();
			LastWindow = mw;
			if (documents.Length > 0)
			{
				mw.OpenFile(documents);
			}
			mw.Show ();
 			return mw;
		}
		public void ShowAboutDialog (DataFormatReference dfr)
		{
			if (dfr == null)
			{
				string dlgfilename = ExpandRelativePath("~/Dialogs/AboutDialog.glade");
				if (dlgfilename != null)
				{
					Dialog dlg = new Dialog();
					dlg.LoadFromMarkup(dlgfilename, "GtkDialog");
					dlg.ShowDialog();
				}
				else
				{
					AboutDialog dlg = new AboutDialog();
					dlg.ProgramName = "Universal Editor";
					dlg.Version = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
					dlg.Copyright = "(c) 1997-2019 Michael Becker";
					dlg.Comments = "A modular, extensible document editor";
					dlg.LicenseType = UniversalWidgetToolkit.LicenseType.GPL30;
					dlg.ShowDialog();
				}
			}
			else
			{
				DataFormatAboutDialog dlg = new DataFormatAboutDialog();
				dlg.DataFormatReference = dfr;
				dlg.ShowDialog();
			}
		}
		public bool ShowCustomOptionDialog (ref CustomOption.CustomOptionCollection customOptions, string title = null, EventHandler aboutButtonClicked = null)
		{
			if (CustomOptionsDialog.ShowDialog(ref customOptions, title, aboutButtonClicked) == DialogResult.Cancel)
			{
				return false;
			}
			return true;
		}
		#endregion

		protected void StopApplicationInternal ()
		{
			UniversalWidgetToolkit.Application.Stop ();
		}


		private static Engine[] m_AvailableEngines = null;
		public static Engine[] GetAvailableEngines()
		{
			return new Engine[] { _TheEngine };
		}

		public bool AttachCommandEventHandler(string commandID, EventHandler handler)
		{
			Command cmd = Commands[commandID];
			if (cmd != null)
			{
				cmd.Executed += handler;
				return true;
			}
			Console.WriteLine("attempted to attach handler for unknown command '" + commandID + "'");
			return false;
		}

		protected virtual void AfterInitialization()
		{
		}

		private void AfterInitializationInternal()
		{
			// Initialize all the commands that are common to UniversalEditor
			#region File
			AttachCommandEventHandler("FileNewDocument", delegate(object sender, EventArgs e)
			{
				LastWindow.NewFile();
			});
			AttachCommandEventHandler("FileNewProject", delegate(object sender, EventArgs e)
			{
				LastWindow.NewProject();
			});
			AttachCommandEventHandler("FileOpenDocument", delegate(object sender, EventArgs e)
			{
				LastWindow.OpenFile();
			});
			AttachCommandEventHandler("FileOpenProject", delegate(object sender, EventArgs e)
			{
				LastWindow.OpenProject();
			});
			AttachCommandEventHandler("FileSaveDocument", delegate(object sender, EventArgs e)
			{
				LastWindow.SaveFile();
			});
			AttachCommandEventHandler("FileSaveDocumentAs", delegate(object sender, EventArgs e)
			{
				LastWindow.SaveFileAs();
			});
			AttachCommandEventHandler("FileSaveProject", delegate(object sender, EventArgs e)
			{
				LastWindow.SaveProject();
			});
			AttachCommandEventHandler("FileSaveProjectAs", delegate(object sender, EventArgs e)
			{
				LastWindow.SaveProjectAs();
			});
			AttachCommandEventHandler("FileSaveAll", delegate(object sender, EventArgs e)
			{
				LastWindow.SaveAll();
			});
			AttachCommandEventHandler("FileCloseDocument", delegate(object sender, EventArgs e)
			{
				LastWindow.CloseFile();
			});
			AttachCommandEventHandler("FileCloseProject", delegate(object sender, EventArgs e)
			{
				LastWindow.CloseProject();
			});
			AttachCommandEventHandler("FileRestart", delegate(object sender, EventArgs e)
			{
				RestartApplication();
			});
			AttachCommandEventHandler("FileExit", delegate(object sender, EventArgs e)
			{
				StopApplication();
			});
			#endregion
			#region Edit
			AttachCommandEventHandler("EditCut", delegate(object sender, EventArgs e)
			{
				Command cmdCopy = mvarCommands["EditCopy"];
				Command cmdDelete = mvarCommands["EditDelete"];
				
				cmdCopy.Execute ();
				cmdDelete.Execute ();
			});
			AttachCommandEventHandler("EditCopy", delegate(object sender, EventArgs e)
			{
				Editor editor = LastWindow.GetCurrentEditor();
				if (editor == null) return;
				editor.Copy();
			});
			AttachCommandEventHandler("EditPaste", delegate(object sender, EventArgs e)
			{
				Editor editor = LastWindow.GetCurrentEditor();
				if (editor == null) return;
				editor.Paste();
			});
			AttachCommandEventHandler("EditDelete", delegate(object sender, EventArgs e)
			{
				Editor editor = LastWindow.GetCurrentEditor();
				if (editor == null) return;
				editor.Delete();
			});
			AttachCommandEventHandler("EditUndo", delegate(object sender, EventArgs e)
			{
				Editor editor = LastWindow.GetCurrentEditor();
				if (editor == null) return;
				editor.Undo();
			});
			AttachCommandEventHandler("EditRedo", delegate(object sender, EventArgs e)
			{
				Editor editor = LastWindow.GetCurrentEditor();
				if (editor == null) return;
				editor.Redo();
			});
			#endregion
			#region View
			AttachCommandEventHandler("ViewFullScreen", delegate(object sender, EventArgs e)
			{
				Command cmd = (sender as Command);
				LastWindow.FullScreen = !LastWindow.FullScreen;
				cmd.Checked = LastWindow.FullScreen;
			});
			#region Perspective
			AttachCommandEventHandler("ViewPerspective1", delegate(object sender, EventArgs e)
			{
				HostApplication.CurrentWindow.SwitchPerspective(1);
			});
			AttachCommandEventHandler("ViewPerspective2", delegate(object sender, EventArgs e)
			{
				HostApplication.CurrentWindow.SwitchPerspective(2);
			});
			AttachCommandEventHandler("ViewPerspective3", delegate(object sender, EventArgs e)
			{
				HostApplication.CurrentWindow.SwitchPerspective(3);
			});
			AttachCommandEventHandler("ViewPerspective4", delegate(object sender, EventArgs e)
			{
				HostApplication.CurrentWindow.SwitchPerspective(4);
			});
			AttachCommandEventHandler("ViewPerspective5", delegate(object sender, EventArgs e)
			{
				HostApplication.CurrentWindow.SwitchPerspective(5);
			});
			AttachCommandEventHandler("ViewPerspective6", delegate(object sender, EventArgs e)
			{
				HostApplication.CurrentWindow.SwitchPerspective(6);
			});
			AttachCommandEventHandler("ViewPerspective7", delegate(object sender, EventArgs e)
			{
				HostApplication.CurrentWindow.SwitchPerspective(7);
			});
			AttachCommandEventHandler("ViewPerspective8", delegate(object sender, EventArgs e)
			{
				HostApplication.CurrentWindow.SwitchPerspective(8);
			});
			AttachCommandEventHandler("ViewPerspective9", delegate(object sender, EventArgs e)
			{
				HostApplication.CurrentWindow.SwitchPerspective(9);
			});
			#endregion

			AttachCommandEventHandler("ViewStartPage", delegate(object sender, EventArgs e)
			{
				HostApplication.CurrentWindow.ShowStartPage();
			});

			#endregion
			#region Tools
			// ToolsOptions should actually be under the Edit menu as "Preferences" on Linux systems
			AttachCommandEventHandler("ToolsOptions", delegate(object sender, EventArgs e)
			{
				LastWindow.ShowOptionsDialog();
			});
			#endregion
			#region Window
			AttachCommandEventHandler("WindowNewWindow", delegate(object sender, EventArgs e)
			{
				OpenWindow();
			});
			AttachCommandEventHandler("WindowWindows", delegate(object sender, EventArgs e)
			{
				LastWindow.SetWindowListVisible(true, true);
			});
			#endregion
			#region Help
			AttachCommandEventHandler("HelpLicensingAndActivation", delegate (object sender, EventArgs e)
			{
				MessageDialog.ShowDialog("This product has already been activated.", "Licensing and Activation", MessageDialogButtons.OK, MessageDialogIcon.Information);
			});
			AttachCommandEventHandler("HelpAboutPlatform", delegate(object sender, EventArgs e)
			{
				ShowAboutDialog();
			});
			#endregion


			#region Dynamic Commands
			#region View
			#region Panels
			for (int i = mvarCommandBars.Count - 1; i >= 0; i--)
			{
				Command cmdViewToolbarsToolbar = new Command();
				cmdViewToolbarsToolbar.ID = "ViewToolbars" + i.ToString();
				cmdViewToolbarsToolbar.Title = mvarCommandBars[i].Title;
				cmdViewToolbarsToolbar.Executed += cmdViewToolbarsToolbar_Executed;
				mvarCommands.Add(cmdViewToolbarsToolbar);
				mvarCommands["ViewToolbars"].Items.Insert(0, new CommandReferenceCommandItem(cmdViewToolbarsToolbar.ID));
			}
			#endregion
			#region Panels
			if (mvarCommands["ViewPanels"] != null)
			{
				Command cmdViewPanels1 = new Command();
				cmdViewPanels1.ID = "ViewPanels1";
				mvarCommands.Add(cmdViewPanels1);
				mvarCommands["ViewPanels"].Items.Add(new CommandReferenceCommandItem("ViewPanels1"));
			}
			#endregion
			#endregion
			#endregion

			#region Language Strings
			#region Help
			Command helpAboutPlatform = mvarCommands["HelpAboutPlatform"];
			if (helpAboutPlatform != null)
			{
				helpAboutPlatform.Title = String.Format(helpAboutPlatform.Title, mvarDefaultLanguage.GetStringTableEntry("ApplicationTitle", "Universal Editor"));
			}

			Command helpLanguage = mvarCommands["HelpLanguage"];
			if (helpLanguage != null)
			{
				foreach (Language lang in mvarLanguages)
				{
					Command cmdLanguage = new Command();
					cmdLanguage.ID = "HelpLanguage_" + lang.ID;
					cmdLanguage.Title = lang.Title;
					cmdLanguage.Executed += delegate(object sender, EventArgs e)
					{
						HostApplication.Messages.Add(HostApplicationMessageSeverity.Notice, "Clicked language " + lang.ID);
					};
					mvarCommands.Add(cmdLanguage);

					helpLanguage.Items.Add(new CommandReferenceCommandItem("HelpLanguage_" + lang.ID));
				}
			}
			#endregion
			#endregion
		}

		void cmdViewToolbarsToolbar_Executed(object sender, EventArgs e)
		{
			Command cmd = (sender as Command);
			
		}

		private IHostApplicationWindowCollection mvarWindows = new IHostApplicationWindowCollection();
		public IHostApplicationWindowCollection Windows { get { return mvarWindows; } }

		public void CloseAllWindows()
		{
			List<IHostApplicationWindow> windowsToClose = new List<IHostApplicationWindow>();
			foreach (IHostApplicationWindow window in mvarWindows)
			{
				windowsToClose.Add(window);
			}
			foreach (IHostApplicationWindow window in windowsToClose)
			{
				window.CloseWindow();
			}
		}

		public static bool Execute()
		{
			Console.WriteLine(" *** There is only UWT *** ");
			mvarCurrentEngine = _TheEngine;

			if (mvarCurrentEngine != null)
			{
				Console.WriteLine("Using engine " + mvarCurrentEngine.GetType().FullName);
			}

#if !DEBUG
			try
			{
#endif
				mvarCurrentEngine.StartApplication();
#if !DEBUG
			}
			catch (Exception ex)
			{
				mvarCurrentEngine.ShowCrashDialog(ex);
			}
#endif
			return true;
		}

		private Command.CommandCollection mvarCommands = new Command.CommandCollection();
		/// <summary>
		/// The commands defined for this application.
		/// </summary>
		public Command.CommandCollection Commands { get { return mvarCommands; } }

		private Language mvarDefaultLanguage = null;
		/// <summary>
		/// The default <see cref="Language"/> used to display translatable text in this application.
		/// </summary>
		public Language DefaultLanguage { get { return mvarDefaultLanguage; } set { mvarDefaultLanguage = value; } }

		private Language.LanguageCollection mvarLanguages = new Language.LanguageCollection();
		/// <summary>
		/// The languages defined for this application. Translations can be added through XML files in the ~/Languages folder.
		/// </summary>
		public Language.LanguageCollection Languages { get { return mvarLanguages; } }
		
		private EngineMainMenu mvarMainMenu = new EngineMainMenu();
		/// <summary>
		/// The main menu of this application, which can hold multiple <see cref="CommandItem"/>s.
		/// </summary>
		public EngineMainMenu MainMenu { get { return mvarMainMenu; } }

		private CommandBar.CommandBarCollection mvarCommandBars = new CommandBar.CommandBarCollection();
		/// <summary>
		/// The command bars loaded in this application, which can each hold multiple <see cref="CommandItem"/>s.
		/// </summary>
		public CommandBar.CommandBarCollection CommandBars { get { return mvarCommandBars; } }
		
		/// <summary>
		/// The aggregated raw markup of all the various XML files loaded in the current search path.
		/// </summary>
		private MarkupObjectModel mvarRawMarkup = new MarkupObjectModel();
		
		private System.Collections.ObjectModel.ReadOnlyCollection<string> mvarSelectedFileNames = null;
		public System.Collections.ObjectModel.ReadOnlyCollection<string> SelectedFileNames { get { return mvarSelectedFileNames; } }

		private string mvarBasePath = String.Empty;
		public string BasePath { get { return mvarBasePath; } }

		private static string mvarDataPath = null;
		public static string DataPath
		{
			get
			{
				if (mvarDataPath == null)
				{
					mvarDataPath = String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new string[]
					{
						Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
						"Mike Becker's Software",
						"Universal Editor"
					});
				}
				return mvarDataPath;
			}
		}

		private IHostApplicationWindow mvarLastWindow = null;
		public IHostApplicationWindow LastWindow { get { return mvarLastWindow; } set { mvarLastWindow = value; } }

		public void OpenFile(params string[] fileNames)
		{
			Document[] documents = new Document[fileNames.Length];
			for (int i = 0; i < fileNames.Length; i++)
			{
				documents[i] = new Document(null, null, new FileAccessor(fileNames[i]));
			}
			OpenFile(documents);
		}
		public void OpenFile(params Document[] documents)
		{
			if (LastWindow == null)
			{
				OpenWindow(documents);
				return;
			}
			LastWindow.OpenFile(documents);
		}

		public void ShowAboutDialog()
		{
			this.ShowAboutDialog(null);
		}

		/// <summary>
		/// Opens a new window, with no documents loaded.
		/// </summary>
		public void OpenWindow()
		{
			OpenWindow(new Document[0]);
		}
		/// <summary>
		/// Opens a new window, optionally loading the specified documents.
		/// </summary>
		/// <param name="fileNames">The file name(s) of the document(s) to load.</param>
		public void OpenWindow(params string[] fileNames)
		{
			Document[] documents = new Document[fileNames.Length];
			for (int i = 0; i < fileNames.Length; i++)
			{
				documents[i] = new Document(null, null, new FileAccessor(fileNames[i]));
			}
			OpenWindow(documents);
		}
		/// <summary>
		/// Opens a new window, optionally loading the specified documents.
		/// </summary>
		/// <param name="documents">The document model(s) of the document(s) to load.</param>
		public void OpenWindow(params Document[] documents)
		{
			IHostApplicationWindow window = OpenWindowInternal(documents);
			window.WindowClosed += delegate(object sender, EventArgs e)
			{
				mvarWindows.Remove(window);
			};
			mvarWindows.Add(window);
		}

		// UniversalDataStorage.Editor.WindowsForms.Program
		private void SingleInstanceManager_Callback(object sender, SingleInstanceManager.InstanceCallbackEventArgs e)
		{
			if (!e.IsFirstInstance)
			{
				if (LastWindow != null)
				{
					Document[] documents = new Document[e.CommandLineArgs.Length - 1];
					for (int i = 1; i < e.CommandLineArgs.Length; i++)
					{
						documents[i - 1] = new Document(null, null, new FileAccessor(e.CommandLineArgs[i]));
					}

					LastWindow.OpenFile(documents);
					LastWindow.ActivateWindow();
				}
			}
		}

		private string[] EnumerateDataPaths()
		{
			return new string[]
			{
				// first look in the application root directory since this will be overridden by everything else
				mvarBasePath,
				// then look in /usr/share/universal-editor or C:\ProgramData\Mike Becker's Software\Universal Editor
				String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new string[]
				{
					System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData),
					"mbs-editor"
				}),
				// then look in ~/.local/share/universal-editor or C:\Users\USERNAME\AppData\Local\Mike Becker's Software\Universal Editor
				String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new string[]
				{
					System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
					"mbs-editor"
				}),
				// then look in ~/.universal-editor or C:\Users\USERNAME\AppData\Roaming\Mike Becker's Software\Universal Editor
				String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new string[]
				{
					System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
					"mbs-editor"
				})
			};
		}
		private string[] EnumerateDataFiles(string filter)
		{
			List<String> xmlFilesList = new List<String>();

			// TODO: change "universal-editor" string to platform-dependent "universal-editor" on *nix or "Mike Becker's Software/Universal Editor" on Windowds
			string[] paths = EnumerateDataPaths();

			foreach (string path in paths)
			{
				// skip this one if the path doesn't exist
				if (!System.IO.Directory.Exists(path)) continue;

				string[] xmlfilesPath = System.IO.Directory.GetFiles(path, filter, System.IO.SearchOption.AllDirectories);
				foreach (string s in xmlfilesPath)
				{
					xmlFilesList.Add(s);
				}
			}
			return xmlFilesList.ToArray();
		}
		public string ExpandRelativePath(string relativePath)
		{
			if (relativePath.StartsWith("~/"))
			{
				string[] potentialFileNames = EnumerateDataPaths();
				for (int i = potentialFileNames.Length - 1; i >= 0; i--)
				{
					potentialFileNames[i] = potentialFileNames[i] + '/' + relativePath.Substring(2);
					Console.WriteLine("Looking for " + potentialFileNames[i]);

					if (System.IO.File.Exists(potentialFileNames[i]))
					{
						return potentialFileNames[i];
					}
				}
			}
			if (System.IO.File.Exists(relativePath))
			{
				return relativePath;
			}
			return null;
		}

		// FIXME: this is the single XML configuration file loader that should be executed at the beginning of engine launch
		protected virtual void InitializeXMLConfiguration()
		{
			#region Load the XML files
			string configurationFileNameFilter = System.Configuration.ConfigurationManager.AppSettings["UniversalEditor.Configuration.ConfigurationFileNameFilter"];
			if (configurationFileNameFilter == null) configurationFileNameFilter = "*.uexml";

			string[] xmlfiles = EnumerateDataFiles(configurationFileNameFilter);

			UpdateSplashScreenStatus("Loading XML configuration files", 0, 0, xmlfiles.Length);

			XMLDataFormat xdf = new XMLDataFormat();
			foreach (string xmlfile in xmlfiles)
			{
				MarkupObjectModel markup = new MarkupObjectModel();
				Document doc = new Document(markup, xdf, new FileAccessor(xmlfile));
				doc.Accessor.DefaultEncoding = IO.Encoding.UTF8;

				doc.Accessor.Open();
				doc.Load();
				doc.Close();

				markup.CopyTo(mvarRawMarkup);

				// UpdateSplashScreenStatus("Loading XML configuration files", Array.IndexOf(xmlfiles, xmlfile) + 1);
			}

			#endregion

			#region Initialize the configuration with the loaded data
			#region Commands
			UpdateSplashScreenStatus("Loading available commands");
			MarkupTagElement tagCommands = (mvarRawMarkup.FindElement("ApplicationFramework", "Commands") as MarkupTagElement);
			if (tagCommands != null)
			{
				foreach (MarkupElement elCommand in tagCommands.Elements)
				{
					MarkupTagElement tagCommand = (elCommand as MarkupTagElement);
					if (tagCommand == null) continue;
					if (tagCommand.FullName != "Command") continue;

					MarkupAttribute attID = tagCommand.Attributes["ID"];
					if (attID == null) continue;

					Command cmd = new Command();
					cmd.ID = attID.Value;

					MarkupAttribute attDefaultCommandID = tagCommand.Attributes["DefaultCommandID"];
					if (attDefaultCommandID != null)
					{
						cmd.DefaultCommandID = attDefaultCommandID.Value;
					}

					MarkupAttribute attTitle = tagCommand.Attributes["Title"];
					if (attTitle != null)
					{
						cmd.Title = attTitle.Value;
					}
					else
					{
						cmd.Title = cmd.ID;
					}

					MarkupTagElement tagShortcut = (tagCommand.Elements["Shortcut"] as MarkupTagElement);
					if (tagShortcut != null)
					{
						MarkupAttribute attModifiers = tagShortcut.Attributes["Modifiers"];
						MarkupAttribute attKey = tagShortcut.Attributes["Key"];
						if (attKey != null)
						{
							CommandShortcutKeyModifiers modifiers = CommandShortcutKeyModifiers.None;
							if (attModifiers != null)
							{
								string[] strModifiers = attModifiers.Value.Split(new char[] { ',' });
								foreach (string strModifier in strModifiers)
								{
									switch (strModifier.Trim().ToLower())
									{
										case "alt":
										{
											modifiers |= CommandShortcutKeyModifiers.Alt;
											break;
										}
										case "control":
										{
											modifiers |= CommandShortcutKeyModifiers.Control;
											break;
										}
										case "meta":
										{
											modifiers |= CommandShortcutKeyModifiers.Meta;
											break;
										}
										case "shift":
										{
											modifiers |= CommandShortcutKeyModifiers.Shift;
											break;
										}
										case "super":
										{
											modifiers |= CommandShortcutKeyModifiers.Super;
											break;
										}
									}
								}
							}

							CommandShortcutKeyValue value = CommandShortcutKeyValue.None;
							value = (CommandShortcutKeyValue)Enum.Parse(typeof(CommandShortcutKeyValue), attKey.Value);

							cmd.ShortcutKey = new CommandShortcutKey(value, modifiers);
						}
					}

					MarkupTagElement tagItems = (tagCommand.Elements["Items"] as MarkupTagElement);
					if (tagItems != null)
					{
						foreach (MarkupElement el in tagItems.Elements)
						{
							MarkupTagElement tag = (el as MarkupTagElement);
							if (tag == null) continue;

							InitializeCommandBarItem(tag, cmd, null);
						}
					}

					mvarCommands.Add(cmd);
				}
			}
			#endregion
			#region Main Menu Items
			UpdateSplashScreenStatus("Loading main menu items");

			MarkupTagElement tagMainMenuItems = (mvarRawMarkup.FindElement("ApplicationFramework", "MainMenu", "Items") as MarkupTagElement);
			if (tagMainMenuItems != null)
			{
				foreach (MarkupElement elItem in tagMainMenuItems.Elements)
				{
					MarkupTagElement tagItem = (elItem as MarkupTagElement);
					if (tagItem == null) continue;
					InitializeCommandBarItem(tagItem, null, null);
				}
			}

			UpdateSplashScreenStatus("Loading command bars");

			MarkupTagElement tagCommandBars = (mvarRawMarkup.FindElement("ApplicationFramework", "CommandBars") as MarkupTagElement);
			if (tagCommandBars != null)
			{
				foreach (MarkupElement elCommandBar in tagCommandBars.Elements)
				{
					MarkupTagElement tagCommandBar = (elCommandBar as MarkupTagElement);
					if (tagCommandBar == null) continue;
					if (tagCommandBar.FullName != "CommandBar") continue;
					InitializeCommandBar(tagCommandBar);
				}
			}
			#endregion
			#region Languages
			UpdateSplashScreenStatus("Loading languages and translations");

			MarkupTagElement tagLanguages = (mvarRawMarkup.FindElement("ApplicationFramework", "Languages") as MarkupTagElement);
			if (tagLanguages != null)
			{
				foreach (MarkupElement elLanguage in tagLanguages.Elements)
				{
					MarkupTagElement tagLanguage = (elLanguage as MarkupTagElement);
					if (tagLanguage == null) continue;
					if (tagLanguage.FullName != "Language") continue;
					InitializeLanguage(tagLanguage);
				}

				MarkupAttribute attDefaultLanguageID = tagLanguages.Attributes["DefaultLanguageID"];
				if (attDefaultLanguageID != null)
				{
					mvarDefaultLanguage = mvarLanguages[attDefaultLanguageID.Value];
				}
			}

			UpdateSplashScreenStatus("Setting language");

			if (mvarDefaultLanguage == null)
			{
				mvarDefaultLanguage = new Language();
			}
			else
			{
				foreach (Command cmd in mvarCommands)
				{
					cmd.Title = mvarDefaultLanguage.GetCommandTitle(cmd.ID, cmd.ID);
				}
			}
			#endregion

			#region Global Configuration
			{
				UpdateSplashScreenStatus("Loading global configuration");

				MarkupTagElement tagConfiguration = (mvarRawMarkup.FindElement("UniversalEditor", "Configuration") as MarkupTagElement);
				if (tagConfiguration != null)
				{
					foreach (MarkupElement el in tagConfiguration.Elements)
					{
						MarkupTagElement tag = (el as MarkupTagElement);
						if (tag == null) continue;
						LoadConfiguration(tag);
					}
				}
			}
			#endregion
			#region Editor Configuration
			{
				UpdateSplashScreenStatus("Loading editor configuration");

				MarkupTagElement tagEditors = (mvarRawMarkup.FindElement("UniversalEditor", "Editors") as MarkupTagElement);
				if (tagEditors != null)
				{
					foreach (MarkupElement el in tagEditors.Elements)
					{
						MarkupTagElement tag = (el as MarkupTagElement);
						if (tag == null) continue;
						if (tag.FullName != "Editor") continue;
						
						switch (System.Environment.OSVersion.Platform)
						{
							case PlatformID.MacOSX:
							case PlatformID.Unix:
							case PlatformID.Xbox:
							{
								// TODO: this fails on Linux and I don't know why
								Console.WriteLine("skipping load editor configuration on Mac OS X, Unix, or Xbox because it fails on Linux and I don't know why");
								break;
							}
							case PlatformID.Win32NT:
							case PlatformID.Win32S:
							case PlatformID.Win32Windows:
							case PlatformID.WinCE:
							{
								EditorReference editor = null;
								try
								{
									Common.Reflection.GetAvailableEditorByID(new Guid(tag.Attributes["ID"].Value));
								}
								catch (Exception ex)
								{
									Console.WriteLine("couldn't load editor " + tag.Attributes["ID"].Value);
								}
								break;
							}
						}
					}
				}
			}
			#endregion
			#region Object Model Configuration
			{
				UpdateSplashScreenStatus("Loading object model configuration");

				MarkupTagElement tagObjectModels = (mvarRawMarkup.FindElement("UniversalEditor", "ObjectModels") as MarkupTagElement);
				if (tagObjectModels != null)
				{
					MarkupTagElement tagDefault = (tagObjectModels.Elements["Default"] as MarkupTagElement);
					if (tagDefault != null)
					{
						ObjectModelReference[] omrs = UniversalEditor.Common.Reflection.GetAvailableObjectModels();
						MarkupAttribute attVisible = tagDefault.Attributes["Visible"];
						foreach (ObjectModelReference omr in omrs)
						{
							if (attVisible != null) omr.Visible = (attVisible.Value == "true");
						}
					}

					foreach (MarkupElement el in tagObjectModels.Elements)
					{
						MarkupTagElement tag = (el as MarkupTagElement);
						if (tag == null) continue;
						if (tag.FullName == "ObjectModel")
						{
							MarkupAttribute attTypeName = tag.Attributes["TypeName"];
							MarkupAttribute attID = tag.Attributes["ID"];
							MarkupAttribute attVisible = tag.Attributes["Visible"];

							if (attTypeName != null)
							{
								ObjectModelReference omr = UniversalEditor.Common.Reflection.GetAvailableObjectModelByTypeName(attTypeName.Value);
								if (attVisible != null) omr.Visible = (attVisible.Value == "true");
							}
							else
							{
								ObjectModelReference omr = UniversalEditor.Common.Reflection.GetAvailableObjectModelByID(new Guid(attID.Value));
								if (attVisible != null) omr.Visible = (attVisible.Value == "true");
							}
						}
					}
				}
			}
			#endregion

			// UpdateSplashScreenStatus("Finalizing configuration");
			// ConfigurationManager.Load();
			#endregion
		}

		private void LoadConfiguration(MarkupTagElement tag, Group group = null)
		{
			if (tag.FullName == "Group")
			{
				Group group1 = new Group();
				group1.Name = tag.Attributes["ID"].Value;
				foreach (MarkupElement el in tag.Elements)
				{
					MarkupTagElement tg = (el as MarkupTagElement);
					if (tg == null) continue;
					LoadConfiguration(tg, group1);
				}

				if (group == null)
				{
					mvarConfigurationManager.AddGroup(group1, ConfigurationManagerPropertyScope.Global);
				}
				else
				{
					group.Groups.Add(group1);
				}
			}
			else if (tag.FullName == "Property")
			{
				Property property = new Property();
				property.Name = tag.Attributes["ID"].Value;
				MarkupAttribute att = tag.Attributes["Value"];
				if (att != null)
				{
					property.Value = att.Value;
				}

				if (group == null)
				{
					mvarConfigurationManager.AddProperty(property, ConfigurationManagerPropertyScope.Global);
				}
				else
				{
					group.Properties.Add(property);
				}
			}
		}

		private void InitializeLanguage(MarkupTagElement tag)
		{
			Language lang = new Language();
			
			MarkupAttribute attID = tag.Attributes["ID"];
			if (attID == null) return;
			lang.ID = attID.Value;

			MarkupAttribute attTitle = tag.Attributes["Title"];
			if (attTitle != null)
			{
				lang.Title = attTitle.Value;
			}
			else
			{
				lang.Title = lang.ID;
			}

			MarkupTagElement tagStringTable = (tag.Elements["StringTable"] as MarkupTagElement);
			if (tagStringTable != null)
			{
				foreach (MarkupElement elStringTableEntry in tagStringTable.Elements)
				{
					MarkupTagElement tagStringTableEntry = (elStringTableEntry as MarkupTagElement);
					if (tagStringTableEntry == null) continue;
					if (tagStringTableEntry.FullName != "StringTableEntry") continue;

					MarkupAttribute attStringTableEntryID = tagStringTableEntry.Attributes["ID"];
					if (attStringTableEntryID == null) continue;

					MarkupAttribute attStringTableEntryValue = tagStringTableEntry.Attributes["Value"];
					if (attStringTableEntryValue == null) continue;

					lang.SetStringTableEntry(attStringTableEntryID.Value, attStringTableEntryValue.Value);
				}
			}

			MarkupTagElement tagCommands = (tag.Elements["Commands"] as MarkupTagElement);
			if (tagCommands != null)
			{
				foreach (MarkupElement elCommand in tagCommands.Elements)
				{
					MarkupTagElement tagCommand = (elCommand as MarkupTagElement);
					if (tagCommand == null) continue;
					if (tagCommand.FullName != "Command") continue;

					MarkupAttribute attCommandID = tagCommand.Attributes["ID"];
					if (attCommandID == null) continue;

					MarkupAttribute attCommandTitle = tagCommand.Attributes["Title"];
					if (attCommandTitle == null) continue;

					lang.SetCommandTitle(attCommandID.Value, attCommandTitle.Value);
				}
			}

			mvarLanguages.Add(lang);
		}
		
		private void InitializeCommandBar(MarkupTagElement tag)
		{
			MarkupAttribute attID = tag.Attributes["ID"];
			if (attID == null) return;

			CommandBar cb = new CommandBar();
			cb.ID = attID.Value;

			MarkupAttribute attTitle = tag.Attributes["Title"];
			if (attTitle != null)
			{
				cb.Title = attTitle.Value;
			}
			else
			{
				cb.Title = cb.ID;
			}

			MarkupTagElement tagItems = tag.Elements["Items"] as MarkupTagElement;
			if (tagItems != null)
			{
				foreach (MarkupElement elItem in tagItems.Elements)
				{
					MarkupTagElement tagItem = (elItem as MarkupTagElement);
					if (tagItem == null) continue;

					InitializeCommandBarItem(tagItem, null, cb);
				}
			}

			mvarCommandBars.Add(cb);
		}

		private void InitializeCommandBarItem(MarkupTagElement tag, Command parent, CommandBar parentCommandBar)
		{
			CommandItem item = null;
			switch (tag.FullName)
			{
				case "CommandReference":
				{
					MarkupAttribute attCommandID = tag.Attributes["CommandID"];
					if (attCommandID != null)
					{
						item = new CommandReferenceCommandItem(attCommandID.Value);
					}
					break;
				}
				case "CommandPlaceholder":
				{
					MarkupAttribute attPlaceholderID = tag.Attributes["PlaceholderID"];
					if (attPlaceholderID != null)
					{
						item = new CommandPlaceholderCommandItem(attPlaceholderID.Value);
					}
					break;
				}
				case "Separator":
				{
					item = new SeparatorCommandItem();
					break;
				}
			}
			
			if (item != null)
			{
				if (parent == null)
				{
					if (parentCommandBar != null)
					{
						parentCommandBar.Items.Add(item);
					}
					else
					{
						mvarMainMenu.Items.Add(item);
					}
				}
				else
				{
					parent.Items.Add(item);
				}
			}
		}
		
		protected virtual void InitializeBranding()
		{
			// I don't know why this ever was WindowsFormsEngine-specific...

			// First, attempt to load the branding from Branding.uxt
			string BrandingFileName = BasePath + System.IO.Path.DirectorySeparatorChar.ToString() + "Branding.uxt";
			if (System.IO.File.Exists(BrandingFileName))
			{
				FileSystemObjectModel fsom = new FileSystemObjectModel();
				FileAccessor fa = new FileAccessor(BrandingFileName);
				Document.Load(fsom, new UXTDataFormat(), fa, false);

				UniversalEditor.ObjectModels.FileSystem.File fileSplashScreenImage = fsom.Files["SplashScreen.png"];
				if (fileSplashScreenImage != null)
				{
					System.IO.MemoryStream ms = new System.IO.MemoryStream(fileSplashScreenImage.GetData());
					// oh, yeah, maybe because of this?
					// LocalConfiguration.SplashScreen.Image = System.Drawing.Image.FromStream(ms);

					// when I did this back in ... who knows when, Universal Widget Toolkit didn't
					// exist, let alone have a UniversalWidgetToolkit.Drawing.Image class...
				}

				UniversalEditor.ObjectModels.FileSystem.File fileSplashScreenSound = fsom.Files["SplashScreen.wav"];
				if (fileSplashScreenSound != null)
				{
					System.IO.MemoryStream ms = new System.IO.MemoryStream(fileSplashScreenSound.GetData());
					// LocalConfiguration.SplashScreen.Sound = ms;
				}

				UniversalEditor.ObjectModels.FileSystem.File fileMainIcon = fsom.Files["MainIcon.ico"];
				if (fileMainIcon != null)
				{
					System.IO.MemoryStream ms = new System.IO.MemoryStream(fileMainIcon.GetData());
					// LocalConfiguration.MainIcon = new System.Drawing.Icon(ms);
				}

				UniversalEditor.ObjectModels.FileSystem.File fileConfiguration = fsom.Files["Configuration.upl"];
				if (fileConfiguration != null)
				{
					System.IO.MemoryStream ms = new System.IO.MemoryStream(fileConfiguration.GetData());

					UniversalEditor.ObjectModels.PropertyList.PropertyListObjectModel plomBranding = new ObjectModels.PropertyList.PropertyListObjectModel();
					Document.Load(plomBranding, new UniversalPropertyListDataFormat(), new StreamAccessor(ms), true);

					LocalConfiguration.ApplicationName = plomBranding.GetValue<string>(new string[] { "Application", "Title" }, String.Empty);

					LocalConfiguration.ColorScheme.DarkColor = Color.FromString(plomBranding.GetValue<string>(new string[] { "ColorScheme", "DarkColor" }, "#2A0068"));
					LocalConfiguration.ColorScheme.LightColor = Color.FromString(plomBranding.GetValue<string>(new string[] { "ColorScheme", "LightColor" }, "#C0C0FF"));
				}

				fa.Close();
			}

			// Now, determine if we should override any branding details with local copies
			if (System.IO.Directory.Exists(BasePath + System.IO.Path.DirectorySeparatorChar.ToString() + "Branding"))
			{
				string SplashScreenImageFileName = String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new string[]
				{
					BasePath,
					"Branding",
					"SplashScreen.png"
				});
				if (System.IO.File.Exists(SplashScreenImageFileName)) LocalConfiguration.SplashScreen.ImageFileName = SplashScreenImageFileName;

				string SplashScreenSoundFileName = String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new string[]
				{
					BasePath,
					"Branding",
					"SplashScreen.wav"
				});
				if (System.IO.File.Exists(SplashScreenSoundFileName)) LocalConfiguration.SplashScreen.SoundFileName = SplashScreenSoundFileName;

				string MainIconFileName = String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new string[]
				{
					BasePath,
					"Branding",
					"MainIcon.ico"
				});
				// if (System.IO.File.Exists(MainIconFileName)) LocalConfiguration.MainIcon = System.Drawing.Icon.ExtractAssociatedIcon(MainIconFileName);

				string ConfigurationFileName = String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new string[]
				{
					BasePath,
					"Branding",
					"Configuration.upl"
				});

				if (System.IO.File.Exists(ConfigurationFileName))
				{
					UniversalEditor.ObjectModels.PropertyList.PropertyListObjectModel plomBranding = new ObjectModels.PropertyList.PropertyListObjectModel();
					Document.Load(plomBranding, new UniversalPropertyListDataFormat(), new FileAccessor(ConfigurationFileName), true);

					LocalConfiguration.ApplicationName = plomBranding.GetValue<string>(new string[] { "Application", "Title" }, "Universal Editor");
					LocalConfiguration.ApplicationShortName = plomBranding.GetValue<string>(new string[] { "Application", "ShortTitle" }, "universal-editor");
					LocalConfiguration.CompanyName = plomBranding.GetValue<string>(new string[] { "Application", "CompanyName" }, "Mike Becker's Software");

					LocalConfiguration.ColorScheme.DarkColor = Color.FromString(plomBranding.GetValue<string>(new string[] { "ColorScheme", "DarkColor" }, "#2A0068"));
					LocalConfiguration.ColorScheme.LightColor = Color.FromString(plomBranding.GetValue<string>(new string[] { "ColorScheme", "LightColor" }, "#C0C0FF"));
				}
			}
		}

		private ConfigurationManager mvarConfigurationManager = new ConfigurationManager();
		public ConfigurationManager ConfigurationManager { get { return mvarConfigurationManager; } }

		private RecentFileManager mvarRecentFileManager = new RecentFileManager();
		public RecentFileManager RecentFileManager { get { return mvarRecentFileManager; } }

		private BookmarksManager mvarBookmarksManager = new BookmarksManager();
		public BookmarksManager BookmarksManager { get { return mvarBookmarksManager; } }

		private SessionManager mvarSessionManager = new SessionManager();
		public SessionManager SessionManager { get { return mvarSessionManager; } set { mvarSessionManager = value; } }

		private static Engine mvarCurrentEngine = null;
		public static Engine CurrentEngine { get { return mvarCurrentEngine; } }

		private Perspective.PerspectiveCollection mvarPerspectives = new Perspective.PerspectiveCollection();
		public Perspective.PerspectiveCollection Perspectives { get { return mvarPerspectives; } }

		protected virtual void UpdateSplashScreenStatus(string message, int progressValue = -1, int progressMinimum = 0, int progressMaximum = 100)
		{
			// most of this is relic from when we had to workaround WinForms
			// threading issues; these threading issues should be
			// automagically handled by the UWT WinForms engine

			/*
			if (LocalConfiguration.SplashScreen.Enabled)
			{
				int spins = 0, maxspins = 30;
				if (splasher == null) return;

				while (splasher == null)
				{
					System.Threading.Thread.Sleep(500);
					if (spins == maxspins) return;
					spins++;
				}
				splasher.InvokeUpdateStatus(message);
			}
			*/
			splasher.SetStatus(message, progressValue, progressMinimum, progressMaximum);
		}

		private void Initialize()
		{
			ShowSplashScreen();

			Application.ShortName = "mbs-editor";
			// Application.Title = "Universal Editor";

			// Initialize the XML files before anything else, since this also loads string tables needed
			// to display the application title
			InitializeXMLConfiguration();

			System.Threading.Thread threadLoader = new System.Threading.Thread(threadLoader_ThreadStart);
			threadLoader.Name = "Initialization Thread";
			threadLoader.Start();

			while (threadLoader.ThreadState == System.Threading.ThreadState.Running)
			{
				System.Threading.Thread.Sleep (500);
			}
		}
		protected virtual void InitializeInternal()
		{
			UpdateSplashScreenStatus("Loading object models...");
			UniversalEditor.Common.Reflection.GetAvailableObjectModels();

			UpdateSplashScreenStatus("Loading data formats...");
			UniversalEditor.Common.Reflection.GetAvailableDataFormats();

			// Initialize Recent File Manager
			mvarRecentFileManager.DataFileName = DataPath + System.IO.Path.DirectorySeparatorChar.ToString() + "RecentItems.xml";
			mvarRecentFileManager.Load();

			// Initialize Bookmarks Manager
			mvarBookmarksManager.DataFileName = DataPath + System.IO.Path.DirectorySeparatorChar.ToString() + "Bookmarks.xml";
			mvarBookmarksManager.Load();

			// Initialize Session Manager
			mvarSessionManager.DataFileName = DataPath + System.IO.Path.DirectorySeparatorChar.ToString() + "Sessions.xml";
			mvarSessionManager.Load();
		}
		private void threadLoader_ThreadStart()
		{
			/*
			if (Configuration.SplashScreen.Enabled)
			{
				while (splasher == null) System.Threading.Thread.Sleep(500);
			}
			*/

			InitializeInternal();
			HideSplashScreen();
		}

		private bool mvarRunning = false;
		public bool Running { get { return mvarRunning; } }

		public void StartApplication()
		{
			Engine.mvarCurrentEngine = this;
			mvarRunning = true;

			string INSTANCEID = GetType().FullName + "$2d429aa3371c421fb63b42525e51a50c$92751853175891031214292357218181357901238$";
			if (ConfigurationManager.GetValue<bool>("SingleInstanceUniquePerDirectory", true))
			{
				// The single instance should be unique per directory
				INSTANCEID += System.Reflection.Assembly.GetEntryAssembly().Location;
			}
			if (!SingleInstanceManager.CreateSingleInstance(INSTANCEID, new EventHandler<SingleInstanceManager.InstanceCallbackEventArgs>(SingleInstanceManager_Callback))) return;

			string[] args1 = Environment.GetCommandLineArgs();
			string[] args = new string[args1.Length - 1];
			Array.Copy(args1, 1, args, 0, args.Length);

			System.Collections.ObjectModel.Collection<string> selectedFileNames = new System.Collections.ObjectModel.Collection<string>();
			if (selectedFileNames.Count == 0 || ConfigurationManager.GetValue<bool>(new string[] { "Application", "Startup", "ForceLoadStartupFileNames" }, false))
			{
				object[] oStartupFileNames = ConfigurationManager.GetValue<object[]>(new string[] { "Application", "Startup", "FileNames" }, new object[0]);
				for (int i = 0; i < oStartupFileNames.Length; i++)
				{
					string startupFileName = oStartupFileNames[i].ToString();
					selectedFileNames.Add(startupFileName);
				}
			}
			foreach (string commandLineArgument in args)
			{
				selectedFileNames.Add(commandLineArgument);
			}
			mvarSelectedFileNames = new System.Collections.ObjectModel.ReadOnlyCollection<string>(selectedFileNames);

			// Set up the base path for the current application. Should this be able to be
			// overridden with a switch (/basepath:...) ?
			mvarBasePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

			BeforeInitialization();

			// Initialize the branding for the selected application
			InitializeBranding();

			Initialize();

			AfterInitializationInternal();
			AfterInitialization();

			OpenWindow(SelectedFileNames.ToArray<string>());

			MainLoop();

			SessionManager.Save();
			BookmarksManager.Save();
			RecentFileManager.Save();
			ConfigurationManager.Save();
		}
		public void RestartApplication()
		{
			RestartApplicationInternal();
		}
		public void StopApplication()
		{
			if (!BeforeStopApplication()) return;
			StopApplicationInternal();
		}
		protected virtual void RestartApplicationInternal()
		{
			// StopApplication();
			// StartApplication();
		}
		protected virtual bool BeforeStopApplication()
		{
			return true;
		}

		public bool ShowCustomOptionDialog(ref DataFormat df, CustomOptionDialogType type)
		{
			CustomOption.CustomOptionCollection coll = null;
			DataFormatReference dfr = df.MakeReference();

			if (type == CustomOptionDialogType.Export)
			{
				coll = dfr.ExportOptions;
			}
			else
			{
				coll = dfr.ImportOptions;
			}
			if (coll.Count == 0) return true;

			bool retval = ShowCustomOptionDialog(ref coll, dfr.Title + " Options", delegate(object sender, EventArgs e)
			{
				ShowAboutDialog(dfr);
			});

			if (retval)
			{
				foreach (CustomOption eo in coll)
				{
					System.Reflection.PropertyInfo pi = dfr.Type.GetProperty(eo.PropertyName);
					if (pi == null) continue;

					if (eo is CustomOptionNumber)
					{
						CustomOptionNumber itm = (eo as CustomOptionNumber);
						pi.SetValue(df, Convert.ChangeType(itm.Value, pi.PropertyType), null);
					}
					else if (eo is CustomOptionBoolean)
					{
						CustomOptionBoolean itm = (eo as CustomOptionBoolean);
						pi.SetValue(df, Convert.ChangeType(itm.Value, pi.PropertyType), null);
					}
					else if (eo is CustomOptionChoice)
					{
						CustomOptionFieldChoice choice = (eo as CustomOptionChoice).Value;
						if (choice != null)
						{
							Type[] interfaces = pi.PropertyType.GetInterfaces();
							bool convertible = false;
							foreach (Type t in interfaces)
							{
								if (t == typeof(IConvertible))
								{
									convertible = true;
									break;
								}
							}
							if (convertible)
							{
								pi.SetValue(df, Convert.ChangeType(choice.Value, pi.PropertyType), null);
							}
							else
							{
								pi.SetValue(df, choice.Value, null);
							}
						}
					}
					else if (eo is CustomOptionText)
					{
						CustomOptionText itm = (eo as CustomOptionText);
						pi.SetValue(df, Convert.ChangeType(itm.Value, pi.PropertyType), null);
					}
				}

				return true;
			}
			return false;
		}
		public bool ShowCustomOptionDialog(ref Accessor df, CustomOptionDialogType type)
		{
			if (df == null) return true;

			CustomOption.CustomOptionCollection coll = null;
			AccessorReference dfr = df.MakeReference();

			if (type == CustomOptionDialogType.Export)
			{
				coll = dfr.ExportOptions;
			}
			else
			{
				coll = dfr.ImportOptions;
			}
			if (coll.Count == 0) return true;

			bool retval = ShowCustomOptionDialog(ref coll, dfr.Title + " Options");

			if (retval)
			{
				foreach (CustomOption eo in coll)
				{
					System.Reflection.PropertyInfo pi = dfr.AccessorType.GetProperty(eo.PropertyName);
					if (pi == null) continue;

					if (eo is CustomOptionNumber)
					{
						CustomOptionNumber itm = (eo as CustomOptionNumber);
						pi.SetValue(df, Convert.ChangeType(itm.Value, pi.PropertyType), null);
					}
					else if (eo is CustomOptionBoolean)
					{
						CustomOptionBoolean itm = (eo as CustomOptionBoolean);
						pi.SetValue(df, Convert.ChangeType(itm.Value, pi.PropertyType), null);
					}
					else if (eo is CustomOptionChoice)
					{
						CustomOptionFieldChoice choice = (eo as CustomOptionChoice).Value;
						if (choice != null)
						{
							Type[] interfaces = pi.PropertyType.GetInterfaces();
							bool convertible = false;
							foreach (Type t in interfaces)
							{
								if (t == typeof(IConvertible))
								{
									convertible = true;
									break;
								}
							}
							if (convertible)
							{
								pi.SetValue(df, Convert.ChangeType(choice.Value, pi.PropertyType), null);
							}
							else
							{
								pi.SetValue(df, choice.Value, null);
							}
						}
					}
					else if (eo is CustomOptionText)
					{
						CustomOptionText itm = (eo as CustomOptionText);
						pi.SetValue(df, Convert.ChangeType(itm.Value, pi.PropertyType), null);
					}
					else if (eo is CustomOptionFile)
					{
						CustomOptionFile itm = (eo as CustomOptionFile);
						pi.SetValue(df, Convert.ChangeType(itm.Value, pi.PropertyType), null);
					}
				}

				return true;
			}
			return false;
		}

		public virtual ActionMenuItem[] CreateMenuItemsFromPlaceholder(PlaceholderMenuItem pmi)
		{
			List<ActionMenuItem> list = new List<ActionMenuItem>();
			switch (pmi.PlaceholderID)
			{
				case "RecentFiles":
				{
					break;
				}
			}
			return list.ToArray();
		}
	}
}
