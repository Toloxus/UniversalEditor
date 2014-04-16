﻿using System;
using System.Collections.Generic;
using System.Text;

using UniversalEditor.DataFormats.Markup.XML;
using UniversalEditor.ObjectModels.Markup;
using UniversalEditor.ObjectModels.PropertyList;

namespace UniversalEditor.DataFormats.PropertyList.XML
{
	public class XMLPropertyListDataFormat : XMLDataFormat
	{
		protected override void BeforeLoadInternal(Stack<ObjectModel> objectModels)
		{
			base.BeforeLoadInternal(objectModels);
			objectModels.Push(new MarkupObjectModel());
		}
		protected override void AfterLoadInternal(Stack<ObjectModel> objectModels)
		{
			base.AfterLoadInternal(objectModels);

			MarkupObjectModel mom = (objectModels.Pop() as MarkupObjectModel);
			PropertyListObjectModel plom = (objectModels.Pop() as PropertyListObjectModel);

			MarkupTagElement tagConfiguration = (mom.Elements["Configuration"] as MarkupTagElement);
			if (tagConfiguration == null) throw new InvalidDataFormatException();

			LoadMarkup(tagConfiguration, ref plom);
		}

		private static DataFormatReference _dfr = null;
		public override DataFormatReference MakeReference()
		{
			if (_dfr == null)
			{
				_dfr = base.MakeReference();
				_dfr.Capabilities.Add(typeof(PropertyListObjectModel), DataFormatCapabilities.All);
				_dfr.Filters.Add("XML property list", new string[] { "*.cfg" });
			}
			return _dfr;
		}

		private static Version mvarFormatVersion = new Version(2, 0);
		public static Version FormatVersion { get { return mvarFormatVersion; } }

		public static void LoadMarkup(MarkupTagElement tagConfiguration, ref PropertyListObjectModel plom)
		{
			foreach (MarkupElement el in tagConfiguration.Elements)
			{
				MarkupTagElement tag = (el as MarkupTagElement);
				if (tag == null) continue;

				if (tag.Name == "Property")
				{
					Property p = LoadPropertyListProperty(tag);
					if (p != null) plom.Properties.Add(p);
				}
				else if (tag.Name == "Group")
				{
					Group g = LoadPropertyListGroup(tag);
					if (g != null) plom.Groups.Add(g);
				}
			}
		}

		private static Property LoadPropertyListProperty(MarkupTagElement tag)
		{
			MarkupAttribute attID = tag.Attributes["ID"];
			MarkupAttribute attValue = tag.Attributes["Value"];
			if (attID == null) return null;

			Property property = new Property();
			property.Name = attID.Value;

			if (attValue != null)
			{
				property.Value = ParseObject(attValue.Value);
			}
			else if (tag.Elements.Count > 0)
			{
				List<object> items = new List<object>();
				foreach (MarkupElement el in tag.Elements)
				{
					MarkupTagElement tag1 = (el as MarkupTagElement);
					if (tag1.FullName != "Value") continue;

					MarkupAttribute attItemDataType = tag1.Attributes["DataType"];
					if (attItemDataType != null)
					{

					}

					items.Add(ParseObject(tag1.Value));
				}
				property.Value = items.ToArray();
			}
			return property;
		}

		private static object ParseObject(string p)
		{
			#region Byte
			{
				byte result = 0;
				if (Byte.TryParse(p, out result)) return result;
			}
			#endregion
			#region SByte
			{
				sbyte result = 0;
				if (SByte.TryParse(p, out result)) return result;
			}
			#endregion
			#region Boolean
			{
				bool result = false;
				if (Boolean.TryParse(p, out result)) return result;
			}
			#endregion
			#region Char
			{
				char result = '\0';
				if (Char.TryParse(p, out result)) return result;
			}
			#endregion
			#region UInt16
			{
				ushort result = 0;
				if (UInt16.TryParse(p, out result)) return result;
			}
			#endregion
			#region UInt32
			{
				uint result = 0;
				if (UInt32.TryParse(p, out result)) return result;
			}
			#endregion
			#region UInt64
			{
				ulong result = 0;
				if (UInt64.TryParse(p, out result)) return result;
			}
			#endregion
			#region Int16
			{
				short result = 0;
				if (Int16.TryParse(p, out result)) return result;
			}
			#endregion
			#region Int32
			{
				int result = 0;
				if (Int32.TryParse(p, out result)) return result;
			}
			#endregion
			#region Int64
			{
				long result = 0;
				if (Int64.TryParse(p, out result)) return result;
			}
			#endregion
			#region Single
			{
				float result = 0.0f;
				if (Single.TryParse(p, out result)) return result;
			}
			#endregion
			#region Double
			{
				double result = 0.0D;
				if (Double.TryParse(p, out result)) return result;
			}
			#endregion
			#region Decimal
			{
				decimal result = 0.0M;
				if (Decimal.TryParse(p, out result)) return result;
			}
			#endregion
			#region TimeSpan
			{
				TimeSpan result = TimeSpan.Zero;
				if (TimeSpan.TryParse(p, out result)) return result;
			}
			#endregion
			#region DateTime
			{
				DateTime result = DateTime.Now;
				if (DateTime.TryParse(p, out result)) return result;
			}
			#endregion
			#region GUID
			{
				Guid result = Guid.Empty;
				try
				{
					return new Guid(p);
				}
				catch
				{
				}
			}
			#endregion

			return p;
		}

		private static Group LoadPropertyListGroup(MarkupTagElement tag)
		{
			MarkupAttribute attID = tag.Attributes["ID"];
			if (attID == null) return null;

			Group group = new Group();
			group.Name = attID.Value;

			foreach (MarkupElement el1 in tag.Elements)
			{
				MarkupTagElement tag1 = (el1 as MarkupTagElement);
				if (tag1 == null) continue;
				if (tag1.Name == "Property")
				{
					Property p = LoadPropertyListProperty(tag1);
					if (p != null) group.Properties.Add(p);
				}
				else if (tag1.Name == "Group")
				{
					Group g = LoadPropertyListGroup(tag1);
					if (g != null) group.Groups.Add(g);
				}
			}

			return group;
		}


		public static void SaveMarkup(ref MarkupTagElement tagConfiguration, PropertyListObjectModel plom)
		{
			tagConfiguration.FullName = "Configuration";
			tagConfiguration.Attributes.Add("Version", mvarFormatVersion.ToString(2));
			if (plom.Properties.Count > 0)
			{
				MarkupTagElement tagProperties = new MarkupTagElement();
				tagProperties.FullName = "Properties";
				foreach (Property property in plom.Properties)
				{
					RecursiveSaveObject(property, tagProperties);
				}
				tagConfiguration.Elements.Add(tagProperties);
			}
			if (plom.Groups.Count > 0)
			{
				MarkupTagElement tagGroups = new MarkupTagElement();
				tagGroups.FullName = "Groups";
				foreach (Group group in plom.Groups)
				{
					RecursiveSaveObject(group, tagGroups);
				}
				tagConfiguration.Elements.Add(tagGroups);
			}
		}

		private static void RecursiveSaveObject(Property item, MarkupTagElement tagParent)
		{
			MarkupTagElement tagProperty = new MarkupTagElement();
			tagProperty.FullName = "Property";
			tagProperty.Attributes.Add("ID", item.Name);
			if (item.Value is Array)
			{
				Array ary = (item.Value as Array);
				for (long i = 0; i < ary.LongLength; i++)
				{
					object obj = ary.GetValue(i);

					MarkupTagElement tagValue = new MarkupTagElement();
					tagValue.Attributes.Add("DataType", obj.GetType().FullName);
					tagValue.FullName = "Value";
					tagValue.Value = obj.ToString();
					tagProperty.Elements.Add(tagValue);
				}
			}
			else
			{
				tagProperty.Attributes.Add("Value", item.Value.ToString());
			}
			tagParent.Elements.Add(tagProperty);
		}
		private static void RecursiveSaveObject(Group item, MarkupTagElement tagParent)
		{
			MarkupTagElement tagGroup = new MarkupTagElement();
			tagGroup.FullName = "Group";
			tagGroup.Attributes.Add("ID", item.Name);
			if (item.Properties.Count > 0)
			{
				MarkupTagElement tagProperties = new MarkupTagElement();
				tagProperties.FullName = "Properties";
				foreach (Property property in item.Properties)
				{
					RecursiveSaveObject(property, tagProperties);
				}
				tagGroup.Elements.Add(tagProperties);
			}
			if (item.Groups.Count > 0)
			{
				MarkupTagElement tagGroups = new MarkupTagElement();
				tagGroups.FullName = "Groups";
				foreach (Group group in item.Groups)
				{
					RecursiveSaveObject(group, tagGroups);
				}
				tagGroup.Elements.Add(tagGroups);
			}
			tagParent.Elements.Add(tagGroup);
		}
	}
}
