// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Refactoring;

namespace SharpRefactoring
{
	public class CreatePropertiesCommand : AbstractRefactoringCommand
	{
		protected override void Run(ITextEditor textEditor, RefactoringProvider provider)
		{
			new Snippet {
				Elements = {
					new CreateProperties().GetElement(new SnippetInfo("refactoring:propall", "${refactoring:propall}", 0))
				}
			}.Insert((TextArea)textEditor.GetService(typeof(TextArea)));
		}
	}
}
