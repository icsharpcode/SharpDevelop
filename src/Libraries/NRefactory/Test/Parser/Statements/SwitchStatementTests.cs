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
	public class SwitchStatementTests
	{
		#region C#
		[Test]
		public void CSharpSwitchStatementTest()
		{
			SwitchStatement switchStmt = (SwitchStatement)ParseUtilCSharp.ParseStatment("switch (a) { case 5: break; case 6: break; default: break; }", typeof(SwitchStatement));
			// TODO : Extend test.
		}
		#endregion
		
		#region VB.NET
			// TODO
		#endregion 
	}
}
