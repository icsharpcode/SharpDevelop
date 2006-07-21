// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public class EqualsCodeGenerator : CodeGeneratorBase
	{
		public override string CategoryName {
			get {
				return "${res:ICSharpCode.SharpDevelop.CodeGenerator.GenerateEqualsAndGetHashCode}";
			}
		}
		
		public override void GenerateCode(List<AbstractNode> nodes, IList items)
		{
			TypeReference intReference = new TypeReference("System.Int32");
			MethodDeclaration method = new MethodDeclaration("GetHashCode", Modifiers.Public | Modifiers.Override, intReference, null, null);
			Expression expr = CallGetHashCode(new IdentifierExpression(currentClass.Fields[0].Name));
			for (int i = 1; i < currentClass.Fields.Count; i++) {
				IdentifierExpression identifier = new IdentifierExpression(currentClass.Fields[i].Name);
				expr = new BinaryOperatorExpression(expr, BinaryOperatorType.ExclusiveOr,
				                                    CallGetHashCode(identifier));
			}
			method.Body = new BlockStatement();
			method.Body.AddChild(new ReturnStatement(expr));
			nodes.Add(method);
			
			TypeReference boolReference = new TypeReference("System.Boolean");
			TypeReference objectReference = new TypeReference("System.Object");
			
			method = new MethodDeclaration("Equals", Modifiers.Public | Modifiers.Override, boolReference, null, null);
			method.Parameters.Add(new ParameterDeclarationExpression(objectReference, "obj"));
			method.Body = new BlockStatement();
			
			TypeReference currentType = ConvertType(currentClass.DefaultReturnType);
			expr = new TypeOfIsExpression(new IdentifierExpression("obj"), currentType);
			expr = new ParenthesizedExpression(expr);
			expr = new UnaryOperatorExpression(expr, UnaryOperatorType.Not);
			method.Body.AddChild(new IfElseStatement(expr, new ReturnStatement(new PrimitiveExpression(false, "false"))));
			
			expr = new BinaryOperatorExpression(new ThisReferenceExpression(),
			                                    BinaryOperatorType.Equality,
			                                    new IdentifierExpression("obj"));
			method.Body.AddChild(new IfElseStatement(expr, new ReturnStatement(new PrimitiveExpression(true, "true"))));
			
			VariableDeclaration var = new VariableDeclaration("my" + currentClass.Name,
			                                                  new CastExpression(currentType, new IdentifierExpression("obj"), CastType.Cast),
			                                                  currentType);
			method.Body.AddChild(new LocalVariableDeclaration(var));
			
			expr = TestEquality(var.Name, currentClass.Fields[0]);
			for (int i = 1; i < currentClass.Fields.Count; i++) {
				expr = new BinaryOperatorExpression(expr, BinaryOperatorType.LogicalAnd,
				                                    TestEquality(var.Name, currentClass.Fields[i]));
			}
			
			method.Body.AddChild(new ReturnStatement(expr));
			
			nodes.Add(method);
		}
		
		static InvocationExpression CallGetHashCode(Expression expr)
		{
			return new InvocationExpression(new FieldReferenceExpression(expr, "GetHashCode"));
		}
		
		static Expression TestEquality(string other, IField field)
		{
			return new BinaryOperatorExpression(new FieldReferenceExpression(new ThisReferenceExpression(), field.Name),
			                                    BinaryOperatorType.Equality,
			                                    new FieldReferenceExpression(new IdentifierExpression(other), field.Name));
		}
		
		public override bool IsActive {
			get {
				return currentClass.Fields != null && currentClass.Fields.Count > 0;
			}
		}
		public override int ImageIndex {
			get {
				return ClassBrowserIconService.MethodIndex;
			}
		}
	}
}
