// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

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
			
			Assert.AreEqual(ClassType.Class, td.Type);
			Assert.AreEqual("MyClass", td.Name);
			Assert.AreEqual("My.Base.Class", td.BaseTypes[0].Type);
			Assert.AreEqual(Modifier.None, td.Modifier);
		}
		
		[Test]
		public void CSharpSimplePartitialClassTypeDeclarationTest()
		{
			TypeDeclaration td = (TypeDeclaration)ParseUtilCSharp.ParseGlobal("partial class MyClass { }", typeof(TypeDeclaration));
			Assert.IsNotNull(td);
			Assert.AreEqual(ClassType.Class, td.Type);
			Assert.AreEqual("MyClass", td.Name);
			Assert.AreEqual(Modifier.Partial, td.Modifier);
		}
		
		[Test]
		public void CSharpSimpleStaticClassTypeDeclarationTest()
		{
			TypeDeclaration td = (TypeDeclaration)ParseUtilCSharp.ParseGlobal("static class MyClass { }", typeof(TypeDeclaration));
			Assert.IsNotNull(td);
			Assert.AreEqual(ClassType.Class, td.Type);
			Assert.AreEqual("MyClass", td.Name);
			Assert.AreEqual(Modifier.Static, td.Modifier);
		}
		
		[Test]
		public void CSharpGenericClassTypeDeclarationTest()
		{
			TypeDeclaration td = (TypeDeclaration)ParseUtilCSharp.ParseGlobal("public class G<T> {}", typeof(TypeDeclaration));
			
			Assert.AreEqual(ClassType.Class, td.Type);
			Assert.AreEqual("G", td.Name);
			Assert.AreEqual(Modifier.Public, td.Modifier);
			Assert.AreEqual(0, td.BaseTypes.Count);
			Assert.AreEqual(1, td.Templates.Count);
			Assert.AreEqual("T", td.Templates[0].Name);
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
			
			Assert.AreEqual(ClassType.Class, td.Type);
			Assert.AreEqual("Test", td.Name);
			
			Assert.AreEqual(1, td.Templates.Count);
			Assert.AreEqual("T", td.Templates[0].Name);
			Assert.AreEqual("IMyInterface", td.Templates[0].Bases[0].Type);
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
			
			Assert.AreEqual(ClassType.Class, td.Type);
			Assert.AreEqual("Generic", td.Name);
			Assert.AreEqual(Modifier.Public, td.Modifier);
			Assert.AreEqual(1, td.BaseTypes.Count);
			Assert.AreEqual("System.IComparable", td.BaseTypes[0].Type);
			
			Assert.AreEqual(2, td.Templates.Count);
			Assert.AreEqual("T", td.Templates[0].Name);
			Assert.AreEqual("MyNamespace.IMyInterface", td.Templates[0].Bases[0].Type);
			
			Assert.AreEqual("S", td.Templates[1].Name);
			Assert.AreEqual("G", td.Templates[1].Bases[0].Type);
			Assert.AreEqual(1, td.Templates[1].Bases[0].GenericTypes.Count);
			Assert.IsTrue(td.Templates[1].Bases[0].GenericTypes[0].IsArrayType);
			Assert.AreEqual("T", td.Templates[1].Bases[0].GenericTypes[0].Type);
			Assert.AreEqual(new int[] {0}, td.Templates[1].Bases[0].GenericTypes[0].RankSpecifier);
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
			
			Assert.AreEqual(ClassType.Class, td.Type);
			Assert.AreEqual("MyClass", td.Name);
			Assert.AreEqual(Modifier.Public | Modifier.Abstract, td.Modifier);
			Assert.AreEqual(1, td.Attributes.Count);
			Assert.AreEqual(3, td.BaseTypes.Count);
			Assert.AreEqual("MyBase", td.BaseTypes[0].Type);
			Assert.AreEqual("Interface1", td.BaseTypes[1].Type);
			Assert.AreEqual("My.Test.Interface2", td.BaseTypes[2].Type);
		}
		
		[Test]
		public void CSharpSimpleStructTypeDeclarationTest()
		{
			TypeDeclaration td = (TypeDeclaration)ParseUtilCSharp.ParseGlobal("struct MyStruct {}", typeof(TypeDeclaration));
			
			Assert.AreEqual(ClassType.Struct, td.Type);
			Assert.AreEqual("MyStruct", td.Name);
		}
		
		[Test]
		public void CSharpSimpleInterfaceTypeDeclarationTest()
		{
			TypeDeclaration td = (TypeDeclaration)ParseUtilCSharp.ParseGlobal("interface MyInterface {}", typeof(TypeDeclaration));
			
			Assert.AreEqual(ClassType.Interface, td.Type);
			Assert.AreEqual("MyInterface", td.Name);
		}
		
		[Test]
		public void CSharpSimpleEnumTypeDeclarationTest()
		{
			TypeDeclaration td = (TypeDeclaration)ParseUtilCSharp.ParseGlobal("enum MyEnum {}", typeof(TypeDeclaration));
			
			Assert.AreEqual(ClassType.Enum, td.Type);
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
			Assert.AreEqual(ClassType.Class, td.Type);
			Assert.AreEqual(1, td.StartLocation.Y, "start line");
			Assert.AreEqual(2, td.EndLocation.Y, "end line");
		}
		
		[Test]
		public void VBNetEnumWithBaseClassDeclarationTest()
		{
			string program = "Enum TestEnum As Byte\n" +
				"End Enum\n";
			TypeDeclaration td = (TypeDeclaration)ParseUtilVBNet.ParseGlobal(program, typeof(TypeDeclaration));
			
			Assert.AreEqual("TestEnum", td.Name);
			Assert.AreEqual(ClassType.Enum, td.Type);
			Assert.AreEqual("Byte", td.BaseTypes[0].Type);
		}
		
		[Test]
		public void VBNetSimpleClassTypeDeclarationWithoutLastNewLineTest()
		{
			string program = "Class TestClass\n" +
				"End Class";
			TypeDeclaration td = (TypeDeclaration)ParseUtilVBNet.ParseGlobal(program, typeof(TypeDeclaration));
			
			Assert.AreEqual("TestClass", td.Name);
			Assert.AreEqual(ClassType.Class, td.Type);
			Assert.AreEqual(1, td.StartLocation.Y, "start line");
			Assert.AreEqual(2, td.EndLocation.Y, "end line");
		}
		
		[Test]
		public void VBNetSimplePartialClassTypeDeclarationTest()
		{
			string program = "Partial Class TestClass\n" +
				"End Class\n";
			TypeDeclaration td = (TypeDeclaration)ParseUtilVBNet.ParseGlobal(program, typeof(TypeDeclaration));
			
			Assert.AreEqual("TestClass", td.Name);
			Assert.AreEqual(ClassType.Class, td.Type);
			Assert.AreEqual(Modifier.Partial, td.Modifier);
		}
		
		[Test]
		public void VBNetPartialPublicClass()
		{
			string program = "Partial Public Class TestClass\nEnd Class\n";
			TypeDeclaration td = (TypeDeclaration)ParseUtilVBNet.ParseGlobal(program, typeof(TypeDeclaration));
			
			Assert.AreEqual("TestClass", td.Name);
			Assert.AreEqual(ClassType.Class, td.Type);
			Assert.AreEqual(Modifier.Partial | Modifier.Public, td.Modifier);
		}
		
		[Test]
		public void VBNetGenericClassTypeDeclarationTest()
		{
			string declr = @"
Public Class Test(Of T)

End Class
";
			TypeDeclaration td = (TypeDeclaration)ParseUtilVBNet.ParseGlobal(declr, typeof(TypeDeclaration));
			
			Assert.AreEqual(ClassType.Class, td.Type);
			Assert.AreEqual("Test", td.Name);
			Assert.AreEqual(Modifier.Public, td.Modifier);
			Assert.AreEqual(0, td.BaseTypes.Count);
			Assert.AreEqual(1, td.Templates.Count);
			Assert.AreEqual("T", td.Templates[0].Name);
		}
		
		[Test]
		public void VBNetGenericClassWithConstraint()
		{
			string declr = @"
Public Class Test(Of T As IMyInterface)

End Class
";
			TypeDeclaration td = (TypeDeclaration)ParseUtilVBNet.ParseGlobal(declr, typeof(TypeDeclaration));
			
			Assert.AreEqual(ClassType.Class, td.Type);
			Assert.AreEqual("Test", td.Name);
			
			Assert.AreEqual(1, td.Templates.Count);
			Assert.AreEqual("T", td.Templates[0].Name);
			Assert.AreEqual("IMyInterface", td.Templates[0].Bases[0].Type);
		}
		
		[Test]
		public void VBNetComplexGenericClassTypeDeclarationTest()
		{
			string declr = @"
Public Class Generic(Of T As MyNamespace.IMyInterface, S As {G(Of T()), IAnotherInterface})
	Implements System.IComparable

End Class
";
			TypeDeclaration td = (TypeDeclaration)ParseUtilVBNet.ParseGlobal(declr, typeof(TypeDeclaration));
			
			Assert.AreEqual(ClassType.Class, td.Type);
			Assert.AreEqual("Generic", td.Name);
			Assert.AreEqual(Modifier.Public, td.Modifier);
			Assert.AreEqual(1, td.BaseTypes.Count);
			Assert.AreEqual("System.IComparable", td.BaseTypes[0].Type);
			
			Assert.AreEqual(2, td.Templates.Count);
			Assert.AreEqual("T", td.Templates[0].Name);
			Assert.AreEqual("MyNamespace.IMyInterface", td.Templates[0].Bases[0].Type);
			
			Assert.AreEqual("S", td.Templates[1].Name);
			Assert.AreEqual(2, td.Templates[1].Bases.Count);
			Assert.AreEqual("G", td.Templates[1].Bases[0].Type);
			Assert.AreEqual(1, td.Templates[1].Bases[0].GenericTypes.Count);
			Assert.IsTrue(td.Templates[1].Bases[0].GenericTypes[0].IsArrayType);
			Assert.AreEqual("T", td.Templates[1].Bases[0].GenericTypes[0].Type);
			Assert.AreEqual(new int[] {0}, td.Templates[1].Bases[0].GenericTypes[0].RankSpecifier);
			Assert.AreEqual("IAnotherInterface", td.Templates[1].Bases[1].Type);
		}
		#endregion
	}
}
