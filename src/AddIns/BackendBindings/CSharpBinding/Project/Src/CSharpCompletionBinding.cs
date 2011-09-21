// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.Completion;
using ICSharpCode.NRefactory.CSharp.Completion;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace CSharpBinding
{
	public class CSharpCompletionBinding : ICodeCompletionBinding
	{
		public CodeCompletionKeyPressResult HandleKeyPress(ITextEditor editor, char ch)
		{
			return CodeCompletionKeyPressResult.None;
		}
		
		public bool HandleKeyPressed(ITextEditor editor, char ch)
		{
			if (ch == '.')
				return CtrlSpace(editor);
			else
				return false;
		}
		
		public bool CtrlSpace(ITextEditor editor)
		{
			return false;
		}
	}
}

