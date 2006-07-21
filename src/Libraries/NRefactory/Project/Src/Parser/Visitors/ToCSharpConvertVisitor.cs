// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.NRefactory.Visitors
{
	/// <summary>
	/// Converts elements not supported by C# to their C# representation.
	/// Not all elements are converted here, most simple elements (e.g. StopStatement)
	/// are converted in the output visitor.
	/// </summary>
	public class ToCSharpConvertVisitor : AbstractAstTransformer
	{
		// The following conversions are implemented:
		//   Public Event EventName(param As String) -> automatic delegate declaration
		
		public override object VisitEventDeclaration(EventDeclaration eventDeclaration, object data)
		{
			if (!eventDeclaration.HasAddRegion && !eventDeclaration.HasRaiseRegion && !eventDeclaration.HasRemoveRegion) {
				if (eventDeclaration.TypeReference.IsNull) {
					DelegateDeclaration dd = new DelegateDeclaration(eventDeclaration.Modifier, null);
					dd.Name = eventDeclaration.Name + "EventHandler";
					dd.Parameters = eventDeclaration.Parameters;
					dd.ReturnType = new TypeReference("System.Void");
					dd.Parent = eventDeclaration.Parent;
					eventDeclaration.Parameters = null;
					int index = eventDeclaration.Parent.Children.IndexOf(eventDeclaration);
					// inserting before current position is not allowed in a Transformer
					eventDeclaration.Parent.Children.Insert(index + 1, dd);
					eventDeclaration.TypeReference = new TypeReference(dd.Name);
				}
			}
			return base.VisitEventDeclaration(eventDeclaration, data);
		}
		
		public override object VisitLocalVariableDeclaration(LocalVariableDeclaration localVariableDeclaration, object data)
		{
			base.VisitLocalVariableDeclaration(localVariableDeclaration, data);
			if ((localVariableDeclaration.Modifier & Modifier.Static) == Modifier.Static) {
				INode parent = localVariableDeclaration.Parent;
				while (parent != null && !IsTypeLevel(parent)) {
					parent = parent.Parent;
				}
				if (parent != null) {
					INode type = parent.Parent;
					if (type != null) {
						int pos = type.Children.IndexOf(parent);
						if (pos >= 0) {
							FieldDeclaration field = new FieldDeclaration(null);
							field.TypeReference = localVariableDeclaration.TypeReference;
							field.Modifier = Modifier.Static;
							field.Fields = localVariableDeclaration.Variables;
							new PrefixFieldsVisitor(field.Fields, "static_" + GetTypeLevelEntityName(parent) + "_").Run(parent);
							type.Children.Insert(pos + 1, field);
							RemoveCurrentNode();
						}
					}
				}
			}
			return null;
		}
		
		static bool IsTypeLevel(INode node)
		{
			return node is MethodDeclaration || node is PropertyDeclaration || node is EventDeclaration
				|| node is OperatorDeclaration || node is FieldDeclaration;
		}
		
		static string GetTypeLevelEntityName(INode node)
		{
			if (node is ParametrizedNode)
				return ((ParametrizedNode)node).Name;
			else if (node is FieldDeclaration)
				return ((FieldDeclaration)node).Fields[0].Name;
			else
				throw new ArgumentException();
		}
	}
}
