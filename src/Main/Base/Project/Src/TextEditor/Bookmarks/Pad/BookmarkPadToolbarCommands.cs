// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Commands;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor.Actions;

namespace ICSharpCode.SharpDevelop.Bookmarks
{
	public class GotoNext : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				return new ICSharpCode.TextEditor.Actions.GotoNextBookmark(PrevBookmark.AcceptOnlyStandardBookmarks);
			}
		}
	}
	
	public class GotoPrev : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				return new ICSharpCode.TextEditor.Actions.GotoPrevBookmark(PrevBookmark.AcceptOnlyStandardBookmarks);
			}
		}
	}
	
	public class DeleteMark : AbstractMenuCommand
	{
		public override void Run()
		{
			BookmarkNode node = BookmarkPad.Instance.CurrentNode;
			if (node != null) {
				if (node.Bookmark.Document != null) {
					node.Bookmark.Document.BookmarkManager.RemoveMark(node.Bookmark);
				} else {
					ICSharpCode.SharpDevelop.Bookmarks.BookmarkManager.RemoveMark(node.Bookmark);
				}
				WorkbenchSingleton.MainForm.Refresh();
			}
		}
	}
	
	public class EnableDisableAll : AbstractMenuCommand
	{
		public override void Run()
		{
			BookmarkPad.Instance.EnableDisableAll();
		}
	}
	
}
