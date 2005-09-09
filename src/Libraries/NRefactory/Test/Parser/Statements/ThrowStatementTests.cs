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
