// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Editor;

namespace CSharpBinding.Refactoring
{
	public class CreateProperties : ISnippetElementProvider
	{
		public SnippetElement GetElement(SnippetInfo snippetInfo)
		{
			if ("refactoring:propall".Equals(snippetInfo.Tag, StringComparison.OrdinalIgnoreCase))
				return new InlineRefactorSnippetElement(context => CreateDialog(context), "{" + snippetInfo.Tag + "}");
			
			return null;
		}
		
		internal static CreatePropertiesDialog CreateDialog(InsertionContext context)
		{
			ITextEditor textEditor = context.TextArea.GetService(typeof(ITextEditor)) as ITextEditor;
			
			if (textEditor == null)
				return null;
			
			using (textEditor.Document.OpenUndoGroup()) {
				IEditorUIService uiService = textEditor.GetService(typeof(IEditorUIService)) as IEditorUIService;
				
				if (uiService == null)
					return null;
				
				ITextAnchor anchor = textEditor.Document.CreateAnchor(context.InsertionPosition);
				anchor.MovementType = AnchorMovementType.BeforeInsertion;
				
				CreatePropertiesDialog dialog = new CreatePropertiesDialog(context, textEditor, anchor);
				
				dialog.Element = uiService.CreateInlineUIElement(anchor, dialog);
				
				// Add creation of this inline dialog as undoable operation
				TextDocument document = textEditor.Document as TextDocument;
				if (document != null) {
					document.UndoStack.Push(dialog.UndoableCreationOperation);
				}
				
				return dialog;
				
			}
		}
	}
}
