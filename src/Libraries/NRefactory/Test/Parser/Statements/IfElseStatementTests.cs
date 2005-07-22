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
	public class IfElseStatementTests
	{
		#region C#
		[Test]
		public void CSharpSimpleIfStatementTest()
		{
			IfElseStatement ifElseStatement = (IfElseStatement)ParseUtilCSharp.ParseStatment("if (true) { }", typeof(IfElseStatement));
			Assert.IsFalse(ifElseStatement.Condition.IsNull);
			Assert.IsTrue(ifElseStatement.TrueStatement.Count == 1, "true count != 1:" + ifElseStatement.TrueStatement.Count);
			Assert.IsTrue(ifElseStatement.FalseStatement.Count == 0, "false count != 0:" + ifElseStatement.FalseStatement.Count);
			
			Assert.IsTrue(ifElseStatement.TrueStatement[0] is BlockStatement);
		}
		
		[Test]
		public void CSharpSimpleIfElseStatementTest()
		{
			IfElseStatement ifElseStatement = (IfElseStatement)ParseUtilCSharp.ParseStatment("if (true) { } else { }", typeof(IfElseStatement));
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
			IfElseStatement ifElseStatement = (IfElseStatement)ParseUtilVBNet.ParseStatment("If True THEN END", typeof(IfElseStatement));
			Assert.IsFalse(ifElseStatement.Condition.IsNull);
			Assert.IsTrue(ifElseStatement.TrueStatement.Count == 1, "true count != 1:" + ifElseStatement.TrueStatement.Count);
			Assert.IsTrue(ifElseStatement.FalseStatement.Count == 0, "false count != 0:" + ifElseStatement.FalseStatement.Count);
			
			Assert.IsTrue(ifElseStatement.TrueStatement[0] is EndStatement, "Statement was: " + ifElseStatement.TrueStatement[0]);
		}
		[Test]
		public void VBNetSimpleIfStatementTest2()
		{
			IfElseStatement ifElseStatement = (IfElseStatement)ParseUtilVBNet.ParseStatment("If True THEN\n END\n END IF", typeof(IfElseStatement));
			Assert.IsFalse(ifElseStatement.Condition.IsNull);
			Assert.IsTrue(ifElseStatement.TrueStatement.Count == 1, "true count != 1:" + ifElseStatement.TrueStatement.Count);
			Assert.IsTrue(ifElseStatement.FalseStatement.Count == 0, "false count != 0:" + ifElseStatement.FalseStatement.Count);
			
			Assert.IsTrue(ifElseStatement.TrueStatement[0] is BlockStatement, "Statement was: " + ifElseStatement.TrueStatement[0]);
		}
		#endregion 
	}
}
