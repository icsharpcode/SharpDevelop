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
using System.Collections.Generic;
using System.Linq;

using System.Threading;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Workbench;
using NUnit.Framework;
using Rhino.Mocks;

namespace ICSharpCode.XamlBinding.Tests
{
	public class TextEditorBasedTests
	{
		protected MockTextEditor textEditor;
		
		protected static readonly IReadOnlyList<string> TaskListTokens = new List<string> {
			"TODO", "HACK", "FIXME"
		};
		
		[SetUp]
		public void SetupTest()
		{
			SD.InitializeForUnitTests();
			textEditor = new MockTextEditor();
		}
		
		[TearDown]
		public void TearDownTest()
		{
			SD.TearDownForUnitTests();
		}
		
		protected static readonly IUnresolvedAssembly Corlib = new CecilLoader().LoadAssemblyFile(typeof(object).Assembly.Location);
		protected static readonly IUnresolvedAssembly PresentationCore = new CecilLoader().LoadAssemblyFile(typeof(System.Windows.UIElement).Assembly.Location);
		protected static readonly IUnresolvedAssembly PresentationFramework = new CecilLoader().LoadAssemblyFile(typeof(System.Windows.Application).Assembly.Location);
		protected static readonly IUnresolvedAssembly SystemXaml = new CecilLoader().LoadAssemblyFile(typeof(System.Xaml.XamlException).Assembly.Location);
		
		void SetUpWithCode(string code, int offset)
		{
			textEditor.Document.Text = code;
			textEditor.Caret.Offset = offset;
			
			var parseInfo = textEditor.CreateParseInformation();
			IProject project = MockRepository.GenerateStrictMock<IProject>();
			var pc = new CSharpProjectContent().AddOrUpdateFiles(parseInfo.UnresolvedFile);
			pc = pc.AddAssemblyReferences(new[] { Corlib, PresentationCore, PresentationFramework, SystemXaml });
			var compilation = pc.CreateCompilation();
			SD.Services.AddService(typeof(IParserService), MockRepository.GenerateStrictMock<IParserService>());
			SD.ParserService.Stub(p => p.GetCachedParseInformation(textEditor.FileName)).Return(parseInfo);
			SD.ParserService.Stub(p => p.GetCompilation(project)).Return(compilation);
			SD.ParserService.Stub(p => p.GetCompilationForFile(textEditor.FileName)).Return(compilation);
			SD.ParserService.Stub(p => p.Parse(textEditor.FileName, textEditor.Document)).WhenCalled(
				i => {
					var p = new XamlParser();
					p.TaskListTokens = TaskListTokens;
					i.ReturnValue = p.Parse(textEditor.FileName, textEditor.Document, true, project, CancellationToken.None);
				}).Return(parseInfo); // fake Return to make it work
			SD.Services.AddService(typeof(IFileService), MockRepository.GenerateStrictMock<IFileService>());
			IViewContent view = MockRepository.GenerateStrictMock<IViewContent>();
			view.Stub(v => v.GetService(typeof(ITextEditor))).Return(textEditor);
			SD.FileService.Stub(f => f.OpenFile(textEditor.FileName, false)).Return(view);
		}
		
		protected void TestCtrlSpace(string fileHeader, string fileFooter, bool expected, Action<ICompletionItemList> constraint)
		{
			SetUpWithCode(fileHeader + fileFooter, fileHeader.Length);
			bool invoked = new XamlCodeCompletionBinding().CtrlSpace(textEditor);
			Assert.AreEqual(expected, invoked);
			ICompletionItemList list = textEditor.LastCompletionItemList;
			Assert.NotNull(list);
			constraint(list);
		}
		
		protected void TestKeyPress(string fileHeader, string fileFooter, char keyPressed, CodeCompletionKeyPressResult keyPressResult, Action<ICompletionItemList> constraint)
		{
			SetUpWithCode(fileHeader + fileFooter, fileHeader.Length);
			CodeCompletionKeyPressResult result = new XamlCodeCompletionBinding().HandleKeyPress(textEditor, keyPressed);
			
			Assert.AreEqual(keyPressResult, result);
			
			ICompletionItemList list = this.textEditor.LastCompletionItemList;
			
			constraint(list);
		}
		
		protected void TestTextInsert(string fileHeader, string fileFooter, char completionChar, ICompletionItemList list, ICompletionItem item, string expectedOutput, int expectedOffset)
		{
			textEditor.Document.Text = fileHeader + fileFooter;
			textEditor.Caret.Offset = fileHeader.Length;
			
			CompletionContext context = new CompletionContext() {
				Editor = this.textEditor,
				CompletionChar = completionChar,
				StartOffset = textEditor.Caret.Offset,
				EndOffset = textEditor.Caret.Offset
			};
			
			list.Complete(context, item);
			
			if (!context.CompletionCharHandled && context.CompletionChar != '\n')
				this.textEditor.Document.Insert(this.textEditor.Caret.Offset, completionChar + "");
			
			string insertedText = this.textEditor.Document.GetText(fileHeader.Length, this.textEditor.Document.TextLength - fileHeader.Length - fileFooter.Length);
			
			Assert.AreEqual(expectedOutput, insertedText);
			Assert.AreEqual(fileHeader.Length + expectedOffset, textEditor.Caret.Offset);
		}
	}
}
