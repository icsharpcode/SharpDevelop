// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Text;

using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop
{
	public class NetAmbience :  AbstractAmbience
	{
		public override string Convert(IClass c)
		{
			StringBuilder builder = new StringBuilder();
			
			if (ShowDefinitionKeyWord) {
				switch (c.ClassType) {
					case ClassType.Delegate:
						builder.Append("Delegate");
						break;
					case ClassType.Class:
						builder.Append("Class");
						break;
					case ClassType.Module:
						builder.Append("Module");
						break;
					case ClassType.Struct:
						builder.Append("Structure");
						break;
					case ClassType.Interface:
						builder.Append("Interface");
						break;
					case ClassType.Enum:
						builder.Append("Enumeration");
						break;
				}
				builder.Append(' ');
			}
			
			if (UseFullyQualifiedMemberNames) {
				builder.Append(c.FullyQualifiedName);
			} else {
				builder.Append(c.Name);
			}
			if (ShowTypeParameterList && c.TypeParameters.Count > 0) {
				builder.Append('<');
				for (int i = 0; i < c.TypeParameters.Count; ++i) {
					if (i > 0) builder.Append(", ");
					builder.Append(ConvertTypeParameter(c.TypeParameters[i]));
				}
				builder.Append('>');
			}
			
			if (ShowParameterList && c.ClassType == ClassType.Delegate) {
				builder.Append('(');
				
				foreach (IMethod m in c.Methods) {
					if (m.Name != "Invoke") continue;
					
					for (int i = 0; i < m.Parameters.Count; ++i) {
						builder.Append(Convert(m.Parameters[i]));
						if (i + 1 < m.Parameters.Count) {
							builder.Append(", ");
						}
					}
				}
				
				builder.Append(')');
				if (c.Methods.Count > 0 && ShowReturnType) {
					builder.Append(" : ");
					builder.Append(Convert(c.Methods[0].ReturnType));
				}
			} else if (ShowInheritanceList) {
				if (c.BaseTypes.Count > 0) {
					builder.Append(" : ");
					for (int i = 0; i < c.BaseTypes.Count; ++i) {
						builder.Append(c.BaseTypes[i]);
						if (i + 1 < c.BaseTypes.Count) {
							builder.Append(", ");
						}
					}
				}
			}
			
			if (IncludeBody) {
				builder.Append("\n{");
			}
			
			return builder.ToString();
		}
		
		public override string ConvertEnd(IClass c)
		{
			return "}";
		}
		
		public override string Convert(IField field)
		{
			StringBuilder builder = new StringBuilder();
			if (ShowDefinitionKeyWord) {
				builder.Append("Field");
				builder.Append(' ');
			}
			
			if (UseFullyQualifiedMemberNames) {
				builder.Append(field.FullyQualifiedName);
			} else {
				builder.Append(field.Name);
			}
			
			if (field.ReturnType != null && ShowReturnType) {
				builder.Append(" : ");
				builder.Append(Convert(field.ReturnType));
			}
			
			return builder.ToString();
		}
		
		public override string Convert(IProperty property)
		{
			StringBuilder builder = new StringBuilder();
			if (ShowDefinitionKeyWord) {
				if (property.IsIndexer)
					builder.Append("Indexer ");
				else
					builder.Append("Property ");
			}
			
			if (UseFullyQualifiedMemberNames) {
				builder.Append(property.FullyQualifiedName);
			} else {
				builder.Append(property.Name);
			}

			if (ShowParameterList) {
				if (property.Parameters.Count > 0) builder.Append('(');
				
				for (int i = 0; i < property.Parameters.Count; ++i) {
					builder.Append(Convert(property.Parameters[i]));
					if (i + 1 < property.Parameters.Count) {
						builder.Append(", ");
					}
				}
				
				if (property.Parameters.Count > 0) builder.Append(')');
			}
			
			if (property.ReturnType != null  && ShowReturnType) {
				builder.Append(" : ");
				builder.Append(Convert(property.ReturnType));
			}
			return builder.ToString();
		}
		
		public override string Convert(IEvent e)
		{
			StringBuilder builder = new StringBuilder();
			if (ShowDefinitionKeyWord) {
				builder.Append("Event ");
			}
			
			if (UseFullyQualifiedMemberNames) {
				builder.Append(e.FullyQualifiedName);
			} else {
				builder.Append(e.Name);
			}
			if (e.ReturnType != null && ShowReturnType) {
				builder.Append(" : ");
				builder.Append(Convert(e.ReturnType));
			}
			return builder.ToString();
		}
		
		string ConvertTypeParameter(ITypeParameter tp)
		{
			if (tp.BoundTo != null)
				return Convert(tp.BoundTo);
			else
				return tp.Name;
		}
		
		public override string Convert(IMethod m)
		{
			StringBuilder builder = new StringBuilder();
			if (ShowDefinitionKeyWord) {
				if (ShowModifiers && m.IsExtensionMethod) {
					builder.Append("[Extension] ");
				}
				builder.Append("Method ");
			}
			
			if (UseFullyQualifiedMemberNames) {
				builder.Append(m.FullyQualifiedName);
			} else {
				builder.Append(m.Name);
			}
			if (ShowTypeParameterList && m.TypeParameters.Count > 0) {
				builder.Append('<');
				for (int i = 0; i < m.TypeParameters.Count; ++i) {
					if (i > 0) builder.Append(", ");
					builder.Append(ConvertTypeParameter(m.TypeParameters[i]));
				}
				builder.Append('>');
			}
			if (ShowParameterList) {
				builder.Append('(');
				for (int i = 0; i < m.Parameters.Count; ++i) {
					builder.Append(Convert(m.Parameters[i]));
					if (i + 1 < m.Parameters.Count) {
						builder.Append(", ");
					}
				}
				builder.Append(")");
			}
			
			if (m.ReturnType != null && ShowReturnType) {
				builder.Append(" : ");
				builder.Append(Convert(m.ReturnType));
			}
			
			if (IncludeBody) {
				builder.Append(" {");
			}
			
			return builder.ToString();
		}
		
		public override string ConvertEnd(IMethod m)
		{
			return "}";
		}
		
		public override string Convert(IReturnType returnType)
		{
			if (returnType == null) {
				return String.Empty;
			}
			
			StringBuilder builder = new StringBuilder();
			
			string name = returnType.DotNetName;
			if (UseFullyQualifiedTypeNames) {
				builder.Append(name);
			} else {
				string rtNamespace = returnType.Namespace + ".";
				if (name.StartsWith(rtNamespace, StringComparison.Ordinal)) {
					builder.Append(name, rtNamespace.Length, name.Length - rtNamespace.Length);
				}
			}
			
			return builder.ToString();
		}
		
		public override string Convert(IParameter param)
		{
			StringBuilder builder = new StringBuilder();
			if (ShowParameterNames) {
				builder.Append(param.Name);
				builder.Append(" : ");
			}
			builder.Append(Convert(param.ReturnType));
			if (param.IsRef) {
				builder.Append("&");
			}
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
		
		public override string GetIntrinsicTypeName(string dotNetTypeName)
		{
			return dotNetTypeName;
		}
		
		public override string ConvertAccessibility(ModifierEnum accessibility)
		{
			return string.Empty;
		}
	}
}
