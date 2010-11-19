// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.Parser.Expression
{
	[TestFixture]
	public class BinaryOperatorExpressionTests
	{
		#region Precedence Tests
		void OperatorPrecedenceTest(string strongOperator, BinaryOperatorType strongOperatorType,
		                            string weakOperator, BinaryOperatorType weakOperatorType, bool vb)
		{
			string program = "a " + weakOperator + " b " + strongOperator + " c";
			BinaryOperatorExpression boe = ParseUtilCSharp.ParseExpression<BinaryOperatorExpression>(program);
			Assert.AreEqual(weakOperatorType, boe.BinaryOperatorType);
			Assert.IsTrue(boe.Left is IdentifierExpression);
			boe = (BinaryOperatorExpression)boe.Right;
			Assert.AreEqual(strongOperatorType, boe.BinaryOperatorType);
			Assert.IsTrue(boe.Left is IdentifierExpression);
			Assert.IsTrue(boe.Right is IdentifierExpression);
			
			program = "a " + strongOperator + " b " + weakOperator + " c";
			
			boe = ParseUtilCSharp.ParseExpression<BinaryOperatorExpression>(program);
			Assert.AreEqual(weakOperatorType, boe.BinaryOperatorType);
			Assert.IsTrue(boe.Right is IdentifierExpression);
			boe = (BinaryOperatorExpression)boe.Left;
			Assert.AreEqual(strongOperatorType, boe.BinaryOperatorType);
			Assert.IsTrue(boe.Left is IdentifierExpression);
			Assert.IsTrue(boe.Right is IdentifierExpression);
		}
		
		void SameOperatorPrecedenceTest(string firstOperator, BinaryOperatorType firstOperatorType,
		                                string secondOperator, BinaryOperatorType secondOperatorType, bool vb)
		{
			string program = "a " + secondOperator + " b " + firstOperator + " c";
			BinaryOperatorExpression boe = ParseUtilCSharp.ParseExpression<BinaryOperatorExpression>(program);
			Assert.AreEqual(firstOperatorType, boe.BinaryOperatorType);
			Assert.IsTrue(boe.Right is IdentifierExpression);
			boe = (BinaryOperatorExpression)boe.Left;
			Assert.AreEqual(secondOperatorType, boe.BinaryOperatorType);
			Assert.IsTrue(boe.Left is IdentifierExpression);
			Assert.IsTrue(boe.Right is IdentifierExpression);
			
			program = "a " + firstOperator + " b " + secondOperator + " c";
			boe = ParseUtilCSharp.ParseExpression<BinaryOperatorExpression>(program);
			Assert.AreEqual(secondOperatorType, boe.BinaryOperatorType);
			Assert.IsTrue(boe.Right is IdentifierExpression);
			boe = (BinaryOperatorExpression)boe.Left;
			Assert.AreEqual(firstOperatorType, boe.BinaryOperatorType);
			Assert.IsTrue(boe.Left is IdentifierExpression);
			Assert.IsTrue(boe.Right is IdentifierExpression);
		}
		
		[Test]
		public void CSharpOperatorPrecedenceTest()
		{
			SameOperatorPrecedenceTest("*", BinaryOperatorType.Multiply, "/", BinaryOperatorType.Divide, false);
			SameOperatorPrecedenceTest("*", BinaryOperatorType.Multiply, "%", BinaryOperatorType.Modulus, false);
			OperatorPrecedenceTest("*", BinaryOperatorType.Multiply, "+", BinaryOperatorType.Add, false);
			SameOperatorPrecedenceTest("-", BinaryOperatorType.Subtract, "+", BinaryOperatorType.Add, false);
			OperatorPrecedenceTest("+", BinaryOperatorType.Add, "<<", BinaryOperatorType.ShiftLeft, false);
			SameOperatorPrecedenceTest(">>", BinaryOperatorType.ShiftRight, "<<", BinaryOperatorType.ShiftLeft, false);
			OperatorPrecedenceTest("<<", BinaryOperatorType.ShiftLeft, "==", BinaryOperatorType.Equality, false);
			SameOperatorPrecedenceTest("!=", BinaryOperatorType.InEquality, "==", BinaryOperatorType.Equality, false);
			OperatorPrecedenceTest("==", BinaryOperatorType.Equality, "&", BinaryOperatorType.BitwiseAnd, false);
			OperatorPrecedenceTest("&", BinaryOperatorType.BitwiseAnd, "^", BinaryOperatorType.ExclusiveOr, false);
			OperatorPrecedenceTest("^", BinaryOperatorType.ExclusiveOr, "|", BinaryOperatorType.BitwiseOr, false);
			OperatorPrecedenceTest("|", BinaryOperatorType.BitwiseOr, "&&", BinaryOperatorType.LogicalAnd, false);
			OperatorPrecedenceTest("&&", BinaryOperatorType.LogicalAnd, "||", BinaryOperatorType.LogicalOr, false);
			OperatorPrecedenceTest("||", BinaryOperatorType.LogicalOr, "??", BinaryOperatorType.NullCoalescing, false);
		}
		#endregion
		
		void CSharpTestBinaryOperatorExpressionTest(string program, BinaryOperatorType op)
		{
			BinaryOperatorExpression boe = ParseUtilCSharp.ParseExpression<BinaryOperatorExpression>(program);
			Assert.AreEqual(op, boe.BinaryOperatorType);
			
			Assert.IsTrue(boe.Left is IdentifierExpression);
			Assert.IsTrue(boe.Right is IdentifierExpression);
			
		}
		
		[Test]
		public void CSharpSubtractionLeftToRight()
		{
			BinaryOperatorExpression boe = ParseUtilCSharp.ParseExpression<BinaryOperatorExpression>("a - b - c");
			Assert.IsTrue(boe.Right is IdentifierExpression);
			Assert.IsTrue(boe.Left is BinaryOperatorExpression);
		}
		
		[Test]
		public void CSharpNullCoalescingRightToLeft()
		{
			BinaryOperatorExpression boe = ParseUtilCSharp.ParseExpression<BinaryOperatorExpression>("a ?? b ?? c");
			Assert.IsTrue(boe.Left is IdentifierExpression);
			Assert.IsTrue(boe.Right is BinaryOperatorExpression);
		}
		
		[Test]
		public void CSharpBitwiseAndTest()
		{
			CSharpTestBinaryOperatorExpressionTest("a & b", BinaryOperatorType.BitwiseAnd);
		}
		
		[Test]
		public void CSharpBitwiseOrTest()
		{
			CSharpTestBinaryOperatorExpressionTest("a | b", BinaryOperatorType.BitwiseOr);
		}
		
		[Test]
		public void CSharpLogicalAndTest()
		{
			CSharpTestBinaryOperatorExpressionTest("a && b", BinaryOperatorType.LogicalAnd);
		}
		
		[Test]
		public void CSharpLogicalOrTest()
		{
			CSharpTestBinaryOperatorExpressionTest("a || b", BinaryOperatorType.LogicalOr);
		}
		
		[Test]
		public void CSharpExclusiveOrTest()
		{
			CSharpTestBinaryOperatorExpressionTest("a ^ b", BinaryOperatorType.ExclusiveOr);
		}
		
		
		[Test]
		public void CSharpGreaterThanTest()
		{
			CSharpTestBinaryOperatorExpressionTest("a > b", BinaryOperatorType.GreaterThan);
		}
		
		[Test]
		public void CSharpGreaterThanOrEqualTest()
		{
			CSharpTestBinaryOperatorExpressionTest("a >= b", BinaryOperatorType.GreaterThanOrEqual);
		}
		
		[Test]
		public void CSharpEqualityTest()
		{
			CSharpTestBinaryOperatorExpressionTest("a == b", BinaryOperatorType.Equality);
		}
		
		[Test]
		public void CSharpInEqualityTest()
		{
			CSharpTestBinaryOperatorExpressionTest("a != b", BinaryOperatorType.InEquality);
		}
		
		[Test]
		public void CSharpLessThanTest()
		{
			CSharpTestBinaryOperatorExpressionTest("a < b", BinaryOperatorType.LessThan);
		}
		
		[Test]
		public void CSharpLessThanOrEqualTest()
		{
			CSharpTestBinaryOperatorExpressionTest("a <= b", BinaryOperatorType.LessThanOrEqual);
		}
		
		[Test]
		public void CSharpAddTest()
		{
			CSharpTestBinaryOperatorExpressionTest("a + b", BinaryOperatorType.Add);
		}
		
		[Test]
		public void CSharpSubtractTest()
		{
			CSharpTestBinaryOperatorExpressionTest("a - b", BinaryOperatorType.Subtract);
		}
		
		[Test]
		public void CSharpMultiplyTest()
		{
			CSharpTestBinaryOperatorExpressionTest("a * b", BinaryOperatorType.Multiply);
		}
		
		[Test]
		public void CSharpDivideTest()
		{
			CSharpTestBinaryOperatorExpressionTest("a / b", BinaryOperatorType.Divide);
		}
		
		[Test]
		public void CSharpModulusTest()
		{
			CSharpTestBinaryOperatorExpressionTest("a % b", BinaryOperatorType.Modulus);
		}
		
		[Test]
		public void CSharpShiftLeftTest()
		{
			CSharpTestBinaryOperatorExpressionTest("a << b", BinaryOperatorType.ShiftLeft);
		}
		
		[Test]
		public void CSharpShiftRightTest()
		{
			CSharpTestBinaryOperatorExpressionTest("a >> b", BinaryOperatorType.ShiftRight);
		}
		
		[Test]
		public void CSharpNullCoalescingTest()
		{
			CSharpTestBinaryOperatorExpressionTest("a ?? b", BinaryOperatorType.NullCoalescing);
		}
		
		[Test]
		public void CSharpLessThanOrGreaterTest()
		{
			const string expr = "i1 < 0 || i1 > (Count - 1)";
			BinaryOperatorExpression boe = ParseUtilCSharp.ParseExpression<BinaryOperatorExpression>(expr);
			Assert.AreEqual(BinaryOperatorType.LogicalOr, boe.BinaryOperatorType);
		}
	}
}
