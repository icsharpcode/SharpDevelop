// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.Editor;

namespace ICSharpCode.AvalonEdit.Document
{
	/// <summary>
	/// Describes a change of the document text.
	/// This class is thread-safe.
	/// </summary>
	[Serializable]
	public class DocumentChangeEventArgs : TextChangeEventArgs
	{
		volatile OffsetChangeMap offsetChangeMap;
		
		/// <summary>
		/// Gets the OffsetChangeMap associated with this document change.
		/// </summary>
		/// <remarks>The OffsetChangeMap instance is guaranteed to be frozen and thus thread-safe.</remarks>
		public OffsetChangeMap OffsetChangeMap {
			get {
				OffsetChangeMap map = offsetChangeMap;
				if (map == null) {
					// create OffsetChangeMap on demand
					map = OffsetChangeMap.FromSingleElement(CreateSingleChangeMapEntry());
					offsetChangeMap = map;
				}
				return map;
			}
		}
		
		internal OffsetChangeMapEntry CreateSingleChangeMapEntry()
		{
			return new OffsetChangeMapEntry(this.Offset, this.RemovalLength, this.InsertionLength);
		}
		
		/// <summary>
		/// Gets the OffsetChangeMap, or null if the default offset map (=single replacement) is being used.
		/// </summary>
		internal OffsetChangeMap OffsetChangeMapOrNull {
			get {
				return offsetChangeMap;
			}
		}
		
		/// <summary>
		/// Gets the new offset where the specified offset moves after this document change.
		/// </summary>
		public int GetNewOffset(int offset, AnchorMovementType movementType)
		{
			if (offsetChangeMap != null)
				return offsetChangeMap.GetNewOffset(offset, movementType);
			else
				return CreateSingleChangeMapEntry().GetNewOffset(offset, movementType);
		}
		
		/// <summary>
		/// Creates a new DocumentChangeEventArgs object.
		/// </summary>
		public DocumentChangeEventArgs(int offset, string removedText, string insertedText)
			: this(offset, removedText, insertedText, null)
		{
		}
		
		/// <summary>
		/// Creates a new DocumentChangeEventArgs object.
		/// </summary>
		public DocumentChangeEventArgs(int offset, string removedText, string insertedText, OffsetChangeMap offsetChangeMap)
			: base(offset, removedText, insertedText)
		{
			ThrowUtil.CheckNotNegative(offset, "offset");
			ThrowUtil.CheckNotNull(removedText, "removedText");
			ThrowUtil.CheckNotNull(insertedText, "insertedText");
			
			if (offsetChangeMap != null) {
				if (!offsetChangeMap.IsFrozen)
					throw new ArgumentException("The OffsetChangeMap must be frozen before it can be used in DocumentChangeEventArgs");
				if (!offsetChangeMap.IsValidForDocumentChange(offset, removedText.Length, insertedText.Length))
					throw new ArgumentException("OffsetChangeMap is not valid for this document change", "offsetChangeMap");
				this.offsetChangeMap = offsetChangeMap;
			}
		}
		
		/// <summary>
		/// Creates DocumentChangeEventArgs for the reverse change.
		/// </summary>
		public DocumentChangeEventArgs Invert()
		{
			OffsetChangeMap map = this.OffsetChangeMapOrNull;
			if (map != null) {
				map = map.Invert();
				map.Freeze();
			}
			return new DocumentChangeEventArgs(this.Offset, this.InsertedText, this.RemovedText, map);
		}
	}
}
