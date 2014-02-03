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
using System.Diagnostics;
using System.Windows.Documents;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;

namespace ICSharpCode.SharpDevelop.Editor
{
	/// <summary>
	/// Extension methods for ITextEditor and IDocument.
	/// </summary>
	public static class DocumentUtilities
	{
		/// <summary>
		/// Creates a new mutable document from the specified text buffer.
		/// </summary>
		/// <remarks>
		/// Use the more efficient <see cref="LoadReadOnlyDocumentFromBuffer"/> if you only need a read-only document.
		/// </remarks>
		[Obsolete("Use the TextDocument constructor instead")]
		public static IDocument LoadDocumentFromBuffer(ITextSource buffer)
		{
			return new TextDocument(buffer);
		}
		
		/// <summary>
		/// Creates a new read-only document from the specified text buffer.
		/// </summary>
		[Obsolete("Use the ReadOnlyDocument constructor instead")]
		public static IDocument LoadReadOnlyDocumentFromBuffer(ITextSource buffer)
		{
			return new ReadOnlyDocument(buffer);
		}
		
		public static void ClearSelection(this ITextEditor editor)
		{
			editor.Select(editor.Document.GetOffset(editor.Caret.Location), 0);
		}
		
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
			string oldLineText = document.GetText(line);
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
		public static int FindPrevWordStart(this ITextSource textSource, int offset)
		{
			return TextUtilities.GetNextCaretPosition(textSource, offset, LogicalDirection.Backward, CaretPositioningMode.WordStart);
		}
		
		/// <summary>
		/// Finds the first word start in the document before offset.
		/// </summary>
		/// <returns>The offset of the word start, or -1 if there is no word start before the specified offset.</returns>
		public static int FindNextWordStart(this ITextSource textSource, int offset)
		{
			return TextUtilities.GetNextCaretPosition(textSource, offset, LogicalDirection.Forward, CaretPositioningMode.WordStart);
		}
		
		/// <summary>
		/// Gets the word at the specified position.
		/// </summary>
		public static string GetWordAt(this ITextSource document, int offset)
		{
			if (offset < 0 || offset >= document.TextLength || !IsWordPart(document.GetCharAt(offset))) {
				return String.Empty;
			}
			int startOffset = offset;
			int endOffset   = offset;
			while (startOffset > 0 && IsWordPart(document.GetCharAt(startOffset - 1))) {
				--startOffset;
			}
			
			while (endOffset < document.TextLength - 1 && IsWordPart(document.GetCharAt(endOffset + 1))) {
				++endOffset;
			}
			
			Debug.Assert(endOffset >= startOffset);
			return document.GetText(startOffset, endOffset - startOffset + 1);
		}
		
		static bool IsWordPart(char ch)
		{
			return char.IsLetterOrDigit(ch) || ch == '_';
		}
		
		public static string GetIndentation(IDocument document, int line)
		{
			return DocumentUtilities.GetWhitespaceAfter(document, document.GetLineByNumber(line).Offset);
		}
		
		/// <summary>
		/// Gets whether the specified document line is empty or contains only whitespace.
		/// </summary>
		public static bool IsEmptyLine(IDocument document, int lineNumber)
		{
			var line = document.GetLineByNumber(lineNumber);
			ISegment segment = TextUtilities.GetWhitespaceAfter(document, line.Offset);
			return segment.Length == line.Length;
		}
		
		/// <summary>
		/// Gets all indentation starting at offset.
		/// </summary>
		/// <param name="textSource">The document.</param>
		/// <param name="offset">The offset where the indentation starts.</param>
		/// <returns>The indentation text.</returns>
		public static string GetWhitespaceAfter(ITextSource textSource, int offset)
		{
			ISegment segment = TextUtilities.GetWhitespaceAfter(textSource, offset);
			return textSource.GetText(segment.Offset, segment.Length);
		}
		
		/// <summary>
		/// Gets all indentation before the offset.
		/// </summary>
		/// <param name="textSource">The document.</param>
		/// <param name="offset">The offset where the indentation ends.</param>
		/// <returns>The indentation text.</returns>
		public static string GetWhitespaceBefore(ITextSource textSource, int offset)
		{
			ISegment segment = TextUtilities.GetWhitespaceBefore(textSource, offset);
			return textSource.GetText(segment.Offset, segment.Length);
		}
		
		/// <summary>
		/// Gets the line terminator for the document around the specified line number.
		/// </summary>
		public static string GetLineTerminator(IDocument document, int lineNumber)
		{
			IDocumentLine line = document.GetLineByNumber(lineNumber);
			if (line.DelimiterLength == 0) {
				// at the end of the document, there's no line delimiter, so use the delimiter
				// from the previous line
				if (lineNumber == 1)
					return Environment.NewLine;
				line = document.GetLineByNumber(lineNumber - 1);
			}
			return document.GetText(line.Offset + line.Length, line.DelimiterLength);
		}
		
		public static string NormalizeNewLines(string input, string newLine)
		{
			return input.Replace("\r\n", "\n").Replace('\r', '\n').Replace("\n", newLine);
		}
		
		public static string NormalizeNewLines(string input, IDocument document, int lineNumber)
		{
			return NormalizeNewLines(input, GetLineTerminator(document, lineNumber));
		}
		
		public static void InsertNormalized(this IDocument document, int offset, string text)
		{
			if (document == null)
				throw new ArgumentNullException("document");
			IDocumentLine line = document.GetLineByOffset(offset);
			text = NormalizeNewLines(text, document, line.LineNumber);
			document.Insert(offset, text);
		}
		
		#region ITextSource implementation
		[Obsolete("We now directly use ITextSource everywhere, no need for adapters")]
		public static ITextSource GetTextSource(ITextSource textBuffer)
		{
			return textBuffer;
		}
		#endregion
	}
}
