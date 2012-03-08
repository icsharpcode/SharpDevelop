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
	public class UnaryOperatorExpressionTests
	{
		#region C#
		void CSharpTestUnaryOperatorExpressionTest(string program, UnaryOperatorType op)
		{
			UnaryOperatorExpression uoe = ParseUtilCSharp.ParseExpression<UnaryOperatorExpression>(program);
			Assert.AreEqual(op, uoe.Op);
			
			Assert.IsTrue(uoe.Expression is IdentifierExpression);
		}
		
		[Test]
		public void CSharpNotTest()
		{
			CSharpTestUnaryOperatorExpressionTest("!a", UnaryOperatorType.Not);
		}
		
		[Test]
		public void CSharpAwaitTest()
		{
			CSharpTestUnaryOperatorExpressionTest("await a", UnaryOperatorType.Await);
		}
		
		[Test]
		public void CSharpBitNotTest()
		{
			CSharpTestUnaryOperatorExpressionTest("~a", UnaryOperatorType.BitNot);
		}
		
		[Test]
		public void CSharpMinusTest()
		{
			CSharpTestUnaryOperatorExpressionTest("-a", UnaryOperatorType.Minus);
		}
		
		[Test]
		public void CSharpPlusTest()
		{
			CSharpTestUnaryOperatorExpressionTest("+a", UnaryOperatorType.Plus);
		}
		
		[Test]
		public void CSharpIncrementTest()
		{
			CSharpTestUnaryOperatorExpressionTest("++a", UnaryOperatorType.Increment);
		}
		
		[Test]
		public void CSharpDecrementTest()
		{
			CSharpTestUnaryOperatorExpressionTest("--a", UnaryOperatorType.Decrement);
		}
		
		[Test]
		public void CSharpPostIncrementTest()
		{
			CSharpTestUnaryOperatorExpressionTest("a++", UnaryOperatorType.PostIncrement);
		}
		
		[Test]
		public void CSharpPostDecrementTest()
		{
			CSharpTestUnaryOperatorExpressionTest("a--", UnaryOperatorType.PostDecrement);
		}
		
		[Test]
		public void CSharpStarTest()
		{
			CSharpTestUnaryOperatorExpressionTest("*a", UnaryOperatorType.Dereference);
		}
		
		[Test]
		public void CSharpBitWiseAndTest()
		{
			CSharpTestUnaryOperatorExpressionTest("&a", UnaryOperatorType.AddressOf);
		}
		
		[Test]
		public void CSharpDereferenceAfterCast()
		{
			UnaryOperatorExpression uoe = ParseUtilCSharp.ParseExpression<UnaryOperatorExpression>("*((SomeType*) &w)");
			Assert.AreEqual(UnaryOperatorType.Dereference, uoe.Op);
			ParenthesizedExpression pe = (ParenthesizedExpression)uoe.Expression;
			CastExpression ce = (CastExpression)pe.Expression;
			Assert.AreEqual("SomeType", ce.CastTo.Type);
			Assert.AreEqual(1, ce.CastTo.PointerNestingLevel);
			UnaryOperatorExpression adrOf = (UnaryOperatorExpression)ce.Expression;
			Assert.AreEqual(UnaryOperatorType.AddressOf, adrOf.Op);
		}
		
		[Test]
		public void NestedAwait()
		{
			var uoe = ParseUtilCSharp.ParseExpression<UnaryOperatorExpression>("await await a");
			Assert.AreEqual(UnaryOperatorType.Await, uoe.Op);
			var nested = (UnaryOperatorExpression)uoe.Expression;
			Assert.AreEqual(UnaryOperatorType.Await, nested.Op);
			Assert.IsTrue(nested.Expression is IdentifierExpression);
		}
		
		[Test]
		public void AwaitStaticMethodCall()
		{
			var uoe = ParseUtilCSharp.ParseExpression<UnaryOperatorExpression>("await Task.WhenAll(a, b)");
			Assert.AreEqual(UnaryOperatorType.Await, uoe.Op);
			Assert.IsTrue(uoe.Expression is InvocationExpression);
		}
		
		[Test]
		public void AwaitStaticMethodCallStatement()
		{
			var es = ParseUtilCSharp.ParseStatement<ExpressionStatement>("await Task.WhenAll(a, b);");
			UnaryOperatorExpression uoe = (UnaryOperatorExpression)es.Expression;
			Assert.AreEqual(UnaryOperatorType.Await, uoe.Op);
			Assert.IsTrue(uoe.Expression is InvocationExpression);
		}
		#endregion
		
		#region VB.NET
		void VBNetTestUnaryOperatorExpressionTest(string program, UnaryOperatorType op)
		{
			UnaryOperatorExpression uoe = ParseUtilVBNet.ParseExpression<UnaryOperatorExpression>(program);
			Assert.AreEqual(op, uoe.Op);
			
			Assert.IsTrue(uoe.Expression is IdentifierExpression);
		}
		
		[Test]
		public void VBNetNotTest()
		{
			VBNetTestUnaryOperatorExpressionTest("Not a", UnaryOperatorType.Not);
		}
		
		[Test]
		public void VBNetInEqualsNotTest()
		{
			BinaryOperatorExpression e = ParseUtilVBNet.ParseExpression<BinaryOperatorExpression>("b <> Not a");
			Assert.AreEqual(BinaryOperatorType.InEquality, e.Op);
			UnaryOperatorExpression ue = (UnaryOperatorExpression)e.Right;
			Assert.AreEqual(UnaryOperatorType.Not, ue.Op);
		}
		
		[Test]
		public void VBNetNotEqualTest()
		{
			UnaryOperatorExpression e = ParseUtilVBNet.ParseExpression<UnaryOperatorExpression>("Not a = b");
			Assert.AreEqual(UnaryOperatorType.Not, e.Op);
			BinaryOperatorExpression boe = (BinaryOperatorExpression)e.Expression;
			Assert.AreEqual(BinaryOperatorType.Equality, boe.Op);
		}
		
		[Test]
		public void VBNetPlusTest()
		{
			VBNetTestUnaryOperatorExpressionTest("+a", UnaryOperatorType.Plus);
		}
		
		[Test]
		public void VBNetMinusTest()
		{
			VBNetTestUnaryOperatorExpressionTest("-a", UnaryOperatorType.Minus);
		}
		#endregion
	}
}
