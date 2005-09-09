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
	public class FixedStatementTests
	{
		#region C#
		[Test]
		public void CSharpFixedStatementTest()
		{
			FixedStatement fixedStmt = (FixedStatement)ParseUtilCSharp.ParseStatment("fixed (int* ptr = &myIntArr) { }", typeof(FixedStatement));
			// TODO : Extend test.
		}
		#endregion
		
		#region VB.NET
			// No VB.NET representation
		#endregion	
	}
}
