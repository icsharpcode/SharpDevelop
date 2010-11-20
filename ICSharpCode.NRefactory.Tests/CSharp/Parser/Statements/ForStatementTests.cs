// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Linq;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.Parser.Statements
{
	[TestFixture]
	public class ForStatementTests
	{
		[Test]
		public void CSharpForeachStatementTest()
		{
			ForeachStatement foreachStmt = ParseUtilCSharp.ParseStatement<ForeachStatement>("foreach (int i in myColl) {} ");
			// TODO : Extend test.
		}
		
		[Test]
		public void CSharpEmptyForStatementTest()
		{
			ForStatement forStmt = ParseUtilCSharp.ParseStatement<ForStatement>("for (;;) ;");
			Assert.AreEqual(0, forStmt.Initializers.Count());
			Assert.AreEqual(0, forStmt.Iterators.Count());
			Assert.IsNull(forStmt.Condition);
			Assert.IsTrue(forStmt.EmbeddedStatement is EmptyStatement);
		}
		
		[Test]
		public void CSharpForStatementTest()
		{
			ForStatement forStmt = ParseUtilCSharp.ParseStatement<ForStatement>("for (int i = 5; i < 6; ++i) {} ");
			// TODO : Extend test.
		}
	}
}
