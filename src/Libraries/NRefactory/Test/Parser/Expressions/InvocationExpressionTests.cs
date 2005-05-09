/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 13.09.2004
 * Time: 19:54
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Tests.AST
{
	[TestFixture]
	public class InvocationExpressionTests
	{
		void CheckSimpleInvoke(InvocationExpression ie)
		{
			Assert.AreEqual(0, ie.Parameters.Count);
			Assert.IsTrue(ie.TargetObject is IdentifierExpression);
			Assert.AreEqual("myMethod", ((IdentifierExpression)ie.TargetObject).Identifier);
		}
		
		#region C#
		[Test]
		public void CSharpSimpleInvocationExpressionTest()
		{
			CheckSimpleInvoke((InvocationExpression)ParseUtilCSharp.ParseExpression("myMethod()", typeof(InvocationExpression)));
		}
		
		[Test]
		public void CSharpInvalidNestedInvocationExpressionTest()
		{
			// this test was written because this bug caused the AbstractASTVisitor to crash
			
			InvocationExpression expr = (InvocationExpression)ParseUtilCSharp.ParseExpression("WriteLine(myMethod(,))", typeof(InvocationExpression), true);
			Assert.IsTrue(expr.TargetObject is IdentifierExpression);
			Assert.AreEqual("WriteLine", ((IdentifierExpression)expr.TargetObject).Identifier);
			
			Assert.AreEqual(1, expr.Parameters.Count); // here a second null parameter was added incorrectly
			
			Assert.IsTrue(expr.Parameters[0] is InvocationExpression);
			CheckSimpleInvoke((InvocationExpression)expr.Parameters[0]);
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetSimpleInvocationExpressionTest()
		{
			CheckSimpleInvoke((InvocationExpression)ParseUtilVBNet.ParseExpression("myMethod()", typeof(InvocationExpression)));
		}
		[Test]
		public void PrimitiveExpression1Test()
		{
			InvocationExpression ie = (InvocationExpression)ParseUtilVBNet.ParseExpression("546.ToString()", typeof(InvocationExpression));
			Assert.AreEqual(0, ie.Parameters.Count);
		}
		
		#endregion
	}
}
