// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Bookmarks.Pad.Controls;
using ICSharpCode.SharpDevelop.Debugging;

namespace ICSharpCode.SharpDevelop.Bookmarks
{
	#region Goto Commands
	public abstract class NextPrevBookmarkPadCommand : AbstractMenuCommand
	{
		public void Run(ListViewPadItemModel item)
		{
			var bookmarkBase = (BookmarkPadBase)Owner;	
			
			if (item == null) return;
			
			// get current mark
			var mark = item.Mark as SDBookmark;
			int line = mark.LineNumber;
			var fileName = new FileName(mark.FileName);
			
			SDBookmark bookmark;
			if (item.Mark is BreakpointBookmark) {
				var bookmarks = DebuggerService.Breakpoints;
				bookmark = bookmarks.FirstOrDefault(b => b.LineNumber == line && b.FileName == fileName);
				if (bookmark == null && bookmarks.Count > 0) {
					bookmark = bookmarks[0]; // jump around to first bookmark
				}
			}
			else {
				var bookmarks = BookmarkManager.Bookmarks;
				bookmark = bookmarks.FirstOrDefault(b => b.LineNumber == line && b.FileName == fileName);
				if (bookmark == null && bookmarks.Count > 0) {
					bookmark = bookmarks[0]; // jump around to first bookmark
				}
			}			
			
			if (bookmark != null) {
				FileService.JumpToFilePosition(bookmark.FileName, bookmark.LineNumber, bookmark.ColumnNumber);
			}	

			// select in tree
			bookmarkBase.SelectItem(item);
		}
	}
	
	public sealed class NextBookmarkPadCommand : NextPrevBookmarkPadCommand
	{
		public override void Run()
		{
			var bookmarkBase = (BookmarkPadBase)Owner;			
			var nextItem = bookmarkBase.NextItem;
			
			Run(nextItem);
		}
	}
	
	public sealed class PrevBookmarkPadCommand : NextPrevBookmarkPadCommand
	{
		public override void Run()
		{
			var bookmarkBase = (BookmarkPadBase)Owner;			
			var prevItem = bookmarkBase.PreviousItem;
			
			Run(prevItem);	
		}
	}
	#endregion Goto Commands
	
	#region Delete BookMark(s) commands
	
	public abstract class AbstractDeleteMarkClass : AbstractMenuCommand
	{
		protected void deleteBookMark (SDBookmark bookmark) {
			if (bookmark == null) return;
			if (bookmark is BreakpointBookmark) return;
			ICSharpCode.SharpDevelop.Bookmarks.BookmarkManager.RemoveMark(bookmark);
		}
	}
	
	/// <summary>
	/// Deletes all <see cref="BookmarkNode" />s in the BookMarkPad.
	/// </summary>
	public class DeleteAllMarks : AbstractDeleteMarkClass
	{
		public override void Run()
		{
			BookmarkManager.RemoveAll(b => !(b is BreakpointBookmark));
		}
	}
	
	/// <summary>
	/// Deletes the currently selected <see cref="BookmarkNode" /> or <see cref="BookmarkFolderNode" />
	/// </summary>
	public class DeleteMark : AbstractDeleteMarkClass
	{
		public override void Run()
		{
			var node = ((BookmarkPadBase)Owner).CurrentItem;
			if (node == null) return;
			
			deleteBookMark(node.Mark as SDBookmark);			
		}
	}
	
	#endregion Delete BookMark(s) commands
	
	public class EnableDisableAll : AbstractMenuCommand
	{
		public override void Run()
		{
			((BookmarkPadBase)Owner).EnableDisableAll();
		}
	}
}
