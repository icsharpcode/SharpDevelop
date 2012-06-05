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
	public class SwitchStatementTests
	{
		#region C#
		[Test]
		public void CSharpSwitchStatementTest()
		{
			SwitchStatement switchStmt = ParseUtilCSharp.ParseStatement<SwitchStatement>("switch (a) { case 4: case 5: break; case 6: break; default: break; }");
			Assert.AreEqual("a", ((IdentifierExpression)switchStmt.SwitchExpression).Identifier);
			// TODO: Extend test
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBSwitchStatementTest()
		{
			SwitchStatement switchStmt = ParseUtilVBNet.ParseStatement<SwitchStatement>("Select Case a\n Case 4, 5\n Case 6\n Case Else\n End Select");
			Assert.AreEqual("a", ((IdentifierExpression)switchStmt.SwitchExpression).Identifier);
			// TODO: Extend test
		}
		
		[Test]
		public void InvalidVBSwitchStatementTest()
		{
			SwitchStatement switchStmt = ParseUtilVBNet.ParseStatement<SwitchStatement>("Select Case a\n Case \n End Select", true);
			Assert.AreEqual("a", ((IdentifierExpression)switchStmt.SwitchExpression).Identifier);
			SwitchSection sec = switchStmt.SwitchSections[0];
			Assert.AreEqual(0, sec.SwitchLabels.Count);
		}
		
		[Test]
		public void SpecialCaseStatement()
		{
			SwitchStatement stmt = ParseUtilVBNet.ParseStatement<SwitchStatement>("Select Case a\nCase < 50\nCase > 20, < 10\nEnd Select");
			Assert.AreEqual("a", ((IdentifierExpression)stmt.SwitchExpression).Identifier);
		}
		
		[Test]
		public void SpecialCaseStatement2()
		{
			SwitchStatement stmt = ParseUtilVBNet.ParseStatement<SwitchStatement>("Select Case a\nCase < 50\nEnd Select");
			Assert.AreEqual("a", ((IdentifierExpression)stmt.SwitchExpression).Identifier);
		}
		#endregion
	}
}
