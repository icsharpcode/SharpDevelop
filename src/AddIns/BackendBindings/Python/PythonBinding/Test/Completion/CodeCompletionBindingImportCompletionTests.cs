// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
