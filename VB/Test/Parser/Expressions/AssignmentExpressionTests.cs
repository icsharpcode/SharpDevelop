// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.VB.Parser;
using ICSharpCode.NRefactory.VB.Dom;

namespace ICSharpCode.NRefactory.VB.Tests.Dom
{
	[TestFixture]
	public class AssignmentExpressionTests
	{
		#region VB.NET
		void VBNetTestAssignmentExpression(string program, AssignmentOperatorType op)
		{
			ExpressionStatement se = ParseUtilVBNet.ParseStatement<ExpressionStatement>(program);
			AssignmentExpression ae = se.Expression as AssignmentExpression;
			Assert.AreEqual(op, ae.Op);
			
			Assert.IsTrue(ae.Left is IdentifierExpression);
			Assert.IsTrue(ae.Right is IdentifierExpression);
		}

		[Test]
		public void VBNetAssignTest()
		{
			VBNetTestAssignmentExpression("a = b", AssignmentOperatorType.Assign);
		}
		
		[Test]
		public void VBNetAddTest()
		{
			VBNetTestAssignmentExpression("a += b", AssignmentOperatorType.Add);
		}
		
		[Test]
		public void VBNetSubtractTest()
		{
			VBNetTestAssignmentExpression("a -= b", AssignmentOperatorType.Subtract);
		}
		
		[Test]
		public void VBNetMultiplyTest()
		{
			VBNetTestAssignmentExpression("a *= b", AssignmentOperatorType.Multiply);
		}
		
		[Test]
		public void VBNetDivideTest()
		{
			VBNetTestAssignmentExpression("a /= b", AssignmentOperatorType.Divide);
		}
		
		[Test]
		public void VBNetExclusiveOrTest()
		{
			VBNetTestAssignmentExpression("a ^= b", AssignmentOperatorType.Power);
		}
		
		[Test]
		public void VBNetStringConcatTest()
		{
			VBNetTestAssignmentExpression("a &= b", AssignmentOperatorType.ConcatString);
		}

		[Test]
		public void VBNetModulusTest()
		{
			VBNetTestAssignmentExpression("a \\= b", AssignmentOperatorType.DivideInteger);
		}
		#endregion
	}
}
