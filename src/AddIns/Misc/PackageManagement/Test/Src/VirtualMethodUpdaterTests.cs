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
//	public class VirtualMethodUpdaterTests
//	{
//		VirtualMethodUpdater updater;
//		MethodHelper methodHelper;
//		IRefactoringDocument document;
//		IDocumentLoader documentLoader;
//		
//		[SetUp]
//		public void Init()
//		{
//			methodHelper = new MethodHelper();
//			document = MockRepository.GenerateStub<IRefactoringDocument>();
//			documentLoader = MockRepository.GenerateStub<IDocumentLoader>();
//		}
//		
//		void CreatePublicCSharpFunction()
//		{
//			methodHelper.CreatePublicMethod("MyMethod");
//			methodHelper.ProjectContentHelper.ProjectContentIsForCSharpProject();
//		}
//		
//		void CreatePublicVisualBasicFunction()
//		{
//			methodHelper.CreatePublicMethod("MyMethod");
//			methodHelper.ProjectContentHelper.ProjectContentIsForVisualBasicProject();
//		}
//		
//		void SetDocumentFileName(string fileName)
//		{
//			documentLoader.Stub(loader => loader.LoadRefactoringDocument(fileName)).Return(document);
//		}
//		
//		void CreateVirtualMethodUpdater()
//		{
//			updater = new VirtualMethodUpdater(methodHelper.Method, documentLoader);
//		}
//		
//		void SetFileNameForMethod(string fileName)
//		{
//			methodHelper.SetCompilationUnitFileName(fileName);
//			SetDocumentFileName(fileName);
//		}
//		
//		void SetMethodDeclarationLineWithOffset(int line, string text, int offset)
//		{
//			methodHelper.FunctionStartsAtLine(line);
//			SetDocumentLineText(line, text, offset);
//		}
//		
//		void SetMethodDeclarationLine(int line, string text)
//		{
//			SetMethodDeclarationLineWithOffset(line, text, 0);
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
//		public void MakeMethodVirtual_PublicCSharpClassWithNoOtherModifiers_AddsVirtualKeywordToMethodDefinition()
//		{
//			CreatePublicCSharpFunction();
//			CreateVirtualMethodUpdater();
//			SetFileNameForMethod(@"d:\projects\MyProject\MyClass.cs");
//			SetMethodDeclarationLine(1, "public void MyMethod()");
//			
//			updater.MakeMethodVirtual();
//			
//			document.AssertWasCalled(doc => doc.Insert(7, "virtual "));
//		}
//		
//		[Test]
//		public void MakeMethodVirtual_MethodAlreadyVirtual_MethodDefinitionIsNotChanged()
//		{
//			CreatePublicCSharpFunction();
//			CreateVirtualMethodUpdater();
//			methodHelper.MakeMethodVirtual();
//			SetFileNameForMethod(@"d:\projects\MyProject\MyClass.cs");
//			SetMethodDeclarationLine(1, "public void MyMethod()");
//			
//			updater.MakeMethodVirtual();
//			
//			document.AssertWasNotCalled(doc => doc.Insert(Arg<int>.Is.Anything, Arg<string>.Is.Anything));
//		}
//		
//		[Test]
//		public void MakeMethodVirtual_PublicVisualBasicClassWithNoOtherModifiers_AddsOverridableKeywordToMethodDefinition()
//		{
//			CreatePublicVisualBasicFunction();
//			CreateVirtualMethodUpdater();
//			SetFileNameForMethod(@"d:\projects\MyProject\MyClass.vb");
//			SetMethodDeclarationLine(1, "Public Sub MyMethod");
//			
//			updater.MakeMethodVirtual();
//			
//			document.AssertWasCalled(doc => doc.Insert(7, "Overridable "));
//		}
//		
//		[Test]
//		public void MakeMethodVirtual_NoPublicKeywordInClassDeclarationLine_ExceptionIsThrown()
//		{
//			CreatePublicCSharpFunction();
//			CreateVirtualMethodUpdater();
//			SetFileNameForMethod(@"d:\projects\MyProject\MyClass.cs");
//			SetMethodDeclarationLine(1, "void test()");
//			
//			Assert.Throws<ApplicationException>(() => updater.MakeMethodVirtual());
//		}
//		
//		[Test]
//		public void MakeMethodVirtual_NoPublicKeywordButMethodNameIncludesPublicKeyword_ExceptionIsThrown()
//		{
//			CreatePublicCSharpFunction();
//			CreateVirtualMethodUpdater();
//			SetFileNameForMethod(@"d:\projects\MyProject\MyClass.cs");
//			SetMethodDeclarationLine(1, "void publicmethod()");
//			
//			Assert.Throws<ApplicationException>(() => updater.MakeMethodVirtual());
//		}
//		
//		[Test]
//		public void MakeMethodVirtual_PublicCSharpMethodNotOnFirstLine_AddsVirtualKeywordToMethodDefinitionAtCorrectOffset()
//		{
//			CreatePublicCSharpFunction();
//			CreateVirtualMethodUpdater();
//			SetFileNameForMethod(@"d:\projects\MyProject\MyClass.cs");
//			SetMethodDeclarationLineWithOffset(1, "public void MyMethod()", offset: 10);
//			
//			updater.MakeMethodVirtual();
//			
//			document.AssertWasCalled(doc => doc.Insert(17, "virtual "));
//		}
//	}
//}
