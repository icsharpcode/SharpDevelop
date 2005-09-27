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
