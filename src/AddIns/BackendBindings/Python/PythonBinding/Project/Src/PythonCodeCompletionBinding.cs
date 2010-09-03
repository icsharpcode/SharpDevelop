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
	/// <summary>
	/// Python code completion binding.
	/// </summary>
	public class PythonCodeCompletionBinding : DefaultCodeCompletionBinding
	{
		public PythonCodeCompletionBinding()
		{
		}
		
		/// <summary>
		/// Shows the code completion window if the keyword is handled.
		/// </summary>
		/// <param name="word">The keyword string.</param>
		/// <returns>true if the keyword is handled; otherwise false.</returns>
		public override bool HandleKeyword(ICSharpCode.SharpDevelop.Editor.ITextEditor editor, string word)
		{
			if (word != null) {
				switch (word.ToLowerInvariant()) {
					case "import":
					case "from":
						AbstractCompletionItemProvider dataProvider = CreateCompletionDataProvider();
						ShowCodeCompletionWindow(editor, dataProvider, ' ');
						return true;
				}
			}
			return false;
		}
		
		protected virtual AbstractCompletionItemProvider CreateCompletionDataProvider()
		{
			return new CodeCompletionItemProvider();
		}
		
		/// <summary>
		/// Shows the code completion window.
		/// </summary>
		protected virtual void ShowCodeCompletionWindow(ICSharpCode.SharpDevelop.Editor.ITextEditor editor, AbstractCompletionItemProvider completionItemProvider, char ch)
		{
			completionItemProvider.ShowCompletion(editor);
		}
	}
}
