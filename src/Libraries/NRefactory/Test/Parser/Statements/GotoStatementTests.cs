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
	public class GotoStatementTests
	{
		#region C#
		[Test]
		public void CSharpGotoStatementTest()
		{
			GotoStatement gotoStmt = (GotoStatement)ParseUtilCSharp.ParseStatment("goto myLabel;", typeof(GotoStatement));
			Assert.AreEqual("myLabel", gotoStmt.Label);
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetGotoStatementTest()
		{
			GotoStatement gotoStmt = (GotoStatement)ParseUtilVBNet.ParseStatment("GoTo myLabel", typeof(GotoStatement));
			Assert.AreEqual("myLabel", gotoStmt.Label);
		}
		#endregion
	}
}
