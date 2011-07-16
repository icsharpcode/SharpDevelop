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
	public class CheckedStatementTests
	{
		#region C#
		[Test]
		public void CSharpCheckedStatementTest()
		{
			CheckedStatement checkedStatement = ParseUtilCSharp.ParseStatement<CheckedStatement>("checked { }");
			Assert.IsFalse(checkedStatement.Block.IsNull);
		}
		
		[Test]
		public void CSharpCheckedStatementAndExpressionTest()
		{
			CheckedStatement checkedStatement = ParseUtilCSharp.ParseStatement<CheckedStatement>("checked { checked(++i); }");
			Assert.IsFalse(checkedStatement.Block.IsNull);
			ExpressionStatement es = (ExpressionStatement)checkedStatement.Block.Children[0];
			CheckedExpression ce = (CheckedExpression)es.Expression;
			Assert.IsInstanceOf<UnaryOperatorExpression>(ce.Expression);
		}
		#endregion
		
		#region VB.NET
		// No VB.NET representation
		#endregion
	}
}
