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
	public class TypeDeclarationTests
	{
		#region C#
		[Test]
		public void CSharpSimpleClassTypeDeclarationTest()
		{
			TypeDeclaration td = (TypeDeclaration)ParseUtilCSharp.ParseGlobal("class MyClass  : My.Base.Class  { }", typeof(TypeDeclaration));
			
			Assert.AreEqual(Types.Class, td.Type);
			Assert.AreEqual("MyClass", td.Name);
			Assert.AreEqual("My.Base.Class", td.BaseTypes[0]);
			Assert.AreEqual(Modifier.None, td.Modifier);
		}
		
		[Test]
		public void CSharpSimplePartitialClassTypeDeclarationTest()
		{
			TypeDeclaration td = (TypeDeclaration)ParseUtilCSharp.ParseGlobal("partial class MyClass { }", typeof(TypeDeclaration));
			Assert.IsNotNull(td);
			Assert.AreEqual(Types.Class, td.Type);
			Assert.AreEqual("MyClass", td.Name);
			Assert.AreEqual(Modifier.Partial, td.Modifier);
		}
		
		[Test]
		public void CSharpSimpleStaticClassTypeDeclarationTest()
		{
			TypeDeclaration td = (TypeDeclaration)ParseUtilCSharp.ParseGlobal("static class MyClass { }", typeof(TypeDeclaration));
			Assert.IsNotNull(td);
			Assert.AreEqual(Types.Class, td.Type);
			Assert.AreEqual("MyClass", td.Name);
			Assert.AreEqual(Modifier.Static, td.Modifier);
		}
		
		[Test]
		public void CSharpGenericClassTypeDeclarationTest()
		{
			TypeDeclaration td = (TypeDeclaration)ParseUtilCSharp.ParseGlobal("public class G<T> {}", typeof(TypeDeclaration));
			
			Assert.AreEqual(Types.Class, td.Type);
			Assert.AreEqual("G", td.Name);
			Assert.AreEqual(Modifier.Public, td.Modifier);
			Assert.AreEqual(0, td.BaseTypes.Count);
			// TODO: test the generic stuff
		}
		
		
		[Test]
		public void CSharpGenericClassWithWhere()
		{
			string declr = @"
public class Test<T> where T : IMyInterface 
{
}
";
			TypeDeclaration td = (TypeDeclaration)ParseUtilCSharp.ParseGlobal(declr, typeof(TypeDeclaration));
			
			Assert.AreEqual(Types.Class, td.Type);
			Assert.AreEqual("Test", td.Name);
			// TODO: test the generic stuff
		}
		
		[Test]
		public void CSharpComplexGenericClassTypeDeclarationTest()
		{
			string declr = @"
public class Generic<T, S> : System.IComparable where S : G<T[]> where  T : MyNamespace.IMyInterface 
{
}
";
			TypeDeclaration td = (TypeDeclaration)ParseUtilCSharp.ParseGlobal(declr, typeof(TypeDeclaration));
			
			Assert.AreEqual(Types.Class, td.Type);
			Assert.AreEqual("Generic", td.Name);
			Assert.AreEqual(Modifier.Public, td.Modifier);
			Assert.AreEqual(1, td.BaseTypes.Count);
			Assert.AreEqual("System.IComparable", td.BaseTypes[0]);
			// TODO: test the generic stuff
		}
		
		[Test]
		public void CSharpComplexClassTypeDeclarationTest()
		{
			string declr = @"
[MyAttr()]
public abstract class MyClass : MyBase, Interface1, My.Test.Interface2 
{
}
";
			TypeDeclaration td = (TypeDeclaration)ParseUtilCSharp.ParseGlobal(declr, typeof(TypeDeclaration));
			
			Assert.AreEqual(Types.Class, td.Type);
			Assert.AreEqual("MyClass", td.Name);
			Assert.AreEqual(Modifier.Public | Modifier.Abstract, td.Modifier);
			Assert.AreEqual(1, td.Attributes.Count);
			Assert.AreEqual(3, td.BaseTypes.Count);
			Assert.AreEqual("MyBase", td.BaseTypes[0]);
			Assert.AreEqual("Interface1", td.BaseTypes[1]);
			Assert.AreEqual("My.Test.Interface2", td.BaseTypes[2]);
		}
		
		[Test]
		public void CSharpSimpleStructTypeDeclarationTest()
		{
			TypeDeclaration td = (TypeDeclaration)ParseUtilCSharp.ParseGlobal("struct MyStruct {}", typeof(TypeDeclaration));
			
			Assert.AreEqual(Types.Struct, td.Type);
			Assert.AreEqual("MyStruct", td.Name);
		}
		
		[Test]
		public void CSharpSimpleInterfaceTypeDeclarationTest()
		{
			TypeDeclaration td = (TypeDeclaration)ParseUtilCSharp.ParseGlobal("interface MyInterface {}", typeof(TypeDeclaration));
			
			Assert.AreEqual(Types.Interface, td.Type);
			Assert.AreEqual("MyInterface", td.Name);
		}
		
		[Test]
		public void CSharpSimpleEnumTypeDeclarationTest()
		{
			TypeDeclaration td = (TypeDeclaration)ParseUtilCSharp.ParseGlobal("enum MyEnum {}", typeof(TypeDeclaration));
			
			Assert.AreEqual(Types.Enum, td.Type);
			Assert.AreEqual("MyEnum", td.Name);
		}
		#endregion
		
		#region VB.NET
		[Test]
		public void VBNetSimpleClassTypeDeclarationTest()
		{
			string program = "Class TestClass\n" +
			                 "End Class\n";
			TypeDeclaration td = (TypeDeclaration)ParseUtilVBNet.ParseGlobal(program, typeof(TypeDeclaration));
			
			Assert.AreEqual("TestClass", td.Name);
			Assert.AreEqual(Types.Class, td.Type);
			Assert.AreEqual(1, td.StartLocation.Y, "start line");
			Assert.AreEqual(2, td.EndLocation.Y, "end line");
//			Assert.IsFalse(td.IsPartialType);
		}
		
		[Test]
		public void VBNetSimpleClassTypeDeclarationWithoutLastNewLineTest()
		{
			string program = "Class TestClass\n" +
			                 "End Class";
			TypeDeclaration td = (TypeDeclaration)ParseUtilVBNet.ParseGlobal(program, typeof(TypeDeclaration));
			
			Assert.AreEqual("TestClass", td.Name);
			Assert.AreEqual(Types.Class, td.Type);
			Assert.AreEqual(1, td.StartLocation.Y, "start line");
			Assert.AreEqual(2, td.EndLocation.Y, "end line");
//			Assert.IsFalse(td.IsPartialType);
		}
		
		[Test]
		public void VBNetSimplePartialClassTypeDeclarationTest()
		{
			string program = "Partial Class TestClass\n" +
			                 "End Class\n";
			TypeDeclaration td = (TypeDeclaration)ParseUtilVBNet.ParseGlobal(program, typeof(TypeDeclaration));
			
			Assert.AreEqual("TestClass", td.Name);
			Assert.AreEqual(Types.Class, td.Type);
//			Assert.IsTrue(td.IsPartialType);
		}
		#endregion
	}
}
