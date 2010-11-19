// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.Parser.Expression
{
	[TestFixture]
	public class UnaryOperatorExpressionTests
	{
		void CSharpTestUnaryOperatorExpressionTest(string program, UnaryOperatorType op)
		{
			UnaryOperatorExpression uoe = ParseUtilCSharp.ParseExpression<UnaryOperatorExpression>(program);
			Assert.AreEqual(op, uoe.UnaryOperatorType);
			
			Assert.IsTrue(uoe.Expression is IdentifierExpression);
		}
		
		[Test]
		public void CSharpNotTest()
		{
			CSharpTestUnaryOperatorExpressionTest("!a", UnaryOperatorType.Not);
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
			Assert.AreEqual(UnaryOperatorType.Dereference, uoe.UnaryOperatorType);
			ParenthesizedExpression pe = (ParenthesizedExpression)uoe.Expression;
			CastExpression ce = (CastExpression)pe.Expression;
			//Assert.AreEqual("SomeType", ce.CastTo.Type);
			//Assert.AreEqual(1, ce.CastTo.PointerNestingLevel);
			Assert.Ignore("need to check target type"); // TODO
			
			UnaryOperatorExpression adrOf = (UnaryOperatorExpression)ce.Expression;
			Assert.AreEqual(UnaryOperatorType.AddressOf, adrOf.UnaryOperatorType);
		}
	}
}
