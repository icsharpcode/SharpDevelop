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
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.Bookmarks;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Editor.Bookmarks
{
	abstract class BookmarkMenuCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ITextEditor editor = SD.GetActiveViewContentService<ITextEditor>();
			if (editor != null) {
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
	
	class ToggleBookmark : BookmarkMenuCommand
	{
		protected override void Run(ITextEditor editor, IBookmarkMargin bookmarkMargin)
		{
			int lineNumber = editor.Caret.Line;
			if (!SD.BookmarkManager.RemoveBookmarkAt(editor.FileName, lineNumber, b => b is Bookmark)) {
				SD.BookmarkManager.AddMark(new Bookmark(), editor.Document, lineNumber);
			}
		}
	}
	class PrevBookmark : BookmarkMenuCommand
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
				SD.FileService.JumpToFilePosition(bookmark.FileName, bookmark.LineNumber, bookmark.ColumnNumber);
			}
		}
	}
	
	class NextBookmark : BookmarkMenuCommand
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
				SD.FileService.JumpToFilePosition(bookmark.FileName, bookmark.LineNumber, bookmark.ColumnNumber);
			}
		}
	}
	
	class ClearBookmarks : BookmarkMenuCommand
	{
		protected override void Run(ITextEditor editor, IBookmarkMargin bookmarkMargin)
		{
			var bookmarks = (from b in bookmarkMargin.Bookmarks.OfType<Bookmark>()
			                 where b.CanToggle
			                 select b).ToList();
			foreach (SDBookmark b in bookmarks)
				SD.BookmarkManager.RemoveMark(b);
		}
	}
}
