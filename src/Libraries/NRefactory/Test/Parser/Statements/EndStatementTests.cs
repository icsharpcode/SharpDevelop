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
	public class EndStatementTests
	{
		#region C#
		// No C# representation
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetEndStatementTest()
		{
			EndStatement endStatement = (EndStatement)ParseUtilVBNet.ParseStatment("End", typeof(EndStatement));
		}
		
		[Test]
		public void VBNetEndStatementInIfThenTest2()
		{
			IfElseStatement endStatement = (IfElseStatement)ParseUtilVBNet.ParseStatment("IF a THEN End", typeof(IfElseStatement));
		}
		#endregion
	}
}
