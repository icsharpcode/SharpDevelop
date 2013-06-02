// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.Windows.Documents;

using ICSharpCode.AvalonEdit.Snippets;

namespace CSharpBinding.Refactoring
{
	class InlineRefactorSnippetElement : SnippetElement
	{
		Func<InsertionContext, AbstractInlineRefactorDialog> createDialog;
		string previewText;
		
		public InlineRefactorSnippetElement(Func<InsertionContext, AbstractInlineRefactorDialog> createDialog, string previewText)
		{
			this.createDialog = createDialog;
			this.previewText = previewText;
		}
		
		public override void Insert(InsertionContext context)
		{
			AbstractInlineRefactorDialog dialog = createDialog(context);
			if (dialog != null)
				context.RegisterActiveElement(this, dialog);
		}
		
		public override Inline ToTextRun()
		{
			return new Italic() { Inlines = { previewText } };
		}
	}
}
