// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
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
		public void Insert(CompletionContext context)
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
			
			var line = textEditor.Document.GetLineForOffset(textEditor.Caret.Offset);
			
			string indent = DocumentUtilitites.GetWhitespaceAfter(textEditor.Document, line.Offset);
			
			textEditor.Document.Insert(anchor.Offset, "public override string ToString()\n" + indent + "{\n" + indent + "\t");
			textEditor.Document.Insert(anchor.Offset + 1, indent + "}\n");
			
			AbstractInlineRefactorDialog dialog = new OverrideToStringMethodDialog(null, textEditor, anchor, current.Fields);
			
			dialog.Element = uiService.CreateInlineUIElement(anchor, dialog);
		}
		
		public bool Handles(ICompletionItem item)
		{
			return item is OverrideCompletionItem && item.Text == "ToString()";
		}
	}
}
