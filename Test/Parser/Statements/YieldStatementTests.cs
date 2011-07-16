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
	public class YieldStatementTests
	{
		[Test]
		public void YieldReturnStatementTest()
		{
			YieldStatement yieldStmt = ParseUtilCSharp.ParseStatement<YieldStatement>("yield return \"Foo\";");
			Assert.IsTrue(yieldStmt.IsYieldReturn);
			ReturnStatement retStmt = (ReturnStatement)yieldStmt.Statement;
			PrimitiveExpression expr =  (PrimitiveExpression)retStmt.Expression;
			Assert.AreEqual("Foo", expr.Value);
		}
		
		[Test]
		public void YieldBreakStatementTest()
		{
			YieldStatement yieldStmt = ParseUtilCSharp.ParseStatement<YieldStatement>("yield break;");
			Assert.IsTrue(yieldStmt.IsYieldBreak);
		}
		
		[Test]
		public void YieldAsVariableTest()
		{
			ExpressionStatement se = ParseUtilCSharp.ParseStatement<ExpressionStatement>("yield = 3;");
			AssignmentExpression ae = se.Expression as AssignmentExpression;
			
			Assert.AreEqual(AssignmentOperatorType.Assign, ae.Op);
			
			Assert.IsTrue(ae.Left is IdentifierExpression);
			Assert.AreEqual("yield", ((IdentifierExpression)ae.Left).Identifier);
			Assert.IsTrue(ae.Right is PrimitiveExpression);
		}
	}
}
