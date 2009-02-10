// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public class ToStringCodeGenerator : AbstractFieldCodeGenerator
	{
		public override void GenerateCode(List<AbstractNode> nodes, IList items)
		{
			TypeReference stringReference = new TypeReference("System.String", true);
			MethodDeclaration method = new MethodDeclaration {
				Name = "ToString",
				Modifier = Modifiers.Public | Modifiers.Override,
				TypeReference = stringReference,
			};
			method.Body = new BlockStatement();
			Expression target = new MemberReferenceExpression(new TypeReferenceExpression(stringReference),
			                                                  "Format");
			InvocationExpression methodCall = new InvocationExpression(target);
			StringBuilder formatString = new StringBuilder();
			formatString.Append('[');
			formatString.Append(currentClass.Name);
			for (int i = 0; i < items.Count; i++) {
				formatString.Append(' ');
				formatString.Append(codeGen.GetPropertyName(((FieldWrapper)items[i]).Field.Name));
				formatString.Append("={");
				formatString.Append(i);
				formatString.Append('}');
			}
			formatString.Append(']');
			methodCall.Arguments.Add(new PrimitiveExpression(formatString.ToString(), formatString.ToString()));
			foreach (FieldWrapper w in items) {
				methodCall.Arguments.Add(new MemberReferenceExpression(new ThisReferenceExpression(), w.Field.Name));
			}
			method.Body.AddChild(new ReturnStatement(methodCall));
			nodes.Add(method);
		}
		
		public override string CategoryName {
			get {
				return "${res:ICSharpCode.SharpDevelop.CodeGenerator.ToString}";
			}
		}
		
		public override string Hint {
			get {
				return "${res:ICSharpCode.SharpDevelop.CodeGenerator.ToString.Hint}";
			}
		}
		public override int ImageIndex {
			get {
				return ClassBrowserIconService.MethodIndex;
			}
		}
	}
}
