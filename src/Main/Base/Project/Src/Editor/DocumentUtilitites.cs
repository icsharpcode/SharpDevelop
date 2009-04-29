// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Gui;
using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;
using System.Windows.Documents;

namespace ICSharpCode.SharpDevelop.Editor
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
		
		static readonly char[] whitespaceChars = {' ', '\t'};
		
		/// <summary>
		/// Replaces the text in a line.
		/// If only whitespace at the beginning and end of the line was changed, this method
		/// only adjusts the whitespace and doesn't replace the other text.
		/// </summary>
		public static void SmartReplaceLine(this IDocument document, IDocumentLine line, string newLineText)
		{
			if (document == null)
				throw new ArgumentNullException("document");
			if (line == null)
				throw new ArgumentNullException("line");
			if (newLineText == null)
				throw new ArgumentNullException("newLineText");
			string newLineTextTrim = newLineText.Trim(whitespaceChars);
			string oldLineText = line.Text;
			if (oldLineText == newLineText)
				return;
			int pos = oldLineText.IndexOf(newLineTextTrim, StringComparison.Ordinal);
			if (newLineTextTrim.Length > 0 && pos >= 0) {
				using (document.OpenUndoGroup()) {
					// find whitespace at beginning
					int startWhitespaceLength = 0;
					while (startWhitespaceLength < newLineText.Length) {
						char c = newLineText[startWhitespaceLength];
						if (c != ' ' && c != '\t')
							break;
						startWhitespaceLength++;
					}
					// find whitespace at end
					int endWhitespaceLength = newLineText.Length - newLineTextTrim.Length - startWhitespaceLength;
					
					// replace whitespace sections
					int lineOffset = line.Offset;
					document.Replace(lineOffset + pos + newLineTextTrim.Length, line.Length - pos - newLineTextTrim.Length, newLineText.Substring(newLineText.Length - endWhitespaceLength));
					document.Replace(lineOffset, pos, newLineText.Substring(0, startWhitespaceLength));
				}
			} else {
				document.Replace(line.Offset, line.Length, newLineText);
			}
		}
		
		/// <summary>
		/// Finds the first word start in the document before offset.
		/// </summary>
		/// <returns>The offset of the word start, or -1 if there is no word start before the specified offset.</returns>
		public static int FindPrevWordStart(this IDocument document, int offset)
		{
			return TextUtilities.GetNextCaretPosition(GetTextSource(document), offset, LogicalDirection.Backward, CaretPositioningMode.WordStart);
		}
		
		/// <summary>
		/// Gets the word at the specified position.
		/// </summary>
		public static string GetWordAt(this IDocument document, int offset)
		{
			if (document == null)
				throw new ArgumentNullException("document");
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Gets all indentation starting at offset.
		/// </summary>
		/// <param name="document">The document.</param>
		/// <param name="offset">The offset where the indentation starts.</param>
		/// <returns>The indentation text.</returns>
		public static string GetIndentation(IDocument document, int offset)
		{
			ISegment segment = TextUtilities.GetIndentation(GetTextSource(document), offset);
			return document.GetText(segment.Offset, segment.Length);
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
