// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class ClassCodeGeneratorTests
	{
		ClassHelper helper;
		ClassCodeGenerator codeGenerator;
		CodeVariable codeVariable;
		CodeFunction codeFunction;
		IDocumentLoader documentLoader;
		IRefactoringDocument document;
		FakeCodeGenerator fakeCodeGenerator;
		IRefactoringDocumentView documentView;
		
		[SetUp]
		public void Init()
		{
			helper = new ClassHelper();
			document = MockRepository.GenerateStub<IRefactoringDocument>();
			documentView = MockRepository.GenerateStub<IRefactoringDocumentView>();
			documentLoader = MockRepository.GenerateStub<IDocumentLoader>();
			fakeCodeGenerator = helper.CompilationUnitHelper.FakeCodeGenerator;
		}
		
		void CreateClass(string name)
		{
			helper.CreatePublicClass(name);
			helper.SetClassType(ICSharpCode.SharpDevelop.Dom.ClassType.Class);
		}
		
		void CreateCodeGenerator()
		{
			codeGenerator = new ClassCodeGenerator(helper.Class, documentLoader);
		}
		
		void SetClassFileName(string fileName)
		{
			helper.SetClassFileName(fileName);
		}
		
		void SetDocumentFileName(string fileName)
		{
			documentView.Stub(view => view.RefactoringDocument).Return(document);
			documentLoader.Stub(loader => loader.LoadRefactoringDocumentView(fileName)).Return(documentView);
		}
		
		void AddFieldToClassForReparse(string name)
		{
			AddFieldToClassForReparse(name, DomRegion.Empty);
		}
		
		void AddFieldToClassForReparse(string name, DomRegion region)
		{
			ClassHelper helper = CreateClassHelper("MyClass");
			helper.AddFieldToClass(name, region);
			AddClassesToReparsedCompilationUnit(helper);
		}
		
		void AddFieldsToClassForReparse(params string[] names)
		{
			ClassHelper helper = CreateClassHelper("MyClass");
			foreach (string name in names) {
				helper.AddFieldToClass(name);
			}
			AddClassesToReparsedCompilationUnit(helper);
		}
		
		void AddClassesToReparsedCompilationUnit(params ClassHelper[] classHelpers)
		{
			var compilationUnitHelper = new CompilationUnitHelper();
			foreach (ClassHelper helper in classHelpers) {
				compilationUnitHelper.AddClass(helper.Class);
			}
			documentView.Stub(d => d.Parse()).Return(compilationUnitHelper.CompilationUnit);
		}
		
		ClassHelper CreateClassHelper(string name)
		{
			var helper = new ClassHelper();
			helper.CreateClass(name);
			return helper;
		}
		
		void AddMethodToClassForReparse(string name)
		{
			AddMethodToClassForReparse(name, DomRegion.Empty, DomRegion.Empty);
		}
		
		void AddMethodToClassForReparse(string name, DomRegion region, DomRegion bodyRegion)
		{
			ClassHelper helper = CreateClassHelper("MyClass");
			helper.AddMethodToClass(name, region, bodyRegion);
			AddClassesToReparsedCompilationUnit(helper);
		}
		
		void AddMethodsToClassForReparse(params string[] names)
		{
			ClassHelper helper = CreateClassHelper("MyClass");
			foreach (string name in names) {
				helper.AddMethodToClass(name);
			}
			AddClassesToReparsedCompilationUnit(helper);
		}
		
		void AddPublicVariable(string name, string type)
		{
			codeVariable = codeGenerator.AddPublicVariable(name, type);
		}
		
		void AddPublicMethod(string name, string type)
		{
			codeFunction = codeGenerator.AddPublicMethod(name, type);
		}
		
		[Test]
		public void AddPublicVariable_VariableTypeIsString_ReturnsCodeVariable()
		{
			CreateClass("MyClass");
			CreateCodeGenerator();
			string fileName = @"d:\projects\myproject\MyClass.cs";
			SetClassFileName(fileName);
			SetDocumentFileName(fileName);
			AddFieldToClassForReparse("MyClass.MyVariable", new DomRegion(1, 2, 1, 5));
			
			AddPublicVariable("MyVariable", "System.String");
			
			TextPoint start = (TextPoint)codeVariable.GetStartPoint();
			TextPoint end = (TextPoint)codeVariable.GetEndPoint();
			Assert.AreEqual("MyVariable", codeVariable.Name);
			Assert.AreEqual(1, start.Line);
			Assert.AreEqual(2, start.LineCharOffset);
			Assert.AreEqual(1, end.Line);
			Assert.AreEqual(5, end.LineCharOffset);
		}

		[Test]
		public void AddPublicVariable_VariableTypeIsCustomType_CodeForFieldAddedAtEndOfClass()
		{
			CreateClass("MyClass");
			CreateCodeGenerator();
			var classRegion = new DomRegion(1, 2, 3, 4);
			helper.SetClassRegion(classRegion);
			string fileName = @"d:\projects\myproject\MyClass.cs";
			SetClassFileName(fileName);
			SetDocumentFileName(fileName);
			AddFieldToClassForReparse("MyClass.MyVariable");
			
			AddPublicVariable("MyVar", "MyType");
			
			FieldDeclaration field = fakeCodeGenerator.NodePassedToInsertCodeAtEnd as FieldDeclaration;
			Assert.AreEqual(classRegion, fakeCodeGenerator.RegionPassedToInsertCodeAtEnd);
			Assert.AreEqual(document, fakeCodeGenerator.DocumentPassedToInsertCodeAtEnd);
			Assert.AreEqual(Modifiers.Public, field.Modifier);
			Assert.AreEqual("MyType", field.TypeReference.Type);
			Assert.AreEqual("MyVar", field.Fields[0].Name);
		}
		
		[Test]
		public void AddPublicVariable_ReparsedClassHasTwoFields_LastFieldReturned()
		{
			CreateClass("MyClass");
			CreateCodeGenerator();
			string fileName = @"d:\projects\myproject\MyClass.cs";
			SetClassFileName(fileName);
			SetDocumentFileName(fileName);
			AddFieldsToClassForReparse("MyClass.First", "MyClass.MyVariable");
			
			AddPublicVariable("MyVariable", "System.String");
			
			Assert.AreEqual("MyVariable", codeVariable.Name);
		}
		
		[Test]
		public void AddPublicVariable_ReparsedCompilationUnitHasThreeClasses_VariableReturnedFromCorrectClass()
		{
			CreateClass("MyClass2");
			CreateCodeGenerator();
			string fileName = @"d:\projects\myproject\MyClass2.cs";
			SetClassFileName(fileName);
			SetDocumentFileName(fileName);
			ClassHelper class1 = CreateClassHelper("MyClass1");
			ClassHelper class2 = CreateClassHelper("MyClass2");
			ClassHelper class3 = CreateClassHelper("MyClass3");
			
			class2.AddFieldToClass("MyClass2.MyVariable");
			
			AddClassesToReparsedCompilationUnit(class1, class2, class3);
			
			AddPublicVariable("MyVariable", "System.String");
			
			Assert.AreEqual("MyVariable", codeVariable.Name);
		}
		
		[Test]
		public void AddPublicMethod_MethodReturnTypeIsSystemCustomType_CodeForMethodAddedAtEndOfClass()
		{
			CreateClass("MyClass");
			CreateCodeGenerator();
			var classRegion = new DomRegion(1, 2, 3, 4);
			helper.SetClassRegion(classRegion);
			string fileName = @"d:\projects\myproject\MyClass.cs";
			SetClassFileName(fileName);
			SetDocumentFileName(fileName);
			AddMethodToClassForReparse("MyClass.MyMethod");
			
			AddPublicMethod("MyMethod", "MyType");
			
			MethodDeclaration method = fakeCodeGenerator.NodePassedToInsertCodeAtEnd as MethodDeclaration;
			Assert.AreEqual(classRegion, fakeCodeGenerator.RegionPassedToInsertCodeAtEnd);
			Assert.AreEqual(document, fakeCodeGenerator.DocumentPassedToInsertCodeAtEnd);
			Assert.AreEqual(Modifiers.None, method.Modifier);
			Assert.AreEqual("MyType", method.TypeReference.Type);
			Assert.AreEqual("MyMethod", method.Name);
			Assert.IsNotNull(method.Body);
			Assert.IsTrue(method.Body.IsNull);
		}
		
		/// <summary>
		/// Workaround NRefactory giving incorrect begin column for interface methods that use fully
		/// qualified return types (e.g. System.Object). Begin column points to the "Object" part of the
		/// return type not the start of the return type.
		/// </summary>
		[Test]
		public void AddPublicMethod_MethodReturnTypeIsSystemObject_SystemNotAddedToReturnType()
		{
			CreateClass("MyClass");
			CreateCodeGenerator();
			var classRegion = new DomRegion(1, 2, 3, 4);
			helper.SetClassRegion(classRegion);
			string fileName = @"d:\projects\myproject\MyClass.cs";
			SetClassFileName(fileName);
			SetDocumentFileName(fileName);
			AddMethodToClassForReparse("MyClass.MyMethod");
			
			AddPublicMethod("MyMethod", "System.Object");
			
			MethodDeclaration method = fakeCodeGenerator.NodePassedToInsertCodeAtEnd as MethodDeclaration;
			Assert.AreEqual("Object", method.TypeReference.Type);
		}
		
		[Test]
		public void AddPublicMethod_MethodReturnTypeIsString_ReturnsCodeMethod()
		{
			CreateClass("MyClass");
			CreateCodeGenerator();
			string fileName = @"d:\projects\myproject\MyClass.cs";
			SetClassFileName(fileName);
			SetDocumentFileName(fileName);
			
			AddMethodToClassForReparse("MyClass.MyMethod", new DomRegion(1, 2, 1, 5), new DomRegion(1, 5, 3, 3));
			
			AddPublicMethod("MyMethod", "System.String");
			
			TextPoint start = (TextPoint)codeFunction.GetStartPoint();
			TextPoint end = (TextPoint)codeFunction.GetEndPoint();
			Assert.AreEqual("MyMethod", codeFunction.Name);
			Assert.AreEqual(1, start.Line);
			Assert.AreEqual(2, start.LineCharOffset);
			Assert.AreEqual(3, end.Line);
			Assert.AreEqual(3, end.LineCharOffset);
		}
		
		[Test]
		public void AddPublicMethod_ReparsedClassHasTwoMethods_LastMethodReturned()
		{
			CreateClass("MyClass");
			CreateCodeGenerator();
			string fileName = @"d:\projects\myproject\MyClass.cs";
			SetClassFileName(fileName);
			SetDocumentFileName(fileName);
			AddMethodsToClassForReparse("MyClass.First", "MyClass.MyMethod");
			
			AddPublicMethod("MyMethod", "System.String");
			
			Assert.AreEqual("MyMethod", codeFunction.Name);
		}
		
		[Test]
		public void AddPublicFunction_ReparsedCompilationUnitHasThreeClasses_MethodReturnedFromCorrectClass()
		{
			CreateClass("MyClass2");
			CreateCodeGenerator();
			string fileName = @"d:\projects\myproject\MyClass2.cs";
			SetClassFileName(fileName);
			SetDocumentFileName(fileName);
			ClassHelper class1 = CreateClassHelper("MyClass1");
			ClassHelper class2 = CreateClassHelper("MyClass2");
			ClassHelper class3 = CreateClassHelper("MyClass3");
			
			class2.AddMethodToClass("MyClass2.MyMethod");
			
			AddClassesToReparsedCompilationUnit(class1, class2, class3);
			
			AddPublicMethod("MyMethod", "System.String");
			
			Assert.AreEqual("MyMethod", codeFunction.Name);
		}
	}
}
