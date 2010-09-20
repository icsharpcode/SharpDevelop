// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.NRefactory.Tests.Ast
{
	[TestFixture]
	public class LambdaExpressionTests
	{
		#region C#
		
		static LambdaExpression ParseCSharp(string program)
		{
			return ParseUtilCSharp.ParseExpression<LambdaExpression>(program);
		}
		
		[Test]
		public void ImplicitlyTypedExpressionBody()
		{
			LambdaExpression e = ParseCSharp("(x) => x + 1");
			Assert.AreEqual("x", e.Parameters[0].ParameterName);
			Assert.IsTrue(e.Parameters[0].TypeReference.IsNull);
			Assert.IsTrue(e.ExpressionBody is BinaryOperatorExpression);
			Assert.IsTrue(e.ReturnType.IsNull);
		}
		
		[Test]
		public void ImplicitlyTypedExpressionBodyWithoutParenthesis()
		{
			LambdaExpression e = ParseCSharp("x => x + 1");
			Assert.AreEqual("x", e.Parameters[0].ParameterName);
			Assert.IsTrue(e.Parameters[0].TypeReference.IsNull);
			Assert.IsTrue(e.ExpressionBody is BinaryOperatorExpression);
			Assert.IsTrue(e.ReturnType.IsNull);
		}
		
		[Test]
		public void ImplicitlyTypedStatementBody()
		{
			LambdaExpression e = ParseCSharp("(x) => { return x + 1; }");
			Assert.AreEqual("x", e.Parameters[0].ParameterName);
			Assert.IsTrue(e.Parameters[0].TypeReference.IsNull);
			Assert.IsTrue(e.StatementBody.Children[0] is ReturnStatement);
			Assert.IsTrue(e.ReturnType.IsNull);
		}
		
		[Test]
		public void ImplicitlyTypedStatementBodyWithoutParenthesis()
		{
			LambdaExpression e = ParseCSharp("x => { return x + 1; }");
			Assert.AreEqual("x", e.Parameters[0].ParameterName);
			Assert.IsTrue(e.Parameters[0].TypeReference.IsNull);
			Assert.IsTrue(e.StatementBody.Children[0] is ReturnStatement);
			Assert.IsTrue(e.ReturnType.IsNull);
		}
		
		[Test]
		public void ExplicitlyTypedStatementBody()
		{
			LambdaExpression e = ParseCSharp("(int x) => { return x + 1; }");
			Assert.AreEqual("x", e.Parameters[0].ParameterName);
			Assert.AreEqual("System.Int32", e.Parameters[0].TypeReference.Type);
			Assert.IsTrue(e.StatementBody.Children[0] is ReturnStatement);
			Assert.IsTrue(e.ReturnType.IsNull);
		}
		
		[Test]
		public void ExplicitlyTypedStatementBodyWithRefParameter()
		{
			LambdaExpression e = ParseCSharp("(ref int i) => i = 1");
			Assert.AreEqual("i", e.Parameters[0].ParameterName);
			Assert.IsTrue((e.Parameters[0].ParamModifier & ParameterModifiers.Ref) == ParameterModifiers.Ref);
			Assert.AreEqual("System.Int32", e.Parameters[0].TypeReference.Type);
			Assert.IsTrue(e.ReturnType.IsNull);
		}
		
		[Test]
		public void LambdaExpressionContainingConditionalExpression()
		{
			LambdaExpression e = ParseCSharp("rr => rr != null ? rr.ResolvedType : null");
			Assert.AreEqual("rr", e.Parameters[0].ParameterName);
			Assert.IsTrue(e.ExpressionBody is ConditionalExpression);
			Assert.IsTrue(e.ReturnType.IsNull);
		}
		
		#endregion
		
		#region VB.NET
		
		static LambdaExpression ParseVBNet(string program)
		{
			return ParseUtilVBNet.ParseExpression<LambdaExpression>(program);
		}
		
		[Test]
		public void VBNetLambdaWithParameters()
		{
			LambdaExpression e = ParseVBNet("Function(x As Boolean) x Or True");
			Assert.AreEqual(1, e.Parameters.Count);
			Assert.AreEqual("x", e.Parameters[0].ParameterName);
			Assert.AreEqual("System.Boolean", e.Parameters[0].TypeReference.Type);
			Assert.IsTrue(e.ExpressionBody is BinaryOperatorExpression);
			Assert.IsTrue(e.ReturnType.IsNull);
		}

		[Test]
		public void VBNetLambdaWithoutParameters()
		{
			LambdaExpression e = ParseVBNet("Function x Or True");
			Assert.AreEqual(0, e.Parameters.Count);
			Assert.IsTrue(e.ExpressionBody is BinaryOperatorExpression);
			Assert.IsTrue(e.ReturnType.IsNull, "ReturnType");
		}
		
		[Test]
		public void VBNetNestedLambda()
		{
			LambdaExpression e = ParseVBNet("Function(x As Boolean) Function(y As Boolean) x And y");
			Assert.AreEqual(1, e.Parameters.Count);
			Assert.IsTrue(e.ExpressionBody is LambdaExpression);
			Assert.IsTrue(e.ReturnType.IsNull, "ReturnType");
		}
		
		[Test]
		public void VBNetSubLambda()
		{
			LambdaExpression e = ParseVBNet("Sub(x As Integer) Console.WriteLine(x)");
			Assert.AreEqual(1, e.Parameters.Count);
			Assert.IsTrue(e.ExpressionBody is InvocationExpression);
			Assert.IsNotNull(e.ReturnType);
			Assert.AreEqual("System.Void", e.ReturnType.Type);
			Assert.IsTrue(e.ReturnType.IsKeyword);
		}
		
		[Test]
		public void VBNetSubWithStatementLambda()
		{
			LambdaExpression e = ParseVBNet("Sub(x As Integer) Call Console.WriteLine(x)");
			Assert.AreEqual(1, e.Parameters.Count);
			Assert.IsTrue(e.StatementBody is ExpressionStatement);
			Assert.IsNotNull(e.ReturnType);
			Assert.AreEqual("System.Void", e.ReturnType.Type);
			Assert.IsTrue(e.ReturnType.IsKeyword);
		}
		
		[Test]
		public void VBNetMultilineSubLambda()
		{
			LambdaExpression e = ParseVBNet("Sub(x As Integer)\n" +
			                                "	For i As Integer = 0 To x\n" +
			                                "		Console.WriteLine(i)\n" +
			                                "	Next\n" +
			                                "End Sub");
			Assert.AreEqual(1, e.Parameters.Count);
			Assert.IsTrue(e.StatementBody is BlockStatement);
			Assert.IsNotNull(e.ReturnType);
			Assert.AreEqual("System.Void", e.ReturnType.Type);
			Assert.IsTrue(e.ReturnType.IsKeyword);
			
			BlockStatement b = e.StatementBody as BlockStatement;
			
			Assert.AreEqual(1, b.Children.Count);
			Assert.IsTrue(b.Children[0] is ForNextStatement);
		}
		
		[Test]
		public void VBNetMultilineFunctionLambda()
		{
			LambdaExpression e = ParseVBNet("Function(x As Integer)\n" +
			                                "	Dim prod As Integer = 1\n" +
			                                "	For i As Integer = 1 To x\n" +
			                                "		prod = prod * i\n" +
			                                "	Next\n" +
			                                "	Return prod\n" +
			                                "End Function");
			Assert.AreEqual(1, e.Parameters.Count);
			Assert.IsTrue(e.StatementBody is BlockStatement);
			Assert.IsTrue(e.ReturnType.IsNull);
			
			BlockStatement b = e.StatementBody as BlockStatement;
			
			Assert.AreEqual(3, b.Children.Count);
			Assert.IsTrue(b.Children[0] is LocalVariableDeclaration);
			Assert.IsTrue(b.Children[1] is ForNextStatement);
			Assert.IsTrue(b.Children[2] is ReturnStatement);
		}
		
		[Test]
		public void VBNetMultilineFunctionWithReturnTypeLambda()
		{
			LambdaExpression e = ParseVBNet("Function(x As Integer) As Integer\n" +
			                                "	Dim prod As Integer = 1\n" +
			                                "	For i As Integer = 1 To x\n" +
			                                "		prod = prod * i\n" +
			                                "	Next\n" +
			                                "	Return prod\n" +
			                                "End Function");
			Assert.AreEqual(1, e.Parameters.Count);
			Assert.IsTrue(e.StatementBody is BlockStatement);
			Assert.IsNotNull(e.ReturnType);
			Assert.AreEqual("System.Int32", e.ReturnType.Type);
			Assert.IsTrue(e.ReturnType.IsKeyword);
			
			BlockStatement b = e.StatementBody as BlockStatement;
			
			Assert.AreEqual(3, b.Children.Count);
			Assert.IsTrue(b.Children[0] is LocalVariableDeclaration);
			Assert.IsTrue(b.Children[1] is ForNextStatement);
			Assert.IsTrue(b.Children[2] is ReturnStatement);
		}
		#endregion
	}
}
