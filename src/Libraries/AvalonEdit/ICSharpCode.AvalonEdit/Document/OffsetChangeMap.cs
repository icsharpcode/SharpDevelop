// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ICSharpCode.AvalonEdit.Document
{
	/// <summary>
	/// Contains predefined offset change mapping types.
	/// </summary>
	public enum OffsetChangeMappingType
	{
		/// <summary>
		/// First the old text is removed, then the new text is inserted.
		/// </summary>
		RemoveAndInsert,
		/// <summary>
		/// The text is replaced character-by-character.
		/// If the new text is longer than the old text, a single insertion at the end is used to account for the difference.
		/// If the new text is shorter than the old text, a single deletion at the end is used to account for the difference.
		/// </summary>
		CharacterReplace
	}
	
	/// <summary>
	/// Describes a series of offset changes.
	/// </summary>
	[Serializable]
	public sealed class OffsetChangeMap : Collection<OffsetChangeMapEntry>
	{
		/// <summary>
		/// Immutable OffsetChangeMap that is empty.
		/// </summary>
		public static readonly OffsetChangeMap Empty = new OffsetChangeMap(Utils.Empty<OffsetChangeMapEntry>.ReadOnlyCollection);
		
		/// <summary>
		/// Creates a new OffsetChangeMap instance.
		/// </summary>
		public OffsetChangeMap()
		{
		}
		
		/// <summary>
		/// Creates a new OffsetChangeMap instance.
		/// </summary>
		public OffsetChangeMap(int capacity)
			: base(new List<OffsetChangeMapEntry>(capacity))
		{
		}
		
		/// <summary>
		/// Private constructor for immutable 'Empty' instance.
		/// </summary>
		private OffsetChangeMap(IList<OffsetChangeMapEntry> entries)
			: base(entries)
		{
		}
		
		/// <summary>
		/// Gets the new offset where the specified offset moves after this document change.
		/// </summary>
		public int GetNewOffset(int offset, AnchorMovementType movementType)
		{
			foreach (OffsetChangeMapEntry entry in this) {
				offset = entry.GetNewOffset(offset, movementType);
			}
			return offset;
		}
		
		/// <summary>
		/// Gets whether this OffsetChangeMap is a valid explanation for the specified document change.
		/// </summary>
		public bool IsValidForDocumentChange(int offset, int removalLength, int insertionLength)
		{
			int endOffset = offset + removalLength;
			foreach (OffsetChangeMapEntry entry in this) {
				// check that ChangeMapEntry is in valid range for this document change
				if (entry.Offset < offset || entry.Offset + entry.RemovalLength > endOffset)
					return false;
				endOffset += entry.Delta;
			}
			// check that the total delta matches
			return endOffset == offset + insertionLength;
		}
		
		/// <summary>
		/// Calculates the inverted OffsetChangeMap (used for the undo operation).
		/// </summary>
		public OffsetChangeMap Invert()
		{
			if (this == Empty)
				return this;
			OffsetChangeMap newMap = new OffsetChangeMap(this.Count);
			for (int i = this.Count - 1; i >= 0; i--) {
				OffsetChangeMapEntry entry = this[i];
				newMap.Add(new OffsetChangeMapEntry(entry.Offset, -entry.Delta));
			}
			return newMap;
		}
	}
	
	/// <summary>
	/// An entry in the OffsetChangeMap.
	/// This represents the offset of a document change (either insertion or removal, not both at once).
	/// </summary>
	[Serializable]
	public struct OffsetChangeMapEntry
	{
		readonly int offset;
		readonly int delta;
		
		/// <summary>
		/// The offset at which the change occurs.
		/// </summary>
		public int Offset {
			get { return offset; }
		}
		
		/// <summary>
		/// The change delta. If positive, it is equal to InsertionLength; if negative, it is equal to RemovalLength.
		/// </summary>
		public int Delta {
			get { return delta; }
		}
		
		/// <summary>
		/// The number of characters removed.
		/// Returns 0 if this entry represents an insertion.
		/// </summary>
		public int RemovalLength { 
			get {
				return delta < 0 ? -delta : 0;
			}
		}
		
		/// <summary>
		/// The number of characters inserted.
		/// Returns 0 if this entry represents a removal.
		/// </summary>
		public int InsertionLength {
			get {
				return delta > 0 ? delta : 0;
			}
		}
		
		/// <summary>
		/// Gets the new offset where the specified offset moves after this document change.
		/// </summary>
		public int GetNewOffset(int oldOffset, AnchorMovementType movementType)
		{
			if (oldOffset < this.Offset)
				return oldOffset;
			if (oldOffset > this.Offset + this.RemovalLength)
				return oldOffset + this.Delta;
			// offset is inside removed region
			if (movementType == AnchorMovementType.AfterInsertion)
				return this.Offset + this.InsertionLength;
			else
				return this.Offset;
		}
		
		/// <summary>
		/// Creates a new OffsetChangeMapEntry instance.
		/// </summary>
		public OffsetChangeMapEntry(int offset, int delta)
		{
			this.offset = offset;
			this.delta = delta;
		}
	}
}
