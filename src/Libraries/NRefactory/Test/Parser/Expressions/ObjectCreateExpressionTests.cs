// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using MbUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Tests.AST
{
	[TestFixture]
	public class ObjectCreateExpressionTests
	{
		void CheckSimpleObjectCreateExpression(ObjectCreateExpression oce)
		{
			Assert.AreEqual("MyObject", oce.CreateType.Type);
			Assert.AreEqual(3, oce.Parameters.Count);
			
			for (int i = 0; i < oce.Parameters.Count; ++i) {
				Assert.IsTrue(oce.Parameters[i] is PrimitiveExpression);
			}
		}
		
		#region C#
		[Test]
		public void CSharpSimpleObjectCreateExpressionTest()
		{
			CheckSimpleObjectCreateExpression((ObjectCreateExpression)ParseUtilCSharp.ParseExpression("new MyObject(1, 2, 3)", typeof(ObjectCreateExpression)));
		}
		
		[Test]
		public void CSharpInvalidNestedObjectCreateExpressionTest()
		{
			// this test was written because this bug caused the AbstractASTVisitor to crash
			
			InvocationExpression expr = (InvocationExpression)ParseUtilCSharp.ParseExpression("WriteLine(new MyObject(1, 2, 3,))", typeof(InvocationExpression), true);
			Assert.IsTrue(expr.TargetObject is IdentifierExpression);
			Assert.AreEqual("WriteLine", ((IdentifierExpression)expr.TargetObject).Identifier);
			
			Assert.AreEqual(1, expr.Arguments.Count); // here a second null parameter was added incorrectly
			
			Assert.IsTrue(expr.Arguments[0] is ObjectCreateExpression);
			CheckSimpleObjectCreateExpression((ObjectCreateExpression)expr.Arguments[0]);
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetSimpleObjectCreateExpressionTest()
		{
			CheckSimpleObjectCreateExpression((ObjectCreateExpression)ParseUtilVBNet.ParseExpression("New MyObject(1, 2, 3)", typeof(ObjectCreateExpression)));
		}

		#endregion
	}
}
