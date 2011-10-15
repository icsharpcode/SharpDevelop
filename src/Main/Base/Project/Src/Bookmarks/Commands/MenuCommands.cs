// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Bookmarks
{
	public abstract class BookmarkMenuCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ITextEditorProvider provider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
			if (provider != null) {
				ITextEditor editor = provider.TextEditor;
				IBookmarkMargin margin = editor.GetService(typeof(IBookmarkMargin)) as IBookmarkMargin;
				if (editor != null && margin != null) {
					Run(editor, margin);
				}
			}
		}
		
		protected static List<Bookmark> GetBookmarks(IBookmarkMargin bookmarkMargin)
		{
			return (from b in bookmarkMargin.Bookmarks.OfType<Bookmark>()
			        where b.CanToggle
			        orderby b.LineNumber
			        select b).ToList();
		}
		
		protected abstract void Run(ITextEditor editor, IBookmarkMargin bookmarkMargin);
	}
	
	public class ToggleBookmark : BookmarkMenuCommand
	{
		protected override void Run(ITextEditor editor, IBookmarkMargin bookmarkMargin)
		{
			BookmarkManager.ToggleBookmark(editor, editor.Caret.Line,
			                               b => b.CanToggle && b.GetType() == typeof(Bookmark),
			                               location => new Bookmark(editor.FileName, location));
		}
	}
	public class PrevBookmark : BookmarkMenuCommand
	{
		protected override void Run(ITextEditor editor, IBookmarkMargin bookmarkMargin)
		{
			int line = editor.Caret.Line;
			var bookmarks = GetBookmarks(bookmarkMargin);
			var bookmark = bookmarks.LastOrDefault(b => b.LineNumber < line);
			if (bookmark == null && bookmarks.Count > 0) {
				bookmark = bookmarks[bookmarks.Count - 1]; // jump around to last bookmark
			}
			if (bookmark != null) {
				FileService.JumpToFilePosition(bookmark.FileName, bookmark.LineNumber, bookmark.ColumnNumber);
			}
		}
	}
	
	public class NextBookmark : BookmarkMenuCommand
	{
		protected override void Run(ITextEditor editor, IBookmarkMargin bookmarkMargin)
		{
			int line = editor.Caret.Line;
			var bookmarks = GetBookmarks(bookmarkMargin);
			var bookmark = bookmarks.FirstOrDefault(b => b.LineNumber > line);
			if (bookmark == null && bookmarks.Count > 0) {
				bookmark = bookmarks[0]; // jump around to first bookmark
			}
			if (bookmark != null) {
				FileService.JumpToFilePosition(bookmark.FileName, bookmark.LineNumber, bookmark.ColumnNumber);
			}
		}
	}
	
	public class ClearBookmarks : BookmarkMenuCommand
	{
		protected override void Run(ITextEditor editor, IBookmarkMargin bookmarkMargin)
		{
			var bookmarks = (from b in bookmarkMargin.Bookmarks.OfType<Bookmark>()
			                 where b.CanToggle
			                 select b).ToList();
			foreach (SDBookmark b in bookmarks)
				BookmarkManager.RemoveMark(b);
		}
	}
}
