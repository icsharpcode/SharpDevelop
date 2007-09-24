// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using SWF = System.Windows.Forms;

namespace ICSharpCode.TextEditor.Document
{
	/// <summary>
	/// Description of Bookmark.
	/// </summary>
	public class Bookmark
	{
		IDocument document;
		LineSegment line;
		int lineNumber;
		bool isEnabled = true;
		
		public IDocument Document {
			get {
				return document;
			}
			set {
				if (document != value) {
					if (line != null) {
						lineNumber = line.LineNumber;
						line = null;
					}
					document = value;
					if (document != null) {
						line = document.GetLineSegment(Math.Min(lineNumber, document.TotalNumberOfLines-1));
					}
					OnDocumentChanged(EventArgs.Empty);
				}
			}
		}
		
		public event EventHandler DocumentChanged;
		
		protected virtual void OnDocumentChanged(EventArgs e)
		{
			if (DocumentChanged != null) {
				DocumentChanged(this, e);
			}
		}
		
		public bool IsEnabled {
			get {
				return isEnabled;
			}
			set {
				if (isEnabled != value) {
					isEnabled = value;
					if (document != null) {
						document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.SingleLine, lineNumber));
						document.CommitUpdate();
					}
					OnIsEnabledChanged(EventArgs.Empty);
				}
			}
		}
		
		public event EventHandler IsEnabledChanged;
		
		protected virtual void OnIsEnabledChanged(EventArgs e)
		{
			if (IsEnabledChanged != null) {
				IsEnabledChanged(this, e);
			}
		}
		
		/// <summary>
		/// Gets the line the bookmark belongs to.
		/// Is null if the bookmark is not connected to a document.
		/// </summary>
		public LineSegment Line {
			get { return line; }
		}
		
		public int LineNumber {
			get {
				if (line != null)
					return line.LineNumber;
				else
					return lineNumber;
			}
			set {
				if (value < 0)
					throw new ArgumentOutOfRangeException("value", value, "line number must be >= 0");
				if (document == null) {
					lineNumber = value;
				} else {
					line = document.GetLineSegment(value);
				}
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
		
		public Bookmark(IDocument document, int lineNumber) : this(document, lineNumber, true)
		{
		}
		
		public Bookmark(IDocument document, int lineNumber, bool isEnabled)
		{
			this.document   = document;
			this.isEnabled  = isEnabled;
			this.LineNumber = lineNumber;
		}
		
		public virtual bool Click(SWF.Control parent, SWF.MouseEventArgs e)
		{
			if (e.Button == SWF.MouseButtons.Left && CanToggle) {
				document.BookmarkManager.RemoveMark(this);
				return true;
			}
			return false;
		}
		
		public virtual void Draw(IconBarMargin margin, Graphics g, Point p)
		{
			margin.DrawBookmark(g, p.Y, isEnabled);
		}
	}
}
