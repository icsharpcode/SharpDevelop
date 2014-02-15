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

using System;
using System.Linq;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop;
using NUnit.Framework;
using Rhino.Mocks;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class EditPointTests : CodeModelTestBase
	{
		CodeVariable codeVariable;
		IField field;
		CodeFunction2 codeFunction;
		TextPoint endPoint;
		EditPoint editPoint;
		IDocument document;
		IDocumentView documentView;
		IDocumentLoader documentLoader;
		
		[SetUp]
		public void Init()
		{
			CreateDocumentLoader();
		}
		
		void CreateDocumentLoader()
		{
			document = MockRepository.GenerateStub<IDocument>();
			documentView = MockRepository.GenerateStub<IDocumentView>();
			documentView.Stub(view => view.Document).Return(document);
			documentLoader = MockRepository.GenerateStub<IDocumentLoader>();
		}
		
		void CreateField(DomRegion region)
		{
			AddCodeFile("class.cs", "class c {}");
			codeModelContext.DocumentLoader = documentLoader;
			
			field = MockRepository.GenerateStub<IField>();
			field.Stub(f => f.Region).Return(region);
			
			codeVariable = new CodeVariable(codeModelContext, field);
		}
		
		void CreateMethod(DomRegion region)
		{
			AddCodeFile("class.cs", "class c {}");
			codeModelContext.DocumentLoader = documentLoader;
			
			IMethod method = MockRepository.GenerateStub<IMethod>();
			method.Stub(m => m.Region).Return(region);
			
			codeFunction = new CodeFunction2(codeModelContext, method);
		}
		
		void DocumentOffsetToReturn(int line, int column, int offset)
		{
			document.Stub(d => d.PositionToOffset(line, column)).Return(offset);
		}
		
		void CreateFieldEditPoint()
		{
			var startPoint = (TextPoint)codeVariable.GetStartPoint();
			endPoint = (TextPoint)codeVariable.GetEndPoint();
			editPoint = (EditPoint)startPoint.CreateEditPoint();
		}
		
		void CreateMethodEditPoint()
		{
			var startPoint = (TextPoint)codeFunction.GetStartPoint();
			endPoint = (TextPoint)codeFunction.GetEndPoint();
			editPoint = (EditPoint)startPoint.CreateEditPoint();
		}
		
		void ReplaceText(string text)
		{
			editPoint.ReplaceText(endPoint, text, (int)global::EnvDTE.vsEPReplaceTextOptions.vsEPReplaceTextAutoformat);
		}
		
		void DocumentFileName(string fileName)
		{
			documentLoader.Stub(loader => loader.LoadDocumentView(fileName)).Return(documentView);
		}
		
		void AssertDocumentViewIndentLinesWasNotCalled()
		{
			documentView.AssertWasNotCalled(view => view.IndentLines(Arg<int>.Is.Anything, Arg<int>.Is.Anything));
		}
		
		[Test]
		public void ReplaceText_EditPointCreatedFromFieldStartPoint_ReplacesTextBetweenStartAndEndPoint()
		{
			string fileName = @"d:\projects\test.cs";
			var fieldRegion = new DomRegion(fileName, 1, 5, 3, 12);
			CreateField(fieldRegion);
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
			var methodRegion = new DomRegion(fileName, 1, 5, 3, 12);
			CreateMethod(methodRegion);
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
			var fieldRegion = new DomRegion(fileName, 1, 5, 1, 10);
			CreateField(fieldRegion);
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
			var fieldRegion = new DomRegion(fileName, 1, 5, 1, 10);
			CreateField(fieldRegion);
			DocumentOffsetToReturn(line: 1, column: 5, offset: 5);
			DocumentOffsetToReturn(line: 1, column: 12, offset: 10);
			DocumentFileName(fileName);
			CreateFieldEditPoint();
			
			ReplaceText("Test");
			
			AssertDocumentViewIndentLinesWasNotCalled();
		}
	}
}
