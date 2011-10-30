// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using SharpRefactoring.Gui;

namespace SharpRefactoring
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
			
			List<FieldWrapper> parameters = FindFields(current).Where(f => !current.Properties.Any(p => p.Name == f.PropertyName)).ToList();
			
			if (!parameters.Any())
				return null;
			
			ITextAnchor anchor = textEditor.Document.CreateAnchor(context.InsertionPosition);
			anchor.MovementType = AnchorMovementType.BeforeInsertion;
			
			CreatePropertiesDialog dialog = new CreatePropertiesDialog(context, textEditor, anchor, current, parameters);
			
			dialog.Element = uiService.CreateInlineUIElement(anchor, dialog);
			
			return dialog;
		}
		
		static IEnumerable<FieldWrapper> FindFields(IClass sourceClass)
		{
			int i = 0;
			
			foreach (var f in sourceClass.Fields.Where(field => !field.IsConst
			                                           && field.ReturnType != null)) {
				yield return new FieldWrapper(f) { Index = i };
				i++;
			}
		}
	}
}
