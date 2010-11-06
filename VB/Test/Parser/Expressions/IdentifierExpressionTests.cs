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
	public class IdentifierExpressionTests
	{
		#region VB.NET
		[Test]
		public void VBNetIdentifierExpressionTest1()
		{
			IdentifierExpression ie = ParseUtilVBNet.ParseExpression<IdentifierExpression>("MyIdentifier");
			Assert.AreEqual("MyIdentifier", ie.Identifier);
		}
		
		[Test]
		public void VBNetIdentifierExpressionTest2()
		{
			IdentifierExpression ie = ParseUtilVBNet.ParseExpression<IdentifierExpression>("[Public]");
			Assert.AreEqual("Public", ie.Identifier);
		}
		
		[Test]
		public void VBNetContextKeywordsTest()
		{
			Assert.AreEqual("Assembly", ParseUtilVBNet.ParseExpression<IdentifierExpression>("Assembly").Identifier);
			Assert.AreEqual("Custom", ParseUtilVBNet.ParseExpression<IdentifierExpression>("Custom").Identifier);
			Assert.AreEqual("Off", ParseUtilVBNet.ParseExpression<IdentifierExpression>("Off").Identifier);
			Assert.AreEqual("Explicit", ParseUtilVBNet.ParseExpression<IdentifierExpression>("Explicit").Identifier);
		}
		#endregion
	}
}
