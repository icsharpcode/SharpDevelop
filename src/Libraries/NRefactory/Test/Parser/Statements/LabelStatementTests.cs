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
	public class LabelStatementTests
	{
		#region C#
		[Test]
		public void CSharpLabelStatementTest()
		{
			LabelStatement labelStmt = (LabelStatement)ParseUtilCSharp.ParseStatment("myLabel: ; ", typeof(LabelStatement));
			Assert.AreEqual("myLabel", labelStmt.Label);
		}
		[Test]
		public void CSharpLabel2StatementTest()
		{
			LabelStatement labelStmt = (LabelStatement)ParseUtilCSharp.ParseStatment("yield: ; ", typeof(LabelStatement));
			Assert.AreEqual("yield", labelStmt.Label);
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetLabelStatementTest()
		{
			LabelStatement labelStmt = (LabelStatement)ParseUtilVBNet.ParseStatment("myLabel: Console.WriteLine()", typeof(LabelStatement));
			Assert.AreEqual("myLabel", labelStmt.Label);
		}
		#endregion 
	}
}
