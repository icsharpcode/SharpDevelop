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
	public class ThrowStatementTests
	{
		#region C#
		[Test]
		public void CSharpEmptyThrowStatementTest()
		{
			ThrowStatement throwStmt = ParseUtilCSharp.ParseStatement<ThrowStatement>("throw;");
			Assert.IsTrue(throwStmt.Expression.IsNull);
		}
		
		[Test]
		public void CSharpThrowStatementTest()
		{
			ThrowStatement throwStmt = ParseUtilCSharp.ParseStatement<ThrowStatement>("throw new Exception();");
			Assert.IsTrue(throwStmt.Expression is ObjectCreateExpression);
		}
		#endregion
		
		#region VB.NET
			// TODO
		#endregion 
	}
}
