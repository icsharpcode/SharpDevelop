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
