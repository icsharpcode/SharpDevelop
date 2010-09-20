// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Completion
{
	/// <summary>
	/// Tests that the From keyword is correctly identified as a 
	/// importable code completion keyword.
	/// </summary>
	[TestFixture]
	public class CodeCompletionBindingFromImportCompletionTests
	{
		MockTextEditor fakeTextEditor;
		TestablePythonCodeCompletionBinding codeCompletionBinding;
		
		void CreatePythonCodeCompletionBinding()
		{
			fakeTextEditor = new MockTextEditor();
			codeCompletionBinding = new TestablePythonCodeCompletionBinding();			
		}
		
		[Test]
		public void HandleKeyword_KeywordIsFrom_ReturnsTrue()
		{
			CreatePythonCodeCompletionBinding();
			bool handled = codeCompletionBinding.HandleKeyword(fakeTextEditor, "from");
			Assert.IsTrue(handled);
		}
		
		[Test]
		public void HandleKeyword_KeywordIsFrom_PythonDotCodeCompletionItemProviderUsedToShowCompletionWindow()
		{
			CreatePythonCodeCompletionBinding();
			codeCompletionBinding.HandleKeyword(fakeTextEditor, "from");
			ITextEditor textEditor = codeCompletionBinding.TextEditorPassedToShowCompletionWindow;
			
			Assert.AreEqual(fakeTextEditor, textEditor);
		}
		
		[Test]
		public void HandleKeyword_KeywordIsFrom_PythonCodeCompletionItemProviderCreated()
		{
			CreatePythonCodeCompletionBinding();
			codeCompletionBinding.HandleKeyword(fakeTextEditor, "from");
			
			PythonCodeCompletionItemProvider provider = codeCompletionBinding.KeywordCompletionItemProviderCreated as PythonCodeCompletionItemProvider;
			
			Assert.IsNotNull(provider);
		}
		
		[Test]
		public void HandleKeyword_KeywordIsFrom_PythonCodeCompletionItemProviderPassedToShowCompletionWindow()
		{
			CreatePythonCodeCompletionBinding();
			codeCompletionBinding.HandleKeyword(fakeTextEditor, "from");
			
			AbstractCompletionItemProvider provider = codeCompletionBinding.CompletionItemProviderUsedWhenDisplayingCodeCompletionWindow;
			
			Assert.AreSame(codeCompletionBinding.KeywordCompletionItemProviderCreated, provider);
		}
	}
}
