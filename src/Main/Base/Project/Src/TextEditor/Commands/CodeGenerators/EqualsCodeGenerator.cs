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
		
		static int[] largePrimes = {
			1000000007, 1000000009, 1000000021, 1000000033,
			1000000087, 1000000093, 1000000097, 1000000103,
			1000000123, 1000000181, 1000000207, 1000000223,
			1000000241, 1000000271, 1000000289, 1000000297,
			1000000321, 1000000349, 1000000363, 1000000403,
			1000000409, 1000000411, 1000000427, 1000000433,
			1000000439, 1000000447, 1000000453, 1000000459,
			1000000483, 1000000513, 1000000531, 1000000579
		};
		
		public override void GenerateCode(List<AbstractNode> nodes, IList items)
		{
			TypeReference intReference = new TypeReference("System.Int32", true);
			MethodDeclaration method = new MethodDeclaration {
				Name = "GetHashCode",
				Modifier = Modifiers.Public | Modifiers.Override,
				TypeReference = intReference,
				Body = new BlockStatement()
			};
			
			VariableDeclaration var;
			var = new VariableDeclaration("hashCode", new PrimitiveExpression(0, "0"), intReference);
			method.Body.AddChild(new LocalVariableDeclaration(var));
			
			bool usePrimeMultiplication = currentClass.ProjectContent.Language == LanguageProperties.CSharp;
			BlockStatement hashCalculationBlock;
			if (usePrimeMultiplication) {
				hashCalculationBlock = new BlockStatement();
				method.Body.AddChild(new UncheckedStatement(hashCalculationBlock));
			} else {
				hashCalculationBlock = method.Body;
			}
			
			int fieldIndex = 0;
			Expression expr;
			
			foreach (IField field in currentClass.Fields) {
				if (field.IsStatic) continue;
				
				expr = new InvocationExpression(new MemberReferenceExpression(new IdentifierExpression(field.Name), "GetHashCode"));
				if (usePrimeMultiplication) {
					int prime = largePrimes[fieldIndex++ % largePrimes.Length];
					expr = new AssignmentExpression(
						new IdentifierExpression(var.Name),
						AssignmentOperatorType.Add,
						new BinaryOperatorExpression(new PrimitiveExpression(prime, prime.ToString()),
						                             BinaryOperatorType.Multiply,
						                             expr));
				} else {
					expr = new AssignmentExpression(new IdentifierExpression(var.Name),
					                                AssignmentOperatorType.ExclusiveOr,
					                                expr);
				}
				if (IsValueType(field.ReturnType)) {
					hashCalculationBlock.AddChild(new ExpressionStatement(expr));
				} else {
					hashCalculationBlock.AddChild(new IfElseStatement(
						new BinaryOperatorExpression(new IdentifierExpression(field.Name),
						                             BinaryOperatorType.ReferenceInequality,
						                             new PrimitiveExpression(null, "null")),
						new ExpressionStatement(expr)
					));
				}
			}
			method.Body.AddChild(new ReturnStatement(new IdentifierExpression(var.Name)));
			nodes.Add(method);
			
			TypeReference boolReference = new TypeReference("System.Boolean", true);
			TypeReference objectReference = new TypeReference("System.Object", true);
			
			method = new MethodDeclaration {
				Name = "Equals",
				Modifier = Modifiers.Public | Modifiers.Override,
				TypeReference = boolReference
			};
			method.Parameters.Add(new ParameterDeclarationExpression(objectReference, "obj"));
			method.Body = new BlockStatement();
			
			TypeReference currentType = ConvertType(currentClass.DefaultReturnType);
			
			if (currentClass.ClassType == Dom.ClassType.Struct) {
				// return obj is CurrentType && Equals((CurrentType)obj);
				expr = new TypeOfIsExpression(new IdentifierExpression("obj"), currentType);
				expr = new ParenthesizedExpression(expr);
				expr = new BinaryOperatorExpression(
					expr, BinaryOperatorType.LogicalAnd,
					new InvocationExpression(
						new IdentifierExpression("Equals"),
						new List<Expression> {
							new CastExpression(currentType, new IdentifierExpression("obj"), CastType.Cast)
						}));
				method.Body.AddChild(new ReturnStatement(expr));
				
				nodes.Add(method);
				
				// IEquatable implementation:
				method = new MethodDeclaration {
					Name = "Equals",
					Modifier = Modifiers.Public | Modifiers.Override,
					TypeReference = boolReference
				};
				method.Parameters.Add(new ParameterDeclarationExpression(currentType, "other"));
				method.Body = new BlockStatement();
			} else {
				method.Body.AddChild(new LocalVariableDeclaration(new VariableDeclaration(
					"other",
					new CastExpression(currentType, new IdentifierExpression("obj"), CastType.TryCast),
					currentType)));
				method.Body.AddChild(new IfElseStatement(
					new BinaryOperatorExpression(new IdentifierExpression("other"), BinaryOperatorType.ReferenceEquality, new PrimitiveExpression(null, "null")),
					new ReturnStatement(new PrimitiveExpression(false, "false"))));
				
//				expr = new BinaryOperatorExpression(new ThisReferenceExpression(),
//				                                    BinaryOperatorType.ReferenceEquality,
//				                                    new IdentifierExpression("obj"));
//				method.Body.AddChild(new IfElseStatement(expr, new ReturnStatement(new PrimitiveExpression(true, "true"))));
			}
			
			
			expr = null;
			foreach (IField field in currentClass.Fields) {
				if (field.IsStatic) continue;
				
				if (expr == null) {
					expr = TestEquality("other", field);
				} else {
					expr = new BinaryOperatorExpression(expr, BinaryOperatorType.LogicalAnd,
					                                    TestEquality("other", field));
				}
			}
			
			method.Body.AddChild(new ReturnStatement(expr ?? new PrimitiveExpression(true, "true")));
			
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
				return new BinaryOperatorExpression(new MemberReferenceExpression(new ThisReferenceExpression(), field.Name),
				                                    BinaryOperatorType.Equality,
				                                    new MemberReferenceExpression(new IdentifierExpression(other), field.Name));
			} else {
				InvocationExpression ie = new InvocationExpression(
					new MemberReferenceExpression(new TypeReferenceExpression(new TypeReference("System.Object", true)), "Equals")
				);
				ie.Arguments.Add(new MemberReferenceExpression(new ThisReferenceExpression(), field.Name));
				ie.Arguments.Add(new MemberReferenceExpression(new IdentifierExpression(other), field.Name));
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
