// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision: 5529 $</version>
// </file>

using System;
using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using SharpRefactoring.Gui;

namespace SharpRefactoring
{
	public class InsertCtorSnippetRefactoring : ISnippetElementProvider
	{
		public SnippetElement GetElement(string tag)
		{
			if (tag.Equals("refactoring:ctor", StringComparison.InvariantCultureIgnoreCase))
				return new InlineRefactorSnippetElement(context => CreateDialog(context));
			
			return null;
		}

		InsertCtorDialog CreateDialog(InsertionContext context)
		{
			ITextEditor textEditor = context.TextArea.GetService(typeof(ITextEditor)) as ITextEditor;
			
			if (textEditor == null)
				return null;
			
			IEditorUIService uiService = textEditor.GetService(typeof(IEditorUIService)) as IEditorUIService;
			
			if (uiService == null)
				return null;
			
			ParseInformation parseInfo = ParserService.GetParseInformation(textEditor.FileName);
			
			if (parseInfo == null)
				return null;
			
			CodeGenerator generator = parseInfo.CompilationUnit.Language.CodeGenerator;
			
			var loc = context.Document.GetLocation(context.InsertionPosition);
			
			IClass current = parseInfo.CompilationUnit.GetInnermostClass(loc.Line, loc.Column);
			
			if (current == null)
				return null;
			
			ITextAnchor anchor = textEditor.Document.CreateAnchor(context.InsertionPosition);
			anchor.MovementType = AnchorMovementType.AfterInsertion;
			
			InsertCtorDialog dialog = new InsertCtorDialog(context, textEditor, anchor, current);
			
			dialog.Element = uiService.CreateInlineUIElement(anchor, dialog);
			
			return dialog;
		}
	}
}
