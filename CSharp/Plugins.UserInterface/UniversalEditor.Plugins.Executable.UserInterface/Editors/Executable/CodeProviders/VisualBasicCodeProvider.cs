﻿using System;
using System.Reflection;
using System.Text;

namespace UniversalEditor.Plugins.Executable.UserInterface.Editors.Executable.CodeProviders
{
	public class VisualBasicCodeProvider : CodeProvider
	{
		public override string Title => "VB.NET";
		public override string CodeFileExtension => ".vb";

		protected override string GetAccessModifiersInternal(bool isPublic, bool isFamily, bool isAssembly, bool isPrivate, bool isAbstract, bool isSealed)
		{
			StringBuilder sb = new StringBuilder();
			if (isPublic) sb.Append("Public ");
			if (isFamily) sb.Append("Protected ");
			if (isAssembly) sb.Append("Friend ");
			if (isPrivate) sb.Append("Private ");
			if (isAbstract) sb.Append("Abstract ");
			if (isSealed) sb.Append("Sealed ");
			return sb.ToString().Trim();
		}
		protected override string GetBeginBlockInternal(int indentLevel)
		{
			return new string('\t', indentLevel);
		}
		protected override string GetEndBlockInternal(string elementName, int indentLevel)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(new string('\t', indentLevel));
			sb.Append("End ");
			sb.Append(elementName);
			return sb.ToString();
		}
		protected override string GetBeginBlockInternal(MethodInfo mi, int indentLevel)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(new string('\t', indentLevel));

			string am = GetAccessModifiers(mi);
			if (!String.IsNullOrEmpty(am))
			{
				sb.Append(am);
				sb.Append(' ');
			}
			sb.Append(GetElementName(mi));
			sb.Append(' ');

			sb.Append(GetMethodSignature(mi));

			if (mi.ReturnType != typeof(void))
			{
				sb.Append(" As ");
				sb.Append(GetTypeName(mi.ReturnType));
			}
			return sb.ToString();
		}
		protected override string GetBeginBlockInternal(Type type, int indentLevel)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(new string('\t', indentLevel));
			string am = GetAccessModifiers(type);
			sb.Append(' ');
			sb.Append(GetElementName(type));
			sb.Append(' ');
			sb.Append(type.Name);
			return sb.ToString();
		}
		protected override string GetEndBlockInternal(MethodInfo mi, int indentLevel)
		{
			return GetEndBlockInternal(GetElementName(mi), indentLevel);
		}

		protected override string GetElementNameInternal(Type type)
		{
			if (type.IsClass)
			{
				return "Class";
			}
			else if (type.IsInterface)
			{
				return "Interface";
			}
			else if (type.IsEnum)
			{
				return "Enum";
			}
			else if (type.IsValueType)
			{
				return "Structure";
			}
			throw new NotSupportedException();
		}
		protected override string GetElementNameInternal(MethodInfo mi)
		{
			if (mi.ReturnType == typeof(void))
			{
				return "Sub";
			}
			return "Function";
		}

		protected override string GetSourceCodeInternal(EventInfo item, int indentLevel)
		{
			StringBuilder sb = new StringBuilder();
			/*
			sb.Append(new string('\t', indentLevel));
			string am = GetAccessModifiers(fi);
			if (String.IsNullOrEmpty(am.Trim()))
			{
				sb.Append("Dim");
			}
			else
			{
				sb.Append(am);
			}
			sb.Append(' ');
			sb.Append(fi.Name);
			sb.Append(" As ");
			sb.Append(fi.FieldType.FullName);
			*/
			return sb.ToString();
		}
		protected override string GetSourceCodeInternal(FieldInfo item, int indentLevel)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(new string('\t', indentLevel));
			string am = GetAccessModifiers(item);
			if (String.IsNullOrEmpty(am.Trim()))
			{
				sb.Append("Dim");
			}
			else
			{
				sb.Append(am);
			}
			sb.Append(' ');
			sb.Append(item.Name);
			sb.Append(" As ");
			sb.Append(item.FieldType.FullName);
			return sb.ToString();
		}
		protected override string GetSourceCodeInternal(Type mi, int indentLevel)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(GetBeginBlock(mi, indentLevel));
			sb.AppendLine();

			MethodInfo[] mis = mi.GetMethods();
			for (int i = 0; i < mis.Length; i++)
			{
				sb.AppendLine(GetBeginBlock(mis[i], indentLevel + 1));
				sb.AppendLine();
				sb.AppendLine(GetEndBlock(mis[i], indentLevel + 1));
				sb.AppendLine();
			}

			sb.AppendLine();
			sb.Append(GetEndBlock(mi, indentLevel));
			return sb.ToString();
		}
		protected override string GetSourceCodeInternal(MethodInfo mi, int indentLevel)
		{
			throw new NotImplementedException();
		}

		protected override string GetMethodSignatureInternal(MethodInfo mi)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(mi.Name);
			sb.Append('(');

			ParameterInfo[] pis = mi.GetParameters();
			for (int i = 0; i < pis.Length; i++)
			{
				if (pis[i].ParameterType.IsByRef)
				{
					sb.Append("ByRef ");
				}
				else
				{
					sb.Append("ByVal ");
				}

				sb.Append(pis[i].Name);
				sb.Append(" As ");
				sb.Append(GetTypeName(pis[i].ParameterType));

				if (i < pis.Length - 1)
					sb.Append(", ");
			}

			sb.Append(')');
			return sb.ToString();
		}

		protected override string GetTypeNameInternal(Type type)
		{
			string fullyQualifiedTypeName = type.FullName ?? type.Name;
			if (fullyQualifiedTypeName.Equals("System.Byte")) return "Byte";
			if (fullyQualifiedTypeName.Equals("System.SByte")) return "SByte";
			if (fullyQualifiedTypeName.Equals("System.Char")) return "Char";
			if (fullyQualifiedTypeName.Equals("System.Int16")) return "Short";
			if (fullyQualifiedTypeName.Equals("System.UInt16")) return "UShort";
			if (fullyQualifiedTypeName.Equals("System.Int32")) return "Int";
			if (fullyQualifiedTypeName.Equals("System.UInt32")) return "UInt";
			if (fullyQualifiedTypeName.Equals("System.Int64")) return "Long";
			if (fullyQualifiedTypeName.Equals("System.UInt64")) return "ULong";
			if (fullyQualifiedTypeName.Equals("System.String")) return "String";
			if (fullyQualifiedTypeName.Equals("System.Single")) return "Single";
			if (fullyQualifiedTypeName.Equals("System.Double")) return "Double";
			if (fullyQualifiedTypeName.Equals("System.Decimal")) return "Decimal";
			if (fullyQualifiedTypeName.Equals("System.Boolean")) return "Boolean";
			if (fullyQualifiedTypeName.Equals("System.Object")) return "Object";
			return fullyQualifiedTypeName;
		}
		protected override string GetSourceCodeInternal(PropertyInfo item, bool autoProperty, int indentLevel)
		{
			StringBuilder sb = new StringBuilder();
			string indentStr = new string('\t', indentLevel);
			sb.Append(indentStr);
			sb.Append(GetAccessModifiers(item));
			sb.Append(' ');
			sb.Append("Property ");
			sb.Append(item.Name);
			sb.Append(" As ");
			sb.Append(GetTypeName(item.PropertyType));
			sb.AppendLine();
			sb.Append(new string('\t', indentLevel + 1));
			sb.AppendLine("Get");
			sb.Append(new string('\t', indentLevel + 1));
			sb.AppendLine("End Get");


			sb.Append(new string('\t', indentLevel + 1));
			sb.AppendLine("Set");
			sb.Append(new string('\t', indentLevel + 1));
			sb.AppendLine("End Set");

			sb.Append(new string('\t', indentLevel));
			sb.Append("End Property");
			return sb.ToString();
		}
	}
}
