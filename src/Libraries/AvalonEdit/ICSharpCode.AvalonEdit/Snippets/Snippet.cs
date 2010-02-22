// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Documents;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Snippets
{
	/// <summary>
	/// A code snippet that can be inserted into the text editor.
	/// </summary>
	[Serializable]
	public class Snippet : SnippetContainerElement
	{
		/// <summary>
		/// Inserts the snippet into the text area.
		/// </summary>
		public void Insert(TextArea textArea)
		{
			if (textArea == null)
				throw new ArgumentNullException("textArea");
			
			ISegment selection = textArea.Selection.SurroundingSegment;
			int insertionPosition = textArea.Caret.Offset;
			
			if (selection != null) // if something is selected
				insertionPosition = selection.Offset; // use selection start instead of caret position,
													     // because caret could be at end of selection or anywhere inside.
													     // Removal of the selected text causes the caret position to be invalid.
			
			InsertionContext context = new InsertionContext(textArea, insertionPosition);
			
			if (selection != null)
				textArea.Document.Remove(selection);
			
			using (context.Document.RunUpdate()) {
				Insert(context);
				context.RaiseInsertionCompleted(EventArgs.Empty);
			}
		}
	}
}
