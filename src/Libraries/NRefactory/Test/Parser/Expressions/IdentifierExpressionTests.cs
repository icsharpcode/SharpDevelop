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
		
		[Test]
		public void VBNetIdentifierExpressionTest2()
		{
			IdentifierExpression ie = (IdentifierExpression)ParseUtilVBNet.ParseExpression("[Public]", typeof(IdentifierExpression));
			Assert.AreEqual("Public", ie.Identifier);
		}
		#endregion
		
	}
}
