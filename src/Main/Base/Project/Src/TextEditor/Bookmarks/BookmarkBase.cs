// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.AvalonEdit.Document;
using System.Windows.Input;

namespace ICSharpCode.SharpDevelop.Bookmarks
{
	/// <summary>
	/// A bookmark that can be attached to an AvalonEdit TextDocument.
	/// </summary>
	public class BookmarkBase : IBookmark
	{
		TextLocation location;
		
		TextDocument document;
		TextAnchor anchor;
		IBookmarkMargin bookmarkMargin;
		
		public TextDocument Document {
			get {
				return document;
			}
			set {
				if (document != value) {
					if (anchor != null) {
						location = anchor.Location;
						anchor = null;
					}
					document = value;
					CreateAnchor();
					OnDocumentChanged(EventArgs.Empty);
				}
			}
		}
		
		public IBookmarkMargin BookmarkMargin {
			get { return bookmarkMargin; }
			set {
				if (bookmarkMargin != value) {
					bookmarkMargin = value;
					OnBookmarkMarginChanged(EventArgs.Empty);
				}
			}
		}
		
		void CreateAnchor()
		{
			if (document != null) {
				int lineNumber = Math.Max(1, Math.Min(location.Line, document.LineCount));
				int lineLength = document.GetLineByNumber(lineNumber).Length;
				int offset = document.GetOffset(
					new TextLocation(
						lineNumber,
						Math.Max(1, Math.Min(location.Column, lineLength + 1))
					));
				anchor = document.CreateAnchor(offset);
				// after insertion: keep bookmarks after the initial whitespace (see DefaultFormattingStrategy.SmartReplaceLine)
				anchor.MovementType = AnchorMovementType.AfterInsertion;
				anchor.Deleted += AnchorDeleted;
			} else {
				anchor = null;
			}
		}
		
		void AnchorDeleted(object sender, EventArgs e)
		{
			RemoveMark();
		}
		
		protected virtual void RemoveMark()
		{
			if (bookmarkMargin != null)
				bookmarkMargin.Bookmarks.Remove(this);
		}
		
		/// <summary>
		/// Gets the TextAnchor used for this bookmark.
		/// Is null if the bookmark is not connected to a document.
		/// </summary>
		public TextAnchor Anchor {
			get { return anchor; }
		}
		
		public TextLocation Location {
			get {
				if (anchor != null)
					return anchor.Location;
				else
					return location;
			}
			set {
				location = value;
				CreateAnchor();
			}
		}
		
		public event EventHandler DocumentChanged;
		
		protected virtual void OnDocumentChanged(EventArgs e)
		{
			if (DocumentChanged != null) {
				DocumentChanged(this, e);
			}
		}
		
		protected virtual void OnBookmarkMarginChanged(EventArgs e)
		{
		}
		
		protected virtual void Redraw()
		{
			if (bookmarkMargin != null)
				bookmarkMargin.Redraw();
		}
		
		public int LineNumber {
			get {
				if (anchor != null)
					return anchor.Line;
				else
					return location.Line;
			}
		}
		
		public int ColumnNumber {
			get {
				if (anchor != null)
					return anchor.Column;
				else
					return location.Column;
			}
		}
		
		/// <summary>
		/// Gets if the bookmark can be toggled off using the 'set/unset bookmark' command.
		/// </summary>
		public virtual bool CanToggle {
			get {
				return true;
			}
		}
		
		public BookmarkBase(TextLocation location)
		{
			this.Location = location;
		}
		
		public static readonly IImage DefaultBookmarkImage = new ResourceServiceImage("Bookmarks.ToggleMark");
		
		public virtual IImage Image {
			get { return DefaultBookmarkImage; }
		}
		
		public virtual void MouseDown(MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left && CanToggle) {
				RemoveMark();
				e.Handled = true;
			}
		}
	}
}
