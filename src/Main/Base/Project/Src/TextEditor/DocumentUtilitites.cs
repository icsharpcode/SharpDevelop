// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.AvalonEdit.Gui;
using System;
using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.SharpDevelop.Dom.Refactoring;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Extension methods for ITextEditor and IDocument.
	/// </summary>
	public static class DocumentUtilitites
	{
		/// <summary>
		/// Gets the word in front of the caret.
		/// </summary>
		public static string GetWordBeforeCaret(this ITextEditor editor)
		{
			if (editor == null)
				throw new ArgumentNullException("editor");
			int endOffset = editor.Caret.Offset;
			int startOffset = FindPrevWordStart(editor.Document, endOffset);
			if (startOffset < 0)
				return string.Empty;
			else
				return editor.Document.GetText(startOffset, endOffset - startOffset);
		}
		
		/// <summary>
		/// Finds the first word start in the document before offset.
		/// </summary>
		/// <returns>The offset of the word start, or -1 if there is no word start before the specified offset.</returns>
		public static int FindPrevWordStart(IDocument document, int offset)
		{
			return TextUtilities.GetNextCaretPosition(GetTextSource(document), offset, true, CaretPositioningMode.WordStart);
		}
		
		#region ITextSource implementation
		public static ICSharpCode.AvalonEdit.Document.ITextSource GetTextSource(IDocument document)
		{
			if (document == null)
				throw new ArgumentNullException("document");
			return new DocumentTextSource(document);
		}
		
		sealed class DocumentTextSource : ICSharpCode.AvalonEdit.Document.ITextSource
		{
			readonly IDocument document;
			
			public DocumentTextSource(IDocument document)
			{
				this.document = document;
			}
			
			public event EventHandler TextChanged {
				add    { document.TextChanged += value; }
				remove { document.TextChanged -= value; }
			}
			
			public string Text {
				get { return document.Text; }
			}
			
			public int TextLength {
				get { return document.TextLength; }
			}
			
			public char GetCharAt(int offset)
			{
				return document.GetCharAt(offset);
			}
			
			public string GetText(int offset, int length)
			{
				return document.GetText(offset, length);
			}
		}
		#endregion
	}
}
