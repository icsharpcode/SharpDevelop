// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System.Drawing;
using System.Windows.Forms;
using System;

using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.TextEditor.Actions 
{
	public class ToggleBookmark : AbstractEditAction
	{
		public override void Execute(TextArea textArea)
		{
			textArea.Document.BookmarkManager.ToggleMarkAt(textArea.Caret.Line);
			textArea.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.SingleLine, textArea.Caret.Line));
			textArea.Document.CommitUpdate();
			
		}
	}
	
	public class GotoPrevBookmark : AbstractEditAction
	{
		public override void Execute(TextArea textArea)
		{
			Bookmark mark = textArea.Document.BookmarkManager.GetPrevMark(textArea.Caret.Line);
			if (mark != null) {
				textArea.Caret.Line = mark.LineNumber;
				textArea.SelectionManager.ClearSelection();
			}
		}
	}
	
	public class GotoNextBookmark : AbstractEditAction
	{
		public override void Execute(TextArea textArea)
		{
			Bookmark mark = textArea.Document.BookmarkManager.GetNextMark(textArea.Caret.Line);
			if (mark != null) {
				textArea.Caret.Line = mark.LineNumber;
				textArea.SelectionManager.ClearSelection();
			}
		}
	}
	
	public class ClearAllBookmarks : AbstractEditAction
	{
		public override void Execute(TextArea textArea)
		{
			textArea.Document.BookmarkManager.Clear();
			textArea.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
			textArea.Document.CommitUpdate();
		}
	}
}
