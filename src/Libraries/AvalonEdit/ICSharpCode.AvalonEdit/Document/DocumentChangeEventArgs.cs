// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.AvalonEdit.Document
{
	/// <summary>
	/// Describes a change of the document text.
	/// </summary>
	[Serializable]
	public class DocumentChangeEventArgs : EventArgs
	{
		/// <summary>
		/// The offset at which the change occurs.
		/// </summary>
		public int Offset { get; private set; }
		
		/// <summary>
		/// The number of characters removed.
		/// </summary>
		public int RemovalLength { get; private set; }
		
		/// <summary>
		/// The text that was inserted.
		/// </summary>
		public string InsertedText { get; private set; }
		
		/// <summary>
		/// The number of characters inserted.
		/// </summary>
		public int InsertionLength {
			get { return InsertedText.Length; }
		}
		
		volatile OffsetChangeMap offsetChangeMap;
		
		/// <summary>
		/// Gets the OffsetChangeMap associated with this document change.
		/// </summary>
		public OffsetChangeMap OffsetChangeMap {
			get {
				OffsetChangeMap map = offsetChangeMap;
				if (map == null) {
					// create OffsetChangeMap on demand
					map = new OffsetChangeMap();
					if (this.RemovalLength > 0)
						map.Add(new OffsetChangeMapEntry(this.Offset, -this.RemovalLength));
					if (this.InsertionLength > 0)
						map.Add(new OffsetChangeMapEntry(this.Offset, this.InsertionLength));
					offsetChangeMap = map;
				}
				return map;
			}
		}
		
		/// <summary>
		/// Gets the OffsetChangeMap, or null if the default offset map (=removal followed by insertion) is being used.
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
			if (offset >= this.Offset) {
				if (offset <= this.Offset + this.RemovalLength) {
					offset = this.Offset;
					if (movementType == AnchorMovementType.AfterInsertion)
						offset += this.InsertionLength;
				} else {
					offset += this.InsertionLength - this.RemovalLength;
				}
			}
			return offset;
		}
		
		/// <summary>
		/// Creates a new DocumentChangeEventArgs object.
		/// </summary>
		public DocumentChangeEventArgs(int offset, int removalLength, string insertedText)
			: this(offset, removalLength, insertedText, null)
		{
		}
		
		/// <summary>
		/// Creates a new DocumentChangeEventArgs object.
		/// </summary>
		public DocumentChangeEventArgs(int offset, int removalLength, string insertedText, OffsetChangeMap offsetChangeMap)
		{
			if (insertedText == null)
				throw new ArgumentNullException("insertedText");
			
			this.Offset = offset;
			this.RemovalLength = removalLength;
			this.InsertedText = insertedText;
			
			if (offsetChangeMap != null) {
				if (!offsetChangeMap.IsValidForDocumentChange(offset, removalLength, insertedText.Length))
					throw new ArgumentException("OffsetChangeMap is not valid for this document change", "offsetChangeMap");
				this.offsetChangeMap = offsetChangeMap;
			}
		}
	}
}
