// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
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
			SwitchStatement switchStmt = ParseUtilCSharp.ParseStatement<SwitchStatement>("switch (a) { case 5: break; case 6: break; default: break; }");
			// TODO : Extend test.
		}
		#endregion
		
		#region VB.NET
			// TODO
		#endregion 
	}
}
