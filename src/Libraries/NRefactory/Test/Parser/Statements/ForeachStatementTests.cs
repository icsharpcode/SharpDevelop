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
	public class ForeachStatementTests
	{
		#region C#
		[Test]
		public void CSharpForeachStatementTest()
		{
			ForeachStatement foreachStmt = (ForeachStatement)ParseUtilCSharp.ParseStatment("foreach (int i in myColl) {} ", typeof(ForeachStatement));
			// TODO : Extend test.
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetForeachStatementTest()
		{
			ForeachStatement foreachStmt = (ForeachStatement)ParseUtilVBNet.ParseStatment("For Each i As Integer In myColl : Next", typeof(ForeachStatement));
			// TODO : Extend test.
		}
		#endregion
		
	}
}
