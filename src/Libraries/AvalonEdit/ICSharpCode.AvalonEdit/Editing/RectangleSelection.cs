// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.TextFormatting;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Editing
{
	/// <summary>
	/// Rectangular selection ("box selection").
	/// </summary>
	public sealed class RectangleSelection : Selection
	{
		TextDocument document;
		readonly int startLine, endLine;
		readonly double startXPos, endXPos;
		readonly int startOffset, endOffset;
		
		readonly List<SelectionSegment> segments = new List<SelectionSegment>();
		
		void InitDocument()
		{
			document = textArea.Document;
			if (document == null)
				throw ThrowUtil.NoDocumentAssigned();
		}
		
		/// <summary>
		/// Creates a new rectangular selection.
		/// </summary>
		public RectangleSelection(TextArea textArea, TextViewPosition start, TextViewPosition end)
			: base(textArea)
		{
			InitDocument();
			this.startLine = start.Line;
			this.endLine = end.Line;
			this.startXPos = GetXPos(start);
			this.endXPos = GetXPos(end);
			this.startOffset = document.GetOffset(start);
			this.endOffset = document.GetOffset(end);
			CalculateSegments();
		}
		
		private RectangleSelection(TextArea textArea, int startLine, double startXPos, int startOffset, TextViewPosition end)
			: base(textArea)
		{
			InitDocument();
			this.startLine = startLine;
			this.endLine = end.Line;
			this.startXPos = startXPos;
			this.endXPos = GetXPos(end);
			this.startOffset = startOffset;
			this.endOffset = document.GetOffset(end);
			CalculateSegments();
		}
		
		private RectangleSelection(TextArea textArea, TextViewPosition start, int endLine, double endXPos, int endOffset)
			: base(textArea)
		{
			InitDocument();
			this.startLine = start.Line;
			this.endLine = endLine;
			this.startXPos = GetXPos(start);
			this.endXPos = endXPos;
			this.startOffset = document.GetOffset(start);
			this.endOffset = endOffset;
			CalculateSegments();
		}
		
		double GetXPos(TextViewPosition pos)
		{
			DocumentLine documentLine = document.GetLineByNumber(pos.Line);
			VisualLine visualLine = textArea.TextView.GetOrConstructVisualLine(documentLine);
			TextLine textLine = visualLine.GetTextLine(pos.VisualColumn);
			return visualLine.GetTextLineVisualXPosition(textLine, pos.VisualColumn);
		}
		
		/// <inheritdoc/>
		public override string GetText()
		{
			StringBuilder b = new StringBuilder();
			foreach (ISegment s in this.Segments) {
				if (b.Length > 0)
					b.AppendLine();
				b.Append(document.GetText(s));
			}
			return b.ToString();
		}
		
		/// <inheritdoc/>
		public override Selection StartSelectionOrSetEndpoint(TextViewPosition startPosition, TextViewPosition endPosition)
		{
			return SetEndpoint(endPosition);
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
				return new SimpleSegment(Math.Min(startOffset, endOffset), Math.Abs(endOffset - startOffset));
			}
		}
		
		/// <inheritdoc/>
		public override IEnumerable<SelectionSegment> Segments {
			get { return segments; }
		}
		
		void CalculateSegments()
		{
			DocumentLine nextLine = document.GetLineByNumber(Math.Min(startLine, endLine));
			do {
				VisualLine vl = textArea.TextView.GetOrConstructVisualLine(nextLine);
				int startVC = vl.GetVisualColumn(new Point(startXPos, 0), true);
				int endVC = vl.GetVisualColumn(new Point(endXPos, 0), true);
				
				int baseOffset = vl.FirstDocumentLine.Offset;
				int startOffset = baseOffset + vl.GetRelativeOffset(startVC);
				int endOffset = baseOffset + vl.GetRelativeOffset(endVC);
				segments.Add(new SelectionSegment(startOffset, startVC, endOffset, endVC, true));
				
				nextLine = vl.LastDocumentLine.NextLine;
			} while (nextLine.LineNumber <= Math.Max(startLine, endLine));
		}
		
		/// <inheritdoc/>
		public override bool Equals(object obj)
		{
			RectangleSelection r = obj as RectangleSelection;
			return r != null && r.textArea == this.textArea
				&& r.startOffset == this.startOffset && r.endOffset == this.endOffset
				&& r.startLine == this.startLine && r.endLine == this.endLine
				&& r.startXPos == this.startXPos && r.endXPos == this.endXPos;
		}
		
		/// <inheritdoc/>
		public override int GetHashCode()
		{
			return startOffset ^ endOffset;
		}
		
		/// <inheritdoc/>
		public override Selection SetEndpoint(TextViewPosition endPosition)
		{
			return new RectangleSelection(textArea, startLine, startXPos, startOffset, endPosition);
		}
		
		/// <inheritdoc/>
		public override Selection UpdateOnDocumentChange(DocumentChangeEventArgs e)
		{
			throw new NotImplementedException();
//			return new RectangleSelection(textArea,
//			                              e.GetNewOffset(StartOffset, AnchorMovementType.AfterInsertion),
//			                              e.GetNewOffset(EndOffset, AnchorMovementType.BeforeInsertion));
		}
		
		/// <inheritdoc/>
		public override void ReplaceSelectionWithText(string newText)
		{
			throw new NotImplementedException(); /*
			if (newText == null)
				throw new ArgumentNullException("newText");
			using (textArea.Document.RunUpdate()) {
				TextLocation start = document.GetLocation(StartOffset);
				TextLocation end = document.GetLocation(EndOffset);
				int editColumn = Math.Min(start.Column, end.Column);
				if (NewLineFinder.NextNewLine(newText, 0) == SimpleSegment.Invalid) {
					// insert same text into every line
					foreach (ISegment lineSegment in this.Segments.Reverse()) {
						ReplaceSingleLineText(textArea, lineSegment, newText);
					}
					
					TextLocation newStart = new TextLocation(start.Line, editColumn + newText.Length);
					TextLocation newEnd = new TextLocation(end.Line, editColumn + newText.Length);
					textArea.Caret.Location = newEnd;
					textArea.Selection = new RectangleSelection(document, document.GetOffset(newStart), document.GetOffset(newEnd));
				} else {
					// convert all segment start/ends to anchors
					var segments = this.Segments.Select(s => new AnchorSegment(this.document, s)).ToList();
					SimpleSegment ds = NewLineFinder.NextNewLine(newText, 0);
					// we'll check whether all lines have the same length. If so, we can continue using a rectangular selection.
					int commonLength = -1;
					// now insert lines into rectangular selection
					int lastDelimiterEnd = 0;
					bool isAtEnd = false;
					int i;
					for (i = 0; i < segments.Count; i++) {
						string lineText;
						if (ds == SimpleSegment.Invalid || (i == segments.Count - 1)) {
							lineText = newText.Substring(lastDelimiterEnd);
							isAtEnd = true;
							// if we have more lines to insert than this selection is long, we cannot continue using a rectangular selection
							if (ds != SimpleSegment.Invalid)
								commonLength = -1;
						} else {
							lineText = newText.Substring(lastDelimiterEnd, ds.Offset - lastDelimiterEnd);
						}
						if (i == 0) {
							commonLength = lineText.Length;
						} else if (commonLength != lineText.Length) {
							commonLength = -1;
						}
						ReplaceSingleLineText(textArea, segments[i], lineText);
						if (isAtEnd)
							break;
						lastDelimiterEnd = ds.EndOffset;
						ds = NewLineFinder.NextNewLine(newText, lastDelimiterEnd);
					}
					if (commonLength >= 0) {
						TextLocation newStart = new TextLocation(start.Line, editColumn + commonLength);
						TextLocation newEnd = new TextLocation(start.Line + i, editColumn + commonLength);
						textArea.Selection = new RectangleSelection(document, document.GetOffset(newStart), document.GetOffset(newEnd));
					} else {
						textArea.Selection = Selection.Empty;
					}
				}
			}*/
		}
		
		static void ReplaceSingleLineText(TextArea textArea, ISegment lineSegment, string newText)
		{
			if (lineSegment.Length == 0) {
				if (newText.Length > 0 && textArea.ReadOnlySectionProvider.CanInsert(lineSegment.Offset)) {
					textArea.Document.Insert(lineSegment.Offset, newText);
				}
			} else {
				ISegment[] segmentsToDelete = textArea.GetDeletableSegments(lineSegment);
				for (int i = segmentsToDelete.Length - 1; i >= 0; i--) {
					if (i == segmentsToDelete.Length - 1) {
						textArea.Document.Replace(segmentsToDelete[i], newText);
					} else {
						textArea.Document.Remove(segmentsToDelete[i]);
					}
				}
			}
		}
		
		/// <summary>
		/// Performs a rectangular paste operation.
		/// </summary>
		public static bool PerformRectangularPaste(TextArea textArea, TextViewPosition startPosition, string text, bool selectInsertedText)
		{
			if (textArea == null)
				throw new ArgumentNullException("textArea");
			if (text == null)
				throw new ArgumentNullException("text");
			int newLineCount = text.Count(c => c == '\n');
			TextLocation endLocation = new TextLocation(startPosition.Line + newLineCount, startPosition.Column);
			if (endLocation.Line <= textArea.Document.LineCount) {
				int endOffset = textArea.Document.GetOffset(endLocation);
				if (textArea.Document.GetLocation(endOffset) == endLocation) {
					RectangleSelection rsel = new RectangleSelection(textArea, startPosition, new TextViewPosition(endLocation));
					rsel.ReplaceSelectionWithText(text);
					if (selectInsertedText && textArea.Selection is RectangleSelection) {
						RectangleSelection sel = (RectangleSelection)textArea.Selection;
						textArea.Selection = new RectangleSelection(textArea, startPosition, sel.endLine, sel.endXPos, sel.endOffset);
					}
					return true;
				}
			}
			return false;
		}
		
		/// <summary>
		/// Gets the name of the entry in the DataObject that signals rectangle selections.
		/// </summary>
		public const string RectangularSelectionDataType = "AvalonEditRectangularSelection";
		
		/// <inheritdoc/>
		public override System.Windows.DataObject CreateDataObject(TextArea textArea)
		{
			var data = base.CreateDataObject(textArea);
			
			MemoryStream isRectangle = new MemoryStream(1);
			isRectangle.WriteByte(1);
			data.SetData(RectangularSelectionDataType, isRectangle, false);
			return data;
		}
		
		/// <inheritdoc/>
		public override string ToString()
		{
			// It's possible that ToString() gets called on old (invalid) selections, e.g. for "change from... to..." debug message
			// make sure we don't crash even when the desired locations don't exist anymore.
			return "[RectangleSelection " + startOffset + " to " + endOffset + "]";
		}
	}
}
