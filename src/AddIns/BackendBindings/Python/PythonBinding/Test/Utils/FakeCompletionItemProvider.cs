// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace PythonBinding.Tests.Utils
{
	public class FakeCompletionItemProvider : AbstractCompletionItemProvider
	{
		public ITextEditor TextEditorPassedToShowCompletion;
		
		public override void ShowCompletion(ITextEditor editor)
		{
			TextEditorPassedToShowCompletion = editor;
		}
		
		public override ICompletionItemList GenerateCompletionList(ITextEditor editor)
		{
			throw new NotImplementedException();
		}
	}
}
