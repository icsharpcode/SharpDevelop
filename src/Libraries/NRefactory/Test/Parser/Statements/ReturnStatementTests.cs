// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using MbUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Tests.AST
{
	[TestFixture]
	public class ReturnStatementTests
	{
		#region C#
		[Test]
		public void CSharpEmptyReturnStatementTest()
		{
			ReturnStatement returnStatement = (ReturnStatement)ParseUtilCSharp.ParseStatment("return;", typeof(ReturnStatement));
			Assert.IsTrue(returnStatement.Expression.IsNull);
		}
		
		[Test]
		public void CSharpReturnStatementTest()
		{
			ReturnStatement returnStatement = (ReturnStatement)ParseUtilCSharp.ParseStatment("return 5;", typeof(ReturnStatement));
			Assert.IsFalse(returnStatement.Expression.IsNull);
			Assert.IsTrue(returnStatement.Expression is PrimitiveExpression);
		}
		[Test]
		public void CSharpReturnStatementTest1()
		{
			ReturnStatement returnStatement = (ReturnStatement)ParseUtilCSharp.ParseStatment("return yield;", typeof(ReturnStatement));
			Assert.IsFalse(returnStatement.Expression.IsNull);
			Assert.IsTrue(returnStatement.Expression is IdentifierExpression);
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetEmptyReturnStatementTest()
		{
			ReturnStatement returnStatement = (ReturnStatement)ParseUtilVBNet.ParseStatment("Return", typeof(ReturnStatement));
			Assert.IsTrue(returnStatement.Expression.IsNull);
		}
		
		[Test]
		public void VBNetReturnStatementTest()
		{
			ReturnStatement returnStatement = (ReturnStatement)ParseUtilVBNet.ParseStatment("Return 5", typeof(ReturnStatement));
			Assert.IsFalse(returnStatement.Expression.IsNull);
			Assert.IsTrue(returnStatement.Expression is PrimitiveExpression);
		}
		#endregion
	}
}
