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
using UniversalEditor.DataFormats.PropertyList.UniversalPropertyList;

using MBS.Framework.UserInterface.Dialogs;
using MBS.Framework.UserInterface.Drawing;
using UniversalEditor.UserInterface.Dialogs;
using MBS.Framework.UserInterface;
using MBS.Framework.UserInterface.Input.Keyboard;
using MBS.Framework.Drawing;
using UniversalEditor.UserInterface.Panels;
using MBS.Framework.UserInterface.Controls;
using MBS.Framework.UserInterface.Controls.Docking;

namespace UniversalEditor.UserInterface
{
	// TODO: Finish implementing ObjectModel Converters... will need to have something
	// like GetAvailableConvertersFrom(Type objectModel) to get a list of available
	// converters that can convert from the specified object model.

	public class Engine
	{
		private static Engine _TheEngine = new Engine();

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
			Application.CommandLine.Options.Add("command", '\0', null, CommandLineOptionValueType.Multiple);

			Application.UniqueName = "net.alcetech.UniversalEditor";
			Application.ShortName = "universal-editor";

			Application.DefaultSettingsProvider.SettingsGroups.Add ("Application:Author Information", new Setting[] {
				new TextSetting("_Name"),
				new TextSetting("_E-mail address")
			});
			Application.DefaultSettingsProvider.SettingsGroups.Add ("Application:Documents", new Setting[] {
			});
			Application.DefaultSettingsProvider.SettingsGroups.Add("Application:Projects and Solutions", new Setting[]
			{
			});
			Application.DefaultSettingsProvider.SettingsGroups.Add("Source Control", new Setting[]
			{
			});

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

			Application.ConfigurationFileNameFilter = "*.uexml";

			Application.BeforeShutdown += Application_BeforeShutdown;
			Application.AfterConfigurationLoaded += Application_AfterConfigurationLoaded;
			Application.Startup += Application_Startup;
			Application.Activated += Application_Activated;

			MBS.Framework.UserInterface.Application.Initialize();
		}

		void Application_AfterConfigurationLoaded(object sender, EventArgs e)
		{
			#region Global Configuration
			{
				UpdateSplashScreenStatus("Loading global configuration");

				MarkupTagElement tagConfiguration = (Application.RawMarkup.FindElement("UniversalEditor", "Configuration") as MarkupTagElement);
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
			#region Object Model Configuration
			{
				UpdateSplashScreenStatus("Loading object model configuration");

				MarkupTagElement tagObjectModels = (Application.RawMarkup.FindElement("UniversalEditor", "ObjectModels") as MarkupTagElement);
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

			Engine.CurrentEngine.UpdateSplashScreenStatus("Loading object models...");
			UniversalEditor.Common.Reflection.GetAvailableObjectModels();

			Engine.CurrentEngine.UpdateSplashScreenStatus("Loading data formats...");
			UniversalEditor.Common.Reflection.GetAvailableDataFormats();

			// Initialize Recent File Manager
			Engine.CurrentEngine.RecentFileManager.DataFileName = Application.DataPath + System.IO.Path.DirectorySeparatorChar.ToString() + "RecentItems.xml";
			Engine.CurrentEngine.RecentFileManager.Load();

			// Initialize Bookmarks Manager
			Engine.CurrentEngine.BookmarksManager.DataFileName = Application.DataPath + System.IO.Path.DirectorySeparatorChar.ToString() + "Bookmarks.xml";
			Engine.CurrentEngine.BookmarksManager.Load();

			// Initialize Session Manager
			Engine.CurrentEngine.SessionManager.DataFileName = Application.DataPath + System.IO.Path.DirectorySeparatorChar.ToString() + "Sessions.xml";
			Engine.CurrentEngine.SessionManager.Load();

			// load editors into memory so we don't wait 10-15 seconds before opening a file
			Common.Reflection.GetAvailableEditors();

			AfterInitialization();
		}


		void Application_BeforeShutdown(object sender, System.ComponentModel.CancelEventArgs e)
		{
			for (int i = 0; i < Application.Windows.Count; i++)
			{
				MainWindow mw = (Application.Windows[i] as MainWindow);
				if (mw == null) continue;

				if (!mw.ConfirmExit())
				{
					e.Cancel = true;
					break;
				}
			}
		}


		void Application_Startup(object sender, EventArgs e)
		{
		}

		void Application_Activated(object sender, ApplicationActivatedEventArgs e)
		{
			Document[] docs = new Document[e.CommandLine.FileNames.Count];
			if (e.CommandLine.FileNames.Count > 0)
			{
				for (int i = 0; i < e.CommandLine.FileNames.Count; i++)
				{
					if (String.IsNullOrEmpty(e.CommandLine.FileNames[i]))
					{
						Console.WriteLine("ue: warning: ignoring empty file name passed to command line");
						continue;
					}

					docs[i] = new Document(new FileAccessor(e.CommandLine.FileNames[i]));
				}
			}

			if (LastWindow != null)
			{
				LastWindow.OpenFile(docs);
			}
			else
			{
				OpenWindow(docs);
			}

			List<string> commandsToExecute = (Application.CommandLine.Options.GetValueOrDefault<List<string>>("command", null));
			if (commandsToExecute != null)
			{
				for (int i = 0; i < commandsToExecute.Count; i++)
				{
					Application.ExecuteCommand(commandsToExecute[i]);
				}
			}
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

			Application.Title = "Universal Editor";
			MBS.Framework.UserInterface.Application.Start();

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
					UniversalEditor.UserInterface.Dialogs.AboutDialog dlg = new UniversalEditor.UserInterface.Dialogs.AboutDialog ();
					dlg.ShowDialog ();
				}
				else
				{
					MBS.Framework.UserInterface.Dialogs.AboutDialog dlg = new MBS.Framework.UserInterface.Dialogs.AboutDialog ();
					dlg.ProgramName = "Universal Editor";
					dlg.Version = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
					dlg.Copyright = "(c) 1997-2019 Michael Becker";
					dlg.Comments = "A modular, extensible document editor";
					dlg.LicenseType = MBS.Framework.UserInterface.LicenseType.GPL30;
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

		private static Engine[] m_AvailableEngines = null;
		public static Engine[] GetAvailableEngines()
		{
			return new Engine[] { _TheEngine };
		}

		private void AfterInitialization()
		{
			// Initialize all the commands that are common to UniversalEditor
			#region File
			Application.AttachCommandEventHandler("FileNewDocument", delegate(object sender, EventArgs e)
			{
				LastWindow.NewFile();
			});
			Application.AttachCommandEventHandler("FileNewProject", delegate(object sender, EventArgs e)
			{
				LastWindow.NewProject();
			});
			Application.AttachCommandEventHandler("FileOpenDocument", delegate(object sender, EventArgs e)
			{
				LastWindow.OpenFile();
			});
			Application.AttachCommandEventHandler("FileOpenProject", delegate(object sender, EventArgs e)
			{
				LastWindow.OpenProject();
			});
			Application.AttachCommandEventHandler("FileSaveDocument", delegate(object sender, EventArgs e)
			{
				LastWindow.SaveFile();
			});
			Application.AttachCommandEventHandler("FileSaveDocumentAs", delegate(object sender, EventArgs e)
			{
				LastWindow.SaveFileAs();
			});
			Application.AttachCommandEventHandler("FileSaveProject", delegate(object sender, EventArgs e)
			{
				LastWindow.SaveProject();
			});
			Application.AttachCommandEventHandler("FileSaveProjectAs", delegate(object sender, EventArgs e)
			{
				LastWindow.SaveProjectAs();
			});
			Application.AttachCommandEventHandler("FileSaveAll", delegate(object sender, EventArgs e)
			{
				LastWindow.SaveAll();
			});
			Application.AttachCommandEventHandler("FileCloseDocument", delegate(object sender, EventArgs e)
			{
				LastWindow.CloseFile();
			});
			Application.AttachCommandEventHandler("FileCloseProject", delegate(object sender, EventArgs e)
			{
				LastWindow.CloseProject();
			});
			Application.AttachCommandEventHandler("FilePrint", delegate(object sender, EventArgs e)
			{
				LastWindow.PrintDocument();
			});
			Application.AttachCommandEventHandler("FileProperties", delegate (object sender, EventArgs e)
			{
				LastWindow.ShowDocumentPropertiesDialog();
			});
			Application.AttachCommandEventHandler("FileRestart", delegate(object sender, EventArgs e)
			{
				RestartApplication();
			});
			Application.AttachCommandEventHandler("FileExit", delegate(object sender, EventArgs e)
			{
				StopApplication();
			});
			#endregion
			#region Edit
			Application.AttachCommandEventHandler("EditCut", delegate(object sender, EventArgs e)
			{
				Editor editor = LastWindow.GetCurrentEditor();
				if (editor == null) return;
				editor.Cut();
			});
			Application.AttachCommandEventHandler("EditCopy", delegate(object sender, EventArgs e)
			{
				Editor editor = LastWindow.GetCurrentEditor();
				if (editor == null) return;
				editor.Copy();
			});
			Application.AttachCommandEventHandler("EditPaste", delegate(object sender, EventArgs e)
			{
				Editor editor = LastWindow.GetCurrentEditor();
				if (editor == null) return;
				editor.Paste();
			});
			Application.AttachCommandEventHandler("EditDelete", delegate(object sender, EventArgs e)
			{
				Editor editor = LastWindow.GetCurrentEditor();
				if (editor != null)
				{
					editor.Delete();
				}
				else
				{
					Control ctl = LastWindow.ActiveControl;
					if (ctl is ListView && ctl.Parent is SolutionExplorerPanel)
					{
						(ctl.Parent as SolutionExplorerPanel).Delete();
					}
				}
			});
			Application.AttachCommandEventHandler("EditUndo", delegate(object sender, EventArgs e)
			{
				Editor editor = LastWindow.GetCurrentEditor();
				if (editor == null) return;
				editor.Undo();
			});
			Application.AttachCommandEventHandler("EditRedo", delegate(object sender, EventArgs e)
			{
				Editor editor = LastWindow.GetCurrentEditor();
				if (editor == null) return;
				editor.Redo();
			});
			#endregion
			#region View
			Application.AttachCommandEventHandler("ViewFullScreen", delegate(object sender, EventArgs e)
			{
				Command cmd = (sender as Command);
				LastWindow.FullScreen = !LastWindow.FullScreen;
				cmd.Checked = LastWindow.FullScreen;
			});
			#region Perspective
			Application.AttachCommandEventHandler("ViewPerspective1", delegate(object sender, EventArgs e)
			{
				HostApplication.CurrentWindow.SwitchPerspective(1);
			});
			Application.AttachCommandEventHandler("ViewPerspective2", delegate(object sender, EventArgs e)
			{
				HostApplication.CurrentWindow.SwitchPerspective(2);
			});
			Application.AttachCommandEventHandler("ViewPerspective3", delegate(object sender, EventArgs e)
			{
				HostApplication.CurrentWindow.SwitchPerspective(3);
			});
			Application.AttachCommandEventHandler("ViewPerspective4", delegate(object sender, EventArgs e)
			{
				HostApplication.CurrentWindow.SwitchPerspective(4);
			});
			Application.AttachCommandEventHandler("ViewPerspective5", delegate(object sender, EventArgs e)
			{
				HostApplication.CurrentWindow.SwitchPerspective(5);
			});
			Application.AttachCommandEventHandler("ViewPerspective6", delegate(object sender, EventArgs e)
			{
				HostApplication.CurrentWindow.SwitchPerspective(6);
			});
			Application.AttachCommandEventHandler("ViewPerspective7", delegate(object sender, EventArgs e)
			{
				HostApplication.CurrentWindow.SwitchPerspective(7);
			});
			Application.AttachCommandEventHandler("ViewPerspective8", delegate(object sender, EventArgs e)
			{
				HostApplication.CurrentWindow.SwitchPerspective(8);
			});
			Application.AttachCommandEventHandler("ViewPerspective9", delegate(object sender, EventArgs e)
			{
				HostApplication.CurrentWindow.SwitchPerspective(9);
			});
			#endregion

			Application.AttachCommandEventHandler("ViewStartPage", delegate(object sender, EventArgs e)
			{
				HostApplication.CurrentWindow.ShowStartPage();
			});
			Application.AttachCommandEventHandler("ViewStatusBar", delegate (object sender, EventArgs e)
			{
				HostApplication.CurrentWindow.StatusBar.Visible = !HostApplication.CurrentWindow.StatusBar.Visible;
				Application.Commands["ViewStatusBar"].Checked = HostApplication.CurrentWindow.StatusBar.Visible;
			});

			#endregion
			#region Tools
			// ToolsOptions should actually be under the Edit menu as "Preferences" on Linux systems
			Application.AttachCommandEventHandler("ToolsOptions", delegate(object sender, EventArgs e)
			{
				LastWindow.ShowOptionsDialog();
			});
			#endregion
			#region Window
			Application.AttachCommandEventHandler("WindowNewWindow", delegate(object sender, EventArgs e)
			{
				OpenWindow();
			});
			Application.AttachCommandEventHandler("WindowWindows", delegate(object sender, EventArgs e)
			{
				LastWindow.SetWindowListVisible(true, true);
			});
			#endregion
			#region Help
			Application.AttachCommandEventHandler("HelpViewHelp", delegate (object sender, EventArgs e)
			{
				Application.ShowHelp();
			});
			Application.AttachCommandEventHandler("HelpCustomerFeedbackOptions", delegate (object sender, EventArgs e)
			{
				// MessageDialog.ShowDialog("This product has already been activated.", "Licensing and Activation", MessageDialogButtons.OK, MessageDialogIcon.Information);
				TaskDialog td = new TaskDialog();
				td.ButtonStyle = TaskDialogButtonStyle.Commands;

				td.Prompt = "Please open a trouble ticket on GitHub if you need support.";
				td.Text = "Customer Experience Improvement Program";
				td.Content = String.Format("You are using the GNU GPLv3 licensed version of {0}. This program comes with ABSOLUTELY NO WARRANTY.\r\n\r\nSupport contracts may be available for purchase; please contact your software distributor.", Application.Title);
				td.Footer = "<a href=\"GPLLicense\">View the license terms</a>";
				td.EnableHyperlinks = true;
				td.HyperlinkClicked += Td_HyperlinkClicked;

				td.Icon = TaskDialogIcon.SecurityOK;
				td.Parent = (Window)CurrentEngine.LastWindow;
				td.ShowDialog();
			});
			Application.AttachCommandEventHandler("HelpLicensingAndActivation", delegate (object sender, EventArgs e)
			{
				// MessageDialog.ShowDialog("This product has already been activated.", "Licensing and Activation", MessageDialogButtons.OK, MessageDialogIcon.Information);
				TaskDialog td = new TaskDialog();
				td.ButtonStyle = TaskDialogButtonStyle.Commands;

				td.Prompt = "This product has already been activated.";
				td.Text = "Licensing and Activation";
				td.Content = String.Format("You are using the GNU GPLv3 licensed version of {0}. No activation is necessary.", Application.Title);
				td.Footer = "<a href=\"GPLLicense\">View the license terms</a>";
				td.EnableHyperlinks = true;
				td.HyperlinkClicked += Td_HyperlinkClicked;

				td.Icon = TaskDialogIcon.SecurityOK;
				td.Parent = (Window)CurrentEngine.LastWindow;
				td.ShowDialog();
			});
			Application.AttachCommandEventHandler("HelpAboutPlatform", delegate(object sender, EventArgs e)
			{
				ShowAboutDialog();
			});
			#endregion


			Application.AttachCommandEventHandler("DockingContainerContextMenu_Close", delegate (object sender, EventArgs e)
			{
				CommandEventArgs ce = (e as CommandEventArgs);
				if (ce != null)
				{
					DockingWindow dw = ce.GetNamedParameter<DockingWindow>("Item");
					LastWindow?.CloseFile(dw);
				}
			});
			Application.AttachCommandEventHandler("DockingContainerContextMenu_CloseAll", delegate (object sender, EventArgs e)
			{
				LastWindow?.CloseWindow();
			});
			Application.AttachCommandEventHandler("DockingContainerContextMenu_CloseAllButThis", delegate (object sender, EventArgs e)
			{
				MessageDialog.ShowDialog("Not implemented ... yet", "Error", MessageDialogButtons.OK, MessageDialogIcon.Error);
			});


			#region Dynamic Commands
			#region View
			#region Panels
			for (int i = Application.CommandBars.Count - 1; i >= 0; i--)
			{
				Command cmdViewToolbarsToolbar = new Command();
				cmdViewToolbarsToolbar.ID = "ViewToolbars" + i.ToString();
				cmdViewToolbarsToolbar.Title = Application.CommandBars[i].Title;
				cmdViewToolbarsToolbar.Executed += cmdViewToolbarsToolbar_Executed;
				Application.Commands.Add(cmdViewToolbarsToolbar);
				Application.Commands["ViewToolbars"].Items.Insert(0, new CommandReferenceCommandItem(cmdViewToolbarsToolbar.ID));
			}
			#endregion
			#region Panels
			if (Application.Commands["ViewPanels"] != null)
			{
				Command cmdViewPanels1 = new Command();
				cmdViewPanels1.ID = "ViewPanels1";
				Application.Commands.Add(cmdViewPanels1);
				Application.Commands["ViewPanels"].Items.Add(new CommandReferenceCommandItem("ViewPanels1"));
			}
			#endregion
			#endregion
			#endregion

			#region Language Strings
			#region Help
			Command helpAboutPlatform = Application.Commands["HelpAboutPlatform"];
			if (helpAboutPlatform != null)
			{
				helpAboutPlatform.Title = String.Format(helpAboutPlatform.Title, Application.DefaultLanguage.GetStringTableEntry("Application.Title", "Universal Editor"));
			}

			Command helpLanguage = Application.Commands["HelpLanguage"];
			if (helpLanguage != null)
			{
				foreach (Language lang in Application.Languages)
				{
					Command cmdLanguage = new Command();
					cmdLanguage.ID = "HelpLanguage_" + lang.ID;
					cmdLanguage.Title = lang.Title;
					cmdLanguage.Executed += delegate(object sender, EventArgs e)
					{
						HostApplication.Messages.Add(HostApplicationMessageSeverity.Notice, "Clicked language " + lang.ID);
					};
					Application.Commands.Add(cmdLanguage);

					helpLanguage.Items.Add(new CommandReferenceCommandItem("HelpLanguage_" + lang.ID));
				}
			}
			#endregion
			#endregion
		}

		void Td_HyperlinkClicked(object sender, TaskDialogHyperlinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("https://www.gnu.org/licenses/gpl-3.0.en.html");
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

		private System.Collections.ObjectModel.ReadOnlyCollection<string> mvarSelectedFileNames = null;
		public System.Collections.ObjectModel.ReadOnlyCollection<string> SelectedFileNames { get { return mvarSelectedFileNames; } }

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
			MainWindow mw = new MainWindow();
			mw.Show();
			// OpenWindow(new Document[0]);
		}
		/// <summary>
		/// Opens a new window, optionally loading the specified documents.
		/// </summary>
		/// <param name="fileNames">The file name(s) of the document(s) to load.</param>
		public void OpenWindow(params string[] fileNames)
		{
			List<Document> loadedDocuments = new List<Document>();
			for (int i = 0; i < fileNames.Length; i++)
			{
				AccessorReference[] accs = UniversalEditor.Common.Reflection.GetAvailableAccessors(fileNames[i]);
				if (accs.Length > 0)
				{
					Accessor fa = accs[0].Create();
					fa.OriginalUri = new Uri(fileNames[i]);
					Document document = new Document(fa);
					loadedDocuments.Add(document);
				}
				else
				{
					Console.Error.WriteLine("ue: accessor not found for path " + fileNames[i] + "; assuming local file");
					loadedDocuments.Add(new Document(new FileAccessor(fileNames[i])));
				}
			}
			OpenWindow(loadedDocuments.ToArray());
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
		public string ExpandRelativePath(string relativePath)
		{
			if (relativePath.StartsWith("~/"))
			{
				string[] potentialFileNames = MBS.Framework.UserInterface.Application.EnumerateDataPaths();
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

		protected virtual void InitializeBranding()
		{
			// I don't know why this ever was WindowsFormsEngine-specific...

			// First, attempt to load the branding from Branding.uxt
			/*
			string BrandingFileName = Application.BasePath + System.IO.Path.DirectorySeparatorChar.ToString() + "Branding.uxt";
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
					// exist, let alone have a MBS.Framework.UserInterface.Drawing.Image class...
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
			if (System.IO.Directory.Exists(Application.BasePath + System.IO.Path.DirectorySeparatorChar.ToString() + "Branding"))
			{
				string SplashScreenImageFileName = String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new string[]
				{
					Application.BasePath,
					"Branding",
					"SplashScreen.png"
				});
				if (System.IO.File.Exists(SplashScreenImageFileName)) LocalConfiguration.SplashScreen.ImageFileName = SplashScreenImageFileName;

				string SplashScreenSoundFileName = String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new string[]
				{
					Application.BasePath,
					"Branding",
					"SplashScreen.wav"
				});
				if (System.IO.File.Exists(SplashScreenSoundFileName)) LocalConfiguration.SplashScreen.SoundFileName = SplashScreenSoundFileName;

				string MainIconFileName = String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new string[]
				{
					Application.BasePath,
					"Branding",
					"MainIcon.ico"
				});
				// if (System.IO.File.Exists(MainIconFileName)) LocalConfiguration.MainIcon = System.Drawing.Icon.ExtractAssociatedIcon(MainIconFileName);

				string ConfigurationFileName = String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new string[]
				{
					Application.BasePath,
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
			*/
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

		protected internal virtual void UpdateSplashScreenStatus(string message, int progressValue = -1, int progressMinimum = 0, int progressMaximum = 100)
		{
			Console.WriteLine(message);
		}

		private void Initialize()
		{
		}
		protected virtual void InitializeInternal()
		{
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

			TemporaryFileManager.RegisterTemporaryDirectory();

			BeforeInitialization();

			// Initialize the branding for the selected application
			InitializeBranding();

			Initialize();

			MainLoop();

			SessionManager.Save();
			BookmarksManager.Save();
			RecentFileManager.Save();
			ConfigurationManager.Save();

			TemporaryFileManager.UnregisterTemporaryDirectory();
		}
		public void RestartApplication()
		{
			RestartApplicationInternal();
		}
		public void StopApplication()
		{
			if (!BeforeStopApplication()) return;
			Application.Stop();
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
				ApplyCustomOptions (ref df, coll);
				return true;
			}
			return false;
		}

		public void ApplyCustomOptions (ref Accessor df, CustomOption.CustomOptionCollection coll)
		{
			AccessorReference dfr = df.MakeReference ();
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