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
			Assert.AreEqual(5, ((PrimitiveExpression)lvd.Variables[0].Initializer).Value);
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
			Assert.AreEqual(new int[] {0}, type.RankSpecifier);
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
			Assert.AreEqual(new int[] {0}, type.GenericTypes[0].RankSpecifier);
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
		
		[Test]
		public void CSharpNullableLocalVariableDeclarationTest1()
		{
			LocalVariableDeclaration lvd = (LocalVariableDeclaration)ParseUtilCSharp.ParseStatment("int? a;", typeof(LocalVariableDeclaration));
			Assert.AreEqual(1, lvd.Variables.Count);
			Assert.AreEqual("a", ((VariableDeclaration)lvd.Variables[0]).Name);
			TypeReference type = lvd.GetTypeForVariable(0);
			Assert.AreEqual("System.Nullable", type.SystemType);
			Assert.AreEqual("System.Int32", type.GenericTypes[0].SystemType);
		}
		
		[Test]
		public void CSharpNullableLocalVariableDeclarationTest2()
		{
			LocalVariableDeclaration lvd = (LocalVariableDeclaration)ParseUtilCSharp.ParseStatment("DateTime? a;", typeof(LocalVariableDeclaration));
			Assert.AreEqual(1, lvd.Variables.Count);
			Assert.AreEqual("a", ((VariableDeclaration)lvd.Variables[0]).Name);
			TypeReference type = lvd.GetTypeForVariable(0);
			Assert.AreEqual("System.Nullable", type.SystemType);
			Assert.AreEqual("DateTime", type.GenericTypes[0].Type);
		}
		
		[Test]
		public void CSharpNullableLocalVariableDeclarationTest3()
		{
			LocalVariableDeclaration lvd = (LocalVariableDeclaration)ParseUtilCSharp.ParseStatment("DateTime?[] a;", typeof(LocalVariableDeclaration));
			Assert.AreEqual(1, lvd.Variables.Count);
			Assert.AreEqual("a", ((VariableDeclaration)lvd.Variables[0]).Name);
			TypeReference type = lvd.GetTypeForVariable(0);
			Assert.IsTrue(type.IsArrayType);
			Assert.AreEqual("System.Nullable", type.SystemType);
			Assert.AreEqual("DateTime", type.GenericTypes[0].Type);
		}
		
		[Test]
		public void CSharpNullableLocalVariableDeclarationTest4()
		{
			LocalVariableDeclaration lvd = (LocalVariableDeclaration)ParseUtilCSharp.ParseStatment("SomeStruct<int?>? a;", typeof(LocalVariableDeclaration));
			Assert.AreEqual(1, lvd.Variables.Count);
			Assert.AreEqual("a", ((VariableDeclaration)lvd.Variables[0]).Name);
			TypeReference type = lvd.GetTypeForVariable(0);
			Assert.AreEqual("System.Nullable", type.SystemType);
			Assert.AreEqual("SomeStruct", type.GenericTypes[0].Type);
			Assert.AreEqual("System.Nullable", type.GenericTypes[0].GenericTypes[0].SystemType);
			Assert.AreEqual("System.Int32", type.GenericTypes[0].GenericTypes[0].GenericTypes[0].SystemType);
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetLocalVariableDeclarationTest()
		{
			LocalVariableDeclaration lvd = (LocalVariableDeclaration)ParseUtilVBNet.ParseStatment("Dim a As Integer = 5", typeof(LocalVariableDeclaration));
			Assert.AreEqual(1, lvd.Variables.Count);
			Assert.AreEqual("a", lvd.Variables[0].Name);
			TypeReference type = lvd.GetTypeForVariable(0);
			Assert.AreEqual("Integer", type.Type);
			Assert.AreEqual(5, ((PrimitiveExpression)lvd.Variables[0].Initializer).Value);
		}
		
		[Test]
		public void VBNetLocalArrayDeclarationTest()
		{
			LocalVariableDeclaration lvd = (LocalVariableDeclaration)ParseUtilVBNet.ParseStatment("Dim a(10) As Integer", typeof(LocalVariableDeclaration));
			Assert.AreEqual(1, lvd.Variables.Count);
			Assert.AreEqual("a", lvd.Variables[0].Name);
			TypeReference type = lvd.GetTypeForVariable(0);
			Assert.AreEqual("Integer", type.Type);
			Assert.AreEqual(new int[] { 0 } , type.RankSpecifier);
			ArrayCreateExpression ace = (ArrayCreateExpression)lvd.Variables[0].Initializer;
			Assert.AreEqual(1, ace.Parameters.Count);
			ArrayCreationParameter acp = (ArrayCreationParameter)ace.Parameters[0];
			Assert.AreEqual(1, acp.Expressions.Count);
			Assert.AreEqual(11, ((PrimitiveExpression)acp.Expressions[0]).Value);
		}
		
		
		[Test]
		public void VBNetComplexGenericLocalVariableDeclarationTest()
		{
			LocalVariableDeclaration lvd = (LocalVariableDeclaration)ParseUtilVBNet.ParseStatment("Dim where As Generic(Of Printable, G(Of Printable()))", typeof(LocalVariableDeclaration));
			Assert.AreEqual(1, lvd.Variables.Count);
			Assert.AreEqual("where", lvd.Variables[0].Name);
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
			Assert.AreEqual(new int[] { 0 }, type.RankSpecifier);
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
