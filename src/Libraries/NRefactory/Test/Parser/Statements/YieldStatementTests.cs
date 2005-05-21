/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 21.05.2005
 * Time: 17:54
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Tests.AST
{
	[TestFixture]
	public class YieldStatementTests
	{
		[Test]
		public void YieldReturnStatementTest()
		{
			YieldStatement yieldStmt = (YieldStatement)ParseUtilCSharp.ParseStatment("yield return \"Foo\";", typeof(YieldStatement));
			Assert.IsTrue(yieldStmt.IsYieldReturn());
			ReturnStatement retStmt = (ReturnStatement)yieldStmt.Statement;
			PrimitiveExpression expr =  (PrimitiveExpression)retStmt.Expression;
			Assert.AreEqual("Foo", expr.Value);
		}
		
		[Test]
		public void YieldBreakStatementTest()
		{
			YieldStatement yieldStmt = (YieldStatement)ParseUtilCSharp.ParseStatment("yield break;", typeof(YieldStatement));
			Assert.IsTrue(yieldStmt.IsYieldBreak());
		}
		
		[Test]
		public void YieldAsVariableTest()
		{
			StatementExpression se = (StatementExpression)ParseUtilCSharp.ParseStatment("yield = 3;", typeof(StatementExpression));
			AssignmentExpression ae = se.Expression as AssignmentExpression;
			
			Assert.AreEqual(AssignmentOperatorType.Assign, ae.Op);
			
			Assert.IsTrue(ae.Left is IdentifierExpression);
			Assert.AreEqual("yield", ((IdentifierExpression)ae.Left).Identifier);
			Assert.IsTrue(ae.Right is PrimitiveExpression);
		}
	}
}
