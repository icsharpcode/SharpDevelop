// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Dom;
using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public class ConstructorCodeGenerator : AbstractFieldCodeGenerator
	{
		public override string CategoryName {
			get {
				return "${res:ICSharpCode.SharpDevelop.CodeGenerator.Constructor}";
			}
		}
		
		public override string Hint {
			get {
				return "${res:ICSharpCode.SharpDevelop.CodeGenerator.Constructor.Hint}";
			}
		}
		
		public override int ImageIndex {
			get {
				return ClassBrowserIconService.MethodIndex;
			}
		}
		
		public override void GenerateCode(List<AbstractNode> nodes, IList items)
		{
			ConstructorDeclaration ctor = new ConstructorDeclaration(currentClass.Name, Modifiers.Public, null, null);
			ctor.Body = new BlockStatement();
			foreach (FieldWrapper w in items) {
				string parameterName = codeGen.GetParameterName(w.Field.Name);
				ctor.Parameters.Add(new ParameterDeclarationExpression(ConvertType(w.Field.ReturnType),
				                                                       parameterName));
				Expression left  = new MemberReferenceExpression(new ThisReferenceExpression(), w.Field.Name);
				Expression right = new IdentifierExpression(parameterName);
				Expression expr  = new AssignmentExpression(left, AssignmentOperatorType.Assign, right);
				ctor.Body.AddChild(new ExpressionStatement(expr));
			}
			nodes.Add(ctor);
		}
		
		protected override void InitContent()
		{
			foreach (IField field in currentClass.Fields) {
				if (!field.IsStatic) {
					Content.Add(new FieldWrapper(field));
				}
			}
		}
	}
}
