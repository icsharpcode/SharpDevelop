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
	public class LocalVariableDeclarationTests
	{
		#region C#
		
		[Test]
		public void CSharpLocalVariableDeclarationTest()
		{
			LocalVariableDeclaration lvd = (LocalVariableDeclaration)ParseUtilCSharp.ParseStatment("int a = 5;", typeof(LocalVariableDeclaration));
			// TODO : Extend test.
		}
		
		[Test]
		public void CSharpComplexGenericLocalVariableDeclarationTest()
		{
			LocalVariableDeclaration lvd = (LocalVariableDeclaration)ParseUtilCSharp.ParseStatment("Generic<Printable, G<Printable[]> > where = new Generic<Printable, G<Printable[]>>();", typeof(LocalVariableDeclaration));
			Assert.AreEqual(1, lvd.Variables.Count);
			Assert.AreEqual("where", ((VariableDeclaration)lvd.Variables[0]).Name);
			TypeReference type = lvd.GetTypeForVariable(0);
			Assert.AreEqual("Generic", type.Type);
			Assert.AreEqual(2, type.GenericTypes.Count);
			Assert.AreEqual("Printable", type.GenericTypes[0].Type);
			Assert.AreEqual(0, type.GenericTypes[0].GenericTypes.Count);
			Assert.AreEqual("G", type.GenericTypes[1].Type);
			Assert.AreEqual(1, type.GenericTypes[1].GenericTypes.Count);
			Assert.AreEqual("Printable", type.GenericTypes[1].GenericTypes[0].Type);
		}
		
		[Test]
		public void CSharpGenericWithArrayLocalVariableDeclarationTest1()
		{
			LocalVariableDeclaration lvd = (LocalVariableDeclaration)ParseUtilCSharp.ParseStatment("G<int>[] a;", typeof(LocalVariableDeclaration));
			
		}
		
		[Test]
		public void CSharpGenericWithArrayLocalVariableDeclarationTest2()
		{
			LocalVariableDeclaration lvd = (LocalVariableDeclaration)ParseUtilCSharp.ParseStatment("G<int[]> a;", typeof(LocalVariableDeclaration));
			
		}
		
		[Test]
		public void CSharpGenericLocalVariableDeclarationTest2()
		{
			LocalVariableDeclaration lvd = (LocalVariableDeclaration)ParseUtilCSharp.ParseStatment("G<G<int> > a;", typeof(LocalVariableDeclaration));
			
		}
		
		[Test]
		public void CSharpGenericLocalVariableDeclarationTest()
		{
			LocalVariableDeclaration lvd = (LocalVariableDeclaration)ParseUtilCSharp.ParseStatment("G<int> a;", typeof(LocalVariableDeclaration));
			
		}
		[Test]
		public void CSharpSimpleLocalVariableDeclarationTest()
		{
			LocalVariableDeclaration lvd = (LocalVariableDeclaration)ParseUtilCSharp.ParseStatment("MyVar var = new MyVar();", typeof(LocalVariableDeclaration));
			
		}
		[Test]
		public void CSharpSimpleLocalVariableDeclarationTest1()
		{
			LocalVariableDeclaration lvd = (LocalVariableDeclaration)ParseUtilCSharp.ParseStatment("yield yield = new yield();", typeof(LocalVariableDeclaration));
		}
		
		#endregion
		
		#region VB.NET
			// TODO
		#endregion 
	}
}
