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
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Completion
{
	/// <summary>
	/// Tests the code completion after an "import" statement.
	/// </summary>
	[TestFixture]
	public class CodeCompletionBindingImportCompletionTests
	{
		TestablePythonCodeCompletionBinding codeCompletionBinding;
		MockTextEditor textEditor;
		bool handled;
		
		public void CreateCompletionBinding()
		{
			textEditor = new MockTextEditor();
			codeCompletionBinding = new TestablePythonCodeCompletionBinding();
		}
		
		public void HandlesImportKeyword()
		{
			CreateCompletionBinding();
			handled = codeCompletionBinding.HandleKeyword(textEditor, "import");
		}
		
		[Test]
		public void HandleKeyword_KeywordIsImport_ReturnsTrue()
		{
			HandlesImportKeyword();
			Assert.IsTrue(handled);
		}
		
		[Test]
		public void HandleKeyword_UnknownKeyword_ReturnsFalse()
		{
			bool handled = codeCompletionBinding.HandleKeyword(textEditor, "Unknown");
			Assert.IsFalse(handled);
		}
		
		[Test]
		public void HandleKeyword_KeywordIsImportInUpperCase_ReturnsTrue()
		{
			bool handled = codeCompletionBinding.HandleKeyword(textEditor, "IMPORT");
			Assert.IsTrue(handled);
		}
		
		[Test]
		public void HandleKeyword_KeywordIsNull_ReturnsFalse()
		{
			bool handled = codeCompletionBinding.HandleKeyword(textEditor, null);
			Assert.IsFalse(handled);
		}
		
		[Test]
		public void HandleKeyword_KeywordIsImport_CodeCompletionWindowDisplayed()
		{
			HandlesImportKeyword();
			Assert.IsTrue(codeCompletionBinding.IsCodeCompletionWindowDisplayed);
		}
		
		[Test]
		public void HandleKeyword_KeywordIsImport_TextEditorPassedToShowCompletionWindowMethod()
		{
			HandlesImportKeyword();
			Assert.AreSame(textEditor, codeCompletionBinding.TextEditorPassedToShowCompletionWindow);
		}
		
		[Test]
		public void HandleKeyword_KeywordIsImport_CompletionProviderUsedWhenDisplayingCodeCompletionWindow()
		{
			HandlesImportKeyword();
			Assert.AreSame(codeCompletionBinding.KeywordCompletionItemProviderCreated, codeCompletionBinding.CompletionItemProviderUsedWhenDisplayingCodeCompletionWindow);
		}
		
		[Test]
		public void HandleKeyword_KeywordIsImport_KeywordCompletionDataProviderIsCodeCompletionItemProvider()
		{
			HandlesImportKeyword();
			CodeCompletionItemProvider provider = codeCompletionBinding.KeywordCompletionItemProviderCreated as CodeCompletionItemProvider;
			Assert.IsNotNull(provider);
		}
		
		[Test]
		public void ShowCompletionWindow_FakeCompletionItemProviderAndTextEditorPassed_asdf()
		{
			CreateCompletionBinding();
			FakeCompletionItemProvider provider = new FakeCompletionItemProvider();
			codeCompletionBinding.CallBaseShowCodeCompletionWindow(provider, textEditor);
			
			Assert.AreEqual(textEditor, provider.TextEditorPassedToShowCompletion);
		}
	}
}
