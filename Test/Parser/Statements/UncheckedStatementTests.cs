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
	public class UncheckedStatementTests
	{
		#region C#
		[Test]
		public void CSharpUncheckedStatementTest()
		{
			UncheckedStatement uncheckedStatement = ParseUtilCSharp.ParseStatement<UncheckedStatement>("unchecked { }");
			Assert.IsFalse(uncheckedStatement.Block.IsNull);
		}
		
		
		[Test]
		public void CSharpUncheckedStatementAndExpressionTest()
		{
			UncheckedStatement uncheckedStatement = ParseUtilCSharp.ParseStatement<UncheckedStatement>("unchecked { unchecked(++i); }");
			Assert.IsFalse(uncheckedStatement.Block.IsNull);
			ExpressionStatement es = (ExpressionStatement)uncheckedStatement.Block.Children[0];
			UncheckedExpression ce = (UncheckedExpression)es.Expression;
			Assert.IsInstanceOf<UnaryOperatorExpression>(ce.Expression);
		}
		#endregion
		
		#region VB.NET
		// No VB.NET representation
		#endregion
	}
}
