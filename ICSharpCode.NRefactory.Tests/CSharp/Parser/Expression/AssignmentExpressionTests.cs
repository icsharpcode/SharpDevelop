// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.Parser.Expression
{
	[TestFixture]
	public class AssignmentExpressionTests
	{
		void CSharpTestAssignmentExpression(string program, AssignmentOperatorType op)
		{
			AssignmentExpression ae = ParseUtilCSharp.ParseExpression<AssignmentExpression>(program);
			
			Assert.AreEqual(op, ae.AssignmentOperatorType);
			
			Assert.IsTrue(ae.Left is IdentifierExpression);
			Assert.IsTrue(ae.Right is IdentifierExpression);
		}
		
		[Test]
		public void CSharpAssignTest()
		{
			CSharpTestAssignmentExpression("a = b", AssignmentOperatorType.Assign);
		}
		
		[Test]
		public void CSharpAddTest()
		{
			CSharpTestAssignmentExpression("a += b", AssignmentOperatorType.Add);
		}
		
		[Test]
		public void CSharpSubtractTest()
		{
			CSharpTestAssignmentExpression("a -= b", AssignmentOperatorType.Subtract);
		}
		
		[Test]
		public void CSharpMultiplyTest()
		{
			CSharpTestAssignmentExpression("a *= b", AssignmentOperatorType.Multiply);
		}
		
		[Test]
		public void CSharpDivideTest()
		{
			CSharpTestAssignmentExpression("a /= b", AssignmentOperatorType.Divide);
		}
		
		[Test]
		public void CSharpModulusTest()
		{
			CSharpTestAssignmentExpression("a %= b", AssignmentOperatorType.Modulus);
		}
		
		[Test]
		public void CSharpShiftLeftTest()
		{
			CSharpTestAssignmentExpression("a <<= b", AssignmentOperatorType.ShiftLeft);
		}
		
		[Test]
		public void CSharpShiftRightTest()
		{
			CSharpTestAssignmentExpression("a >>= b", AssignmentOperatorType.ShiftRight);
		}
		
		[Test]
		public void CSharpBitwiseAndTest()
		{
			CSharpTestAssignmentExpression("a &= b", AssignmentOperatorType.BitwiseAnd);
		}
		
		[Test]
		public void CSharpBitwiseOrTest()
		{
			CSharpTestAssignmentExpression("a |= b", AssignmentOperatorType.BitwiseOr);
		}
		
		[Test]
		public void CSharpExclusiveOrTest()
		{
			CSharpTestAssignmentExpression("a ^= b", AssignmentOperatorType.ExclusiveOr);
		}
	}
}
