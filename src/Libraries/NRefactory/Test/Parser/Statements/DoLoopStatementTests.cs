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
	public class DoLoopStatementTests
	{
		#region C#
		[Test]
		public void CSharpWhileStatementTest()
		{
			DoLoopStatement doLoopStmt = (DoLoopStatement)ParseUtilCSharp.ParseStatment("while (true) { }", typeof(DoLoopStatement));
			Assert.AreEqual(ConditionPosition.Start, doLoopStmt.ConditionPosition);
			Assert.AreEqual(ConditionType.While, doLoopStmt.ConditionType);
			Assert.IsTrue(doLoopStmt.Condition is PrimitiveExpression);
			Assert.IsTrue(doLoopStmt.EmbeddedStatement is BlockStatement);
		}
		
		[Test]
		public void CSharpDoWhileStatementTest()
		{
			DoLoopStatement doLoopStmt = (DoLoopStatement)ParseUtilCSharp.ParseStatment("do { } while (true);", typeof(DoLoopStatement));
			Assert.AreEqual(ConditionPosition.End, doLoopStmt.ConditionPosition);
			Assert.AreEqual(ConditionType.While, doLoopStmt.ConditionType);
			Assert.IsTrue(doLoopStmt.Condition is PrimitiveExpression);
			Assert.IsTrue(doLoopStmt.EmbeddedStatement is BlockStatement);
		}
		#endregion
		
		#region VB.NET
			// TODO
		#endregion 
	}
}
