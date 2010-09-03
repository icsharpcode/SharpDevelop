// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;

namespace SharpRefactoring.Gui
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
