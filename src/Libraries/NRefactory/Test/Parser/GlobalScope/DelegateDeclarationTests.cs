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
	public class DelegateDeclarationTests
	{
		void TestDelegateDeclaration(DelegateDeclaration dd)
		{
			Assert.AreEqual("System.Void", dd.ReturnType.SystemType);
			Assert.AreEqual("MyDelegate", dd.Name);
			Assert.AreEqual(3, dd.Parameters.Count);
			
			Assert.AreEqual("a", ((ParameterDeclarationExpression)dd.Parameters[0]).ParameterName);
			Assert.AreEqual("System.Int32", ((ParameterDeclarationExpression)dd.Parameters[0]).TypeReference.SystemType);
			
			Assert.AreEqual("secondParam", ((ParameterDeclarationExpression)dd.Parameters[1]).ParameterName);
			Assert.AreEqual("System.Int32", ((ParameterDeclarationExpression)dd.Parameters[1]).TypeReference.SystemType);
			
			Assert.AreEqual("lastParam", ((ParameterDeclarationExpression)dd.Parameters[2]).ParameterName);
			Assert.AreEqual("MyObj", ((ParameterDeclarationExpression)dd.Parameters[2]).TypeReference.Type);
		}
		
		#region C#
		[Test]
		public void SimpleCSharpDelegateDeclarationTest()
		{
			string program = "public delegate void MyDelegate(int a, int secondParam, MyObj lastParam);\n";
			TestDelegateDeclaration((DelegateDeclaration)ParseUtilCSharp.ParseGlobal(program, typeof(DelegateDeclaration)));
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void SimpleVBNetDelegateDeclarationTest()
		{
			string program = "Public Delegate Sub MyDelegate(ByVal a As Integer, ByVal secondParam As Integer, ByVal lastParam As MyObj)\n";
			TestDelegateDeclaration((DelegateDeclaration)ParseUtilVBNet.ParseGlobal(program, typeof(DelegateDeclaration)));
		}
		#endregion
	}
}
