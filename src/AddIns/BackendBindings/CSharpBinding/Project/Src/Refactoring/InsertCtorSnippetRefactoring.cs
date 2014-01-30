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
using System.Linq;

using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.NRefactory.Editor;
using CSharpBinding.Refactoring;
using ICSharpCode.SharpDevelop.Editor;

namespace CSharpBinding.Refactoring
{
	public class InsertCtorSnippetRefactoring : ISnippetElementProvider
	{
		public SnippetElement GetElement(SnippetInfo snippetInfo)
		{
			if ("refactoring:ctor".Equals(snippetInfo.Tag, StringComparison.OrdinalIgnoreCase))
				return new InlineRefactorSnippetElement(CreateDialog, "{" + snippetInfo.Tag + "}");
			
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
			
			ITextAnchor anchor = textEditor.Document.CreateAnchor(context.InsertionPosition);
			anchor.MovementType = AnchorMovementType.BeforeInsertion;
			
			InsertCtorDialog dialog = new InsertCtorDialog(context, textEditor, anchor);
			
			dialog.Element = uiService.CreateInlineUIElement(anchor, dialog);
			
			return dialog;
		}
	}
}
