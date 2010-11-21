// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Linq;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.Parser.Statements
{
	[TestFixture]
	public class CheckedStatementTests
	{
		[Test]
		public void CheckedStatementTest()
		{
			CheckedStatement checkedStatement = ParseUtilCSharp.ParseStatement<CheckedStatement>("checked { }");
			Assert.IsNotNull(checkedStatement.Block);
		}
		
		[Test]
		public void CheckedStatementAndExpressionTest()
		{
			CheckedStatement checkedStatement = ParseUtilCSharp.ParseStatement<CheckedStatement>("checked { checked(++i).ToString(); }");
			ExpressionStatement es = (ExpressionStatement)checkedStatement.Block.Statements.Single();
			CheckedExpression ce = (CheckedExpression)((MemberReferenceExpression)((InvocationExpression)es.Expression).Target).Target;
			Assert.IsTrue(ce.Expression is UnaryOperatorExpression);
		}
		
		[Test]
		public void UncheckedStatementTest()
		{
			UncheckedStatement uncheckedStatement = ParseUtilCSharp.ParseStatement<UncheckedStatement>("unchecked { }");
			Assert.IsNotNull(uncheckedStatement.Block);
		}
		
		[Test]
		public void UncheckedStatementAndExpressionTest()
		{
			UncheckedStatement uncheckedStatement = ParseUtilCSharp.ParseStatement<UncheckedStatement>("unchecked { unchecked(++i).ToString(); }");
			ExpressionStatement es = (ExpressionStatement)uncheckedStatement.Block.Statements.Single();
			UncheckedExpression ce = (UncheckedExpression)((MemberReferenceExpression)((InvocationExpression)es.Expression).Target).Target;
			Assert.IsTrue(ce.Expression is UnaryOperatorExpression);
		}
	}
}
