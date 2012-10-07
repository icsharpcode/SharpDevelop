// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class EditPointTests
	{
		FieldHelper fieldHelper;
		MethodHelper methodHelper;
		TextPoint endPoint;
		EditPoint editPoint;
		IRefactoringDocument document;
		IRefactoringDocumentView documentView;
		IDocumentLoader documentLoader;
		
		[SetUp]
		public void Init()
		{
			fieldHelper = new FieldHelper();
			methodHelper = new MethodHelper();
			CreateDocumentLoader();
		}
		
		void CreateDocumentLoader()
		{
			document = MockRepository.GenerateStub<IRefactoringDocument>();
			documentView = MockRepository.GenerateStub<IRefactoringDocumentView>();
			documentView.Stub(view => view.RefactoringDocument).Return(document);
			documentLoader = MockRepository.GenerateStub<IDocumentLoader>();
		}

		void CreateField(string fileName, DomRegion region)
		{
			fieldHelper.CreateField("Class1.MyField");
			fieldHelper.SetRegion(region);
			fieldHelper.SetCompilationUnitFileName(fileName);
		}
		
		void CreateMethod(string fileName, DomRegion region, DomRegion bodyRegion)
		{
			methodHelper.CreateMethod("Class1.MyMethod");
			methodHelper.SetRegion(region);
			methodHelper.SetBodyRegion(bodyRegion);
			methodHelper.SetCompilationUnitFileName(fileName);
		}
		
		void DocumentOffsetToReturn(int line, int column, int offset)
		{
			document.Stub(d => d.PositionToOffset(line, column)).Return(offset);
		}
		
		void CreateFieldEditPoint()
		{
			var codeVariable = new CodeVariable(fieldHelper.Field, documentLoader);
			TextPoint startPoint = (TextPoint)codeVariable.GetStartPoint();
			endPoint = (TextPoint)codeVariable.GetEndPoint();
			editPoint = (EditPoint)startPoint.CreateEditPoint();
		}
		
		void CreateMethodEditPoint()
		{
			var codeFunction = new CodeFunction(methodHelper.Method, documentLoader, null);
			TextPoint startPoint = (TextPoint)codeFunction.GetStartPoint();
			endPoint = (TextPoint)codeFunction.GetEndPoint();
			editPoint = (EditPoint)startPoint.CreateEditPoint();
		}
		
		void ReplaceText(string text)
		{
			editPoint.ReplaceText(endPoint, text, (int)global::EnvDTE.vsEPReplaceTextOptions.vsEPReplaceTextAutoformat);
		}
		
		void DocumentFileName(string fileName)
		{
			documentLoader.Stub(loader => loader.LoadRefactoringDocumentView(fileName)).Return(documentView);
		}
		
		void AssertDocumentViewIndentLinesWasNotCalled()
		{
			documentView.AssertWasNotCalled(view => view.IndentLines(Arg<int>.Is.Anything, Arg<int>.Is.Anything));
		}
		
		[Test]
		public void ReplaceText_EditPointCreatedFromFieldStartPoint_ReplacesTextBetweenStartAndEndPoint()
		{
			string fileName = @"d:\projects\test.cs";
			var fieldRegion = new DomRegion(1, 5, 3, 12);
			CreateField(fileName, fieldRegion);
			DocumentOffsetToReturn(line: 1, column: 5, offset: 5);
			DocumentOffsetToReturn(line: 3, column: 12, offset: 20);
			DocumentFileName(fileName);
			CreateFieldEditPoint();
			
			ReplaceText("Test");
			
			document.AssertWasCalled(d => d.Replace(5, 15, "Test"));
		}
		
		[Test]
		public void ReplaceText_EditPointCreatedFromMethodStartPoint_ReplacesTextBetweenStartAndEndPoint()
		{
			string fileName = @"d:\projects\test.cs";
			var methodRegion = new DomRegion(1, 5, 1, 10);
			var methodBodyRegion = new DomRegion(1, 10, 3, 12);
			CreateMethod(fileName, methodRegion, methodBodyRegion);
			methodHelper.AddDeclaringType("MyClass");
			DocumentOffsetToReturn(line: 1, column: 5, offset: 5);
			DocumentOffsetToReturn(line: 3, column: 12, offset: 20);
			DocumentFileName(fileName);
			CreateMethodEditPoint();
			
			ReplaceText("Test");
			
			document.AssertWasCalled(d => d.Replace(5, 15, "Test"));
		}
		
		[Test]
		public void ReplaceText_EditPointCreatedFromFieldStartPointAndTextIsFourLines_IndentsLinesTwoThreeFourFiveAndSix()
		{
			string fileName = @"d:\projects\test.cs";
			var fieldRegion = new DomRegion(1, 5, 1, 10);
			CreateField(fileName, fieldRegion);
			DocumentOffsetToReturn(line: 1, column: 5, offset: 5);
			DocumentOffsetToReturn(line: 1, column: 12, offset: 10);
			DocumentFileName(fileName);
			CreateFieldEditPoint();
			
			string replacementText = 
				"First\r\n" +
				"Second\r\n" +
				"Third\r\n" +
				"Fourth\r\n" +
				"Five";
			
			ReplaceText(replacementText);
			
			documentView.AssertWasCalled(view => view.IndentLines(2, 6));
		}
		
		[Test]
		public void ReplaceText_EditPointCreatedFromFieldStartPointAndTextIsSingleLine_TextIsNotIndented()
		{
			string fileName = @"d:\projects\test.cs";
			var fieldRegion = new DomRegion(1, 5, 1, 10);
			CreateField(fileName, fieldRegion);
			DocumentOffsetToReturn(line: 1, column: 5, offset: 5);
			DocumentOffsetToReturn(line: 1, column: 12, offset: 10);
			DocumentFileName(fileName);
			CreateFieldEditPoint();
			
			ReplaceText("Test");
			
			AssertDocumentViewIndentLinesWasNotCalled();
		}
	}
}
