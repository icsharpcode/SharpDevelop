/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 26.12.2004
 * Time: 19:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

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
		}
		
		public bool IsEnabled {
			get {
				return isEnabled;
			}
			set {
				isEnabled = value;
				document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.SingleLine, lineNumber));
				document.CommitUpdate();
			}
		}
		
		public int LineNumber {
			get {
				return lineNumber;
			}
			set {
				lineNumber = value;
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
	}
}
