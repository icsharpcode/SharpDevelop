// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

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
