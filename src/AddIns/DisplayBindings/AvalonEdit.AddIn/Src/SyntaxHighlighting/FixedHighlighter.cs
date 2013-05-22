// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// IHighlighter implementation that returns a fixed set of HighlightedLines.
	/// </summary>
	public class FixedHighlighter : IHighlighter
	{
		/// <summary>
		/// Creates a new <see cref="FixedHighlighter"/> for a copy of a portion
		/// of the input document (including the original highlighting).
		/// </summary>
		public static FixedHighlighter CreateView(IHighlighter highlighter, int offset, int endOffset)
		{
			var oldDocument = highlighter.Document;
			// ReadOnlyDocument would be better; but displaying the view in AvalonEdit
			// requires a TextDocument
			var newDocument = new TextDocument(oldDocument.CreateSnapshot(offset, endOffset - offset));
			
			var oldStartLine = oldDocument.GetLineByOffset(offset);
			var oldEndLine = oldDocument.GetLineByOffset(endOffset);
			int oldStartLineNumber = oldStartLine.LineNumber;
			HighlightedLine[] newLines = new HighlightedLine[oldEndLine.LineNumber - oldStartLineNumber + 1];
			highlighter.BeginHighlighting();
			try {
				for (int i = 0; i < newLines.Length; i++) {
					HighlightedLine oldHighlightedLine = highlighter.HighlightLine(oldStartLineNumber + i);
					IDocumentLine newLine = newDocument.GetLineByNumber(1 + i);
					HighlightedLine newHighlightedLine = new HighlightedLine(newDocument, newLine);
					MoveSections(oldHighlightedLine.Sections, -offset, newLine.Offset, newLine.EndOffset, newHighlightedLine.Sections);
					newHighlightedLine.ValidateInvariants();
					newLines[i] = newHighlightedLine;
				}
			} finally {
				highlighter.EndHighlighting();
			}
			return new FixedHighlighter(newDocument, newLines);
		}
		
		/// <summary>
		/// Moves the section start by <paramref name="delta"/> positions;
		/// and removes the sections outside the (newLineStart,newLineEnd) range.
		/// </summary>
		static void MoveSections(IEnumerable<HighlightedSection> sections, int delta, int newLineStart, int newLineEnd, ICollection<HighlightedSection> result)
		{
			foreach (var section in sections) {
				int newOffset = section.Offset + delta;
				int newEndOffset = section.Offset + section.Length + delta;
				newOffset = newOffset.CoerceValue(newLineStart, newLineEnd);
				newEndOffset = newEndOffset.CoerceValue(newLineStart, newLineEnd);
				
				if (newOffset < newLineEnd && newEndOffset > newLineStart) {
					result.Add(
						new HighlightedSection {
							Offset = newOffset,
							Length = newEndOffset - newOffset,
							Color = section.Color
						});
				}
			}
		}
		
		readonly IDocument document;
		readonly HighlightedLine[] lines;
		
		public FixedHighlighter(IDocument document, HighlightedLine[] lines)
		{
			if (lines.Length != document.LineCount)
				throw new ArgumentException("Wrong number of highlighted lines");
			this.document = document;
			this.lines = lines;
		}
		
		event HighlightingStateChangedEventHandler IHighlighter.HighlightingStateChanged {
			add { }
			remove { }
		}
		
		public IDocument Document {
			get { return document; }
		}
		
		HighlightingColor IHighlighter.DefaultTextColor {
			get { return null; }
		}
		
		IEnumerable<HighlightingColor> IHighlighter.GetColorStack(int lineNumber)
		{
			return Enumerable.Empty<HighlightingColor>();
		}
		
		public HighlightedLine HighlightLine(int lineNumber)
		{
			return lines[lineNumber - 1];
		}
		
		void IHighlighter.UpdateHighlightingState(int lineNumber)
		{
		}
		
		void IHighlighter.BeginHighlighting()
		{
		}
		
		void IHighlighter.EndHighlighting()
		{
		}
		
		HighlightingColor IHighlighter.GetNamedColor(string name)
		{
			return null;
		}
		
		void IDisposable.Dispose()
		{
		}
	}
}
