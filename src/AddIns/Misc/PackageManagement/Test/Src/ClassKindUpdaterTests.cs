// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

//using System;
//using ICSharpCode.PackageManagement;
//using ICSharpCode.SharpDevelop.Dom.Refactoring;
//using NUnit.Framework;
//using PackageManagement.Tests.Helpers;
//using Rhino.Mocks;
//
//namespace PackageManagement.Tests
//{
//	[TestFixture]
//	public class ClassKindUpdaterTests
//	{
//		ClassKindUpdater updater;
//		ClassHelper classHelper;
//		IRefactoringDocument document;
//		IDocumentLoader documentLoader;
//		
//		[SetUp]
//		public void Init()
//		{
//			classHelper = new ClassHelper();
//			document = MockRepository.GenerateStub<IRefactoringDocument>();
//			documentLoader = MockRepository.GenerateStub<IDocumentLoader>();
//		}
//		
//		void CreatePublicCSharpClass()
//		{
//			classHelper.CreatePublicClass("MyClass");
//			classHelper.ProjectContentHelper.ProjectContentIsForCSharpProject();
//		}
//		
//		void CreatePublicVisualBasicClass()
//		{
//			classHelper.CreatePublicClass("MyClass");
//			classHelper.ProjectContentHelper.ProjectContentIsForVisualBasicProject();
//		}
//		
//		void SetDocumentFileName(string fileName)
//		{
//			documentLoader.Stub(loader => loader.LoadRefactoringDocument(fileName)).Return(document);
//		}
//		
//		void CreateClassKindUpdater()
//		{
//			updater = new ClassKindUpdater(classHelper.Class, documentLoader);
//		}
//		
//		void SetClassFileName(string fileName)
//		{
//			classHelper.SetClassFileName(fileName);
//			SetDocumentFileName(fileName);
//		}
//		
//		void SetClassDeclarationLineWithOffset(int line, string text, int offset)
//		{
//			classHelper.SetRegionBeginLine(line);
//			SetDocumentLineText(line, text, offset);
//		}
//		
//		void SetClassDeclarationLine(int line, string text)
//		{
//			SetClassDeclarationLineWithOffset(line, text, 0);
//		}
//		
//		void SetDocumentLineText(int lineNumber, string text, int offset)
//		{
//			IRefactoringDocumentLine documentLine = MockRepository.GenerateStub<IRefactoringDocumentLine>();
//			documentLine.Stub(line => line.Text).Return(text);
//			documentLine.Stub(line => line.Offset).Return(offset);
//			document.Stub(doc => doc.GetLine(lineNumber)).Return(documentLine);
//		}
//		
//		[Test]
//		public void MakeClassPartial_PublicCSharpClassWithNoOtherModifiers_OpensFileContainingClassInSharpDevelop()
//		{
//			CreatePublicCSharpClass();
//			CreateClassKindUpdater();
//			string fileName = @"d:\projects\MyProject\MyClass.cs";
//			SetClassFileName(fileName);
//			SetClassDeclarationLine(1, "public class MyClass");
//			
//			updater.MakeClassPartial();
//			
//			documentLoader.AssertWasCalled(loader => loader.LoadRefactoringDocument(fileName));
//		}
//		
//		[Test]
//		public void MakeClassPartial_PublicCSharpClassWithNoOtherModifiers_AddsPartialKeywordToClassDefinition()
//		{
//			CreatePublicCSharpClass();
//			CreateClassKindUpdater();
//			SetClassFileName(@"d:\projects\MyProject\MyClass.cs");
//			SetClassDeclarationLine(1, "public class MyClass");
//			
//			updater.MakeClassPartial();
//			
//			document.AssertWasCalled(doc => doc.Insert(7, "partial "));
//		}
//		
//		[Test]
//		public void MakeClassPartial_PublicCSharpClassThatIsAlreadyPartial_ClassDefinitionIsUnchanged()
//		{
//			CreatePublicCSharpClass();
//			CreateClassKindUpdater();
//			classHelper.MakeClassPartial();
//			SetClassFileName(@"d:\projects\MyProject\MyClass.cs");
//			SetClassDeclarationLine(1, "public class MyClass");
//			
//			updater.MakeClassPartial();
//			
//			document.AssertWasNotCalled(doc => doc.Insert(Arg<int>.Is.Anything, Arg<string>.Is.Anything));
//		}
//		
//		[Test]
//		public void MakeClassPartial_PublicVisualBasicClassWithNoOtherModifiers_AddsVisualBasicPartialKeywordToClassDefinition()
//		{
//			CreatePublicVisualBasicClass();
//			CreateClassKindUpdater();
//			SetClassFileName(@"d:\projects\MyProject\MyClass.vb");
//			SetClassDeclarationLine(1, "Public Class MyClass");
//			
//			updater.MakeClassPartial();
//			
//			document.AssertWasCalled(doc => doc.Insert(7, "Partial "));
//		}
//		
//		[Test]
//		public void MakeClassPartial_NoClassKeywordInClassDeclarationLine_ExceptionIsThrown()
//		{
//			CreatePublicCSharpClass();
//			CreateClassKindUpdater();
//			SetClassFileName(@"d:\projects\MyProject\test.cs");
//			SetClassDeclarationLine(1, "public test");
//			
//			Assert.Throws<ApplicationException>(() => updater.MakeClassPartial());
//		}
//		
//		[Test]
//		public void MakeClassPartial_NoClassKeywordButClassNameIncludesClassKeyword_ExceptionIsThrown()
//		{
//			CreatePublicCSharpClass();
//			CreateClassKindUpdater();
//			SetClassFileName(@"d:\projects\MyProject\MyClass.cs");
//			SetClassDeclarationLine(1, "public MyClass");
//			
//			Assert.Throws<ApplicationException>(() => updater.MakeClassPartial());
//		}
//		
//		[Test]
//		public void MakeClassPartial_PublicCSharpClassNotOnFirstLine_AddsPartialKeywordToClassDefinitionAtCorrectOffset()
//		{
//			CreatePublicCSharpClass();
//			CreateClassKindUpdater();
//			SetClassFileName(@"d:\projects\MyProject\MyClass.cs");
//			SetClassDeclarationLineWithOffset(1, "public class MyClass", offset: 10);
//			
//			updater.MakeClassPartial();
//			
//			document.AssertWasCalled(doc => doc.Insert(17, "partial "));
//		}
//	}
//}
