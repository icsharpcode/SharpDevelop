// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.Editor
{
	/// <summary>
	/// Read-only implementation of <see cref="IDocument"/>.
	/// </summary>
	public sealed class ReadOnlyDocument : IDocument
	{
		readonly ITextSource textBuffer;
		int[] lines;
		
		static readonly char[] newline = { '\r', '\n' };
		
		/// <summary>
		/// Creates a new ReadOnlyDocument from the given text buffer.
		/// </summary>
		public ReadOnlyDocument(ITextSource textBuffer)
		{
			if (textBuffer == null)
				throw new ArgumentNullException("textBuffer");
			// ensure that underlying buffer is immutable
			this.textBuffer = textBuffer.CreateSnapshot();
			List<int> lines = new List<int>();
			lines.Add(0);
			int offset = 0;
			int textLength = textBuffer.TextLength;
			while ((offset = textBuffer.IndexOfAny(newline, offset, textLength - offset)) >= 0) {
				offset++;
				if (textBuffer.GetCharAt(offset - 1) == '\r' && offset < textLength && textBuffer.GetCharAt(offset) == '\n') {
					offset++;
				}
				lines.Add(offset);
			}
			this.lines = lines.ToArray();
		}
		
		/// <summary>
		/// Creates a new ReadOnlyDocument from the given string.
		/// </summary>
		public ReadOnlyDocument(string text)
			: this(new StringTextSource(text))
		{
		}
		
		/// <inheritdoc/>
		public IDocumentLine GetLine(int lineNumber)
		{
			if (lineNumber < 1 || lineNumber > lines.Length)
				throw new ArgumentOutOfRangeException("lineNumber", lineNumber, "Value must be between 1 and " + lines.Length);
			return new ReadOnlyDocumentLine(this, lineNumber);
		}
		
		sealed class ReadOnlyDocumentLine : IDocumentLine
		{
			readonly ReadOnlyDocument doc;
			readonly int lineNumber;
			readonly int offset, endOffset;
			
			public ReadOnlyDocumentLine(ReadOnlyDocument doc, int lineNumber)
			{
				this.doc = doc;
				this.lineNumber = lineNumber;
				this.offset = doc.GetStartOffset(lineNumber);
				this.endOffset = doc.GetEndOffset(lineNumber);
			}
			
			public int Offset {
				get { return offset; }
			}
			
			public int Length {
				get { return endOffset - offset; }
			}
			
			public int EndOffset {
				get { return endOffset; }
			}
			
			public int TotalLength {
				get {
					return doc.GetTotalEndOffset(lineNumber) - offset;
				}
			}
			
			public int DelimiterLength {
				get {
					return doc.GetTotalEndOffset(lineNumber) - endOffset;
				}
			}
			
			public int LineNumber {
				get { return lineNumber; }
			}
			
			public string Text {
				get {
					return doc.GetText(this.Offset, this.Length);
				}
			}
		}
		
		int GetStartOffset(int lineNumber)
		{
			return lines[lineNumber-1];
		}
		
		int GetTotalEndOffset(int lineNumber)
		{
			return lineNumber < lines.Length ? lines[lineNumber] : textBuffer.TextLength;
		}
		
		int GetEndOffset(int lineNumber)
		{
			if (lineNumber == lines.Length)
				return textBuffer.TextLength;
			int off = lines[lineNumber] - 1;
			if (off > 0 && textBuffer.GetCharAt(off - 1) == '\r' && textBuffer.GetCharAt(off) == '\n')
				off--;
			return off;
		}
		
		/// <inheritdoc/>
		public IDocumentLine GetLineByOffset(int offset)
		{
			return GetLine(GetLineNumberForOffset(offset));
		}
		
		int GetLineNumberForOffset(int offset)
		{
			int r = Array.BinarySearch(lines, offset);
			return r < 0 ? ~r : r + 1;
		}
		
		/// <inheritdoc/>
		public int GetOffset(int line, int column)
		{
			if (line < 1 || line > lines.Length)
				throw new ArgumentOutOfRangeException("line", line, "Value must be between 1 and " + lines.Length);
			int lineStart = GetStartOffset(line);
			if (column <= 0)
				return lineStart;
			int lineEnd = GetEndOffset(line);
			if (column >= lineEnd - lineStart)
				return lineEnd;
			return lineStart + column - 1;
		}
		
		/// <inheritdoc/>
		public int GetOffset(TextLocation location)
		{
			return GetOffset(location.Line, location.Column);
		}
		
		/// <inheritdoc/>
		public TextLocation GetLocation(int offset)
		{
			if (offset < 0 || offset > textBuffer.TextLength)
				throw new ArgumentOutOfRangeException("offset", offset, "Value must be between 0 and " + textBuffer.TextLength);
			int line = GetLineNumberForOffset(offset);
			return new TextLocation(offset-GetStartOffset(line)+1, line);
		}
		
		/// <inheritdoc/>
		public string Text {
			get { return textBuffer.Text; }
			set {
				throw new NotSupportedException();
			}
		}
		
		/// <inheritdoc/>
		public int TotalNumberOfLines {
			get { return lines.Length; }
		}
		
		ITextSourceVersion ITextSource.Version {
			get { return null; }
		}
		
		/// <inheritdoc/>
		public int TextLength {
			get { return textBuffer.TextLength; }
		}
		
		event EventHandler<TextChangeEventArgs> IDocument.Changing { add {} remove {} }
		
		event EventHandler<TextChangeEventArgs> IDocument.Changed { add {} remove {} }
		
		event EventHandler IDocument.TextChanged { add {} remove {} }
		
		void IDocument.Insert(int offset, string text)
		{
			throw new NotSupportedException();
		}
		
		void IDocument.Insert(int offset, string text, AnchorMovementType defaultAnchorMovementType)
		{
			throw new NotSupportedException();
		}
		
		void IDocument.Remove(int offset, int length)
		{
			throw new NotSupportedException();
		}
		
		void IDocument.Replace(int offset, int length, string newText)
		{
			throw new NotSupportedException();
		}
		
		void IDocument.StartUndoableAction()
		{
		}
		
		void IDocument.EndUndoableAction()
		{
		}
		
		IDisposable IDocument.OpenUndoGroup()
		{
			return null;
		}
		
		/// <inheritdoc/>
		public ITextAnchor CreateAnchor(int offset)
		{
			return new ReadOnlyDocumentTextAnchor(GetLocation(offset), offset);
		}
		
		sealed class ReadOnlyDocumentTextAnchor : ITextAnchor
		{
			readonly TextLocation location;
			readonly int offset;
			
			public ReadOnlyDocumentTextAnchor(TextLocation location, int offset)
			{
				this.location = location;
				this.offset = offset;
			}
			
			public event EventHandler Deleted { add {} remove {} }
			
			public TextLocation Location {
				get { return location; }
			}
			
			public int Offset {
				get { return offset; }
			}
			
			public AnchorMovementType MovementType { get; set; }
			
			public bool SurviveDeletion { get; set; }
			
			public bool IsDeleted {
				get { return false; }
			}
			
			public int Line {
				get { return location.Line; }
			}
			
			public int Column {
				get { return location.Column; }
			}
		}
		
		/// <inheritdoc/>
		public ITextSource CreateSnapshot()
		{
			return textBuffer; // textBuffer is immutable
		}
		
		/// <inheritdoc/>
		public ITextSource CreateSnapshot(int offset, int length)
		{
			return textBuffer.CreateSnapshot(offset, length);
		}
		
		/// <inheritdoc/>
		public System.IO.TextReader CreateReader()
		{
			return textBuffer.CreateReader();
		}
		
		/// <inheritdoc/>
		public System.IO.TextReader CreateReader(int offset, int length)
		{
			return textBuffer.CreateReader(offset, length);
		}
		
		/// <inheritdoc/>
		public char GetCharAt(int offset)
		{
			return textBuffer.GetCharAt(offset);
		}
		
		/// <inheritdoc/>
		public string GetText(int offset, int length)
		{
			return textBuffer.GetText(offset, length);
		}
		
		/// <inheritdoc/>
		public string GetText(ISegment segment)
		{
			return textBuffer.GetText(segment);
		}
		
		/// <inheritdoc/>
		public int IndexOfAny(char[] anyOf, int startIndex, int count)
		{
			return textBuffer.IndexOfAny(anyOf, startIndex, count);
		}
		
		/// <inheritdoc/>
		public object GetService(Type serviceType)
		{
			return null;
		}
	}
}
