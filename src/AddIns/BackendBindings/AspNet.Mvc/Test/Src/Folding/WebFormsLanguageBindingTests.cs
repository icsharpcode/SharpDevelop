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
using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.SharpDevelop.Editor;
using NUnit.Framework;
using Rhino.Mocks;

namespace AspNet.Mvc.Tests.Folding
{
	[TestFixture]
	public class WebFormsLanguageBindingTests
	{
		ITextEditorWithParseInformationFolding fakeTextEditorWithParseInformationFolding;
		WebFormsTextEditorExtension editorExtension;
		ITextEditor fakeTextEditor;
		ITextEditorWithParseInformationFoldingFactory fakeTextEditorFactory;
		IFoldGeneratorFactory fakeFoldGeneratorFactory;
		IFoldGenerator fakeFoldGenerator;
		
		[SetUp]
		public void Init()
		{
			CreateFakeTextEditor();
			CreateFakeTextEditorWithParseInfo();
			CreateFakeTextEditorWithParseInfoFoldingFactory();
			AddFakeTextEditorWithParseInfoFoldingToFactory();
			CreateFakeFoldGeneratorFactory();
			AddFakeFoldGeneratorToFactory();
			CreateLanguageBinding();
		}
		
		void CreateLanguageBinding()
		{
			editorExtension = new WebFormsTextEditorExtension(fakeTextEditorFactory, fakeFoldGeneratorFactory);
		}
		
		void CreateFakeFoldGeneratorFactory()
		{
			fakeFoldGeneratorFactory = MockRepository.GenerateStub<IFoldGeneratorFactory>();
		}
		
		void AddFakeFoldGeneratorToFactory()
		{
			fakeFoldGenerator = MockRepository.GenerateStub<IFoldGenerator>();
			fakeFoldGeneratorFactory.Stub(factory => factory.CreateFoldGenerator(fakeTextEditorWithParseInformationFolding))
				.Return(fakeFoldGenerator);
		}
		
		void CreateFakeTextEditorWithParseInfo()
		{
			fakeTextEditorWithParseInformationFolding = MockRepository.GenerateStub<ITextEditorWithParseInformationFolding>();
		}
		
		void CreateFakeTextEditor()
		{
			fakeTextEditor = MockRepository.GenerateStub<ITextEditor>();
		}
		
		void CreateFakeTextEditorWithParseInfoFoldingFactory()
		{
			fakeTextEditorFactory = MockRepository.GenerateStub<ITextEditorWithParseInformationFoldingFactory>();
		}
		
		void AddFakeTextEditorWithParseInfoFoldingToFactory()
		{
			fakeTextEditorFactory.Stub(factory => factory.CreateTextEditor(fakeTextEditor))
				.Return(fakeTextEditorWithParseInformationFolding);
		}
		
		[Test]
		public void Attach_TextEditor_FoldGeneratorCreated()
		{
			editorExtension.Attach(fakeTextEditor);
			
			fakeFoldGeneratorFactory.AssertWasCalled(
				factory => factory.CreateFoldGenerator(fakeTextEditorWithParseInformationFolding));
		}
		
		[Test]
		public void Detach_TextEditor_FoldGeneratorDisposed()
		{
			editorExtension.Attach(fakeTextEditor);
			editorExtension.Detach();
			
			fakeFoldGenerator.AssertWasCalled(generator => generator.Dispose());
		}
	}
}
