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
	public class ForeachStatementTests
	{
		#region C#
		[Test]
		public void CSharpForeachStatementTest()
		{
			ForeachStatement foreachStmt = ParseUtilCSharp.ParseStatement<ForeachStatement>("foreach (int i in myColl) {} ");
			// TODO : Extend test.
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetForeachStatementTest()
		{
			ForeachStatement foreachStmt = ParseUtilVBNet.ParseStatement<ForeachStatement>("For Each i As Integer In myColl : Next");
			// TODO : Extend test.
		}
		#endregion
		
	}
}
