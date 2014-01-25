// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
