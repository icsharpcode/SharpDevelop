// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Refactoring;
using SharpRefactoring.Gui;

namespace SharpRefactoring
{
	public class CreatePropertiesCommand : AbstractRefactoringCommand
	{
		protected override void Run(ITextEditor textEditor, RefactoringProvider provider)
		{
			new Snippet {
				Elements = {
					new InlineRefactorSnippetElement(context => CreateProperties.CreateDialog(context), "")
				}
			}.Insert((TextArea)textEditor.GetService(typeof(TextArea)));
		}
	}
}
