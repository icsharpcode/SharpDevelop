/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 13.09.2004
 * Time: 19:54
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
	public class StatementExpressionTests
	{
		#region C#
		[Test]
		public void CSharpStatementExpressionTest()
		{
			StatementExpression stmtExprStmt = (StatementExpression)ParseUtilCSharp.ParseStatment("my.Obj.PropCall;", typeof(StatementExpression));
			Assert.IsTrue(stmtExprStmt.Expression is FieldReferenceExpression);
		}
		[Test]
		public void CSharpStatementExpressionTest1()
		{
			StatementExpression stmtExprStmt = (StatementExpression)ParseUtilCSharp.ParseStatment("yield.yield;", typeof(StatementExpression));
			Assert.IsTrue(stmtExprStmt.Expression is FieldReferenceExpression);
		}
		#endregion
		
		#region VB.NET
			// TODO
		#endregion 
	}
}
