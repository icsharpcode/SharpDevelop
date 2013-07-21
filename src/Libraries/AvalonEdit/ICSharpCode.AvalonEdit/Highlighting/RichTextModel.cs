// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Highlighting
{
	/// <summary>
	/// Stores rich-text formatting.
	/// </summary>
	public sealed class RichTextModel : AbstractFreezable
	{
		List<int> stateChangeOffsets = new List<int>();
		List<HighlightingColor> stateChanges = new List<HighlightingColor>();
		
		int GetIndexForOffset(int offset)
		{
			if (offset < 0)
				throw new ArgumentOutOfRangeException("offset");
			int index = stateChangeOffsets.BinarySearch(offset);
			if (index < 0) {
				// If no color change exists directly at offset,
				// create a new one.
				index = ~index;
				stateChanges.Insert(index, stateChanges[index - 1].Clone());
				stateChangeOffsets.Insert(index, offset);
			}
			return index;
		}
		
		int GetIndexForOffsetUseExistingSegment(int offset)
		{
			if (offset < 0)
				throw new ArgumentOutOfRangeException("offset");
			int index = stateChangeOffsets.BinarySearch(offset);
			if (index < 0) {
				// If no color change exists directly at offset,
				// return the index of the color segment that contains offset.
				index = ~index - 1;
			}
			return index;
		}
		
		int GetEnd(int index)
		{
			// Gets the end of the color segment no. index.
			if (index + 1 < stateChangeOffsets.Count)
				return stateChangeOffsets[index + 1];
			else
				return int.MaxValue;
		}
		
		/// <summary>
		/// Creates a new RichTextModel.
		/// </summary>
		public RichTextModel()
		{
			stateChangeOffsets.Add(0);
			stateChanges.Add(new HighlightingColor());
		}
		
		/// <summary>
		/// Creates a RichTextModel from a CONTIGUOUS list of HighlightedSections.
		/// </summary>
		internal RichTextModel(int[] stateChangeOffsets, HighlightingColor[] stateChanges)
		{
			this.stateChangeOffsets.AddRange(stateChangeOffsets);
			this.stateChanges.AddRange(stateChanges);
		}
		
		#region UpdateOffsets
		/// <summary>
		/// Updates the start and end offsets of all segments stored in this collection.
		/// </summary>
		/// <param name="e">TextChangeEventArgs instance describing the change to the document.</param>
		public void UpdateOffsets(TextChangeEventArgs e)
		{
			if (e == null)
				throw new ArgumentNullException("e");
			for (int i = 0; i < stateChangeOffsets.Count; i++) {
				stateChangeOffsets[i] = e.GetNewOffset(stateChangeOffsets[i]);
			}
		}
		
		/// <summary>
		/// Updates the start and end offsets of all segments stored in this collection.
		/// </summary>
		/// <param name="change">OffsetChangeMapEntry instance describing the change to the document.</param>
		public void UpdateOffsets(OffsetChangeMapEntry change)
		{
			for (int i = 0; i < stateChangeOffsets.Count; i++) {
				stateChangeOffsets[i] = change.GetNewOffset(stateChangeOffsets[i]);
			}
		}
		#endregion
		
		/// <summary>
		/// Gets a copy of the HighlightingColor for the specified offset.
		/// </summary>
		public HighlightingColor GetHighlightingAt(int offset)
		{
			return stateChanges[GetIndexForOffsetUseExistingSegment(offset)].Clone();
		}
		
		/// <summary>
		/// Applies the HighlightingColor to the specified range of text.
		/// If the color specifies <c>null</c> for some properties, existing highlighting is preserved.
		/// </summary>
		public void ApplyHighlighting(int offset, int length, HighlightingColor color)
		{
			if (color == null || color.IsEmptyForMerge) {
				// Optimization: don't split the HighlightingState when we're not changing
				// any property. For example, the "Punctuation" color in C# is
				// empty by default.
				return;
			}
			int startIndex = GetIndexForOffset(offset);
			int endIndex = GetIndexForOffset(offset + length);
			for (int i = startIndex; i < endIndex; i++) {
				stateChanges[i].MergeWith(color);
			}
		}
		
		/// <summary>
		/// Sets the HighlightingColor for the specified range of text,
		/// completely replacing the existing highlighting in that area.
		/// </summary>
		public void SetHighlighting(int offset, int length, HighlightingColor color)
		{
			if (length <= 0)
				return;
			int startIndex = GetIndexForOffset(offset);
			int endIndex = GetIndexForOffset(offset + length);
			stateChanges[startIndex] = color != null ? color.Clone() : new HighlightingColor();
			stateChanges.RemoveRange(startIndex + 1, endIndex - (startIndex + 1));
			stateChangeOffsets.RemoveRange(startIndex + 1, endIndex - (startIndex + 1));
		}
		
		/// <summary>
		/// Sets the foreground brush on the specified text segment.
		/// </summary>
		public void SetForeground(int offset, int length, HighlightingBrush brush)
		{
			int startIndex = GetIndexForOffset(offset);
			int endIndex = GetIndexForOffset(offset + length);
			for (int i = startIndex; i < endIndex; i++) {
				stateChanges[i].Foreground = brush;
			}
		}
		
		/// <summary>
		/// Sets the background brush on the specified text segment.
		/// </summary>
		public void SetBackground(int offset, int length, HighlightingBrush brush)
		{
			int startIndex = GetIndexForOffset(offset);
			int endIndex = GetIndexForOffset(offset + length);
			for (int i = startIndex; i < endIndex; i++) {
				stateChanges[i].Background = brush;
			}
		}
		
		/// <summary>
		/// Sets the font weight on the specified text segment.
		/// </summary>
		public void SetFontWeight(int offset, int length, FontWeight weight)
		{
			int startIndex = GetIndexForOffset(offset);
			int endIndex = GetIndexForOffset(offset + length);
			for (int i = startIndex; i < endIndex; i++) {
				stateChanges[i].FontWeight = weight;
			}
		}
		
		/// <summary>
		/// Sets the font style on the specified text segment.
		/// </summary>
		public void SetFontStyle(int offset, int length, FontStyle style)
		{
			int startIndex = GetIndexForOffset(offset);
			int endIndex = GetIndexForOffset(offset + length);
			for (int i = startIndex; i < endIndex; i++) {
				stateChanges[i].FontStyle = style;
			}
		}
		
		/// <summary>
		/// Retrieves the highlighted sections in the specified range.
		/// The highlighted sections will be sorted by offset, and there will not be any nested or overlapping sections.
		/// </summary>
		public IEnumerable<HighlightedSection> GetHighlightedSections(int offset, int length)
		{
			int index = GetIndexForOffsetUseExistingSegment(offset);
			int pos = offset;
			int endOffset = offset + length;
			while (pos < endOffset) {
				int endPos = Math.Min(endOffset, GetEnd(index));
				yield return new HighlightedSection {
					Offset = pos,
					Length = endPos - pos,
					Color = stateChanges[index].Clone()
				};
				pos = endPos;
				index++;
			}
		}
	}
}
