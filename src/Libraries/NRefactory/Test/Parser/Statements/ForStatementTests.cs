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
	public class ForStatementTests
	{
		#region C#
		[Test]
		public void CSharpEmptyForStatementTest()
		{
			ForStatement forStmt = ParseUtilCSharp.ParseStatement<ForStatement>("for (;;) ;");
			Assert.AreEqual(0, forStmt.Initializers.Count);
			Assert.AreEqual(0, forStmt.Iterator.Count);
			Assert.IsTrue(forStmt.Condition.IsNull);
			Assert.IsTrue(forStmt.EmbeddedStatement is EmptyStatement);
		}
		
		[Test]
		public void CSharpForStatementTest()
		{
			ForStatement forStmt = ParseUtilCSharp.ParseStatement<ForStatement>("for (int i = 5; i < 6; ++i) {} ");
			// TODO : Extend test.
		}
		#endregion
		
		#region VB.NET
			// No VB.NET representation (for ... next is different)
		#endregion
		
	}
}
