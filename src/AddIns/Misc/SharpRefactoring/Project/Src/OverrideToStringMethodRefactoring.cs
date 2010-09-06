// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using SharpRefactoring.Gui;

namespace SharpRefactoring
{
	public class OverrideToStringMethodRefactoring : ICompletionItemHandler
	{
		public void Insert(CompletionContext context, ICompletionItem item)
		{
			ITextEditor textEditor = context.Editor;
			
			IEditorUIService uiService = textEditor.GetService(typeof(IEditorUIService)) as IEditorUIService;
			
			if (uiService == null)
				return;
			
			ParseInformation parseInfo = ParserService.GetParseInformation(textEditor.FileName);
			
			if (parseInfo == null)
				return;
			
			CodeGenerator generator = parseInfo.CompilationUnit.Language.CodeGenerator;
			IClass current = parseInfo.CompilationUnit.GetInnermostClass(textEditor.Caret.Line, textEditor.Caret.Column);
			
			if (current == null)
				return;
			
			ITextAnchor anchor = textEditor.Document.CreateAnchor(textEditor.Caret.Offset);
			anchor.MovementType = AnchorMovementType.AfterInsertion;
			
			ITextAnchor startAnchor = textEditor.Document.CreateAnchor(textEditor.Caret.Offset);
			anchor.MovementType = AnchorMovementType.BeforeInsertion;
			
			InsertionContext insertionContext = new InsertionContext(textEditor.GetService(typeof(TextArea)) as TextArea, startAnchor.Offset);
			
			AbstractInlineRefactorDialog dialog = new OverrideToStringMethodDialog(insertionContext, textEditor, startAnchor, anchor, current.Fields);
			
			dialog.Element = uiService.CreateInlineUIElement(anchor, dialog);
			
			insertionContext.RegisterActiveElement(new InlineRefactorSnippetElement(cxt => null, ""), dialog);
			insertionContext.RaiseInsertionCompleted(EventArgs.Empty);
		}
		
		public bool Handles(ICompletionItem item)
		{
			return item is OverrideCompletionItem && item.Text == "ToString()";
		}
	}
}
