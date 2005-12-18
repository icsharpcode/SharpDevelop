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
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Tests.AST
{
	[TestFixture]
	public class IfElseStatementTests
	{
		#region C#
		[Test]
		public void CSharpSimpleIfStatementTest()
		{
			IfElseStatement ifElseStatement = ParseUtilCSharp.ParseStatement<IfElseStatement>("if (true) { }");
			Assert.IsFalse(ifElseStatement.Condition.IsNull);
			Assert.IsTrue(ifElseStatement.TrueStatement.Count == 1, "true count != 1:" + ifElseStatement.TrueStatement.Count);
			Assert.IsTrue(ifElseStatement.FalseStatement.Count == 0, "false count != 0:" + ifElseStatement.FalseStatement.Count);
			
			Assert.IsTrue(ifElseStatement.TrueStatement[0] is BlockStatement);
		}
		
		[Test]
		public void CSharpSimpleIfElseStatementTest()
		{
			IfElseStatement ifElseStatement = ParseUtilCSharp.ParseStatement<IfElseStatement>("if (true) { } else { }");
			Assert.IsFalse(ifElseStatement.Condition.IsNull);
			Assert.IsTrue(ifElseStatement.TrueStatement.Count == 1, "true count != 1:" + ifElseStatement.TrueStatement.Count);
			Assert.IsTrue(ifElseStatement.FalseStatement.Count == 1, "false count != 1:" + ifElseStatement.FalseStatement.Count);
			
			Assert.IsTrue(ifElseStatement.TrueStatement[0] is BlockStatement, "Statement was: " + ifElseStatement.TrueStatement[0]);
			Assert.IsTrue(ifElseStatement.FalseStatement[0] is BlockStatement, "Statement was: " + ifElseStatement.FalseStatement[0]);
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetSimpleIfStatementTest()
		{
			IfElseStatement ifElseStatement = ParseUtilVBNet.ParseStatement<IfElseStatement>("If True THEN END");
			Assert.IsFalse(ifElseStatement.Condition.IsNull);
			Assert.IsTrue(ifElseStatement.TrueStatement.Count == 1, "true count != 1:" + ifElseStatement.TrueStatement.Count);
			Assert.IsTrue(ifElseStatement.FalseStatement.Count == 0, "false count != 0:" + ifElseStatement.FalseStatement.Count);
			
			Assert.IsTrue(ifElseStatement.TrueStatement[0] is EndStatement, "Statement was: " + ifElseStatement.TrueStatement[0]);
		}
		[Test]
		public void VBNetSimpleIfStatementTest2()
		{
			IfElseStatement ifElseStatement = ParseUtilVBNet.ParseStatement<IfElseStatement>("If True THEN\n END\n END IF");
			Assert.IsFalse(ifElseStatement.Condition.IsNull);
			Assert.IsTrue(ifElseStatement.TrueStatement.Count == 1, "true count != 1:" + ifElseStatement.TrueStatement.Count);
			Assert.IsTrue(ifElseStatement.FalseStatement.Count == 0, "false count != 0:" + ifElseStatement.FalseStatement.Count);
			
			Assert.IsTrue(ifElseStatement.TrueStatement[0] is BlockStatement, "Statement was: " + ifElseStatement.TrueStatement[0]);
		}
		#endregion 
	}
}
