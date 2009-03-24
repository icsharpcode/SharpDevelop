// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Document
{
	/// <summary>
	/// The TextAnchor class references a text location - a position between two characters.
	/// It automatically updates its offset when text is inserted/removed in front of the anchor.
	/// </summary>
	public sealed class TextAnchor
	{
		readonly TextDocument document;
		internal TextAnchorNode node;
		
		internal TextAnchor(TextDocument document)
		{
			this.document = document;
		}
		
		/// <summary>
		/// Gets the document owning the anchor.
		/// </summary>
		public TextDocument Document {
			get { return document; }
		}
		
		/// <summary>
		/// Controls how the anchor moves.
		/// </summary>
		public AnchorMovementType MovementType { get; set; }
		
		/// <summary>
		/// Specifies whether the anchor survives deletion of the text containing it.
		/// <c>false</c>: The anchor is deleted when the a selection that includes the anchor is deleted.
		/// <c>true</c>: The anchor is not deleted.
		/// </summary>
		public bool SurviveDeletion { get; set; }
		
		/// <summary>
		/// Gets whether the anchor was deleted.
		/// </summary>
		public bool IsDeleted {
			get {
				document.DebugVerifyAccess();
				return node == null;
			}
		}
		
		/// <summary>
		/// Occurs after the anchor was deleted.
		/// </summary>
		public event EventHandler Deleted;
		
		internal void OnDeleted(DelayedEvents delayedEvents)
		{
			node = null;
			delayedEvents.DelayedRaise(Deleted, this, EventArgs.Empty);
		}
		
		/// <summary>
		/// Gets the offset of the text anchor.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown when trying to get the Offset from a deleted anchor.</exception>
		public int Offset {
			get {
				document.DebugVerifyAccess();
				
				TextAnchorNode n = this.node;
				if (n == null)
					throw new InvalidOperationException();
				
				int offset = n.length;
				if (n.left != null)
					offset += n.left.totalLength;
				while (n.parent != null) {
					if (n == n.parent.right) {
						if (n.parent.left != null)
							offset += n.parent.left.totalLength;
						offset += n.parent.length;
					}
					n = n.parent;
				}
				return offset;
			}
		}
		
		/// <summary>
		/// Gets the line number of the anchor.
		/// </summary>
		public int Line {
			get {
				return document.GetLineByOffset(this.Offset).LineNumber;
			}
		}
		
		/// <summary>
		/// Gets the column number of this anchor.
		/// </summary>
		public int Column {
			get {
				int offset = this.Offset;
				return offset - document.GetLineByOffset(offset).Offset + 1;
			}
		}
		
		/// <summary>
		/// Gets the text location of this anchor.
		/// </summary>
		public TextLocation Location {
			get {
				return document.GetLocation(this.Offset);
			}
		}
		
		/// <inheritdoc/>
		public override string ToString()
		{
			return "[TextAnchor Offset=" + Offset + "]";
		}
	}
	
	/// <summary>
	/// Defines how a text anchor moves.
	/// </summary>
	public enum AnchorMovementType
	{
		/// <summary>
		/// Behaves like a start marker - when text is inserted at the anchor position, the anchor will stay
		/// before the inserted text.
		/// </summary>
		BeforeInsertion,
		/// <summary>
		/// Behave like an end marker - when text is insered at the anchor position, the anchor will move
		/// after the inserted text.
		/// </summary>
		AfterInsertion
	}
}
