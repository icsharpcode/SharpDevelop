// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;

namespace Grunwald.BooBinding
{
	public class BooAmbience : AbstractAmbience
	{
		// Fields
		static Dictionary<string, string> reverseTypeConversionTable = new Dictionary<string, string>();
		static Dictionary<string, string> typeConversionTable = new Dictionary<string, string>();
		
		public readonly static BooAmbience Instance = new BooAmbience();
		
		/// <summary>
		/// Gets a dictionary with boo's short names as keys and
		/// the fully qualified type names as values.
		/// </summary>
		public static Dictionary<string, string> ReverseTypeConversionTable {
			get {
				return reverseTypeConversionTable;
			}
		}
		
		/// <summary>
		/// Gets a dictionary with the fully qualified type names as keys and
		/// boo's short names as values.
		/// </summary>
		public static Dictionary<string, string> TypeConversionTable {
			get {
				return typeConversionTable;
			}
		}
		
		static BooAmbience()
		{
			typeConversionTable.Add("System.Void", "void");
			typeConversionTable.Add("System.Object", "object");
			typeConversionTable.Add("System.Boolean", "bool");
			typeConversionTable.Add("System.Byte", "byte");
			typeConversionTable.Add("System.SByte", "sbyte");
			typeConversionTable.Add("System.Char", "char");
			typeConversionTable.Add("System.Int16", "short");
			typeConversionTable.Add("System.Int32", "int");
			typeConversionTable.Add("System.Int64", "long");
			typeConversionTable.Add("System.UInt16", "ushort");
			typeConversionTable.Add("System.UInt32", "uint");
			typeConversionTable.Add("System.UInt64", "ulong");
			typeConversionTable.Add("System.Single", "single");
			typeConversionTable.Add("System.Double", "double");
			typeConversionTable.Add("System.Decimal", "decimal");
			typeConversionTable.Add("System.String", "string");
			typeConversionTable.Add("System.DateTime", "date");
			typeConversionTable.Add("System.TimeSpan", "timespan");
			typeConversionTable.Add("System.Type", "type");
			typeConversionTable.Add("System.Array", "array");
			typeConversionTable.Add("System.Text.RegularExpressions.Regex", "regex");
			foreach (KeyValuePair<string, string> pair in typeConversionTable) {
				reverseTypeConversionTable.Add(pair.Value, pair.Key);
			}
		}
		
		public override string GetIntrinsicTypeName(string typeName)
		{
			if (typeConversionTable.ContainsKey(typeName))
				return typeConversionTable[typeName];
			else
				return typeName;
		}
		
		public static string GetFullTypeName(string shortName)
		{
			if (reverseTypeConversionTable.ContainsKey(shortName))
				return reverseTypeConversionTable[shortName];
			else
				return shortName;
		}
		
		public static IEnumerable<KeyValuePair<string, string>> BooSpecialTypes {
			get {
				return typeConversionTable;
			}
		}
		
		// Methods
		
		bool ModifierIsSet(ModifierEnum modifier, ModifierEnum query)
		{
			return (modifier & query) == query;
		}
		
		public override string Convert(ModifierEnum modifier)
		{
			if (ShowAccessibility) {
				if (ModifierIsSet(modifier, ModifierEnum.Public)) {
					return "public ";
				} else if (ModifierIsSet(modifier, ModifierEnum.Private)) {
					return "private ";
				} else if (ModifierIsSet(modifier, ModifierEnum.ProtectedAndInternal)) {
					return "protected internal ";
				} else if (ModifierIsSet(modifier, ModifierEnum.Internal)) {
					return "internal ";
				} else if (ModifierIsSet(modifier, ModifierEnum.Protected)) {
					return "protected ";
				}
			}
			return string.Empty;
		}
		
		string GetModifier(IDecoration decoration)
		{
			string ret = "";
			
			if (IncludeHTMLMarkup) {
				ret += "<i>";
			}
			if (decoration.IsStatic) {
				ret += "static ";
			} else if (decoration.IsSealed) {
				ret += "final ";
			} else if (decoration.IsOverride) {
				ret += "override ";
			} else if (decoration.IsNew) {
				ret += "new ";
			}
			if (IncludeHTMLMarkup) {
				ret += "</i>";
			}
			return ret;
		}
		
		
		public override string Convert(IClass c)
		{
			StringBuilder builder = new StringBuilder();
			
			builder.Append(Convert(c.Modifiers));
			
			if (IncludeHTMLMarkup) {
				builder.Append("<i>");
			}
			
			if (ShowModifiers) {
				if (c.IsSealed) {
					switch (c.ClassType) {
						case ClassType.Delegate:
						case ClassType.Struct:
						case ClassType.Enum:
							break;
							
						default:
							builder.Append("final ");
							break;
					}
				} else if (c.IsAbstract && c.ClassType != ClassType.Interface) {
					builder.Append("abstract ");
				}
			}
			
			if (IncludeHTMLMarkup) {
				builder.Append("</i>");
			}
			
			if (ShowModifiers) {
				switch (c.ClassType) {
					case ClassType.Delegate:
						builder.Append("callable");
						break;
					case ClassType.Class:
					case ClassType.Module:
						builder.Append("class");
						break;
					case ClassType.Struct:
						builder.Append("struct");
						break;
					case ClassType.Interface:
						builder.Append("interface");
						break;
					case ClassType.Enum:
						builder.Append("enum");
						break;
				}
				builder.Append(' ');
			}
			
			if (IncludeHTMLMarkup) {
				builder.Append("<b>");
			}
			
			if (UseFullyQualifiedMemberNames) {
				builder.Append(c.FullyQualifiedName);
			} else {
				builder.Append(c.Name);
			}
			
			if (IncludeHTMLMarkup) {
				builder.Append("</b>");
			}
			if (c.TypeParameters.Count > 0) {
				builder.Append("[of ");
				for (int i = 0; i < c.TypeParameters.Count; ++i) {
					if (i > 0) builder.Append(", ");
					builder.Append(c.TypeParameters[i].Name);
				}
				builder.Append(']');
			}
			
			if (ShowReturnType && c.ClassType == ClassType.Delegate) {
				builder.Append(" (");
				if (IncludeHTMLMarkup) builder.Append("<br>");
				
				foreach(IMethod m in c.Methods) {
					if (m.Name != "Invoke") continue;
					
					for (int i = 0; i < m.Parameters.Count; ++i) {
						if (IncludeHTMLMarkup) builder.Append("&nbsp;&nbsp;&nbsp;");
						
						builder.Append(Convert(m.Parameters[i]));
						if (i + 1 < m.Parameters.Count) builder.Append(", ");
						
						if (IncludeHTMLMarkup) builder.Append("<br>");
					}
				}
				builder.Append(')');
				
				foreach(IMethod m in c.Methods) {
					if (m.Name != "Invoke") continue;
					
					builder.Append(" as ");
					builder.Append(Convert(m.ReturnType));
				}
				
			} else if (ShowInheritanceList) {
				if (c.BaseTypes.Count > 0) {
					builder.Append("(");
					for (int i = 0; i < c.BaseTypes.Count; ++i) {
						builder.Append(c.BaseTypes[i]);
						if (i + 1 < c.BaseTypes.Count) {
							builder.Append(", ");
						}
					}
					builder.Append(")");
				}
			}
			
			if (IncludeBodies) {
				builder.Append(":\n");
			}
			
			return builder.ToString();
		}
		
		public override string ConvertEnd(IClass c)
		{
			return "";
		}
		
		public override string Convert(IField field)
		{
			StringBuilder builder = new StringBuilder();
			
			builder.Append(Convert(field.Modifiers));
			
			if (IncludeHTMLMarkup) {
				builder.Append("<i>");
			}
			
			if (ShowModifiers) {
				if (field.IsConst) {
					builder.Append("static final ");
				} else if (field.IsStatic) {
					builder.Append("static ");
				}
				
				if (field.IsReadonly) {
					builder.Append("final ");
				}
			}
			
			if (IncludeHTMLMarkup) {
				builder.Append("</i>");
			}
			
			if (IncludeHTMLMarkup) {
				builder.Append("<b>");
			}
			
			if (UseFullyQualifiedMemberNames) {
				builder.Append(field.FullyQualifiedName);
			} else {
				builder.Append(field.Name);
			}
			
			if (IncludeHTMLMarkup) {
				builder.Append("</b>");
			}
			
			if (field.ReturnType != null  && ShowReturnType) {
				builder.Append(" as ");
				builder.Append(Convert(field.ReturnType));
			}
			
			return builder.ToString();
		}
		
		public override string Convert(IProperty property)
		{
			StringBuilder builder = new StringBuilder();
			
			builder.Append(Convert(property.Modifiers));
			
			if (ShowModifiers) {
				builder.Append(GetModifier(property));
			}
			
			if (property.IsIndexer) {
				builder.Append("self");
			} else {
				if (IncludeHTMLMarkup) {
					builder.Append("<b>");
				}
				if (UseFullyQualifiedMemberNames) {
					builder.Append(property.FullyQualifiedName);
				} else {
					builder.Append(property.Name);
				}
				if (IncludeHTMLMarkup) {
					builder.Append("</b>");
				}
			}
			
			if (property.Parameters.Count > 0) {
				builder.Append('[');
				if (IncludeHTMLMarkup) builder.Append("<br>");
				
				for (int i = 0; i < property.Parameters.Count; ++i) {
					if (IncludeHTMLMarkup) builder.Append("&nbsp;&nbsp;&nbsp;");
					builder.Append(Convert(property.Parameters[i]));
					if (i + 1 < property.Parameters.Count) {
						builder.Append(", ");
					}
					if (IncludeHTMLMarkup) builder.Append("<br>");
				}
				
				builder.Append(']');
			}
			
			if (property.ReturnType != null && ShowReturnType) {
				builder.Append(" as ");
				builder.Append(Convert(property.ReturnType));
			}
			
			if (IncludeBodies) {
				builder.Append(":");
				
				if (property.CanGet) {
					builder.Append(" get");
				}
				if (property.CanSet) {
					builder.Append(" set");
				}
			}
			
			return builder.ToString();
		}
		
		public override string Convert(IEvent e)
		{
			StringBuilder builder = new StringBuilder();
			
			builder.Append(Convert(e.Modifiers));
			
			if (ShowModifiers) {
				builder.Append(GetModifier(e));
			}
			
			builder.Append("event ");
			
			if (IncludeHTMLMarkup) {
				builder.Append("<b>");
			}
			
			if (UseFullyQualifiedMemberNames) {
				builder.Append(e.FullyQualifiedName);
			} else {
				builder.Append(e.Name);
			}
			
			if (IncludeHTMLMarkup) {
				builder.Append("</b>");
			}
			
			if (e.ReturnType != null && ShowReturnType) {
				builder.Append(" as ");
				builder.Append(Convert(e.ReturnType));
			}
			
			return builder.ToString();
		}
		
		public override string Convert(IMethod m)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(Convert(m.Modifiers));
			
			if (ShowModifiers) {
				builder.Append(GetModifier(m));
			}
			
			builder.Append("def ");
			
			if (IncludeHTMLMarkup) {
				builder.Append("<b>");
			}
			
			if (m.IsConstructor) {
				builder.Append("constructor");
			} else {
				if (UseFullyQualifiedMemberNames) {
					builder.Append(m.FullyQualifiedName);
				} else {
					builder.Append(m.Name);
				}
			}
			
			if (IncludeHTMLMarkup) {
				builder.Append("</b>");
			}
			
			if (m.TypeParameters.Count > 0) {
				builder.Append("[of ");
				for (int i = 0; i < m.TypeParameters.Count; ++i) {
					if (i > 0) builder.Append(", ");
					builder.Append(m.TypeParameters[i].Name);
				}
				builder.Append(']');
			}
			
			builder.Append("(");
			if (IncludeHTMLMarkup) builder.Append("<br>");
			
			for (int i = 0; i < m.Parameters.Count; ++i) {
				if (IncludeHTMLMarkup) builder.Append("&nbsp;&nbsp;&nbsp;");
				builder.Append(Convert(m.Parameters[i]));
				if (i + 1 < m.Parameters.Count) {
					builder.Append(", ");
				}
				if (IncludeHTMLMarkup) builder.Append("<br>");
			}
			
			builder.Append(')');
			
			if (m.ReturnType != null && ShowReturnType) {
				builder.Append(" as ");
				builder.Append(Convert(m.ReturnType));
			}
			
			return builder.ToString();
		}
		
		public override string ConvertEnd(IMethod m)
		{
			return "";
		}
		
		public override string Convert(IReturnType returnType)
		{
			if (returnType == null) {
				return String.Empty;
			}
			StringBuilder builder = new StringBuilder();
			UnpackNestedType(builder, returnType);
			return builder.ToString();
		}
		
		void UnpackNestedType(StringBuilder builder, IReturnType returnType)
		{
			if (returnType.ArrayDimensions > 0) {
				builder.Append('(');
				UnpackNestedType(builder, returnType.ArrayElementType);
				if (returnType.ArrayDimensions > 1) {
					builder.Append(',');
					builder.Append(returnType.ArrayDimensions);
				}
				builder.Append(')');
			} else if (returnType.TypeArguments != null) {
				UnpackNestedType(builder, returnType.UnboundType);
				builder.Append("[of ");
				for (int i = 0; i < returnType.TypeArguments.Count; ++i) {
					if (i > 0) builder.Append(", ");
					builder.Append(Convert(returnType.TypeArguments[i]));
				}
				builder.Append(']');
			} else {
				string fullName = returnType.FullyQualifiedName;
				if (fullName != null && typeConversionTable.ContainsKey(fullName)) {
					builder.Append(typeConversionTable[fullName].ToString());
				} else {
					if (UseFullyQualifiedNames) {
						builder.Append(fullName);
					} else {
						builder.Append(returnType.Name);
					}
				}
			}
		}
		
		public override string Convert(IParameter param)
		{
			StringBuilder builder = new StringBuilder();
			
			if (IncludeHTMLMarkup) {
				builder.Append("<i>");
			}
			
			if (param.IsRef) {
				builder.Append("ref ");
			} else if (param.IsOut) {
				builder.Append("ref ");
			} else if (param.IsParams) {
				builder.Append("*");
			}
			
			if (IncludeHTMLMarkup) {
				builder.Append("</i>");
			}
			
			if (ShowParameterNames) {
				builder.Append(param.Name);
				builder.Append(" as ");
			}
			
			builder.Append(Convert(param.ReturnType));
			
			return builder.ToString();
		}
		
		public override string WrapAttribute(string attribute)
		{
			return "[" + attribute + "]";
		}
		
		public override string WrapComment(string comment)
		{
			return "// " + comment;
		}
	}
}
