// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;

namespace ICSharpCode.SharpDevelop.Editor
{
	/// <summary>
	/// Read-only implementation of IDocument.
	/// </summary>
	sealed class ReadOnlyDocument : IDocument
	{
		ITextBuffer textBuffer;
		int[] lines;
		
		public ReadOnlyDocument(ITextBuffer textBuffer)
		{
			if (textBuffer == null)
				throw new ArgumentNullException("textBuffer");
			// ensure that underlying buffer is immutable
			this.textBuffer = textBuffer.CreateSnapshot();
			List<int> lines = new List<int>();
			lines.Add(0);
			int offset = 0;
			string newlineType;
			var textSource = DocumentUtilitites.GetTextSource(this.textBuffer);
			while ((offset = ICSharpCode.AvalonEdit.Document.TextUtilities.FindNextNewLine(textSource, offset, out newlineType)) >= 0) {
				offset += newlineType.Length;
				lines.Add(offset);
			}
			this.lines = lines.ToArray();
		}
		
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
		
		public IDocumentLine GetLineForOffset(int offset)
		{
			return GetLine(GetLineNumberForOffset(offset));
		}
		
		int GetLineNumberForOffset(int offset)
		{
			int r = Array.BinarySearch(lines, offset);
			return r < 0 ? ~r : r + 1;
		}
		
		public int PositionToOffset(int line, int column)
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
		
		public Location OffsetToPosition(int offset)
		{
			if (offset < 0 || offset > textBuffer.TextLength)
				throw new ArgumentOutOfRangeException("offset", offset, "Value must be between 0 and " + textBuffer.TextLength);
			int line = GetLineNumberForOffset(offset);
			return new Location(offset-GetStartOffset(line)+1, line);
		}
		
		public event EventHandler<TextChangeEventArgs> Changing { add {} remove {} }
		
		public event EventHandler<TextChangeEventArgs> Changed { add {} remove {} }
		
		public event EventHandler TextChanged { add {} remove {} }
		
		public string Text {
			get { return textBuffer.Text; }
			set {
				throw new NotSupportedException();
			}
		}
		
		public int TotalNumberOfLines {
			get { return lines.Length; }
		}
		
		public ITextBufferVersion Version {
			get { return null; }
		}
		
		public int TextLength {
			get { return textBuffer.TextLength; }
		}
		
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
		
		public void StartUndoableAction()
		{
		}
		
		public void EndUndoableAction()
		{
		}
		
		public IDisposable OpenUndoGroup()
		{
			return new CallbackOnDispose(EndUndoableAction);
		}
		
		public ITextAnchor CreateAnchor(int offset)
		{
			return new ReadOnlyDocumentTextAnchor(OffsetToPosition(offset), offset);
		}
		
		sealed class ReadOnlyDocumentTextAnchor : ITextAnchor
		{
			readonly Location location;
			readonly int offset;
			
			public ReadOnlyDocumentTextAnchor(Location location, int offset)
			{
				this.location = location;
				this.offset = offset;
			}
			
			public event EventHandler Deleted { add {} remove {} }
			
			public Location Location {
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
		
		public ITextBuffer CreateSnapshot()
		{
			return textBuffer; // textBuffer is immutable
		}
		
		public ITextBuffer CreateSnapshot(int offset, int length)
		{
			return textBuffer.CreateSnapshot(offset, length);
		}
		
		public System.IO.TextReader CreateReader()
		{
			return textBuffer.CreateReader();
		}
		
		public System.IO.TextReader CreateReader(int offset, int length)
		{
			return textBuffer.CreateReader(offset, length);
		}
		
		public char GetCharAt(int offset)
		{
			return textBuffer.GetCharAt(offset);
		}
		
		public string GetText(int offset, int length)
		{
			return textBuffer.GetText(offset, length);
		}
		
		public object GetService(Type serviceType)
		{
			return null;
		}
	}
}
