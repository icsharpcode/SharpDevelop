// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision: 5529 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;
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
		public SnippetElement GetElement(SnippetInfo snippetInfo)
		{
			if ("refactoring:ctor".Equals(snippetInfo.Tag, StringComparison.OrdinalIgnoreCase))
				return new InlineRefactorSnippetElement(context => CreateDialog(context), "{" + snippetInfo.Tag + "}");
			
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
			
			// cannot use insertion position at this point, because it might not be
			// valid, because we are still generating the elements.
			// DOM is not updated
			ICSharpCode.AvalonEdit.Document.TextLocation loc = context.Document.GetLocation(context.StartPosition);
			
			IClass current = parseInfo.CompilationUnit.GetInnermostClass(loc.Line, loc.Column);
			
			if (current == null)
				return null;
			
			List<CtorParamWrapper> parameters = CreateCtorParams(current).ToList();
			
			ITextAnchor anchor = textEditor.Document.CreateAnchor(context.InsertionPosition);
			anchor.MovementType = AnchorMovementType.BeforeInsertion;
			
			InsertCtorDialog dialog = new InsertCtorDialog(context, textEditor, anchor, current, parameters);
			
			dialog.Element = uiService.CreateInlineUIElement(anchor, dialog);
			
			return dialog;
		}
		
		IEnumerable<CtorParamWrapper> CreateCtorParams(IClass sourceClass)
		{
			int i = 0;
			
			foreach (var f in sourceClass.Fields) {
				yield return new CtorParamWrapper(f) { Index = i, IsSelected = !f.IsReadonly };
				i++;
			}
			
			foreach (var p in sourceClass.Properties.Where(prop => prop.CanSet && !prop.IsIndexer)) {
				yield return new CtorParamWrapper(p) { Index = i, IsSelected = !p.IsReadonly };
				i++;
			}
		}
	}
}
