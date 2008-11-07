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
		
		/// <summary>
		/// Gets the new offset where the specified offset moves after this document change.
		/// </summary>
		public int GetNewOffset(int offset, AnchorMovementType movementType)
		{
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
		{
			if (insertedText == null)
				throw new ArgumentNullException("insertedText");
			this.Offset = offset;
			this.RemovalLength = removalLength;
			this.InsertedText = insertedText;
		}
	}
}
