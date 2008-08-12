// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 3287 $</version>
// </file>
using System;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;
using System.Collections.Generic;

namespace SharpRefactoring.Transformers
{
	/// <summary>
	/// Description of ReplaceUnnecessaryVariableDeclarationsTransformer.
	/// </summary>
	public class ReplaceUnnecessaryVariableDeclarationsTransformer : AbstractAstTransformer
	{
		List<VariableDeclaration> unneededVarDecls;
		
		public ReplaceUnnecessaryVariableDeclarationsTransformer(List<VariableDeclaration> unneededVarDecls)
		{
			this.unneededVarDecls = unneededVarDecls;
		}
		
		bool Contains(VariableDeclaration varDecl)
		{
			foreach (VariableDeclaration v in this.unneededVarDecls) {
				if (v.Name == varDecl.Name) {
					return true;
				}
			}
			
			return false;
		}
		
		public override object VisitLocalVariableDeclaration(ICSharpCode.NRefactory.Ast.LocalVariableDeclaration localVariableDeclaration, object data)
		{
			bool containsAll = true;
			foreach (VariableDeclaration v in localVariableDeclaration.Variables) {
				if (Contains(v)) {
					localVariableDeclaration.Parent.Children.Add(new ExpressionStatement(
						new AssignmentExpression(new IdentifierExpression(v.Name),
						                         AssignmentOperatorType.Assign,
						                         v.Initializer)));
				} else
					containsAll = false;
			}

			if (containsAll)
				this.RemoveCurrentNode();
			
			return base.VisitLocalVariableDeclaration(localVariableDeclaration, data);
		}
		
		public override object VisitVariableDeclaration(ICSharpCode.NRefactory.Ast.VariableDeclaration variableDeclaration, object data)
		{
			if (!(variableDeclaration.Parent is LocalVariableDeclaration)) {
				this.ReplaceCurrentNode(new ExpressionStatement(new AssignmentExpression(new IdentifierExpression(variableDeclaration.Name), AssignmentOperatorType.Assign, variableDeclaration.Initializer)));
			}
			return base.VisitVariableDeclaration(variableDeclaration, data);
		}
		
	}
}
