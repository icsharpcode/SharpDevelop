// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Editing
{
	/// <summary>
	/// Base class for selections.
	/// </summary>
	public abstract class Selection
	{
		/// <summary>
		/// Gets the empty selection.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification="Empty selection is immutable")]
		public static readonly Selection Empty = new SimpleSelection(-1, -1);
		
		/// <summary>
		/// Gets the selected text segments.
		/// </summary>
		public abstract IEnumerable<ISegment> Segments { get; }
		
		/// <summary>
		/// Gets the smallest segment that contains all segments in this selection.
		/// Returns null if the selection is empty.
		/// </summary>
		public abstract ISegment SurroundingSegment { get; }
		
		/// <summary>
		/// Removes the selected text from the document.
		/// </summary>
		public abstract void RemoveSelectedText(TextArea textArea);
		
		/// <summary>
		/// Updates the selection when the document changes.
		/// </summary>
		public abstract Selection UpdateOnDocumentChange(DocumentChangeEventArgs e);
		
		/// <summary>
		/// Gets whether the selection is empty.
		/// </summary>
		public virtual bool IsEmpty {
			get { return Length == 0; }
		}
		
		/// <summary>
		/// Gets the selection length.
		/// </summary>
		public abstract int Length { get; }
		
		/// <summary>
		/// Returns a new selection with the changed end point.
		/// </summary>
		/// <exception cref="NotSupportedException">Cannot set endpoint for empty selection</exception>
		public abstract Selection SetEndpoint(int newEndOffset);
		
		/// <summary>
		/// If this selection is empty, starts a new selection from <paramref name="startOffset"/> to
		/// <paramref name="newEndOffset"/>, otherwise, changes the endpoint of this selection.
		/// </summary>
		public virtual Selection StartSelectionOrSetEndpoint(int startOffset, int newEndOffset)
		{
			if (IsEmpty)
				return new SimpleSelection(startOffset, newEndOffset);
			else
				return SetEndpoint(newEndOffset);
		}
		
		/// <summary>
		/// Gets whether the selection is multi-line.
		/// </summary>
		public virtual bool IsMultiline(TextDocument document)
		{
			if (document == null)
				throw new ArgumentNullException("document");
			ISegment surroundingSegment = this.SurroundingSegment;
			if (surroundingSegment == null)
				return false;
			int start = surroundingSegment.Offset;
			int end = start + surroundingSegment.Length;
			return document.GetLineByOffset(start) != document.GetLineByOffset(end);
		}
		
		/// <summary>
		/// Gets the selected text.
		/// </summary>
		public virtual string GetText(TextDocument document)
		{
			if (document == null)
				throw new ArgumentNullException("document");
			StringBuilder b = null;
			string text = null;
			foreach (ISegment s in Segments) {
				if (text != null) {
					if (b == null)
						b = new StringBuilder(text);
					else
						b.Append(text);
				}
				text = document.GetText(s);
			}
			if (b != null) {
				if (text != null) b.Append(text);
				return b.ToString();
			} else {
				return text ?? string.Empty;
			}
		}
		
		/// <inheritdoc/>
		public abstract override bool Equals(object obj);
		
		/// <inheritdoc/>
		public abstract override int GetHashCode();
		
		/// <summary>
		/// Gets whether the specified offset is included in the selection.
		/// </summary>
		/// <returns>True, if the selection contains the offset (selection borders inclusive);
		/// otherwise, false.</returns>
		public bool Contains(int offset)
		{
			if (this.IsEmpty)
				return false;
			if (this.SurroundingSegment.Contains(offset)) {
				foreach (ISegment s in this.Segments) {
					if (s.Contains(offset)) {
						return true;
					}
				}
			}
			return false;
		}
	}
	
	/// <summary>
	/// A simple selection.
	/// </summary>
	public sealed class SimpleSelection : Selection, ISegment
	{
		readonly int startOffset, endOffset;
		
		/// <summary>
		/// Creates a new SimpleSelection instance.
		/// </summary>
		public SimpleSelection(int startOffset, int endOffset)
		{
			this.startOffset = startOffset;
			this.endOffset = endOffset;
		}
		
		/// <summary>
		/// Creates a new SimpleSelection instance.
		/// </summary>
		public SimpleSelection(ISegment segment)
		{
			if (segment == null)
				throw new ArgumentNullException("segment");
			this.startOffset = segment.Offset;
			this.endOffset = startOffset + segment.Length;
		}
		
		/// <inheritdoc/>
		public override IEnumerable<ISegment> Segments {
			get {
				if (!IsEmpty) {
					return ExtensionMethods.Sequence<ISegment>(this);
				} else {
					return Empty<ISegment>.Array;
				}
			}
		}
		
		/// <inheritdoc/>
		public override ISegment SurroundingSegment {
			get {
				if (IsEmpty)
					return null;
				else
					return this;
			}
		}
		
		/// <inheritdoc/>
		public override void RemoveSelectedText(TextArea textArea)
		{
			if (!IsEmpty) {
				var segmentsToDelete = textArea.ReadOnlySectionProvider.GetDeletableSegments(this).ToList();
				using (textArea.Document.RunUpdate()) {
					for (int i = segmentsToDelete.Count - 1; i >= 0; i--) {
						textArea.Document.Remove(segmentsToDelete[i].Offset, segmentsToDelete[i].Length);
					}
				}
			}
		}
		
		/// <summary>
		/// Gets the start offset.
		/// </summary>
		public int StartOffset {
			get { return startOffset; }
		}
		
		/// <summary>
		/// Gets the end offset.
		/// </summary>
		public int EndOffset {
			get { return endOffset; }
		}
		
		/// <inheritdoc/>
		public override Selection UpdateOnDocumentChange(DocumentChangeEventArgs e)
		{
			if (e == null)
				throw new ArgumentNullException("e");
			return new SimpleSelection(
				e.GetNewOffset(startOffset, AnchorMovementType.AfterInsertion),
				e.GetNewOffset(endOffset, AnchorMovementType.AfterInsertion)
			);
		}
		
		/// <inheritdoc/>
		public override bool IsEmpty {
			get { return startOffset == endOffset; }
		}
		
		// For segments, Offset must be less than or equal to EndOffset;
		// so we must use Min/Max.
		int ISegment.Offset {
			get { return Math.Min(startOffset, endOffset); }
		}
		
		int ISegment.EndOffset {
			get { return Math.Max(startOffset, endOffset); }
		}
		
		/// <inheritdoc/>
		public override int Length {
			get {
				return Math.Abs(endOffset - startOffset);
			}
		}
		
		/// <inheritdoc/>
		public override Selection SetEndpoint(int newEndOffset)
		{
			if (IsEmpty)
				throw new NotSupportedException();
			else
				return new SimpleSelection(startOffset, newEndOffset);
		}
		
		/// <inheritdoc/>
		public override int GetHashCode()
		{
			return startOffset ^ endOffset;
		}
		
		/// <inheritdoc/>
		public override bool Equals(object obj)
		{
			SimpleSelection other = obj as SimpleSelection;
			if (other == null) return false;
			if (IsEmpty && other.IsEmpty)
				return true;
			return this.startOffset == other.startOffset && this.endOffset == other.endOffset;
		}
		
		/// <inheritdoc/>
		public override string ToString()
		{
			return "[SimpleSelection Start=" + startOffset + " End=" + endOffset + "]";
		}
	}
}
