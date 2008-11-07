// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Globalization;

namespace ICSharpCode.AvalonEdit.Document
{
	/// <summary>
	/// Represents a line inside a <see cref="TextDocument"/>.
	/// </summary>
	public sealed partial class DocumentLine : ISegment
	{
		#region Constructor
		readonly TextDocument document;
		internal bool isDeleted;
		
		internal DocumentLine(TextDocument document)
		{
			Debug.Assert(document != null);
			this.document = document;
		}
		#endregion
		
		#region Document / Text
		/// <summary>
		/// Gets the text document that owns this DocumentLine. O(1).
		/// </summary>
		/// <remarks>This property is still available even if the line was deleted.</remarks>
		public TextDocument Document {
			get {
				document.DebugVerifyAccess();
				return document;
			}
		}
		
		
		/// <summary>
		/// Gets the text on this line.
		/// </summary>
		/// <exception cref="InvalidOperationException">The line was deleted.</exception>
		public string Text {
			get {
				return document.GetText(this.Offset, this.Length);
			}
		}
		#endregion
		
		#region Events
//		/// <summary>
//		/// Is raised when the line is deleted.
//		/// </summary>
//		public event EventHandler Deleted;
//
//		/// <summary>
//		/// Is raised when the line's text changes.
//		/// </summary>
//		public event EventHandler TextChanged;
//
//		/// <summary>
//		/// Raises the Deleted or TextChanged event.
//		/// </summary>
//		internal void RaiseChanged()
//		{
//			if (IsDeleted) {
//				if (Deleted != null)
//					Deleted(this, EventArgs.Empty);
//			} else {
//				if (TextChanged != null)
//					TextChanged(this, EventArgs.Empty);
//			}
//		}
		#endregion
		
		#region Properties stored in tree
		/// <summary>
		/// Gets if this line was deleted from the document.
		/// </summary>
		public bool IsDeleted {
			get {
				document.DebugVerifyAccess();
				return isDeleted;
			}
		}
		
		/// <summary>
		/// Gets the number of this line.
		/// Runtime: O(log n)
		/// </summary>
		/// <exception cref="InvalidOperationException">The line was deleted.</exception>
		public int LineNumber {
			get {
				if (IsDeleted)
					throw new InvalidOperationException();
				return DocumentLineTree.GetIndexFromNode(this) + 1;
			}
		}
		
		/// <summary>
		/// Gets the starting offset of the line in the document's text.
		/// Runtime: O(log n)
		/// </summary>
		/// <exception cref="InvalidOperationException">The line was deleted.</exception>
		public int Offset {
			get {
				if (IsDeleted)
					throw new InvalidOperationException();
				return DocumentLineTree.GetOffsetFromNode(this);
			}
		}
		#endregion
		
		#region Length
		int totalLength;
		byte delimiterLength;
		
		/// <summary>
		/// Gets the length of this line. O(1)
		/// </summary>
		/// <remarks>This property is still available even if the line was deleted;
		/// in that case, it contains the line's length before the deletion.</remarks>
		public int Length {
			get {
				document.DebugVerifyAccess();
				return totalLength - delimiterLength;
			}
		}
		
		/// <summary>
		/// Gets the length of this line, including the line delimiter. O(1)
		/// </summary>
		/// <remarks>This property is still available even if the line was deleted;
		/// in that case, it contains the line's length before the deletion.</remarks>
		public int TotalLength {
			get {
				document.DebugVerifyAccess();
				return totalLength;
			}
			internal set {
				// this is set by DocumentLineTree
				totalLength = value;
			}
		}
		
		/// <summary>
		/// Gets the length of the newline.
		/// </summary>
		/// <remarks>This property is still available even if the line was deleted;
		/// in that case, it contains the line's length before the deletion.</remarks>
		public int DelimiterLength {
			get {
				document.DebugVerifyAccess();
				return delimiterLength;
			}
			internal set {
				Debug.Assert(value >= 0 && value <= 2);
				delimiterLength = (byte)value;
			}
		}
		#endregion
		
		#region ParserState
//		/// <summary>
//		/// Gets the parser state array associated with this line.
//		/// </summary>
//		public object[] ParserState { get; internal set; }
		#endregion
		
		#region ToString
		/// <summary>
		/// Gets a string representation of the line.
		/// </summary>
		public override string ToString()
		{
			if (IsDeleted)
				return "[DocumentLine deleted]";
			else
				return string.Format(
					CultureInfo.InvariantCulture,
					"[DocumentLine Number={0} Offset={1} Length={2}]", LineNumber, Offset, Length);
		}
		#endregion
	}
}
