// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Editing
{
	/// <summary>
	/// Rectangular selection.
	/// </summary>
	public sealed class RectangleSelection : Selection
	{
		TextDocument document;
		
		/// <summary>
		/// Gets the start position of the selection.
		/// </summary>
		public int StartOffset { get; private set; }
		
		/// <summary>
		/// Gets the end position of the selection.
		/// </summary>
		public int EndOffset { get; private set; }
		
		/// <summary>
		/// Creates a new rectangular selection.
		/// </summary>
		public RectangleSelection(TextDocument document, int start, int end)
		{
			if (document == null)
				throw new ArgumentNullException("document");
			this.document = document;
			this.StartOffset = start;
			this.EndOffset = end;
		}
		
		/// <inheritdoc/>
		public override bool IsEmpty {
			get {
				TextLocation start = document.GetLocation(StartOffset);
				TextLocation end = document.GetLocation(EndOffset);
				return start.Column == end.Column;
			}
		}
		
		/// <inheritdoc/>
		public override bool Contains(int offset)
		{
			if (StartOffset <= offset && offset <= EndOffset) {
				foreach (ISegment s in this.Segments) {
					if (s.Contains(offset))
						return true;
				}
			}
			return false;
		}
		
		/// <inheritdoc/>
		public override Selection StartSelectionOrSetEndpoint(int startOffset, int newEndOffset)
		{
			return SetEndpoint(newEndOffset);
		}
		
		/// <inheritdoc/>
		public override int Length {
			get {
				return this.Segments.Sum(s => s.Length);
			}
		}
		
		/// <inheritdoc/>
		public override ISegment SurroundingSegment {
			get {
				return new SimpleSegment(Math.Min(StartOffset, EndOffset), Math.Abs(EndOffset - StartOffset));
			}
		}
		
		/// <inheritdoc/>
		public override IEnumerable<ISegment> Segments {
			get {
				TextLocation start = document.GetLocation(StartOffset);
				TextLocation end = document.GetLocation(EndOffset);
				DocumentLine line = document.GetLineByNumber(Math.Min(start.Line, end.Line));
				int numberOfLines = Math.Abs(start.Line - end.Line);
				int startCol = Math.Min(start.Column, end.Column);
				int endCol = Math.Max(start.Column, end.Column);
				for (int i = 0; i <= numberOfLines; i++) {
					if (line.Length + 1 >= startCol) {
						int thisLineEndCol = Math.Min(endCol, line.Length + 1);
						yield return new SimpleSegment(line.Offset + startCol - 1, thisLineEndCol - startCol);
					}
					line = line.NextLine;
				}
			}
		}
		
		/// <inheritdoc/>
		public override bool Equals(object obj)
		{
			RectangleSelection r = obj as RectangleSelection;
			return r != null && r.document == this.document && r.StartOffset == this.StartOffset && r.EndOffset == this.EndOffset;
		}
		
		/// <inheritdoc/>
		public override int GetHashCode()
		{
			return StartOffset ^ EndOffset;
		}
		
		/// <inheritdoc/>
		public override Selection SetEndpoint(int newEndOffset)
		{
			return new RectangleSelection(this.document, this.StartOffset, newEndOffset);
		}
		
		/// <inheritdoc/>
		public override Selection UpdateOnDocumentChange(DocumentChangeEventArgs e)
		{
			return Selection.Empty;
		}
		
		/// <inheritdoc/>
		public override void ReplaceSelectionWithText(TextArea textArea, string newText)
		{
			if (textArea == null)
				throw new ArgumentNullException("textArea");
			if (newText == null)
				throw new ArgumentNullException("newText");
			using (textArea.Document.RunUpdate()) {
				TextLocation start = document.GetLocation(StartOffset);
				TextLocation end = document.GetLocation(EndOffset);
				int editColumn = Math.Min(start.Column, end.Column);
				foreach (ISegment lineSegment in this.Segments.Reverse()) {
					if (lineSegment.Length == 0) {
						if (newText.Length > 0 && textArea.ReadOnlySectionProvider.CanInsert(lineSegment.Offset)) {
							textArea.Document.Insert(lineSegment.Offset, newText);
						}
					} else {
						var segmentsToDelete = textArea.ReadOnlySectionProvider.GetDeletableSegments(lineSegment).ToList();
						for (int i = segmentsToDelete.Count - 1; i >= 0; i--) {
							if (i == segmentsToDelete.Count - 1) {
								textArea.Document.Replace(segmentsToDelete[i], newText);
							} else {
								textArea.Document.Remove(segmentsToDelete[i]);
							}
						}
					}
				}
				if (NewLineFinder.NextNewLine(newText, 0) == SimpleSegment.Invalid) {
					TextLocation newStart = new TextLocation(start.Line, editColumn + newText.Length);
					TextLocation newEnd = new TextLocation(end.Line, editColumn + newText.Length);
					textArea.Caret.Location = newEnd;
					textArea.Selection = new RectangleSelection(document, document.GetOffset(newStart), document.GetOffset(newEnd));
				} else {
					textArea.Selection = Selection.Empty;
				}
			}
		}
		
		/// <inheritdoc/>
		public override string ToString()
		{
			return "[RectangleSelection " + document.GetLocation(StartOffset) + " to " + document.GetLocation(EndOffset) + "]";
		}
	}
}
