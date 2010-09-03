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
	public class DerivedPythonCodeCompletionBinding : PythonCodeCompletionBinding
	{
		bool completionDataProviderCreated;
		bool codeCompletionWindowDisplayed;
		ICSharpCode.SharpDevelop.Editor.ITextEditor textEditorUsedToShowCompletionWindow;
		AbstractCompletionItemProvider completionProviderUsedWhenDisplayingCodeCompletionWindow;
		AbstractCompletionItemProvider completionDataProvider;
		char completionCharacter = '\0';
		
		public DerivedPythonCodeCompletionBinding()
		{
		}
		
		/// <summary>
		/// Gets whether the data provider was created by the 
		/// base class PythonCodeCompletionBinding.
		/// </summary>
		public bool IsCompletionDataProviderCreated {
			get { return completionDataProviderCreated; }
		}
		
		/// <summary>
		/// Gets whether the base class PythonCodeCompletionBinding
		/// displayed the code completion window.
		/// </summary>
		public bool IsCodeCompletionWindowDisplayed {
			get { return codeCompletionWindowDisplayed; }
		}
		
		public ICSharpCode.SharpDevelop.Editor.ITextEditor TextEditorUsedToShowCompletionWindow {
			get { return textEditorUsedToShowCompletionWindow; }
		}
		
		public AbstractCompletionItemProvider CompletionProviderUsedWhenDisplayingCodeCompletionWindow {
			get { return completionProviderUsedWhenDisplayingCodeCompletionWindow; }
		}
		
		/// <summary>
		/// Gets the CompletionDataProvider created via the
		/// CreateCompletionDataProvider method.
		/// </summary>
		public AbstractCompletionItemProvider CompletionDataProvider {
			get { return completionDataProvider; }
		}
		
		/// <summary>
		/// Gets the character used when calling the TextAreaControl's 
		/// ShowCompletionWindow method.
		/// </summary>
		public char CompletionCharacter {
			get { return completionCharacter; }
		}
		
		/// <summary>
		/// Overrides the completion data provider creation to make sure
		/// it is called at the correct time. 
		/// </summary>
		protected override AbstractCompletionItemProvider CreateCompletionDataProvider()
		{
			completionDataProviderCreated = true;
			completionDataProvider = base.CreateCompletionDataProvider();
			return completionDataProvider;
		}
		
		/// <summary>
		/// Overrides the base class method so a code completion window is
		/// not displayed but the fact that this method is called is
		/// recorded. 
		/// </summary>
		protected override void ShowCodeCompletionWindow(ICSharpCode.SharpDevelop.Editor.ITextEditor textEditor, AbstractCompletionItemProvider completionDataProvider, char ch)
		{
			textEditorUsedToShowCompletionWindow = textEditor;
			codeCompletionWindowDisplayed = true;
			completionCharacter = ch;
			completionProviderUsedWhenDisplayingCodeCompletionWindow = completionDataProvider;
		}
	}
}
