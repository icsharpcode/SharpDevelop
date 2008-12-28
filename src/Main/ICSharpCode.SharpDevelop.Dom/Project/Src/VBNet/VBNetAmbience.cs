// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1421 $</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ICSharpCode.SharpDevelop.Dom.VBNet
{
	public class VBNetAmbience :  AbstractAmbience
	{
		public static IDictionary<string, string> TypeConversionTable {
			get { return ICSharpCode.NRefactory.Ast.TypeReference.PrimitiveTypesVBReverse; }
		}
		
		string GetModifier(IEntity decoration)
		{
			StringBuilder builder = new StringBuilder();
			
			if (IncludeHtmlMarkup) {
				builder.Append("<i>");
			}
			
			if (decoration.IsStatic) {
				builder.Append("Shared ");
			}
			if (decoration.IsAbstract) {
				builder.Append("MustOverride ");
			} else if (decoration.IsSealed) {
				builder.Append("NotOverridable ");
			}
			if (decoration.IsVirtual) {
				builder.Append("Overridable ");
			} else if (decoration.IsOverride) {
				builder.Append("Overrides ");
			}
			if (decoration.IsNew) {
				builder.Append("Shadows ");
			}
			
			if (IncludeHtmlMarkup) {
				builder.Append("</i>");
			}
			
			return builder.ToString();
		}
		
		string ConvertAccessibility(ModifierEnum modifier)
		{
			StringBuilder builder = new StringBuilder();
			if (ShowAccessibility) {
				if ((modifier & ModifierEnum.Public) == ModifierEnum.Public) {
					builder.Append("Public");
				} else if ((modifier & ModifierEnum.Private) == ModifierEnum.Private) {
					builder.Append("Private");
				} else if ((modifier & (ModifierEnum.Protected | ModifierEnum.Internal)) == (ModifierEnum.Protected | ModifierEnum.Internal)) {
					builder.Append("Protected Friend");
				} else if ((modifier & ModifierEnum.Internal) == ModifierEnum.Internal) {
					builder.Append("Friend");
				} else if ((modifier & ModifierEnum.Protected) == ModifierEnum.Protected) {
					builder.Append("Protected");
				}
				builder.Append(' ');
			}
			return builder.ToString();
		}
		
		public override string Convert(IClass c)
		{
			CheckThread();
			
			StringBuilder builder = new StringBuilder();
			
			builder.Append(ConvertAccessibility(c.Modifiers));
			
			if (IncludeHtmlMarkup) {
				builder.Append("<i>");
			}
			
			if (ShowModifiers) {
				if (c.IsSealed) {
					if (c.ClassType == ClassType.Class) {
						builder.Append("NotInheritable ");
					}
				} else if (c.IsAbstract && c.ClassType != ClassType.Interface) {
					builder.Append("MustInherit ");
				}
			}
			
			if (IncludeHtmlMarkup) {
				builder.Append("</i>");
			}
			
			if (ShowDefinitionKeyWord) {
				switch (c.ClassType) {
					case ClassType.Delegate:
						builder.Append("Delegate ");
						if (ShowReturnType) {
							foreach (IMethod m in c.Methods) {
								if (m.Name != "Invoke") {
									continue;
								}
								
								if (m.ReturnType == null || m.ReturnType.FullyQualifiedName == "System.Void") {
									builder.Append("Sub");
								} else {
									builder.Append("Function");
								}
							}
						}
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
						builder.Append("Enum");
						break;
				}
				builder.Append(' ');
			}
			
			if (IncludeHtmlMarkup) {
				builder.Append("<b>");
			}
			
			if (UseFullyQualifiedMemberNames) {
				builder.Append(c.FullyQualifiedName);
			} else {
				builder.Append(c.Name);
			}
			
			if (IncludeHtmlMarkup) {
				builder.Append("</b>");
			}
			
			if (ShowTypeParameterList && c.TypeParameters.Count > 0) {
				builder.Append("(Of ");
				for (int i = 0; i < c.TypeParameters.Count; ++i) {
					if (i > 0) builder.Append(", ");
					builder.Append(ConvertTypeParameter(c.TypeParameters[i]));
				}
				builder.Append(')');
			}
			
			if (ShowParameterList && c.ClassType == ClassType.Delegate) {
				builder.Append("(");
				if (IncludeHtmlMarkup) builder.Append("<br>");
				
				foreach (IMethod m in c.Methods) {
					if (m.Name != "Invoke") continue;
					
					for (int i = 0; i < m.Parameters.Count; ++i) {
						if (IncludeHtmlMarkup) builder.Append("&nbsp;&nbsp;&nbsp;");
						
						builder.Append(Convert(m.Parameters[i]));
						if (i + 1 < m.Parameters.Count) builder.Append(", ");

						if (IncludeHtmlMarkup) builder.Append("<br>");
					}
				}

				builder.Append(")");
			}
			if (ShowReturnType && c.ClassType == ClassType.Delegate) {
				foreach (IMethod m in c.Methods) {
					if (m.Name != "Invoke") continue;
					
					if (m.ReturnType == null || m.ReturnType.FullyQualifiedName == "System.Void") {
					} else {
						if (ShowReturnType) {
							builder.Append(" As ");
							builder.Append(Convert(m.ReturnType));
						}
					}
				}
			} else if (ShowInheritanceList && c.ClassType != ClassType.Delegate) {
				if (c.BaseTypes.Count > 0) {
					builder.Append(" Inherits ");
					for (int i = 0; i < c.BaseTypes.Count; ++i) {
						builder.Append(c.BaseTypes[i]);
						if (i + 1 < c.BaseTypes.Count) {
							builder.Append(", ");
						}
					}
				}
			}
			
			return builder.ToString();
		}
		
		public override string ConvertEnd(IClass c)
		{
			if (c == null)
				throw new ArgumentNullException("c");
			
			StringBuilder builder = new StringBuilder();
			
			builder.Append("End ");
			
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
					builder.Append("Enum");
					break;
			}
			
			return builder.ToString();
		}
		
		public override string Convert(IField field)
		{
			CheckThread();
			
			if (field == null)
				throw new ArgumentNullException("field");
			
			StringBuilder builder = new StringBuilder();
			
			builder.Append(ConvertAccessibility(field.Modifiers));
			
			if (IncludeHtmlMarkup) {
				builder.Append("<i>");
			}
			
			if (ShowModifiers) {
				if (field.IsConst) {
					builder.Append("Const ");
				} else if (field.IsStatic) {
					builder.Append("Shared ");
				}
			}
			
			if (IncludeHtmlMarkup) {
				builder.Append("</i>");
				builder.Append("<b>");
			}
			
			if (UseFullyQualifiedMemberNames) {
				builder.Append(field.FullyQualifiedName);
			} else {
				builder.Append(field.Name);
			}
			
			if (IncludeHtmlMarkup) {
				builder.Append("</b>");
			}
			
			if (field.ReturnType != null && ShowReturnType) {
				builder.Append(" As ");
				builder.Append(Convert(field.ReturnType));
			}
			
			return builder.ToString();
		}
		
		public override string Convert(IProperty property)
		{
			CheckThread();
			
			StringBuilder builder = new StringBuilder();
			
			builder.Append(ConvertAccessibility(property.Modifiers));
			
			if (ShowModifiers) {
				builder.Append(GetModifier(property));
				
				if (property.IsIndexer) {
					builder.Append("Default ");
				}
				
				if (property.CanGet && !property.CanSet) {
					builder.Append("ReadOnly ");
				}
				if (property.CanSet && !property.CanGet) {
					builder.Append("WriteOnly ");
				}
			}
			
			if (ShowDefinitionKeyWord) {
				builder.Append("Property ");
			}
			
			if (IncludeHtmlMarkup) {
				builder.Append("<b>");
			}
			
			if (UseFullyQualifiedMemberNames) {
				builder.Append(property.FullyQualifiedName);
			} else {
				builder.Append(property.Name);
			}
			
			if (IncludeHtmlMarkup) {
				builder.Append("</b>");
			}
			
			if (ShowParameterList && property.Parameters.Count > 0) {
				builder.Append("(");
				if (IncludeHtmlMarkup) builder.Append("<br>");
				
				for (int i = 0; i < property.Parameters.Count; ++i) {
					if (IncludeHtmlMarkup) builder.Append("&nbsp;&nbsp;&nbsp;");
					builder.Append(Convert(property.Parameters[i]));
					if (i + 1 < property.Parameters.Count) {
						builder.Append(", ");
					}
					if (IncludeHtmlMarkup) builder.Append("<br>");
				}
				
				builder.Append(')');
			}
			
			if (property.ReturnType != null && ShowReturnType) {
				builder.Append(" As ");
				builder.Append(Convert(property.ReturnType));
			}
			
			return builder.ToString();
		}
		
		public override string Convert(IEvent e)
		{
			CheckThread();
			
			if (e == null)
				throw new ArgumentNullException("e");
			
			StringBuilder builder = new StringBuilder();
			
			builder.Append(ConvertAccessibility(e.Modifiers));
			
			if (ShowModifiers) {
				builder.Append(GetModifier(e));
			}
			
			if (ShowDefinitionKeyWord) {
				builder.Append("Event ");
			}
			
			if (IncludeHtmlMarkup) {
				builder.Append("<b>");
			}
			
			if (UseFullyQualifiedMemberNames) {
				builder.Append(e.FullyQualifiedName);
			} else {
				builder.Append(e.Name);
			}
			
			if (IncludeHtmlMarkup) {
				builder.Append("</b>");
			}
			
			if (e.ReturnType != null && ShowReturnType) {
				builder.Append(" As ");
				builder.Append(Convert(e.ReturnType));
			}
			
			return builder.ToString();
		}
		
		public override string Convert(IMethod m)
		{
			CheckThread();
			
			StringBuilder builder = new StringBuilder();
			if (ShowModifiers && m.IsExtensionMethod) {
				builder.Append("<Extension> ");
			}
			
			builder.Append(ConvertAccessibility(m.Modifiers)); // show visibility
			
			if (ShowModifiers) {
				builder.Append(GetModifier(m));
			}
			if (ShowDefinitionKeyWord) {
				if (m.ReturnType == null || m.ReturnType.FullyQualifiedName == "System.Void") {
					builder.Append("Sub ");
				} else {
					builder.Append("Function ");
				}
			}

			string dispName = UseFullyQualifiedMemberNames ? m.FullyQualifiedName : m.Name;
			if (m.IsConstructor) {
				dispName = "New";
			}
			
			if (IncludeHtmlMarkup) {
				builder.Append("<b>");
			}
			
			builder.Append(dispName);
			
			if (IncludeHtmlMarkup) {
				builder.Append("</b>");
			}
			
			if (ShowTypeParameterList && m.TypeParameters.Count > 0) {
				builder.Append("(Of ");
				for (int i = 0; i < m.TypeParameters.Count; ++i) {
					if (i > 0) builder.Append(", ");
					builder.Append(ConvertTypeParameter(m.TypeParameters[i]));
				}
				builder.Append(')');
			}
			
			if (ShowParameterList) {
				builder.Append("(");
				if (IncludeHtmlMarkup) builder.Append("<br>");

				for (int i = 0; i < m.Parameters.Count; ++i) {
					if (IncludeHtmlMarkup) builder.Append("&nbsp;&nbsp;&nbsp;");
					builder.Append(Convert(m.Parameters[i]));
					if (i + 1 < m.Parameters.Count) {
						builder.Append(", ");
					}
					if (IncludeHtmlMarkup) builder.Append("<br>");
				}
				
				builder.Append(')');
			}
			
			if (ShowReturnType && m.ReturnType != null && m.ReturnType.FullyQualifiedName != "System.Void") {
				builder.Append(" As ");
				builder.Append(Convert(m.ReturnType));
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
		
		public override string ConvertEnd(IMethod m)
		{
			if (m == null)
				throw new ArgumentNullException("m");
			
			if (m.ReturnType == null || m.ReturnType.FullyQualifiedName == "System.Void") {
				return "End Sub";
			} else {
				return "End Function";
			}
		}
		
		public override string Convert(IReturnType returnType)
		{
			CheckThread();
			
			if (returnType == null) {
				return String.Empty;
			}
			StringBuilder builder = new StringBuilder();
			
			string fullName = returnType.FullyQualifiedName;
			string shortName;
			if (fullName != null && TypeConversionTable.TryGetValue(fullName, out shortName)) {
				builder.Append(shortName);
			} else {
				if (UseFullyQualifiedTypeNames) {
					builder.Append(fullName);
				} else {
					builder.Append(returnType.Name);
				}
			}
			
			UnpackNestedType(builder, returnType);
			
			return builder.ToString();
		}
		
		void UnpackNestedType(StringBuilder builder, IReturnType returnType)
		{
			if (returnType.IsArrayReturnType) {
				builder.Append('(');
				int dimensions = returnType.CastToArrayReturnType().ArrayDimensions;
				for (int i = 1; i < dimensions; ++i) {
					builder.Append(',');
				}
				builder.Append(')');
				UnpackNestedType(builder, returnType.CastToArrayReturnType().ArrayElementType);
			} else if (returnType.IsConstructedReturnType) {
				UnpackNestedType(builder, returnType.CastToConstructedReturnType().UnboundType);
				builder.Append("(Of ");
				IList<IReturnType> ta = returnType.CastToConstructedReturnType().TypeArguments;
				for (int i = 0; i < ta.Count; ++i) {
					if (i > 0) builder.Append(", ");
					builder.Append(Convert(ta[i]));
				}
				builder.Append(')');
			}
		}
		
		public override string Convert(IParameter param)
		{
			CheckThread();
			
			if (param == null)
				throw new ArgumentNullException("param");
			
			StringBuilder builder = new StringBuilder();
			if (IncludeHtmlMarkup) {
				builder.Append("<i>");
			}
			
			if (param.IsOptional) {
				builder.Append("Optional ");
			}
			if (param.IsRef || param.IsOut) {
				builder.Append("ByRef ");
			} else if (param.IsParams) {
				builder.Append("ParamArray ");
			}
			if (IncludeHtmlMarkup) {
				builder.Append("</i>");
			}
			
			if (ShowParameterNames) {
				builder.Append(param.Name);
				builder.Append(" As ");
			}

			builder.Append(Convert(param.ReturnType));
			
			return builder.ToString();
		}

		public override string WrapAttribute(string attribute)
		{
			return "<" + attribute + ">";
		}
		
		public override string WrapComment(string comment)
		{
			return "' " + comment;
		}
		
		public override string GetIntrinsicTypeName(string dotNetTypeName)
		{
			string shortName;
			if (TypeConversionTable.TryGetValue(dotNetTypeName, out shortName)) {
				return shortName;
			}
			return dotNetTypeName;
		}
	}
	
}
