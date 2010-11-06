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
	public class ForStatementTests
	{
		#region C#
		[Test]
		public void CSharpEmptyForStatementTest()
		{
			ForStatement forStmt = ParseUtilCSharp.ParseStatement<ForStatement>("for (;;) ;");
			Assert.AreEqual(0, forStmt.Initializers.Count);
			Assert.AreEqual(0, forStmt.Iterator.Count);
			Assert.IsTrue(forStmt.Condition.IsNull);
			Assert.IsTrue(forStmt.EmbeddedStatement is EmptyStatement);
		}
		
		[Test]
		public void CSharpForStatementTest()
		{
			ForStatement forStmt = ParseUtilCSharp.ParseStatement<ForStatement>("for (int i = 5; i < 6; ++i) {} ");
			// TODO : Extend test.
		}
		#endregion
		
		#region VB.NET
			// No VB.NET representation (for ... next is different)
		#endregion
		
	}
}
