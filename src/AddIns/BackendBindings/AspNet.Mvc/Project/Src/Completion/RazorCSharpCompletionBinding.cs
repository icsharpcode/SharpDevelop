// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.AspNet.Mvc.Completion
{
	public class RazorCSharpCompletionBinding : DefaultCodeCompletionBinding
	{
		public RazorCSharpCompletionBinding()
		{
		}
		
		public override CodeCompletionKeyPressResult HandleKeyPress(ITextEditor editor, char ch)
		{
			if (ch == '.') {
				new RazorCSharpDotCompletionDataProvider().ShowCompletion(editor);
				return CodeCompletionKeyPressResult.Completed;
			} else if (ch == '(') {
				return base.HandleKeyPress(editor, ch);
			}
			return CodeCompletionKeyPressResult.None;
		}
	}
}
