// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.AspNet.Mvc.Completion
{
	public class RazorCSharpCompletionBinding : ICodeCompletionBinding
	{
		public RazorCSharpCompletionBinding()
		{
		}
		
		public CodeCompletionKeyPressResult HandleKeyPress(ITextEditor editor, char ch)
		{
			if (ch == '.') {
				new RazorCSharpDotCompletionDataProvider().ShowCompletion(editor);
				return CodeCompletionKeyPressResult.Completed;
			}
			return CodeCompletionKeyPressResult.None;
		}
		
		public bool CtrlSpace(ITextEditor editor)
		{
			return false;
		}
	}
}
