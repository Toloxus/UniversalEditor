//
//  PropertyListItem.cs
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
using System.Linq;
using MBS.Framework;
using MBS.Framework.Collections.Generic;

namespace UniversalEditor.ObjectModels.PropertyList
{
	public abstract class PropertyListItem : ICloneable, ISupportsExtraData
	{
		public class PropertyListItemCollection
			: System.Collections.ObjectModel.Collection<PropertyListItem>
		{
			private Group _parent = null;
			public PropertyListItemCollection(Group parent)
			{
				this._parent = parent;
			}

			public PropertyListItem this[string name]
			{
				get
				{
					for (int i = 0; i < Count; i++)
					{
						if (this[i].Name == name)
							return this[i];
					}
					return null;
				}
			}

			private Dictionary<Type, System.Collections.IList> cacheOfT = new Dictionary<Type, System.Collections.IList>();
			public T OfType<T>(string name) where T : PropertyListItem
			{
				T[] items = this.OfType<T>();
				for (int i = 0; i < items.Length; i++)
				{
					if (items[i].Name == name)
						return items[i];
				}
				return null;
			}

			public Property AddProperty(string name, object value = null)
			{
				Property property = new Property(name, value);
				Add(property);
				return property;
			}
			public Group AddGroup(string name, PropertyListItem[] items = null)
			{
				Group group = new Group(name, items);
				Add(group);
				return group;
			}
			public Group AddGroup(string name, string groupHierarchySeparator, PropertyListItem[] items = null)
			{
				if (groupHierarchySeparator == null)
				{
					Group group = new Group(name, items);
					Add(group);
					return group;
				}

				string[] path = name.Split(groupHierarchySeparator);
				Group parent = this[path[0]] as Group;
				if (parent == null)
				{
					parent = new Group(path[0]);
					Add(parent);
				}

				for (int i = 1; i < path.Length; i++)
				{
					Group pg = parent.Items[path[i]] as Group;
					if (pg == null)
					{
						pg = parent.Items.AddGroup(path[i]);
					}
					parent = pg;
				}

				if (items != null)
					parent.Items.AddRange(items);

				return parent;
			}

			protected override void ClearItems()
			{
				for (int i = 0; i < Count; i++)
					this[i].Parent = null;
				base.ClearItems();
			}
			protected override void InsertItem(int index, PropertyListItem item)
			{
				base.InsertItem(index, item);
				item.Parent = _parent;
			}
			protected override void RemoveItem(int index)
			{
				this[index].Parent = null;
				base.RemoveItem(index);
			}
			protected override void SetItem(int index, PropertyListItem item)
			{
				this[index].Parent = null;
				base.SetItem(index, item);
				item.Parent = _parent;
			}

			public bool Contains<T>(string name) where T : PropertyListItem
			{
				return this.OfType<T>(name) != null;
			}
			public bool Contains(string name)
			{
				return this[name] != null;
			}

			public void AddRange(PropertyListItem[] items)
			{
				for (int i = 0; i < items.Length; i++)
					Add(items[i]);
			}
		}

		public abstract void Combine(PropertyListItem item);
		public abstract object Clone();

		private Dictionary<string, object> extraDataDictionary = new Dictionary<string, object>();
		public T GetExtraData<T>(string key, T defaultValue = default(T))
		{
			if (extraDataDictionary.ContainsKey(key)  && extraDataDictionary[key] is T)
			{
				return (T)extraDataDictionary[key];
			}
			return defaultValue;
		}

		public void SetExtraData<T>(string key, T value)
		{
			extraDataDictionary[key] = value;
		}

		public object GetExtraData(string key, object defaultValue = null)
		{
			if (extraDataDictionary.ContainsKey(key))
			{
				return extraDataDictionary[key];
			}
			return defaultValue;
		}

		public void SetExtraData(string key, object value)
		{
			extraDataDictionary[key] = value;
		}

		/// <summary>
		/// The name of this <see cref="PropertyListItem"/>.
		/// </summary>
		public string Name { get; set; } = string.Empty;
		/// <summary>
		/// The <see cref="Group" /> that contains this <see cref="PropertyListItem" /> as a child.
		/// </summary>
		public Group Parent { get; protected set; } = null;
	}
}
