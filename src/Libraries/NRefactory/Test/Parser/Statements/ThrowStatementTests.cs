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
	public class ThrowStatementTests
	{
		#region C#
		[Test]
		public void CSharpEmptyThrowStatementTest()
		{
			ThrowStatement throwStmt = (ThrowStatement)ParseUtilCSharp.ParseStatment("throw;", typeof(ThrowStatement));
			Assert.IsTrue(throwStmt.Expression.IsNull);
		}
		
		[Test]
		public void CSharpThrowStatementTest()
		{
			ThrowStatement throwStmt = (ThrowStatement)ParseUtilCSharp.ParseStatment("throw new Exception();", typeof(ThrowStatement));
			Assert.IsTrue(throwStmt.Expression is ObjectCreateExpression);
		}
		#endregion
		
		#region VB.NET
			// TODO
		#endregion 
	}
}
