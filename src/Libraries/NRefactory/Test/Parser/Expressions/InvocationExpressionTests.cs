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
		#endregion
		
		#region VB.NET
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
