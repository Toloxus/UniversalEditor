﻿//
//  IcarusScriptEditor.cs - provides a UWT-based Editor for an IcarusScriptObjectModel
//
//  Author:
//       Michael Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2019-2020 Mike Becker's Software
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
using System.ComponentModel;
using System.Text;

using MBS.Framework.UserInterface;
using MBS.Framework.UserInterface.Controls;
using MBS.Framework.UserInterface.Dialogs;

using UniversalEditor.ObjectModels.Icarus;
using UniversalEditor.ObjectModels.Icarus.Commands;
using UniversalEditor.ObjectModels.Icarus.Expressions;
using UniversalEditor.ObjectModels.Icarus.Parameters;
using UniversalEditor.ObjectModels.Markup;
using UniversalEditor.Plugins.RavenSoftware.UserInterface.Dialogs.Icarus;
using UniversalEditor.UserInterface;

namespace UniversalEditor.Plugins.RavenSoftware.UserInterface.Editors.Icarus
{
	/// <summary>
	/// Provides a UWT-based <see cref="Editor" /> for an <see cref="IcarusScriptObjectModel" />.
	/// </summary>
	public partial class IcarusScriptEditor : Editor
	{
		public static IcarusScriptEditorConfiguration IcarusConfiguration { get; } = new IcarusScriptEditorConfiguration();

		private static EditorReference _er = null;
		public override EditorReference MakeReference()
		{
			if (_er == null)
			{
				_er = base.MakeReference();
				_er.SupportedObjectModels.Add(typeof(IcarusScriptObjectModel));
				_er.ConfigurationLoaded += _er_ConfigurationLoaded;
			}
			return _er;
		}

		protected override void OnToolboxItemActivated(ToolboxItemEventArgs e)
		{
			base.OnToolboxItemActivated(e);
			IcarusScriptEditorCommand cmd = e.Item.GetExtraData<IcarusScriptEditorCommand>("command");
			RecursiveAddCommand(ScriptEditorCommandToOMCommand(cmd));
		}

		private IcarusCommand ScriptEditorCommandToOMCommand(IcarusScriptEditorCommand cmd)
		{
			IcarusCommand command = IcarusCommand.CreateFromName(cmd.Name);
			for (int i = 0; i < cmd.Parameters.Count; i++)
			{
				if (i < command.Parameters.Count)
				{
					command.Parameters[i] = cmd.Parameters[i];
				}
				else
				{
					command.Parameters.Add(cmd.Parameters[i]);
				}
			}
			return command;
		}

		private void _er_ConfigurationLoaded(object sender, EventArgs e)
		{
			if (_er.Configuration != null)
			{
				MarkupTagElement tagConfiguration = (_er.Configuration.Elements["Configuration"] as MarkupTagElement);
				if (tagConfiguration != null)
				{
					MarkupTagElement tagEnumerations = (tagConfiguration.Elements["Enumerations"] as MarkupTagElement);
					if (tagEnumerations != null)
					{
						for (int i = 0; i < tagEnumerations.Elements.Count; i++)
						{
							MarkupTagElement tagEnumeration = (tagEnumerations.Elements[i] as MarkupTagElement);
							if (tagEnumeration == null) continue;
							if (tagEnumeration.FullName != "Enumeration") continue;

							MarkupAttribute attName = tagEnumeration.Attributes["Name"];
							if (attName == null) continue;

							IcarusScriptEditorEnumeration _enum = new IcarusScriptEditorEnumeration();
							_enum.Name = attName.Value;

							MarkupAttribute attDescription = tagEnumeration.Attributes["Description"];
							if (attDescription != null)
							{
								_enum.Description = attDescription.Value;
							}

							MarkupAttribute attValue = tagEnumeration.Attributes["Value"];
							if (attValue != null)
							{
								_enum.Value = new IcarusConstantExpression(attValue.Value);
							}
							else
							{
								_enum.Value = new IcarusConstantExpression(attName.Value);
							}

							IcarusConfiguration.Enumerations.Add(_enum);
						}
					}

					MarkupTagElement tagCommands = (tagConfiguration.Elements["IcarusCommands"] as MarkupTagElement);
					if (tagCommands != null)
					{
						for (int i = 0; i < tagCommands.Elements.Count; i++)
						{
							MarkupTagElement tagCommand = (tagCommands.Elements[i] as MarkupTagElement);
							if (tagCommand == null) continue;
							if (tagCommand.FullName != "IcarusCommand") continue;

							MarkupAttribute attName = tagCommand.Attributes["Name"];
							if (attName == null) continue;

							IcarusScriptEditorCommand cmd = new IcarusScriptEditorCommand();
							cmd.Name = attName.Value;

							MarkupAttribute attIcon = tagCommand.Attributes["Icon"];
							if (attIcon != null)
							{
								cmd.IconName = attIcon.Value;
							}
							MarkupAttribute attDescription = tagCommand.Attributes["Description"];
							if (attDescription != null)
							{
								cmd.Description = attDescription.Value;
							}

							MarkupTagElement tagParameters = tagCommand.Elements["Parameters"] as MarkupTagElement;
							if (tagParameters != null)
							{
								for (int j = 0; j < tagParameters.Elements.Count; j++)
								{
									MarkupTagElement tagParameter = tagParameters.Elements[j] as MarkupTagElement;
									if (tagParameter == null) continue;
									if (tagParameter.FullName != "Parameter") continue;

									MarkupAttribute attParameterName = tagParameter.Attributes["Name"];
									if (attParameterName == null) continue;

									IcarusGenericParameter parm = new IcarusGenericParameter(attParameterName.Value);

									MarkupAttribute attParameterValue = tagParameter.Attributes["Value"];
									MarkupAttribute attParameterEnumeration = tagParameter.Attributes["Enumeration"];

									if (attParameterValue != null)
									{
										parm.Value = new IcarusConstantExpression(attParameterValue.Value);
									}
									if (attParameterEnumeration != null)
									{
										parm.EnumerationName = attParameterEnumeration.Value;
									}
									cmd.Parameters.Add(parm);
								}
							}

							IcarusConfiguration.Commands.Add(cmd);

							ToolboxItem tbi = new ToolboxCommandItem(cmd.Name, cmd.Name);
							tbi.SetExtraData<IcarusScriptEditorCommand>("command", cmd);
							_er.Toolbox.Items.Add(tbi);
						}
					}
				}
			}
		}

		protected override EditorSelection CreateSelectionInternal(object content)
		{
			return null;
		}
		public override void UpdateSelections()
		{
		}

		public IcarusScriptEditor()
		{
			InitializeComponent();
			// mnuContextRun.Font = new Font(mnuContextRun.Font, FontStyle.Bold);
			// mnuContextRun.IsDefault = true;

			/*
			string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
			string iconPath = path + System.IO.Path.DirectorySeparatorChar.ToString() + "../Editors/Icarus/Images";
			string[] iconFileNames = System.IO.Directory.GetFiles(iconPath, "*.png");
			foreach (string iconFileName in iconFileNames)
			{
				Image image = Image.FromFile(iconFileName);
				string fileTitle = System.IO.Path.GetFileNameWithoutExtension(iconFileName);
				imlSmallIcons.Images.Add(fileTitle, image);
			}
			*/

			// tv.ImageList = SmallImageList;

			// TODO: figure out why menutiems have to use Application.AttachCommand... and context menus have to use Context
			Application.AttachCommandEventHandler("Icarus_Debug_StartDebugging", mnuDebugStart_Click);
			Application.AttachCommandEventHandler("Icarus_Debug_BreakExecution", mnuDebugBreak_Click);
			Application.AttachCommandEventHandler("Icarus_Debug_StopDebugging", mnuDebugStop_Click);
			Application.AttachCommandEventHandler("Icarus_Debug_StepInto", mnuDebugStepInto_Click);
			Application.AttachCommandEventHandler("Icarus_Debug_StepOver", mnuDebugStepOver_Click);

			Context.AttachCommandEventHandler("Icarus_ContextMenu_TEST_EXPRESSION_EDITOR", TestExpressionEditor);

			// Commands["Icarus_Debug_BreakExecution"].Visible = false;
			// Commands["Icarus_Debug_BreakExecution"].Visible = false;
			// Commands["Icarus_Debug_StopDebugging"].Visible = false;
		}

		private System.Threading.Thread tDebugger = null;
		private void mnuDebugStart_Click(object sender, EventArgs e)
		{
			// Commands["Icarus_Debug_StartDebugging"].Visible = false;
			// Commands["Icarus_Debug_BreakExecution"].Visible = true;
			// Commands["Icarus_Debug_StopDebugging"].Visible = true;

			IcarusScriptObjectModel script = (ObjectModel as IcarusScriptObjectModel);
			tv.SelectedRows.Clear();
			tasksByName.Clear();

			if (tDebugger != null)
			{
				if (tDebugger.IsAlive) tDebugger.Abort();
				tDebugger = null;
			}

			tDebugger = new System.Threading.Thread(tDebugger_Start);
			tDebugger.Start();
		}
		private void mnuDebugBreak_Click(object sender, EventArgs e)
		{

		}
		private void mnuDebugStop_Click(object sender, EventArgs e)
		{
			// (MenuBar.Items["mnuDebug"] as ActionMenuItem).Items["mnuDebugStart"].Visible = true;
			// (MenuBar.Items["mnuDebug"] as ActionMenuItem).Items["mnuDebugBreak"].Visible = false;
			// (MenuBar.Items["mnuDebug"] as ActionMenuItem).Items["mnuDebugStop"].Visible = false;

			if (tDebugger != null)
			{
				if (tDebugger.IsAlive) tDebugger.Abort();
				tDebugger = null;
			}

			if (_prevTreeNode != null)
			{
				// _prevTreeNode.BackColor = Color.Empty;
				_prevTreeNode = null;
			}
		}

		private void LogOutputWindow(string text)
		{
			HostApplication.OutputWindow.WriteLine(text);
		}
		private void ClearOutputWindow()
		{
			HostApplication.OutputWindow.Clear();
		}

		private Dictionary<IcarusCommand, TreeModelRow> treeNodesForCommands = new Dictionary<IcarusCommand, TreeModelRow>();

		private void tDebugger_Start()
		{
			ClearOutputWindow();
			LogOutputWindow("=== ICARUS Engine Debugger v1.0 - copyright (c) 2013 Mike Becker's Software ===");

			DateTime dtStart = DateTime.Now;

			IcarusScriptObjectModel script = (ObjectModel as IcarusScriptObjectModel);
			foreach (IcarusCommand command in script.Commands)
			{
				try
				{
					DebugCommand(command);
				}
				catch (InvalidOperationException ex)
				{
					LogOutputWindow("unknown command (" + (script.Commands.IndexOf(command) + 1).ToString() + " of " + script.Commands.Count.ToString() + "): " + command.GetType().Name);
				}
			}

			if (_prevTreeNode != null)
			{
				ReleaseTreeNode(_prevTreeNode);
				_prevTreeNode = null;
			}

			DateTime dtEnd = DateTime.Now;

			TimeSpan tsDiff = dtEnd - dtStart;
			LogOutputWindow("execution complete, " + tsDiff.ToString() + " elapsed since execution started");

			UpdateMenuItems(true);
		}

		private void UpdateMenuItems(bool enable)
		{
			// (MenuBar.Items["mnuDebug"] as ActionMenuItem).Items["mnuDebugStart"].Visible = enable;
			// (MenuBar.Items["mnuDebug"] as ActionMenuItem).Items["mnuDebugBreak"].Visible = !enable;
			// (MenuBar.Items["mnuDebug"] as ActionMenuItem).Items["mnuDebugStop"].Visible = !enable;
		}

		private TreeModelRow _prevTreeNode = null;
		private void ActivateTreeNode(TreeModelRow tn)
		{
			// tn.EnsureVisible();
			// tn.BackColor = Color.Yellow;
		}
		private void ReleaseTreeNode(TreeModelRow tn)
		{
			// tn.BackColor = Color.Empty;
		}

		private Dictionary<string, IcarusCommandTask> tasksByName = new Dictionary<string, IcarusCommandTask>();

		private void DebugCommand(IcarusCommand command)
		{
			if (_prevTreeNode != null)
			{
				ReleaseTreeNode(_prevTreeNode);
				_prevTreeNode = null;
			}

			TreeModelRow tn = treeNodesForCommands[command];
			ActivateTreeNode(tn);
			_prevTreeNode = tn;

			Action<string> _LogOutputWindow = new Action<string>(LogOutputWindow);
			if (command is IcarusCommandAffect)
			{
				IcarusCommandAffect cmd = (command as IcarusCommandAffect);
				LogOutputWindow("on " + cmd.Target.GetValue<string>() + "\r\n{");
				foreach (IcarusCommand command1 in cmd.Commands)
				{
					DebugCommand(command1);
				}
				LogOutputWindow("}");
			}
			else if (command is IcarusCommandSet)
			{
				IcarusCommandSet cmd = (command as IcarusCommandSet);
				LogOutputWindow("set " + cmd.ObjectName + " = " + (cmd.Value == null ? "(null)" : cmd.Value.ToString()));
			}
			else if (command is IcarusCommandWait)
			{
				IcarusCommandWait cmd = (command as IcarusCommandWait);
				int timeout = (int)cmd.Duration.GetValue<int>();
				System.Threading.Thread.Sleep(timeout);
			}
			else if (command is IcarusCommandPrint)
			{
				IcarusCommandPrint cmd = (command as IcarusCommandPrint);
				string text = cmd.Text.GetValue<string>();
				LogOutputWindow(text);
			}
			else if (command is IcarusCommandTask)
			{
				IcarusCommandTask cmd = (command as IcarusCommandTask);
				if (tasksByName.ContainsKey(cmd.TaskName.GetValue<string>()))
				{
					LogOutputWindow("WARNING: redefining task \"" + cmd.TaskName + "\"");
				}
				tasksByName[cmd.TaskName.GetValue<string>()] = cmd;
			}
			else if (command is IcarusCommandControlFlowDo)
			{
				IcarusCommandControlFlowDo cmd = (command as IcarusCommandControlFlowDo);
				string targetName = cmd.Target.GetValue<string>();
				if (targetName != null)
				{
					if (!tasksByName.ContainsKey(targetName))
					{
						LogOutputWindow("ERROR: task \"" + cmd.Target + "\" not found!");
						return;
					}

					IcarusCommandTask task = tasksByName[targetName];
					foreach (IcarusCommand command1 in task.Commands)
					{
						DebugCommand(command1);
					}
				}
				else
				{
					LogOutputWindow("ERROR: cmd called null target");
				}
			}
			else if (command is IcarusCommandLoop)
			{
				IcarusCommandLoop cmd = (command as IcarusCommandLoop);
				float timeout = (float)cmd.Count.GetValue<float>();
				if (timeout == -1)
				{
					while (true)
					{
						foreach (IcarusCommand command1 in cmd.Commands)
						{
							DebugCommand(command1);
						}
					}
				}
				else
				{
					for (float i = 0; i < timeout; i++)
					{
						foreach (IcarusCommand command1 in cmd.Commands)
						{
							DebugCommand(command1);
						}
					}
				}
			}
			else if (command is IcarusCommandControlFlowDo)
			{
			}
			else
			{
				throw new InvalidOperationException();
			}

			System.Threading.Thread.Sleep(50);
		}

		private void mnuDebugStepInto_Click(object sender, EventArgs e)
		{
			MessageDialog.ShowDialog("Step Into Icarus Script", "Information", MessageDialogButtons.OK);
		}
		private void mnuDebugStepOver_Click(object sender, EventArgs e)
		{
			MessageDialog.ShowDialog("Step Over Icarus Script", "Information", MessageDialogButtons.OK);
		}

		protected override void OnDocumentClosing(CancelEventArgs e)
		{
			base.OnDocumentClosing(e);

			if (tDebugger != null && tDebugger.IsAlive)
			{
				if (MessageDialog.ShowDialog("Do you want to stop debugging?", "ICARUS Debugger", MessageDialogButtons.YesNo, MessageDialogIcon.Warning) == DialogResult.No)
				{
					e.Cancel = true;
					return;
				}

				tDebugger.Abort();
			}
		}

		protected override void OnObjectModelChanged(EventArgs e)
		{
			base.OnObjectModelChanged(e);

			tv.Model.Rows.Clear();
			treeNodesForCommands.Clear();

			IcarusScriptObjectModel script = (ObjectModel as IcarusScriptObjectModel);
			if (script == null) return;

			foreach (IcarusCommand command in script.Commands)
			{
				RecursiveAddCommand(command);
			}
		}

		private void RecursiveAddCommand(IcarusCommand command, TreeModelRow parent = null)
		{
			TreeModelRow tn = new TreeModelRow();
			if (command == null) return;

			tn.RowColumns.Add(new TreeModelRowColumn(tv.Model.Columns[0], GetCommandText(command)));

			if (command is IIcarusContainerCommand)
			{
				IIcarusContainerCommand container = (command as IIcarusContainerCommand);
				foreach (IcarusCommand ic1 in container.Commands)
				{
					RecursiveAddCommand(ic1, tn);
				}
			}
			tn.SetExtraData<IcarusCommand>("cmd", command);
			treeNodesForCommands.Add(command, tn);

			if (parent == null)
			{
				tv.Model.Rows.Add(tn);
			}
			else
			{
				parent.Rows.Add(tn);
			}
		}

		private string GetCommandText(IcarusCommand command)
		{
			StringBuilder sb = new StringBuilder();
			if (command == null) return sb.ToString();

			if (command is IcarusPredefinedCommand)
			{
				sb.Append((command as IcarusPredefinedCommand).Name);
			}
			else if (command is IcarusCustomCommand)
			{
				sb.Append((command as IcarusCustomCommand).CommandType.ToString());
			}
			// tn.ImageKey = command.GetType().Name;
			// tn.SelectedImageKey = command.GetType().Name;

			if (!(command is IcarusCommandMacro))
			{
				sb.Append("                ( ");
				for (int i = 0; i < command.Parameters.Count; i++)
				{
					if (command.Parameters[i].Value != null)
					{
						sb.Append(command.Parameters[i].Value);
					}
					else
					{
						sb.Append("null");
					}

					if (i < command.Parameters.Count - 1)
						sb.Append(", ");
				}
				sb.Append(" )");
			}

			if (command is IIcarusContainerCommand)
			{
				IIcarusContainerCommand container = (command as IIcarusContainerCommand);
				sb.Append("                (" + container.Commands.Count.ToString() + " commands)");
			}
			return sb.ToString();
		}

		private void tv_RowActivated(object sender, ListViewRowActivatedEventArgs e)
		{
			if (e.Row != null)
			{
				IcarusExpressionHelperDialog dlg = new IcarusExpressionHelperDialog();

				IcarusCommand cmd = e.Row.GetExtraData<IcarusCommand>("cmd");
				if (cmd is IIcarusContainerCommand && cmd.Parameters.Count == 0)
					return; // nothing to edit, so don't interrupt expanding the container row for a useless dialog

				dlg.Command = cmd;
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					tv.SelectedRows[0].RowColumns[0].Value = GetCommandText(tv.SelectedRows[0].GetExtraData<IcarusCommand>("cmd"));
				}
			}
		}

		private void TestExpressionEditor(object sender, EventArgs e)
		{
			IcarusExpressionHelperDialog dlg = new IcarusExpressionHelperDialog();
			dlg.Command = new IcarusCommandSet();
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				tv.SelectedRows[0].RowColumns[0].Value = GetCommandText(tv.SelectedRows[0].GetExtraData<IcarusCommand>("cmd"));
			}
		}
	}
}