// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Windows.Documents;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;

namespace ICSharpCode.SharpDevelop.Editor
{
	/// <summary>
	/// Extension methods for ITextEditor and IDocument.
	/// </summary>
	public static class DocumentUtilitites
	{
		/// <summary>
		/// Creates a new mutable document from the specified text buffer.
		/// </summary>
		/// <remarks>
		/// Use the more efficient <see cref="LoadReadOnlyDocumentFromBuffer"/> if you only need a read-only document.
		/// </remarks>
		public static IDocument LoadDocumentFromBuffer(ITextBuffer buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException("buffer");
			var doc = new TextDocument(GetTextSource(buffer));
			return new AvalonEditDocumentAdapter(doc, null);
		}
		
		/// <summary>
		/// Creates a new read-only document from the specified text buffer.
		/// </summary>
		public static IDocument LoadReadOnlyDocumentFromBuffer(ITextBuffer buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException("buffer");
			return new ReadOnlyDocument(buffer);
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
		public static int FindPrevWordStart(this ITextBuffer textBuffer, int offset)
		{
			return TextUtilities.GetNextCaretPosition(GetTextSource(textBuffer), offset, LogicalDirection.Backward, CaretPositioningMode.WordStart);
		}
		
		/// <summary>
		/// Finds the first word start in the document before offset.
		/// </summary>
		/// <returns>The offset of the word start, or -1 if there is no word start before the specified offset.</returns>
		public static int FindNextWordStart(this ITextBuffer textBuffer, int offset)
		{
			return TextUtilities.GetNextCaretPosition(GetTextSource(textBuffer), offset, LogicalDirection.Forward, CaretPositioningMode.WordStart);
		}
		
		/// <summary>
		/// Gets the word at the specified position.
		/// </summary>
		public static string GetWordAt(this ITextBuffer document, int offset)
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
		
		/// <summary>
		/// Gets all indentation starting at offset.
		/// </summary>
		/// <param name="document">The document.</param>
		/// <param name="offset">The offset where the indentation starts.</param>
		/// <returns>The indentation text.</returns>
		public static string GetWhitespaceAfter(ITextBuffer textBuffer, int offset)
		{
			ISegment segment = TextUtilities.GetWhitespaceAfter(GetTextSource(textBuffer), offset);
			return textBuffer.GetText(segment.Offset, segment.Length);
		}
		
		/// <summary>
		/// Gets all indentation before the offset.
		/// </summary>
		/// <param name="document">The document.</param>
		/// <param name="offset">The offset where the indentation ends.</param>
		/// <returns>The indentation text.</returns>
		public static string GetWhitespaceBefore(ITextBuffer textBuffer, int offset)
		{
			ISegment segment = TextUtilities.GetWhitespaceBefore(GetTextSource(textBuffer), offset);
			return textBuffer.GetText(segment.Offset, segment.Length);
		}
		
		/// <summary>
		/// Gets the line terminator for the document around the specified line number.
		/// </summary>
		public static string GetLineTerminator(IDocument document, int lineNumber)
		{
			IDocumentLine line = document.GetLine(lineNumber);
			if (line.DelimiterLength == 0) {
				// at the end of the document, there's no line delimiter, so use the delimiter
				// from the previous line
				if (lineNumber == 1)
					return Environment.NewLine;
				line = document.GetLine(lineNumber - 1);
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
			IDocumentLine line = document.GetLineForOffset(offset);
			text = NormalizeNewLines(text, document, line.LineNumber);
			document.Insert(offset, text);
		}
		
		#region ITextSource implementation
		public static ICSharpCode.AvalonEdit.Document.ITextSource GetTextSource(ITextBuffer textBuffer)
		{
			if (textBuffer == null)
				throw new ArgumentNullException("textBuffer");
			
			var textSourceAdapter = textBuffer as AvalonEditTextSourceAdapter;
			if (textSourceAdapter != null)
				return textSourceAdapter.textSource;
			
			var documentAdapter = textBuffer as AvalonEditDocumentAdapter;
			if (documentAdapter != null)
				return documentAdapter.document;
			
			return new TextBufferTextSource(textBuffer);
		}
		
		sealed class TextBufferTextSource : ICSharpCode.AvalonEdit.Document.ITextSource
		{
			readonly ITextBuffer textBuffer;
			
			public TextBufferTextSource(ITextBuffer textBuffer)
			{
				this.textBuffer = textBuffer;
			}
			
			public event EventHandler TextChanged {
				add    { textBuffer.TextChanged += value; }
				remove { textBuffer.TextChanged -= value; }
			}
			
			public string Text {
				get { return textBuffer.Text; }
			}
			
			public int TextLength {
				get { return textBuffer.TextLength; }
			}
			
			public char GetCharAt(int offset)
			{
				return textBuffer.GetCharAt(offset);
			}
			
			public string GetText(int offset, int length)
			{
				return textBuffer.GetText(offset, length);
			}
			
			public System.IO.TextReader CreateReader()
			{
				return textBuffer.CreateReader();
			}
			
			public ITextSource CreateSnapshot()
			{
				return GetTextSource(textBuffer.CreateSnapshot());
			}
			
			public ITextSource CreateSnapshot(int offset, int length)
			{
				return GetTextSource(textBuffer.CreateSnapshot(offset, length));
			}
			
			public int IndexOfAny(char[] anyOf, int startIndex, int count)
			{
				int endIndex = startIndex + count;
				for (int i = startIndex; i < endIndex; i++) {
					char c = textBuffer.GetCharAt(i);
					foreach (char d in anyOf) {
						if (c == d)
							return i;
					}
				}
				return -1;
			}
		}
		#endregion
	}
}
