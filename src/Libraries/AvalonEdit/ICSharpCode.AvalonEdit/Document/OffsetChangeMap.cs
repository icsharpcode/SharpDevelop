// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.AvalonEdit.Utils;
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
		/// Normal replace.
		/// Anchors in front of the replaced region will stay in front, anchors after the replaced region will stay after.
		/// Anchors in the middle of the removed region will move depending on their AnchorMovementType.
		/// </summary>
		/// <remarks>
		/// This is the default implementation of DocumentChangeEventArgs when OffsetChangeMap is null,
		/// so using this option usually works without creating an OffsetChangeMap instance.
		/// This is equivalent to an OffsetChangeMap with a single entry describing the replace operation.
		/// </remarks>
		Normal,
		/// <summary>
		/// First the old text is removed, then the new text is inserted.
		/// Anchors immediately in front (or after) the replaced region may move to the other side of the insertion,
		/// depending on the AnchorMovementType.
		/// </summary>
		/// <remarks>
		/// This is implemented as an OffsetChangeMap with two entries: the removal, and the insertion.
		/// </remarks>
		RemoveAndInsert,
		/// <summary>
		/// The text is replaced character-by-character.
		/// Anchors keep their position inside the replaced text.
		/// Anchors after the replaced region will move accordingly if the replacement text has a different length than the replaced text.
		/// If the new text is shorter than the old text, anchors inside the old text that would end up behind the replacement text
		/// will be moved so that they point to the end of the replacement text.
		/// </summary>
		/// <remarks>
		/// On the OffsetChangeMap level, growing text is implemented by replacing the last character in the replaced text
		/// with itself and the additional text segment. A simple insertion of the additional text would have the undesired
		/// effect of moving anchors immediately after the replaced text into the replacement text if they used
		/// AnchorMovementStyle.BeforeInsertion.
		/// Shrinking text is implemented by removal the text segment that's too long.
		/// If the text keeps its old size, this is implemented as OffsetChangeMap.Empty.
		/// </remarks>
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
				endOffset += entry.InsertionLength - entry.RemovalLength;
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
				// swap InsertionLength and RemovalLength
				newMap.Add(new OffsetChangeMapEntry(entry.Offset, entry.InsertionLength, entry.RemovalLength));
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
		readonly int removalLength;
		readonly int insertionLength;
		
		/// <summary>
		/// The offset at which the change occurs.
		/// </summary>
		public int Offset {
			get { return offset; }
		}
		
		/// <summary>
		/// The number of characters removed.
		/// Returns 0 if this entry represents an insertion.
		/// </summary>
		public int RemovalLength { 
			get { return removalLength; }
		}
		
		/// <summary>
		/// The number of characters inserted.
		/// Returns 0 if this entry represents a removal.
		/// </summary>
		public int InsertionLength {
			get { return insertionLength; }
		}
		
		/// <summary>
		/// Gets the new offset where the specified offset moves after this document change.
		/// </summary>
		public int GetNewOffset(int oldOffset, AnchorMovementType movementType)
		{
			if (!(this.removalLength == 0 && oldOffset == this.offset)) {
				// we're getting trouble (both if statements in here would apply)
				// if there's no removal and we insert at the offset
				// -> we'd need to disambiguate by movementType, which is handled after the if
				
				// offset is before start of change: no movement
				if (oldOffset <= this.offset)
					return oldOffset;
				// offset is after end of change: movement by normal delta
				if (oldOffset >= this.offset + this.removalLength)
					return oldOffset + this.insertionLength - this.removalLength;
			}
			// we reach this point if
			// a) the oldOffset is inside the deleted segment
			// b) there was no removal and we insert at the caret position
			if (movementType == AnchorMovementType.AfterInsertion)
				return this.offset + this.insertionLength;
			else
				return this.offset;
		}
		
		/// <summary>
		/// Creates a new OffsetChangeMapEntry instance.
		/// </summary>
		public OffsetChangeMapEntry(int offset, int removalLength, int insertionLength)
		{
			ThrowUtil.CheckNotNegative(offset, "offset");
			ThrowUtil.CheckNotNegative(removalLength, "removalLength");
			ThrowUtil.CheckNotNegative(insertionLength, "insertionLength");
			
			this.offset = offset;
			this.removalLength = removalLength;
			this.insertionLength = insertionLength;
		}
	}
}
