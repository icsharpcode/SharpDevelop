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
	public class ReturnStatementTests
	{
		#region C#
		[Test]
		public void CSharpEmptyReturnStatementTest()
		{
			ReturnStatement returnStatement = ParseUtilCSharp.ParseStatement<ReturnStatement>("return;");
			Assert.IsTrue(returnStatement.Expression.IsNull);
		}
		
		[Test]
		public void CSharpReturnStatementTest()
		{
			ReturnStatement returnStatement = ParseUtilCSharp.ParseStatement<ReturnStatement>("return 5;");
			Assert.IsFalse(returnStatement.Expression.IsNull);
			Assert.IsTrue(returnStatement.Expression is PrimitiveExpression);
		}
		[Test]
		public void CSharpReturnStatementTest1()
		{
			ReturnStatement returnStatement = ParseUtilCSharp.ParseStatement<ReturnStatement>("return yield;");
			Assert.IsFalse(returnStatement.Expression.IsNull);
			Assert.IsTrue(returnStatement.Expression is IdentifierExpression);
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetEmptyReturnStatementTest()
		{
			ReturnStatement returnStatement = ParseUtilVBNet.ParseStatement<ReturnStatement>("Return");
			Assert.IsTrue(returnStatement.Expression.IsNull);
		}
		
		[Test]
		public void VBNetReturnStatementTest()
		{
			ReturnStatement returnStatement = ParseUtilVBNet.ParseStatement<ReturnStatement>("Return 5");
			Assert.IsFalse(returnStatement.Expression.IsNull);
			Assert.IsTrue(returnStatement.Expression is PrimitiveExpression);
		}
		#endregion
	}
}
