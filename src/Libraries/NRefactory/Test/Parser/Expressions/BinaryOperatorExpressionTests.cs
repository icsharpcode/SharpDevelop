// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Tests.AST
{
	[TestFixture]
	public class BinaryOperatorExpressionTests
	{
		#region C#
		void CSharpTestBinaryOperatorExpressionTest(string program, BinaryOperatorType op)
		{
			BinaryOperatorExpression boe = (BinaryOperatorExpression)ParseUtilCSharp.ParseExpression(program, typeof(BinaryOperatorExpression));
			Assert.AreEqual(op, boe.Op);
			
			Assert.IsTrue(boe.Left is IdentifierExpression);
			Assert.IsTrue(boe.Right is IdentifierExpression);
			
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
		public void CSharpISTest()
		{
			BinaryOperatorExpression boe = (BinaryOperatorExpression)ParseUtilCSharp.ParseExpression("a is b", typeof(BinaryOperatorExpression));
			Assert.AreEqual(BinaryOperatorType.TypeCheck, boe.Op);
			
			Assert.IsTrue(boe.Left is IdentifierExpression);
			Assert.IsTrue(boe.Right is TypeReferenceExpression);
		}
		
		[Test]
		public void CSharpASTest()
		{
			BinaryOperatorExpression boe = (BinaryOperatorExpression)ParseUtilCSharp.ParseExpression("a as b", typeof(BinaryOperatorExpression));
			Assert.AreEqual(BinaryOperatorType.AsCast, boe.Op);
			
			Assert.IsTrue(boe.Left is IdentifierExpression);
			Assert.IsTrue(boe.Right is TypeReferenceExpression);
		}
		#endregion
		
		#region VB.NET
		void VBNetTestBinaryOperatorExpressionTest(string program, BinaryOperatorType op)
		{
			BinaryOperatorExpression boe = (BinaryOperatorExpression)ParseUtilVBNet.ParseExpression(program, typeof(BinaryOperatorExpression));
			Assert.AreEqual(op, boe.Op);
			
			Assert.IsTrue(boe.Left is IdentifierExpression);
			Assert.IsTrue(boe.Right is IdentifierExpression);
			
		}
		
		[Test]
		public void VBNetPowerTest()
		{
			VBNetTestBinaryOperatorExpressionTest("a ^ b", BinaryOperatorType.Power);
		}
		
		[Test]
		public void VBNetConcatTest()
		{
			VBNetTestBinaryOperatorExpressionTest("a & b", BinaryOperatorType.Concat);
		}
		
		[Test]
		public void VBNetLogicalAndTest()
		{
			VBNetTestBinaryOperatorExpressionTest("a AndAlso b", BinaryOperatorType.LogicalAnd);
		}
		[Test]
		public void VBNetLogicalAndNotLazyTest()
		{
			VBNetTestBinaryOperatorExpressionTest("a And b", BinaryOperatorType.BitwiseAnd);
		}
		
		[Test]
		public void VBNetLogicalOrTest()
		{
			VBNetTestBinaryOperatorExpressionTest("a OrElse b", BinaryOperatorType.LogicalOr);
		}
		[Test]
		public void VBNetLogicalOrNotLazyTest()
		{
			VBNetTestBinaryOperatorExpressionTest("a Or b", BinaryOperatorType.BitwiseOr);
		}
		
		[Test]
		public void VBNetExclusiveOrTest()
		{
			VBNetTestBinaryOperatorExpressionTest("a Xor b", BinaryOperatorType.ExclusiveOr);
		}
		
		
		[Test]
		public void VBNetGreaterThanTest()
		{
			VBNetTestBinaryOperatorExpressionTest("a > b", BinaryOperatorType.GreaterThan);
		}
		
		[Test]
		public void VBNetGreaterThanOrEqualTest()
		{
			VBNetTestBinaryOperatorExpressionTest("a >= b", BinaryOperatorType.GreaterThanOrEqual);
		}
		
		[Test]
		public void VBNetEqualityTest()
		{
			VBNetTestBinaryOperatorExpressionTest("a = b", BinaryOperatorType.Equality);
		}
		
		[Test]
		public void VBNetInEqualityTest()
		{
			VBNetTestBinaryOperatorExpressionTest("a <> b", BinaryOperatorType.InEquality);
		}
		
		[Test]
		public void VBNetLessThanTest()
		{
			VBNetTestBinaryOperatorExpressionTest("a < b", BinaryOperatorType.LessThan);
		}
		
		[Test]
		public void VBNetLessThanOrEqualTest()
		{
			VBNetTestBinaryOperatorExpressionTest("a <= b", BinaryOperatorType.LessThanOrEqual);
		}
		
		[Test]
		public void VBNetAddTest()
		{
			VBNetTestBinaryOperatorExpressionTest("a + b", BinaryOperatorType.Add);
		}
		
		[Test]
		public void VBNetSubtractTest()
		{
			VBNetTestBinaryOperatorExpressionTest("a - b", BinaryOperatorType.Subtract);
		}
		
		[Test]
		public void VBNetMultiplyTest()
		{
			VBNetTestBinaryOperatorExpressionTest("a * b", BinaryOperatorType.Multiply);
		}
		
		[Test]
		public void VBNetDivideTest()
		{
			VBNetTestBinaryOperatorExpressionTest("a / b", BinaryOperatorType.Divide);
		}
		
		[Test]
		public void VBNetDivideIntegerTest()
		{
			VBNetTestBinaryOperatorExpressionTest("a \\ b", BinaryOperatorType.DivideInteger);
		}
		
		[Test]
		public void VBNetModulusTest()
		{
			VBNetTestBinaryOperatorExpressionTest("a Mod b", BinaryOperatorType.Modulus);
		}
		
		[Test]
		public void VBNetShiftLeftTest()
		{
			VBNetTestBinaryOperatorExpressionTest("a << b", BinaryOperatorType.ShiftLeft);
		}
		
		[Test]
		public void VBNetShiftRightTest()
		{
			VBNetTestBinaryOperatorExpressionTest("a >> b", BinaryOperatorType.ShiftRight);
		}
		
		[Test]
		public void VBNetISTest()
		{
			VBNetTestBinaryOperatorExpressionTest("a is b", BinaryOperatorType.ReferenceEquality);
		}
		
		[Test]
		public void VBNetISNotTest()
		{
			VBNetTestBinaryOperatorExpressionTest("a IsNot b", BinaryOperatorType.ReferenceInequality);
		}
		
		[Test]
		public void VBNetLikeTest()
		{
			VBNetTestBinaryOperatorExpressionTest("a Like b", BinaryOperatorType.Like);
		}
		#endregion
	}
}
