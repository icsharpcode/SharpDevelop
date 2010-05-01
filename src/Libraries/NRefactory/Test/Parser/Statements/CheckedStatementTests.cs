// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

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
