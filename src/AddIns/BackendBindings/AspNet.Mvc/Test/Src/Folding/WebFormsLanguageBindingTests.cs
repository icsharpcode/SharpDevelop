// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		WebFormsLanguageBinding languageBinding;
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
			languageBinding = new WebFormsLanguageBinding(fakeTextEditorFactory, fakeFoldGeneratorFactory);
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
			languageBinding.Attach(fakeTextEditor);
			
			fakeFoldGeneratorFactory.AssertWasCalled(
				factory => factory.CreateFoldGenerator(fakeTextEditorWithParseInformationFolding));
		}
		
		[Test]
		public void Detach_TextEditor_FoldGeneratorDisposed()
		{
			languageBinding.Attach(fakeTextEditor);
			languageBinding.Detach();
			
			fakeFoldGenerator.AssertWasCalled(generator => generator.Dispose());
		}
	}
}
