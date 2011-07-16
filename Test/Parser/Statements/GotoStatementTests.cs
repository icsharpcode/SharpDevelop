// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.NRefactory.Tests.Ast
{
	[TestFixture]
	public class GotoStatementTests
	{
		#region C#
		[Test]
		public void CSharpGotoStatementTest()
		{
			GotoStatement gotoStmt = ParseUtilCSharp.ParseStatement<GotoStatement>("goto myLabel;");
			Assert.AreEqual("myLabel", gotoStmt.Label);
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetGotoStatementTest()
		{
			GotoStatement gotoStmt = ParseUtilVBNet.ParseStatement<GotoStatement>("GoTo myLabel");
			Assert.AreEqual("myLabel", gotoStmt.Label);
		}
		#endregion
	}
}
