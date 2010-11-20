// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.Parser.Statements
{
	[TestFixture]
	public class LabelStatementTests
	{
		[Test]
		public void LabelStatementTest()
		{
			LabelStatement labelStmt = ParseUtilCSharp.ParseStatement<LabelStatement>("myLabel: ; ");
			Assert.AreEqual("myLabel", labelStmt.Label);
		}
		
		[Test]
		public void Label2StatementTest()
		{
			LabelStatement labelStmt = ParseUtilCSharp.ParseStatement<LabelStatement>("yield: ; ");
			Assert.AreEqual("yield", labelStmt.Label);
		}
	}
}
