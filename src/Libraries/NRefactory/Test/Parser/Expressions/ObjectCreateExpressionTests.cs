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
