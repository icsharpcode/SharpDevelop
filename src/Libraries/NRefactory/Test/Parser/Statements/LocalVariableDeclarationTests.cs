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
			Assert.AreEqual(1, lvd.Variables.Count);
			Assert.AreEqual("a", ((VariableDeclaration)lvd.Variables[0]).Name);
			TypeReference type = lvd.GetTypeForVariable(0);
			Assert.AreEqual("int", type.Type);
			// TODO: Check initializer
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
			
			// TODO: Check initializer
		}
		
		[Test]
		public void CSharpGenericWithArrayLocalVariableDeclarationTest1()
		{
			LocalVariableDeclaration lvd = (LocalVariableDeclaration)ParseUtilCSharp.ParseStatment("G<int>[] a;", typeof(LocalVariableDeclaration));
			Assert.AreEqual(1, lvd.Variables.Count);
			TypeReference type = lvd.GetTypeForVariable(0);
			Assert.AreEqual("G", type.Type);
			Assert.AreEqual(1, type.GenericTypes.Count);
			Assert.AreEqual("int", type.GenericTypes[0].Type);
			Assert.AreEqual(0, type.GenericTypes[0].GenericTypes.Count);
			Assert.IsFalse(type.GenericTypes[0].IsArrayType);
			Assert.AreEqual(1, type.RankSpecifier.Length);
			Assert.AreEqual(0, type.RankSpecifier[0]);
		}
		
		[Test]
		public void CSharpGenericWithArrayLocalVariableDeclarationTest2()
		{
			LocalVariableDeclaration lvd = (LocalVariableDeclaration)ParseUtilCSharp.ParseStatment("G<int[]> a;", typeof(LocalVariableDeclaration));
			Assert.AreEqual(1, lvd.Variables.Count);
			TypeReference type = lvd.GetTypeForVariable(0);
			Assert.AreEqual("G", type.Type);
			Assert.AreEqual(1, type.GenericTypes.Count);
			Assert.AreEqual("int", type.GenericTypes[0].Type);
			Assert.AreEqual(0, type.GenericTypes[0].GenericTypes.Count);
			Assert.IsFalse(type.IsArrayType);
			Assert.AreEqual(1, type.GenericTypes[0].RankSpecifier.Length);
			Assert.AreEqual(0, type.GenericTypes[0].RankSpecifier[0]);
		}
		
		[Test]
		public void CSharpGenericLocalVariableDeclarationTest2()
		{
			LocalVariableDeclaration lvd = (LocalVariableDeclaration)ParseUtilCSharp.ParseStatment("G<G<int> > a;", typeof(LocalVariableDeclaration));
			Assert.AreEqual(1, lvd.Variables.Count);
			TypeReference type = lvd.GetTypeForVariable(0);
			Assert.AreEqual("G", type.Type);
			Assert.AreEqual(1, type.GenericTypes.Count);
			Assert.AreEqual("G", type.GenericTypes[0].Type);
			Assert.AreEqual(1, type.GenericTypes[0].GenericTypes.Count);
			Assert.AreEqual("int", type.GenericTypes[0].GenericTypes[0].Type);
		}
		
		[Test]
		public void CSharpGenericLocalVariableDeclarationTest2WithoutSpace()
		{
			LocalVariableDeclaration lvd = (LocalVariableDeclaration)ParseUtilCSharp.ParseStatment("G<G<int>> a;", typeof(LocalVariableDeclaration));
			Assert.AreEqual(1, lvd.Variables.Count);
			TypeReference type = lvd.GetTypeForVariable(0);
			Assert.AreEqual("G", type.Type);
			Assert.AreEqual(1, type.GenericTypes.Count);
			Assert.AreEqual("G", type.GenericTypes[0].Type);
			Assert.AreEqual(1, type.GenericTypes[0].GenericTypes.Count);
			Assert.AreEqual("int", type.GenericTypes[0].GenericTypes[0].Type);
		}
		
		[Test]
		public void CSharpGenericLocalVariableDeclarationTest()
		{
			LocalVariableDeclaration lvd = (LocalVariableDeclaration)ParseUtilCSharp.ParseStatment("G<int> a;", typeof(LocalVariableDeclaration));
			Assert.AreEqual(1, lvd.Variables.Count);
			TypeReference type = lvd.GetTypeForVariable(0);
			Assert.AreEqual("G", type.Type);
			Assert.AreEqual(1, type.GenericTypes.Count);
			Assert.AreEqual("int", type.GenericTypes[0].Type);
		}
		[Test]
		public void CSharpSimpleLocalVariableDeclarationTest()
		{
			LocalVariableDeclaration lvd = (LocalVariableDeclaration)ParseUtilCSharp.ParseStatment("MyVar var = new MyVar();", typeof(LocalVariableDeclaration));
			Assert.AreEqual(1, lvd.Variables.Count);
			Assert.AreEqual("var", ((VariableDeclaration)lvd.Variables[0]).Name);
			TypeReference type = lvd.GetTypeForVariable(0);
			Assert.AreEqual("MyVar", type.Type);
			// TODO: Check initializer
		}
		[Test]
		public void CSharpSimpleLocalVariableDeclarationTest1()
		{
			LocalVariableDeclaration lvd = (LocalVariableDeclaration)ParseUtilCSharp.ParseStatment("yield yield = new yield();", typeof(LocalVariableDeclaration));
			Assert.AreEqual(1, lvd.Variables.Count);
			Assert.AreEqual("yield", ((VariableDeclaration)lvd.Variables[0]).Name);
			TypeReference type = lvd.GetTypeForVariable(0);
			Assert.AreEqual("yield", type.Type);
			// TODO: Check initializer
		}
		
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetLocalVariableDeclarationTest()
		{
			LocalVariableDeclaration lvd = (LocalVariableDeclaration)ParseUtilVBNet.ParseStatment("Dim a As Integer = 5", typeof(LocalVariableDeclaration));
			Assert.AreEqual(1, lvd.Variables.Count);
			Assert.AreEqual("a", ((VariableDeclaration)lvd.Variables[0]).Name);
			TypeReference type = lvd.GetTypeForVariable(0);
			Assert.AreEqual("Integer", type.Type);
			// TODO: Check initializer
		}
		
		[Test]
		public void VBNetComplexGenericLocalVariableDeclarationTest()
		{
			LocalVariableDeclaration lvd = (LocalVariableDeclaration)ParseUtilVBNet.ParseStatment("Dim where As Generic(Of Printable, G(Of Printable()))", typeof(LocalVariableDeclaration));
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
		public void VBNetGenericWithArrayLocalVariableDeclarationTest1()
		{
			LocalVariableDeclaration lvd = (LocalVariableDeclaration)ParseUtilVBNet.ParseStatment("Dim a As G(Of Integer)()", typeof(LocalVariableDeclaration));
			Assert.AreEqual(1, lvd.Variables.Count);
			TypeReference type = lvd.GetTypeForVariable(0);
			Assert.AreEqual("G", type.Type);
			Assert.AreEqual(1, type.GenericTypes.Count);
			Assert.AreEqual("Integer", type.GenericTypes[0].Type);
			Assert.AreEqual(0, type.GenericTypes[0].GenericTypes.Count);
			Assert.IsFalse(type.GenericTypes[0].IsArrayType);
			Assert.AreEqual(1, type.RankSpecifier.Length);
			Assert.AreEqual(0, type.RankSpecifier[0]);
		}
		
		[Test]
		public void VBNetGenericWithArrayLocalVariableDeclarationTest2()
		{
			LocalVariableDeclaration lvd = (LocalVariableDeclaration)ParseUtilVBNet.ParseStatment("Dim a As G(Of Integer())", typeof(LocalVariableDeclaration));
			Assert.AreEqual(1, lvd.Variables.Count);
			TypeReference type = lvd.GetTypeForVariable(0);
			Assert.AreEqual("G", type.Type);
			Assert.AreEqual(1, type.GenericTypes.Count);
			Assert.AreEqual("Integer", type.GenericTypes[0].Type);
			Assert.AreEqual(0, type.GenericTypes[0].GenericTypes.Count);
			Assert.IsFalse(type.IsArrayType);
			Assert.AreEqual(1, type.GenericTypes[0].RankSpecifier.Length);
			Assert.AreEqual(0, type.GenericTypes[0].RankSpecifier[0]);
		}
		
		[Test]
		public void VBNetGenericLocalVariableDeclarationTest2()
		{
			LocalVariableDeclaration lvd = (LocalVariableDeclaration)ParseUtilVBNet.ParseStatment("Dim a As G(Of G(Of Integer))", typeof(LocalVariableDeclaration));
			Assert.AreEqual(1, lvd.Variables.Count);
			TypeReference type = lvd.GetTypeForVariable(0);
			Assert.AreEqual("G", type.Type);
			Assert.AreEqual(1, type.GenericTypes.Count);
			Assert.AreEqual("G", type.GenericTypes[0].Type);
			Assert.AreEqual(1, type.GenericTypes[0].GenericTypes.Count);
			Assert.AreEqual("Integer", type.GenericTypes[0].GenericTypes[0].Type);
		}
		
		[Test]
		public void VBNetGenericLocalVariableDeclarationTest()
		{
			LocalVariableDeclaration lvd = (LocalVariableDeclaration)ParseUtilVBNet.ParseStatment("Dim a As G(Of Integer)", typeof(LocalVariableDeclaration));
			Assert.AreEqual(1, lvd.Variables.Count);
			TypeReference type = lvd.GetTypeForVariable(0);
			Assert.AreEqual("G", type.Type);
			Assert.AreEqual(1, type.GenericTypes.Count);
			Assert.AreEqual("Integer", type.GenericTypes[0].Type);
		}
		
		[Test]
		public void VBNetGenericLocalVariableInitializationTest()
		{
			LocalVariableDeclaration lvd = (LocalVariableDeclaration)ParseUtilVBNet.ParseStatment("Dim a As New G(Of Integer)", typeof(LocalVariableDeclaration));
			Assert.AreEqual(1, lvd.Variables.Count);
			TypeReference type = lvd.GetTypeForVariable(0);
			Assert.AreEqual("G", type.Type);
			Assert.AreEqual(1, type.GenericTypes.Count);
			Assert.AreEqual("Integer", type.GenericTypes[0].Type);
			// TODO: Check initializer
		}
		#endregion
	}
}
