// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
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
		int       lineNumber;
		bool      isEnabled = true;
		
		public IDocument Document {
			get {
				return document;
			}
			set {
				if (document != value) {
					document = value;
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
				isEnabled = value;
				if (document != null) {
					document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.SingleLine, lineNumber));
					document.CommitUpdate();
				}
			}
		}
		
		public int LineNumber {
			get {
				return lineNumber;
			}
			set {
				if (lineNumber != value) {
					lineNumber = value;
					OnLineNumberChanged(EventArgs.Empty);
				}
			}
		}
		
		public event EventHandler LineNumberChanged;
		
		protected virtual void OnLineNumberChanged(EventArgs e)
		{
			if (LineNumberChanged != null) {
				LineNumberChanged(this, e);
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
			this.lineNumber = lineNumber;
			this.isEnabled  = isEnabled;
		}
		
		public virtual void Click(SWF.MouseButtons mouseButtons)
		{
			if (mouseButtons == SWF.MouseButtons.Left) {
				document.BookmarkManager.RemoveMark(this);
			}
		}
		
		public virtual void Draw(IconBarMargin margin, Graphics g, Point p)
		{
			margin.DrawBookmark(g, p.Y, isEnabled);
		}
	}
}
