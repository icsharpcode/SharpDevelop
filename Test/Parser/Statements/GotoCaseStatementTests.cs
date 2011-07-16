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
	public class GotoCaseStatementTests
	{
		#region C#
		[Test]
		public void CSharpGotoCaseDefaltStatementTest()
		{
			GotoCaseStatement gotoCaseStmt = ParseUtilCSharp.ParseStatement<GotoCaseStatement>("goto default;");
			Assert.IsTrue(gotoCaseStmt.IsDefaultCase);
		}
		
		[Test]
		public void CSharpGotoCaseStatementTest()
		{
			GotoCaseStatement gotoCaseStmt = ParseUtilCSharp.ParseStatement<GotoCaseStatement>("goto case 6;");
			Assert.IsFalse(gotoCaseStmt.IsDefaultCase);
			Assert.IsTrue(gotoCaseStmt.Expression is PrimitiveExpression);
		}
		#endregion
		
		#region VB.NET
			// No VB.NET representation
		#endregion
	}
}
