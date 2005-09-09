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
