// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using System.Collections.Generic;

namespace ICSharpCode.PythonBinding
{
	public class PythonCodeCompletionBinding : DefaultCodeCompletionBinding
	{
		public PythonCodeCompletionBinding()
		{
			base.insightHandler = new PythonInsightWindowHandler();
		}
		
		/// <summary>
		/// Shows the code completion window if the keyword is handled.
		/// </summary>
		/// <param name="word">The keyword string.</param>
		/// <returns>true if the keyword is handled; otherwise false.</returns>
		public override bool HandleKeyword(ITextEditor editor, string word)
		{
			if (word != null) {
				switch (word.ToLowerInvariant()) {
					case "import":
					case "from":
						return HandleImportKeyword(editor);
				}
			}
			return false;
		}
		
		bool HandleImportKeyword(ITextEditor editor)
		{
			AbstractCompletionItemProvider provider = CreateKeywordCompletionItemProvider();
			ShowCodeCompletionWindow(provider, editor);
			return true;
		}
		
		protected virtual AbstractCompletionItemProvider CreateKeywordCompletionItemProvider()
		{
			return new PythonCodeCompletionItemProvider();
		}
		
		protected virtual void ShowCodeCompletionWindow(AbstractCompletionItemProvider completionItemProvider, ITextEditor editor)
		{
			completionItemProvider.ShowCompletion(editor);
		}
	}
}
