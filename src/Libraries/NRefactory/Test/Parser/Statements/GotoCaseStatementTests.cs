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
	public class GotoCaseStatementTests
	{
		#region C#
		[Test]
		public void CSharpGotoCaseDefaltStatementTest()
		{
			GotoCaseStatement gotoCaseStmt = (GotoCaseStatement)ParseUtilCSharp.ParseStatment("goto default;", typeof(GotoCaseStatement));
			Assert.IsTrue(gotoCaseStmt.IsDefaultCase);
		}
		
		[Test]
		public void CSharpGotoCaseStatementTest()
		{
			GotoCaseStatement gotoCaseStmt = (GotoCaseStatement)ParseUtilCSharp.ParseStatment("goto case 6;", typeof(GotoCaseStatement));
			Assert.IsFalse(gotoCaseStmt.IsDefaultCase);
			Assert.IsTrue(gotoCaseStmt.Expression is PrimitiveExpression);
		}
		#endregion
		
		#region VB.NET
			// No VB.NET representation
		#endregion
	}
}
