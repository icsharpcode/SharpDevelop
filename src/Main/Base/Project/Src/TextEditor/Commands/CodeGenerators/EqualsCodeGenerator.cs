// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;

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
			method.Body = new BlockStatement();
			
			VariableDeclaration var;
			var = new VariableDeclaration("hashCode", new PrimitiveExpression(0, "0"), intReference);
			method.Body.AddChild(new LocalVariableDeclaration(var));
			
			Expression expr;
			
			foreach (IField field in currentClass.Fields) {
				expr = new AssignmentExpression(new IdentifierExpression(var.Name),
				                                AssignmentOperatorType.ExclusiveOr,
				                                new InvocationExpression(new FieldReferenceExpression(new IdentifierExpression(field.Name), "GetHashCode")));
				if (IsValueType(field.ReturnType)) {
					method.Body.AddChild(new ExpressionStatement(expr));
				} else {
					method.Body.AddChild(new IfElseStatement(
						new BinaryOperatorExpression(new IdentifierExpression(field.Name),
						                             BinaryOperatorType.ReferenceInequality,
						                             new PrimitiveExpression(null, "null")),
						new ExpressionStatement(expr)
					));
				}
			}
			method.Body.AddChild(new ReturnStatement(new IdentifierExpression(var.Name)));
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
			
//			expr = new BinaryOperatorExpression(new ThisReferenceExpression(),
//			                                    BinaryOperatorType.ReferenceEquality,
//			                                    new IdentifierExpression("obj"));
//			method.Body.AddChild(new IfElseStatement(expr, new ReturnStatement(new PrimitiveExpression(true, "true"))));
			
			var = new VariableDeclaration("my" + currentClass.Name,
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
		
		static bool IsValueType(IReturnType type)
		{
			IClass c = type.GetUnderlyingClass();
			return c != null && (c.ClassType == Dom.ClassType.Struct || c.ClassType == Dom.ClassType.Enum);
		}
		
		static bool CanCompareEqualityWithOperator(IReturnType type)
		{
			// return true for value types except float and double
			// return false for reference types except string.
			IClass c = type.GetUnderlyingClass();
			return c != null
				&& c.FullyQualifiedName != "System.Single"
				&& c.FullyQualifiedName != "System.Double"
				&& (c.ClassType == Dom.ClassType.Struct
				    || c.ClassType == Dom.ClassType.Enum
				    || c.FullyQualifiedName == "System.String");
		}
		
		static Expression TestEquality(string other, IField field)
		{
			if (CanCompareEqualityWithOperator(field.ReturnType)) {
				return new BinaryOperatorExpression(new FieldReferenceExpression(new ThisReferenceExpression(), field.Name),
				                                    BinaryOperatorType.Equality,
				                                    new FieldReferenceExpression(new IdentifierExpression(other), field.Name));
			} else {
				InvocationExpression ie = new InvocationExpression(
					new FieldReferenceExpression(new TypeReferenceExpression("System.Object"), "Equals")
				);
				ie.Arguments.Add(new FieldReferenceExpression(new ThisReferenceExpression(), field.Name));
				ie.Arguments.Add(new FieldReferenceExpression(new IdentifierExpression(other), field.Name));
				return ie;
			}
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
