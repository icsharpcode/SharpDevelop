// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Highlighting
{
	/// <summary>
	/// Stores rich-text formatting.
	/// </summary>
	public sealed class RichTextModel // TODO: maybe rename to HighlightingModel?
	{
		CompressingTreeList<HighlightingColor> list = new CompressingTreeList<HighlightingColor>(object.Equals);
		
		/// <summary>
		/// Gets the length of the document.
		/// This has an effect on which coordinates are valid for this RichTextModel.
		/// </summary>
		public int DocumentLength {
			get { return list.Count; }
		}
		
		/// <summary>
		/// Creates a new RichTextModel that needs manual calls to <see cref="UpdateOffsets(DocumentChangeEventArgs)"/>.
		/// </summary>
		public RichTextModel(int documentLength)
		{
			list.InsertRange(0, documentLength, HighlightingColor.Empty);
		}
		
		/// <summary>
		/// Creates a RichTextModel from a CONTIGUOUS list of HighlightedSections.
		/// </summary>
		internal RichTextModel(IEnumerable<HighlightedSection> sections)
		{
			foreach (var section in sections) {
				list.InsertRange(section.Offset, section.Length, section.Color);
			}
		}
		
		#region UpdateOffsets
		/// <summary>
		/// Updates the start and end offsets of all segments stored in this collection.
		/// </summary>
		/// <param name="e">DocumentChangeEventArgs instance describing the change to the document.</param>
		public void UpdateOffsets(DocumentChangeEventArgs e)
		{
			if (e == null)
				throw new ArgumentNullException("e");
			OffsetChangeMap map = e.OffsetChangeMapOrNull;
			if (map != null) {
				foreach (OffsetChangeMapEntry entry in map) {
					UpdateOffsetsInternal(entry);
				}
			} else {
				UpdateOffsetsInternal(e.CreateSingleChangeMapEntry());
			}
		}
		
		/// <summary>
		/// Updates the start and end offsets of all segments stored in this collection.
		/// </summary>
		/// <param name="change">OffsetChangeMapEntry instance describing the change to the document.</param>
		public void UpdateOffsets(OffsetChangeMapEntry change)
		{
			UpdateOffsetsInternal(change);
		}
		
		void UpdateOffsetsInternal(OffsetChangeMapEntry entry)
		{
			HighlightingColor color;
			if (entry.RemovalLength > 0) {
				color = list[entry.Offset];
				list.RemoveRange(entry.Offset, entry.RemovalLength);
			} else if (list.Count > 0) {
				color = list[Math.Max(0, entry.Offset - 1)];
			} else {
				color = HighlightingColor.Empty;
			}
			list.InsertRange(entry.Offset, entry.InsertionLength, color);
		}
		#endregion
		
		/// <summary>
		/// Gets the HighlightingColor for the specified offset.
		/// </summary>
		public HighlightingColor GetHighlightingAt(int offset)
		{
			return list[offset];
		}
		
		/// <summary>
		/// Applies the HighlightingColor to the specified range of text.
		/// If the color specifies <c>null</c> for some properties, existing highlighting is preserved.
		/// </summary>
		public void ApplyHighlighting(int offset, int length, HighlightingColor color)
		{
			list.TransformRange(offset, length, c => {
			                    	var newColor = c.Clone();
			                    	newColor.MergeWith(color);
			                    	newColor.Freeze();
			                    	return newColor;
			                    });
		}
		
		/// <summary>
		/// Sets the HighlightingColor for the specified range of text,
		/// completely replacing the existing highlighting in that area.
		/// </summary>
		public void SetHighlighting(int offset, int length, HighlightingColor color)
		{
			list.SetRange(offset, length, FreezableHelper.GetFrozenClone(color));
		}
		
		/// <summary>
		/// Retrieves the highlighted sections in the specified range.
		/// The highlighted sections will be sorted by offset, and there will not be any nested or overlapping sections.
		/// </summary>
		public IEnumerable<HighlightedSection> GetHighlightedSections(int offset, int length)
		{
			int pos = offset;
			int endOffset = offset + length;
			while (pos < endOffset) {
				int endPos = Math.Min(endOffset, list.GetEndOfRun(pos));
				yield return new HighlightedSection {
					Offset = pos,
					Length = endPos - pos,
					Color = list[pos]
				};
				pos = endPos;
			}
		}
		
		#region WriteDocumentTo
		/// <summary>
		/// Writes the specified document, with the formatting from this rich text model applied,
		/// to the RichTextWriter.
		/// </summary>
		public void WriteDocumentTo(ITextSource document, RichTextWriter writer)
		{
			WriteDocumentTo(document, new SimpleSegment(0, DocumentLength), writer);
		}
		
		/// <summary>
		/// Writes a segment of the specified document, with the formatting from this rich text model applied,
		/// to the RichTextWriter.
		/// </summary>
		public void WriteDocumentTo(ITextSource document, ISegment segment, RichTextWriter writer)
		{
			if (document == null)
				throw new ArgumentNullException("document");
			if (segment == null)
				throw new ArgumentNullException("segment");
			if (writer == null)
				throw new ArgumentNullException("writer");
			
			int pos = segment.Offset;
			int endOffset = segment.EndOffset;
			while (pos < endOffset) {
				int endPos = Math.Min(endOffset, list.GetEndOfRun(pos));
				writer.BeginSpan(list[pos]);
				document.WriteTextTo(writer, pos, endPos - pos);
				writer.EndSpan();
				pos = endPos;
			}
		}
		#endregion
	}
}
