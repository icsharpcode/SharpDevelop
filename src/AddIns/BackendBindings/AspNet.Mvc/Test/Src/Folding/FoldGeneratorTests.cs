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
using ICSharpCode.AspNet.Mvc.Folding;
using ICSharpCode.AvalonEdit.Folding;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace AspNet.Mvc.Tests.Folding
{
	[TestFixture]
	public class FoldGeneratorTests
	{
		FoldGenerator foldGenerator;
		ITextEditorWithParseInformationFolding fakeTextEditor;
		IFoldParser fakeFoldParser;
		
		void CreateFakeTextEditor()
		{
			fakeTextEditor = MockRepository.GenerateStub<ITextEditorWithParseInformationFolding>();
		}
		
		void CreateFoldGenerator()
		{
			fakeFoldParser = MockRepository.GenerateStub<IFoldParser>();
			foldGenerator = new FoldGenerator(fakeTextEditor, fakeFoldParser);
		}
		
		void SetFoldsToReturnFromFoldParserForHtml(NewFolding[] expectedFolds, string html)
		{
			fakeFoldParser.Stub(parser => parser.GetFolds(html))
				.Return(expectedFolds);
		}
		
		void SetTextInTextEditor(string text)
		{
			fakeTextEditor.Stub(textEditor => textEditor.GetTextSnapshot())
				.Return(text);
		}
		
		void SetExceptionToThrowFromFoldParserGetFoldsMethod(Exception ex)
		{
			fakeFoldParser.Stub(parser => parser.GetFolds(null))
				.IgnoreArguments()
				.Throw(ex);
		}
		
		void AssertTextEditorUpdateFoldsWasNotCalled()
		{
			fakeTextEditor.AssertWasNotCalled(
				textEditor => textEditor.UpdateFolds(null), 
				textEditor => textEditor.IgnoreArguments());
		}
		
		[Test]
		public void Constructor_TextEditor_ParseInformationIsDisabled()
		{
			CreateFakeTextEditor();
			fakeTextEditor.IsParseInformationFoldingEnabled = true;
			CreateFoldGenerator();
			
			Assert.IsFalse(fakeTextEditor.IsParseInformationFoldingEnabled);
		}
		
		[Test]
		public void Dispose_TextEditor_ParseInformationIsEnabled()
		{
			CreateFakeTextEditor();
			CreateFoldGenerator();
			fakeTextEditor.IsParseInformationFoldingEnabled = false;
			
			foldGenerator.Dispose();
			
			Assert.IsTrue(fakeTextEditor.IsParseInformationFoldingEnabled);
		}
		
		[Test]
		public void Constructor_TextEditor_InstallFoldingManagerIsInstalled()
		{
			CreateFakeTextEditor();
			CreateFoldGenerator();
			
			fakeTextEditor.AssertWasCalled(textEditor => textEditor.InstallFoldingManager());
		}
		
		[Test]
		public void Constructor_TextEditor_InstallFoldingManagerIsInstalledAfterParseInfoIsDisabled()
		{
			CreateFakeTextEditor();
			fakeTextEditor.IsParseInformationFoldingEnabled = true;
			bool parseInfoEnabled = true;
			fakeTextEditor
				.Stub(t => t.InstallFoldingManager())
				.Do((Action)delegate { parseInfoEnabled = fakeTextEditor.IsParseInformationFoldingEnabled; });
			CreateFoldGenerator();
			
			Assert.IsFalse(parseInfoEnabled);
		}
		
		[Test]
		public void GenerateFolds_MethodCalled_FoldsFromFoldParserUsedToUpdateTextEditor()
		{
			CreateFakeTextEditor();
			CreateFoldGenerator();
			string html = "<p></p>";
			SetTextInTextEditor(html);
			var expectedFolds = new NewFolding[] {
				new NewFolding()
			};
			SetFoldsToReturnFromFoldParserForHtml(expectedFolds, html);
			
			foldGenerator.GenerateFolds();
			
			fakeTextEditor.AssertWasCalled(textEditor => textEditor.UpdateFolds(expectedFolds));
		}
		
		[Test]
		public void GenerateFolds_FoldParserThrowsException_FoldsNotUpdated()
		{
			CreateFakeTextEditor();
			CreateFoldGenerator();
			string html = "<p></p>";
			SetTextInTextEditor(html);
			var expectedException = new ApplicationException("Test");
			SetExceptionToThrowFromFoldParserGetFoldsMethod(expectedException);
			
			foldGenerator.GenerateFolds();
			
			AssertTextEditorUpdateFoldsWasNotCalled();
		}
		
		[Test]
		public void Dispose_TextEditor_TextEditorWithFoldingIsDisposed()
		{
			CreateFakeTextEditor();
			CreateFoldGenerator();
			
			foldGenerator.Dispose();
			
			fakeTextEditor.AssertWasCalled(t => t.Dispose());
		}
	}
}
