/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 13.09.2004
 * Time: 19:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.IO;

using NUnit.Framework;

using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Tests.AST
{
	[TestFixture]
	public class MethodDeclarationTests
	{
		#region C#
		[Test]
		public void CSharpSimpleMethodDeclarationTest()
		{
			MethodDeclaration md = (MethodDeclaration)ParseUtilCSharp.ParseTypeMember("void MyMethod() {} ", typeof(MethodDeclaration));
			Assert.AreEqual("void", md.TypeReference.Type);
			Assert.AreEqual(0, md.Parameters.Count);
		}
		#endregion
		
		#region VB.NET
			// TODO
		#endregion 
	}
}
