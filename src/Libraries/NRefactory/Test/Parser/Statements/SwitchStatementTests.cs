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
