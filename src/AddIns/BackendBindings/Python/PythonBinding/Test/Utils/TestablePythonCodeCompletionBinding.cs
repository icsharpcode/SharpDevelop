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

using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;

namespace PythonBinding.Tests.Utils
{
	/// <summary>
	/// Derived PythonCodeCompletion class that gives us access to 
	/// various protected methods for testing.
	/// </summary>
	public class TestablePythonCodeCompletionBinding : PythonCodeCompletionBinding
	{
		public bool IsCodeCompletionWindowDisplayed;
		public ITextEditor TextEditorPassedToShowCompletionWindow;
		public AbstractCompletionItemProvider CompletionItemProviderUsedWhenDisplayingCodeCompletionWindow;
		public AbstractCompletionItemProvider KeywordCompletionItemProviderCreated;
		
		/// <summary>
		/// Overrides the completion data provider creation to make sure
		/// it is called at the correct time. 
		/// </summary>
		protected override AbstractCompletionItemProvider CreateKeywordCompletionItemProvider()
		{
			KeywordCompletionItemProviderCreated = base.CreateKeywordCompletionItemProvider();
			return KeywordCompletionItemProviderCreated;
		}
		
		public void CallBaseShowCodeCompletionWindow(AbstractCompletionItemProvider completionItemProvider, ITextEditor textEditor)
		{
			base.ShowCodeCompletionWindow(completionItemProvider, textEditor);
		}
		
		/// <summary>
		/// Overrides the base class method so a code completion window is
		/// not displayed but the fact that this method is called is
		/// recorded. 
		/// </summary>
		protected override void ShowCodeCompletionWindow(AbstractCompletionItemProvider completionItemProvider, ITextEditor textEditor)
		{
			TextEditorPassedToShowCompletionWindow = textEditor;
			IsCodeCompletionWindowDisplayed = true;
			CompletionItemProviderUsedWhenDisplayingCodeCompletionWindow = completionItemProvider;
		}
		
		public PythonInsightWindowHandler PythonInsightWindowHandler {
			get { return base.insightHandler as PythonInsightWindowHandler; }
		}
	}
}
