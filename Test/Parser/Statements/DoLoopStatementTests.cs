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
	public class DoLoopStatementTests
	{
		#region C#
		[Test]
		public void CSharpWhileStatementTest()
		{
			DoLoopStatement doLoopStmt = ParseUtilCSharp.ParseStatement<DoLoopStatement>("while (true) { }");
			Assert.AreEqual(ConditionPosition.Start, doLoopStmt.ConditionPosition);
			Assert.AreEqual(ConditionType.While, doLoopStmt.ConditionType);
			Assert.IsTrue(doLoopStmt.Condition is PrimitiveExpression);
			Assert.IsTrue(doLoopStmt.EmbeddedStatement is BlockStatement);
		}
		
		[Test]
		public void CSharpDoWhileStatementTest()
		{
			DoLoopStatement doLoopStmt = ParseUtilCSharp.ParseStatement<DoLoopStatement>("do { } while (true);");
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
