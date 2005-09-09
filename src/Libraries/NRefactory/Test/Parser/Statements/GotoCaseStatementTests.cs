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
