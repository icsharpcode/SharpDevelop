// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;

namespace ICSharpCode.SharpDevelop.Editor.Bookmarks
{
	/// <summary>
	/// A bookmark that can be attached to an AvalonEdit TextDocument.
	/// </summary>
	public class BookmarkBase : IBookmark
	{
		TextLocation location;
		
		IDocument document;
		ITextAnchor anchor;
		
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IDocument Document {
			get {
				return document;
			}
			set {
				if (document != value) {
					if (anchor != null) {
						location = anchor.Location;
					}
					document = value;
					CreateAnchor();
					OnDocumentChanged(EventArgs.Empty);
				}
			}
		}
		
		void CreateAnchor()
		{
			if (anchor != null) {
				// Detach from Deleted event: don't delete the bookmark
				// if the anchor at the old position is deleted after the anchor was moved
				anchor.Deleted -= AnchorDeleted;
			}
			if (document != null) {
				int lineNumber = Math.Max(1, Math.Min(location.Line, document.LineCount));
				int lineLength = document.GetLineByNumber(lineNumber).Length;
				int offset = document.GetOffset(
					lineNumber,
					Math.Max(1, Math.Min(location.Column, lineLength + 1))
				);
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
			// the anchor just became invalid, so don't try to use it again
			location = TextLocation.Empty;
			anchor = null;
			RemoveMark();
		}
		
		protected virtual void RemoveMark()
		{
			if (document != null) {
				IBookmarkMargin bookmarkMargin = document.GetService(typeof(IBookmarkMargin)) as IBookmarkMargin;
				if (bookmarkMargin != null)
					bookmarkMargin.Bookmarks.Remove(this);
			}
		}
		
		/// <summary>
		/// Gets the TextAnchor used for this bookmark.
		/// Is null if the bookmark is not connected to a document.
		/// </summary>
		public ITextAnchor Anchor {
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
		
		protected virtual void Redraw()
		{
			if (document != null) {
				IBookmarkMargin bookmarkMargin = document.GetService(typeof(IBookmarkMargin)) as IBookmarkMargin;
				if (bookmarkMargin != null)
					bookmarkMargin.Redraw();
			}
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
		
		public virtual int ZOrder {
			get { return 0; }
		}
		
		/// <summary>
		/// Gets if the bookmark can be toggled off using the 'set/unset bookmark' command.
		/// </summary>
		public virtual bool CanToggle {
			get {
				return true;
			}
		}
		
		public static IImage DefaultBookmarkImage {
			get { return SD.ResourceService.GetImage("Bookmarks.ToggleMark"); }
		}
		
		public virtual IImage Image {
			get { return DefaultBookmarkImage; }
		}
		
		public ImageSource ImageSource {
			get { return this.Image != null ? this.Image.ImageSource : null; }
		}
		
		public virtual void MouseDown(MouseButtonEventArgs e)
		{
		}
		
		public virtual void MouseUp(MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left && CanToggle) {
				RemoveMark();
				e.Handled = true;
			}
		}
		
		public virtual bool CanDragDrop {
			get { return false; }
		}
		
		public virtual void Drop(int lineNumber)
		{
		}
		
		public virtual object CreateTooltipContent()
		{
			return null;
		}
		
		public const string BreakpointMarkerName = "Breakpoint";
		
		public static readonly Color BreakpointDefaultBackground = Color.FromRgb(180, 38, 38);
		public static readonly Color BreakpointDefaultForeground = Colors.White;
		
		public const string CurrentLineBookmarkName = "Current statement";
		
		public static readonly Color CurrentLineDefaultBackground = Colors.Yellow;
		public static readonly Color CurrentLineDefaultForeground = Colors.Blue;
	}
	
	public interface IHaveStateEnabled
	{
		bool IsEnabled { get; set; }
	}
}
