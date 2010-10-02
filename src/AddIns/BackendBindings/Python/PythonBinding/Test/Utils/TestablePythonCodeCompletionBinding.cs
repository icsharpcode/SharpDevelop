// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
