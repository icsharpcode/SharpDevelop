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
