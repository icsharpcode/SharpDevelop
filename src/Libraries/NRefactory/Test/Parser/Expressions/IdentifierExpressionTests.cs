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
	public class IdentifierExpressionTests
	{
		#region C#
		[Test]
		public void CSharpIdentifierExpressionTest1()
		{
			IdentifierExpression ident = (IdentifierExpression)ParseUtilCSharp.ParseExpression("MyIdentifier", typeof(IdentifierExpression));
			Assert.AreEqual("MyIdentifier", ident.Identifier);
		}
		
		[Test]
		public void CSharpIdentifierExpressionTest2()
		{
			IdentifierExpression ident = (IdentifierExpression)ParseUtilCSharp.ParseExpression("@public", typeof(IdentifierExpression));
			Assert.AreEqual("public", ident.Identifier);
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetIdentifierExpressionTest1()
		{
			IdentifierExpression ie = (IdentifierExpression)ParseUtilVBNet.ParseExpression("MyIdentifier", typeof(IdentifierExpression));
			Assert.AreEqual("MyIdentifier", ie.Identifier);
		}
		#endregion
		
	}
}
