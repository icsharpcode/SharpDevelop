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
	public class ForStatementTests
	{
		#region C#
		[Test]
		public void CSharpEmptyForStatementTest()
		{
			ForStatement forStmt = (ForStatement)ParseUtilCSharp.ParseStatment("for (;;) ;", typeof(ForStatement));
			Assert.AreEqual(0, forStmt.Initializers.Count);
			Assert.AreEqual(0, forStmt.Iterator.Count);
			Assert.IsTrue(forStmt.Condition.IsNull);
			Assert.IsTrue(forStmt.EmbeddedStatement is EmptyStatement);
		}
		
		[Test]
		public void CSharpForStatementTest()
		{
			ForStatement forStmt = (ForStatement)ParseUtilCSharp.ParseStatment("for (int i = 5; i < 6; ++i) {} ", typeof(ForStatement));
			// TODO : Extend test.
		}
		#endregion
		
		#region VB.NET
			// No VB.NET representation (for ... next is different)
		#endregion
		
	}
}
