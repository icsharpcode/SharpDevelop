// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace ICSharpCode.AvalonEdit.Snippets
{
	sealed class SnippetInputHandler : ITextAreaInputHandler
	{
		readonly InsertionContext context;
		
		public SnippetInputHandler(InsertionContext context)
		{
			this.context = context;
		}
		
		public TextArea TextArea {
			get { return context.TextArea; }
		}
		
		public void Attach()
		{
			TextArea.PreviewKeyDown += TextArea_PreviewKeyDown;
			
			SelectElement(FindNextEditableElement(-1, false));
		}
		
		void SelectElement(IActiveElement element)
		{
			if (element != null) {
				TextArea.Selection = new SimpleSelection(element.Segment);
				TextArea.Caret.Offset = element.Segment.EndOffset;
			}
		}
		
		public void Detach()
		{
			TextArea.PreviewKeyDown -= TextArea_PreviewKeyDown;
			context.Deactivate(EventArgs.Empty);
		}
		
		void TextArea_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape || e.Key == Key.Return) {
				context.Deactivate(e);
				e.Handled = true;
			} else if (e.Key == Key.Tab) {
				bool backwards = e.KeyboardDevice.Modifiers == ModifierKeys.Shift;
				SelectElement(FindNextEditableElement(TextArea.Caret.Offset, backwards));
				e.Handled = true;
			}
		}
		
		IActiveElement FindNextEditableElement(int offset, bool backwards)
		{
			IEnumerable<IActiveElement> elements = context.ActiveElements.Where(e => e.IsEditable && e.Segment != null);
			if (backwards) {
				elements = elements.Reverse();
				foreach (IActiveElement element in elements) {
					if (offset > element.Segment.EndOffset)
						return element;
				}
			} else {
				foreach (IActiveElement element in elements) {
					if (offset < element.Segment.Offset)
						return element;
				}
			}
			return elements.FirstOrDefault();
		}
	}
}
