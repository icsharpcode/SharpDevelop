// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Debugging;

namespace ICSharpCode.SharpDevelop.Bookmarks
{
	#region Goto Commands
	/*
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
	*/
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
			var result = BookmarkManager.Bookmarks
				.Where(b => !(b is SDMarkerBookmark))
				.Select(b => b);
			foreach (var b in result.ToArray())
				BookmarkManager.RemoveMark(b);
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
