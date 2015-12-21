
using System;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using Rhino.Mocks;

namespace TypeScriptBinding.Tests.Parsing
{
	[TestFixture]
	public class ParseClassTests : ParseTests
	{
		[Test]
		public void Parse_EmptyStudentClass_OneClassFoundWithNameOfStudent()
		{
			string code =
				"class Student {\r\n" +
				"}\r\n";
			
			Parse(code);
			
			IUnresolvedTypeDefinition c = GetFirstClass();
			Assert.AreEqual(1, ParseInfo.UnresolvedFile.TopLevelTypeDefinitions.Count);
			Assert.AreEqual("Student", c.Name);
		}
		
		[Test]
		public void Parse_EmptyStudentClass_ClassHasCorrectBodyRegion()
		{
			string code =
				"class Student {\r\n" +
				"}\r\n";
			var expectedBodyRegion = new DomRegion(
				beginLine: 1,
				beginColumn: 14,
				endLine: 2,
				endColumn: 2);
			
			Parse(code);
			
			IUnresolvedTypeDefinition c = GetFirstClass();
			Assert.AreEqual(expectedBodyRegion, c.BodyRegion);
		}
		
		[Test]
		public void Parse_EmptyStudentClass_ClassHasBeginLineAndColumnSetForRegion()
		{
			string code =
				"class Student {\r\n" +
				"}\r\n";
			var expectedRegion = new DomRegion(
				beginLine: 1,
				beginColumn: 14,
				endLine: 2,
				endColumn: 2);
			
			Parse(code);
			
			IUnresolvedTypeDefinition c = GetFirstClass();
			Assert.AreEqual(expectedRegion, c.Region);
		}
		
		[Test]
		public void Parse_ClassWithOneMethod_ClassHasOneMethodWithCorrectBodyRegionAndName()
		{
			string code =
				"class Student {\r\n" +
				"    sayHello() {\r\n" +
				"        return \"Hello\";\r\n" +
				"    }\r\n" +
				"}\r\n";
			
			Parse(code);
			
			IUnresolvedTypeDefinition c = GetFirstClass();
			IUnresolvedMethod method = c.Methods.First();
			Assert.AreEqual("sayHello", method.Name);
			Assert.AreEqual(2, method.Region.EndLine);
			Assert.AreEqual(15, method.Region.EndColumn);
			Assert.AreEqual(4, method.BodyRegion.EndLine);
			Assert.AreEqual(6, method.BodyRegion.EndColumn);
		}
		
		[Test]
		public void Parse_TwoClassesAndSecondOneHasOneMethod_SecondClassHasOneMethodWithCorrectBodyRegionAndName()
		{
			string code =
				"class Class1 {\r\n" +
				"}\r\n" +
				"\r\n"+ 
				"class Class2 {\r\n" +
				"    sayHello() {\r\n" +
				"        return \"Hello\";\r\n" +
				"    }\r\n" +
				"}\r\n";
			
			Parse(code);
			
			IUnresolvedTypeDefinition c = GetSecondClass();
			IUnresolvedMethod method = c.Methods.FirstOrDefault();
			Assert.AreEqual(2, ParseInfo.UnresolvedFile.TopLevelTypeDefinitions.Count);
			Assert.AreEqual("sayHello", method.Name);
			Assert.AreEqual(5, method.Region.EndLine);
			Assert.AreEqual(15, method.Region.EndColumn);
			Assert.AreEqual(7, method.BodyRegion.EndLine);
			Assert.AreEqual(6, method.BodyRegion.EndColumn);
		}
		
		[Test]
		public void Parse_ClassWithConstructor_ClassHasOneConstructorMethodWithCorrectBodyRegionAndName()
		{
			string code =
				"class Student {\r\n" +
				"    constructor() {\r\n" +
				"    \r\n" +
				"    }\r\n" +
				"}\r\n";
			
			Parse(code);
			
			IUnresolvedTypeDefinition c = GetFirstClass();
			IUnresolvedMethod method = c.Methods.First();
			Assert.AreEqual("constructor", method.Name);
			Assert.AreEqual(2, method.Region.EndLine);
			Assert.AreEqual(18, method.Region.EndColumn);
			Assert.AreEqual(4, method.BodyRegion.EndLine);
			Assert.AreEqual(6, method.BodyRegion.EndColumn);
		}
		
		[Test]
		public void Parse_EmptyInterface_InterfaceAddedToCompilationUnit()
		{
			string code =
				"interface Student {\r\n" +
				"}\r\n";
			
			Parse(code);
			
			IUnresolvedTypeDefinition c = GetFirstClass();
			Assert.AreEqual("Student", c.Name);
			Assert.AreEqual(TypeKind.Interface, c.Kind);
		}
		
		[Test]
		public void Parse_ModuleWithOneClass_ModuleClassHasOneNestedClass()
		{
			string code =
				"module MyModule {\r\n" +
				"    class Student {\r\n" +
				"    }\r\n" +
				"}\r\n";
			
			Parse(code);
			
			IUnresolvedTypeDefinition module = GetFirstClass();
			Assert.AreEqual("MyModule", module.Name);
			Assert.AreEqual(TypeKind.Module, module.Kind);
			IUnresolvedTypeDefinition c = module.NestedTypes.FirstOrDefault();
			Assert.AreEqual("MyModule.Student", c.FullName);
			Assert.AreEqual(1, ParseInfo.UnresolvedFile.TopLevelTypeDefinitions.Count);
		}
		
		[Test]
		public void Parse_ModuleWithOneClassThatHasOneMethod_NestedClassHasOneMethod()
		{
			string code =
				"module MyModule {\r\n" +
				"    class Student {\r\n" +
				"         speak() {\r\n" +
				"         }\r\n" +
				"    }\r\n" +
				"}\r\n";
			
			Parse(code);
			
			IUnresolvedTypeDefinition module = GetFirstClass();
			IUnresolvedTypeDefinition c = module.NestedTypes.FirstOrDefault();
			IUnresolvedMethod method = c.Methods.First();
			Assert.AreEqual("speak", method.Name);
		}
		
		[Test]
		public void Parse_EmptyStudentClassAndGlobalVariable_OneClassFoundAndNoGlobalModuleCreated()
		{
			string code =
				"class Student {\r\n" +
				"}\r\n" +
				"\r\n" +
				"var foo = 'abc';\r\n" +
				"\r\n";
			
			Parse(code);
			
			IUnresolvedTypeDefinition c = GetFirstClass();
			Assert.AreEqual(1, ParseInfo.UnresolvedFile.TopLevelTypeDefinitions.Count);
			Assert.AreEqual("Student", c.Name);
		}
		
		[Test]
		public void Parse_EmptyStudentClass_FileNameSetForClass()
		{
			string code =
				"class Student {\r\n" +
				"}\r\n";
			string expectedFilename = @"d:\projects\MyProject\MyClass.ts";
			Parse(code, expectedFilename);
			
			IUnresolvedTypeDefinition c = GetFirstClass();
			Assert.AreEqual(expectedFilename, c.UnresolvedFile.FileName);
		}
		
		[Test]
		public void Resolve_EmptyStudentClass_ReturnsTypeDefinitionForClass()
		{
			string code =
				"class Student {\r\n" +
				"}\r\n";
			Parse(code);
			IUnresolvedTypeDefinition c = GetFirstClass();
			var context = MockRepository.GenerateStub<ITypeResolveContext>();
			context.Stub(ctx => ctx.CurrentAssembly).Return(MockRepository.GenerateStub<IAssembly>());
			
			ITypeDefinition typeDefinition = c.Resolve(context).GetDefinition();
			
			Assert.AreEqual("Student", typeDefinition.Name);
		}
	}
}