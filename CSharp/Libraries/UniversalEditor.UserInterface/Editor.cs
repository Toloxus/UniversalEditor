﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using UniversalEditor.Accessors;
using UniversalEditor.DataFormats.PropertyList.XML;
using UniversalEditor.ObjectModels.PropertyList;

using UniversalWidgetToolkit;
using UniversalWidgetToolkit.Input.Keyboard;

namespace UniversalEditor.UserInterface
{
	/// <summary>
	/// Provides an interface for custom editor implementations not using the Universal Widget Toolkit.
	/// </summary>
	public abstract class Editor : UniversalWidgetToolkit.Container
	{
		public EditorSelection.EditorSelectionCollection Selections { get; } = new EditorSelection.EditorSelectionCollection();
		public abstract void UpdateSelections();
		public EditorSelection[] GetSelections()
		{
			UpdateSelections();

			EditorSelection[] sels = new EditorSelection[Selections.Count];
			for (int i = 0; i < Selections.Count; i++)
			{
				sels[i] = Selections[i];
			}
			return sels;
		}

		protected abstract EditorSelection CreateSelectionInternal(object content);
		public EditorSelection CreateSelection(object content)
		{
			return CreateSelectionInternal(content);
		}

		/// <summary>
		/// Copies the content of all selections to the system clipboard, and then clears the content.
		/// </summary>
		public void Cut()
		{
			StringBuilder sb = new StringBuilder();
			EditorSelection[] sels = GetSelections();
			foreach (EditorSelection sel in sels)
			{
				if (sel.Content != null)
				{
					sb.Append(sel.Content.ToString());
				}
				sel.Content = null;
			}
			Clipboard.Default.SetText(sb.ToString());
		}
		/// <summary>
		/// Copies the content of all selections to the system clipboard.
		/// </summary>
		public void Copy()
		{
			StringBuilder sb = new StringBuilder();
			EditorSelection[] sels = GetSelections();
			foreach (EditorSelection sel in sels)
			{
				if (sel.Content != null)
				{
					sb.Append(sel.Content.ToString());
				}
			}
			Clipboard.Default.SetText(sb.ToString());
		}
		/// <summary>
		/// Pastes the content from the system clipboard into a new selection, overwriting any selected content.
		/// </summary>
		public void Paste()
		{
			Selections.Clear();

			string clipboardText = Clipboard.Default.GetText();
			Selections.Add(CreateSelection(clipboardText));
		}
		public void Delete()
		{
			EditorSelection[] sels = GetSelections();
			foreach (EditorSelection sel in sels)
			{
				sel.Content = null;
			}
		}

		#region IEditorImplementation Members
		public virtual string Title { get { return String.Empty; } }

		private ObjectModel mvarObjectModel = null;
		public ObjectModel ObjectModel
		{
			get { return mvarObjectModel; }
			set
			{
				ObjectModelChangingEventArgs omce = new ObjectModelChangingEventArgs(mvarObjectModel, value);

				BeginUpdate();
				OnObjectModelChanging(omce);
				EndUpdate();

				if (omce.Cancel) return;

				mvarObjectModel = omce.NewObjectModel;

				BeginUpdate();
				OnObjectModelChanged(EventArgs.Empty);
				EndUpdate();
			}
		}
		#endregion

		private static EditorReference _er = null;
		public virtual EditorReference MakeReference()
		{
			return new EditorReference(GetType());

			if (_er == null)
			{
				_er = new EditorReference(GetType());
			}
			return _er;
		}

		private bool mvarInhibitUndo = false;
		protected bool InhibitUndo { get { return mvarInhibitUndo; } set { mvarInhibitUndo = value; } }

		protected Toolbox Toolbox { get; } = new Toolbox();

		// private AwesomeControls.PropertyGrid.PropertyGroup.PropertyGroupCollection mvarPropertyGroups = new AwesomeControls.PropertyGrid.PropertyGroup.PropertyGroupCollection(null);
		// public AwesomeControls.PropertyGrid.PropertyGroup.PropertyGroupCollection PropertyGroups { get { return mvarPropertyGroups; } }

		/// <summary>
		/// The event raised when a toolbox item is selected. Use this to change the current Editor's internal mode without
		/// actually affecting the content of the ObjectModel.
		/// </summary>
		public event ToolboxItemEventHandler ToolboxItemSelected;
		protected virtual void OnToolboxItemSelected(ToolboxItemEventArgs e)
		{
			if (ToolboxItemSelected != null) ToolboxItemSelected(this, e);
		}
		/// <summary>
		/// The event raised when a toolbox item is added to the Editor. Use this to adjust the content of the ObjectModel
		/// based on which toolbox item was added.
		/// </summary>
		public event ToolboxItemEventHandler ToolboxItemAdded;
		protected virtual void OnToolboxItemAdded(ToolboxItemEventArgs e)
		{
			if (ToolboxItemAdded != null) ToolboxItemAdded(this, e);
		}

		/// <summary>
		/// Causes the editor to select the specified toolbox item.
		/// </summary>
		/// <param name="item"></param>
		/// <returns>True if the editor accepted the new selection; false otherwise. Update the toolbox user interface accordingly.</returns>
		public bool SelectToolboxItem(ToolboxItem item)
		{
			ToolboxItemEventArgs e = new ToolboxItemEventArgs(item);
			OnToolboxItemSelected(e);
			if (e.Cancel) return false;
			return true;
		}

		protected override void OnDragEnter(DragEventArgs e)
		{
			base.OnDragEnter(e);
			if (e.Data.ContainsData(typeof(ToolboxItem)))
			{
				e.Effect = DragDropEffect.Copy;
			}
		}
		protected override void OnDragDrop(DragEventArgs e)
		{
			base.OnDragDrop(e);
			if (e.Data.ContainsData(typeof(ToolboxItem)))
			{
				ToolboxItem item = (e.Data.GetData(typeof(ToolboxItem)) as ToolboxItem);
				ToolboxItemEventArgs e1 = new ToolboxItemEventArgs(item);
				OnToolboxItemAdded(e1);
			}
		}

		public event CancelEventHandler DocumentClosing;
		protected virtual void OnDocumentClosing(CancelEventArgs e)
		{
			if (DocumentClosing != null) DocumentClosing(this, e);
		}
		public event EventHandler DocumentClosed;
		protected virtual void OnDocumentClosed(EventArgs e)
		{
			if (DocumentClosed != null) DocumentClosed(this, e);
		}

		private UniversalEditor.ObjectModels.PropertyList.PropertyListObjectModel mvarConfiguration = new UniversalEditor.ObjectModels.PropertyList.PropertyListObjectModel();
		public UniversalEditor.ObjectModels.PropertyList.PropertyListObjectModel Configuration { get { return mvarConfiguration; } }

		public Editor()
		{
			// mvarLargeImageList.ColorDepth = ColorDepth.Depth32Bit;
			// mvarLargeImageList.ImageSize = new System.Drawing.Size(32, 32);
			// mvarLargeImageList.PopulateSystemIcons();

			string largeImageListPath = String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new string[]
			{
				DataPath,
				"Images",
				"ImageList",
				"32x32"
			});
			if (System.IO.Directory.Exists(largeImageListPath))
			{
				string[] fileNames = System.IO.Directory.GetFiles(largeImageListPath);
				foreach (string fileName in fileNames)
				{
					try
					{
						// Image image = Image.FromFile(fileName);
						// mvarLargeImageList.Images.Add(System.IO.Path.GetFileNameWithoutExtension(fileName), image);
					}
					catch (System.OutOfMemoryException)
					{
					}
				}
			}

			// mvarSmallImageList.ColorDepth = ColorDepth.Depth32Bit;
			// mvarSmallImageList.ImageSize = new System.Drawing.Size(16, 16);
			// mvarSmallImageList.PopulateSystemIcons();

			string smallImageListPath = String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new string[]
			{
				DataPath,
				"Images",
				"ImageList",
				"16x16"
			});
			if (System.IO.Directory.Exists(smallImageListPath))
			{
				string[] fileNames = System.IO.Directory.GetFiles(smallImageListPath);
				foreach (string fileName in fileNames)
				{
					try
					{
						// Image image = Image.FromFile(fileName);
						// mvarSmallImageList.Images.Add(System.IO.Path.GetFileNameWithoutExtension(fileName), image);
					}
					catch (System.OutOfMemoryException)
					{
					}
				}
			}

			string configurationPath = String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new string[]
			{
				DataPath,
				"Configuration"
			});

			// FIXME: refactor this into a single XML configuration file loader at the beginning of engine launch
			if (System.IO.Directory.Exists(configurationPath))
			{
				string configurationFileNameFilter = System.Configuration.ConfigurationManager.AppSettings["UniversalEditor.Configuration.ConfigurationFileNameFilter"];
				if (configurationFileNameFilter == null) configurationFileNameFilter = "*.uexml";

				string[] fileNames = System.IO.Directory.GetFiles(configurationPath, configurationFileNameFilter);
				XMLPropertyListDataFormat xmpl = new XMLPropertyListDataFormat();

				foreach (string fileName in fileNames)
				{
					try
					{
						PropertyListObjectModel plom = new PropertyListObjectModel();
						Document.Load(plom, xmpl, new FileAccessor(fileName), true);
						plom.CopyTo(mvarConfiguration);
					}
					catch (InvalidDataFormatException ex)
					{
					}
				}
			}
		}

		#region Implementation

		public event ObjectModelChangingEventHandler ObjectModelChanging;
		protected virtual void OnObjectModelChanging(ObjectModelChangingEventArgs e)
		{
			if (ObjectModelChanging != null) ObjectModelChanging(this, e);
		}
		public event EventHandler ObjectModelChanged;
		protected virtual void OnObjectModelChanged(EventArgs e)
		{
			if (ObjectModelChanged != null) ObjectModelChanged(this, e);
		}

		public event CancelEventHandler ObjectModelSaving;
		protected virtual void OnObjectModelSaving(CancelEventArgs e)
		{
			if (ObjectModelSaving != null) ObjectModelSaving(this, e);
		}

		private struct EDITINFO
		{
			public object item;
			public string propertyName;
			public object oldValue;
			public bool closed;

			public Control EditingControl;

			public EDITINFO(object item, string propertyName, object oldValue, Control editingControl = null)
			{
				this.item = item;
				this.propertyName = propertyName;
				this.oldValue = oldValue;
				this.closed = false;
				this.EditingControl = editingControl;
			}
		}

		private Stack<EDITINFO> undo = new Stack<EDITINFO>();
		public int UndoItemCount { get { return undo.Count; } }
		private Stack<EDITINFO> redo = new Stack<EDITINFO>();
		public int RedoItemCount { get { return redo.Count; } }

		public event EventHandler DocumentEdited;
		protected virtual void OnDocumentEdited(EventArgs e)
		{
			if (DocumentEdited != null) DocumentEdited(this, e);
		}

		private int mvarEditing = 0;
		private int mvarUpdating = 0;

		protected void BeginUpdate()
		{
			mvarUpdating++;
		}
		protected void EndUpdate()
		{
			if (mvarUpdating == 0) return;
			mvarUpdating--;
		}

		protected void BeginEdit()
		{
			if (mvarUpdating > 0) return;

			if (mvarEditing > 0)
			{
				mvarEditing++;
				return;
			}
			mvarEditing++;

			// check to see if this property has been edited before
			if (undo.Count > 0)
			{
				EDITINFO oldedit = undo.Pop();
				if (oldedit.closed) undo.Push(oldedit);
			}

			// push the new edit
			EDITINFO edit = new EDITINFO(null, null, mvarObjectModel);
			undo.Push(edit);

			// clear out all the redos
			redo.Clear();
		}
		protected void BeginEdit(string PropertyName, object Value = null, object ParentObject = null, Control editingControl = null)
		{
			if (mvarEditing > 0)
			{
				mvarEditing++;
				return;
			}
			mvarEditing++;

			if (ParentObject == null) ParentObject = ObjectModel;

			// check to see if this property has been edited before
			if (undo.Count > 0)
			{
				EDITINFO oldedit = undo.Pop();
				if (oldedit.propertyName != PropertyName || oldedit.closed) undo.Push(oldedit);
			}

			// push the new edit
			if (Value == null)
			{
				System.Reflection.PropertyInfo pi = ParentObject.GetType().GetProperty(PropertyName);
				if (pi != null)
				{
					Value = ParentObject.GetType().GetProperty(PropertyName).GetValue(ParentObject, null);
				}
			}

			EDITINFO edit = new EDITINFO(ParentObject, PropertyName, Value, editingControl);
			undo.Push(edit);

			// clear out all the redos
			redo.Clear();
		}
		protected void EndEdit()
		{
			if (mvarUpdating > 0) return;

			if (mvarEditing == 0) return; // throw new InvalidOperationException();
			if (mvarEditing > 1)
			{
				mvarEditing--;
				return;
			}

			if (undo.Count == 0) return;
			EDITINFO oldedit = undo.Pop();
			oldedit.closed = true;
			undo.Push(oldedit);

			// notify the object model that it's being edited
			OnDocumentEdited(EventArgs.Empty);

			mvarEditing--;
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="Editor"/> is currently processing an undo/redo operation.
		/// </summary>
		/// <value><c>true</c> if processing undo redo; otherwise, <c>false</c>.</value>
		public bool ProcessingUndoRedo { get; private set; } = false;

		/// <summary>
		/// Restores the previous object model in the stack.
		/// </summary>
		public void Undo()
		{
			if (undo.Count == 0) return;

			ProcessingUndoRedo = true;

			EDITINFO edi = undo.Pop();
			EDITINFO newedi = edi;

			if (edi.propertyName != null)
			{
				// get the property that owns this edit
				System.Reflection.PropertyInfo pi = edi.item.GetType().GetProperty(edi.propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

				// get the current value of the property, for a "redo"
				object newValue = pi.GetValue(edi.item, null);
				newedi = new EDITINFO(edi.item, edi.propertyName, newValue, edi.EditingControl);

				// set the current value to the "un-done" value
				pi.SetValue(edi.item, edi.oldValue, null);
			}
			else
			{
				newedi = new EDITINFO(null, null, mvarObjectModel, edi.EditingControl);
				mvarObjectModel = (edi.oldValue as ObjectModel);
			}

			// cause a refresh of the editor
			OnObjectModelChanged(EventArgs.Empty);

			if (edi.EditingControl != null) {
				edi.EditingControl.Focus ();
			}

			// push the previous value into the redo log
			redo.Push(newedi);

			ProcessingUndoRedo = false;
		}

		/// <summary>
		/// Restores the previously-undone object model from the stack.
		/// </summary>
		public void Redo()
		{
			// this is EXACTLY like undo, only in reverse ;)
			if (redo.Count == 0) return;

			ProcessingUndoRedo = true;

			EDITINFO edi = redo.Pop();
			EDITINFO newedi = edi;

			if (edi.propertyName != null)
			{
				// get the property that owns this edit
				System.Reflection.PropertyInfo pi = edi.item.GetType().GetProperty(edi.propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

				// get the current value of the property, for a "redo"
				object newValue = pi.GetValue(edi.item, null);
				newedi = new EDITINFO(edi.item, edi.propertyName, newValue, edi.EditingControl);

				// set the current value to the "un-done" value
				pi.SetValue(edi.item, edi.oldValue, null);
			}
			else
			{
				newedi = new EDITINFO(null, null, mvarObjectModel, edi.EditingControl);
				mvarObjectModel = (edi.oldValue as ObjectModel);
			}

			// cause a refresh of the editor
			OnObjectModelChanged(EventArgs.Empty);

			if (edi.EditingControl != null)
				edi.EditingControl.Focus ();

			// push the previous value into the undo log
			undo.Push(newedi);

			ProcessingUndoRedo = false;
		}
		#endregion

		private MenuBar mvarMenuBar = new MenuBar();
		public MenuBar MenuBar { get { return mvarMenuBar; } }

		private CommandBar.CommandBarCollection mvarToolbars = new CommandBar.CommandBarCollection();
		public CommandBar.CommandBarCollection Toolbars { get { return mvarToolbars; } }

		public bool NotifySaving()
		{
			CancelEventArgs ce = new CancelEventArgs();
			OnObjectModelSaving(ce);
			if (ce.Cancel) return false;
			return true;
		}

		public void NotifyClosing(CancelEventArgs ce)
		{
			OnDocumentClosing(ce);
		}
		public void NotifyClosed(EventArgs e)
		{
			OnDocumentClosed(e);
		}

		public string DataPath { get { return String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new string[] { "Editors", this.GetType().FullName }); } }

		private Command.CommandCollection mvarCommands = new Command.CommandCollection();
		public Command.CommandCollection Commands { get { return mvarCommands; } }

		/*
		protected override bool ProcessKeyPreview(ref Message m)
		{
			Keys keys = (Keys)m.WParam;
			OnKeyDown(new KeyEventArgs(keys));

			return base.ProcessKeyPreview(ref m);
		}
		*/
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			// look at this editor's configuration to see if we have any registered keybindings
			foreach (Command cmd in mvarCommands)
			{
				/*
				if (cmd.Shortcut.CompareTo(e.KeyData))
				{
					cmd.Execute();
				}
				*/
			}
		}

		public object /*Image*/ GetThemeImage(string path)
		{
			string fileName = String.Join(System.IO.Path.DirectorySeparatorChar.ToString(), new string[]
			{
				"Editors",
				this.GetType().FullName,
				path
			});
			return null;
			// return AwesomeControls.Theming.Theme.CurrentTheme.GetImage(fileName);
		}

		public bool Changed { get; private set; } = false;

		/// <summary>
		/// Gets the value of the non-indexed public property named <see cref="Name" /> on this <see cref="ObjectModel" />. 
		/// </summary>
		/// <param name="name">The name of the public property whose value should be retrieved.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public T GetProperty<T>(string name, T defaultValue = default(T), object childObject = null)
		{
			if (childObject == null) {
				childObject = ObjectModel;
			}

			if (childObject == null) // ???
				return defaultValue;

			Type t = childObject.GetType ();
			System.Reflection.PropertyInfo pi = t.GetProperty (name);
			try {
				return (T) pi.GetValue(childObject, null);
			}
			catch {
				return defaultValue;
			}
		}
		/// <summary>
		/// Sets the value of the non-indexed public property named <see cref="Name" /> on the specified object and marks the editor as changed.
		/// </summary>
		/// <param name="name">The name of the public property whose value should be set.</param>
		/// <param name="value">The value to set.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void SetProperty<T>(string name, T value, object childObject = null, Control editingControl = null)
		{
			if (childObject == null)
				childObject = ObjectModel;

			if (childObject == null) // ???
				return;
			
			Type t = childObject.GetType ();
			System.Reflection.PropertyInfo pi = t.GetProperty (name);

			object oldvalue = pi.GetValue (childObject, null);
			BeginEdit (name, oldvalue, childObject, editingControl);

			pi.SetValue(childObject, value, null);
			Changed = true;

			EndEdit ();
		}
	}
}
