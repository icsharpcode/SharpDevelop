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
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.Bookmarks;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Editor.Bookmarks
{
	sealed class BookmarkManager : IBookmarkManager
	{
		public BookmarkManager()
		{
			Project.ProjectService.SolutionClosed += delegate { Clear(); };
		}
		
		List<SDBookmark> bookmarks = new List<SDBookmark>();
		
		public IReadOnlyCollection<SDBookmark> Bookmarks {
			get {
				SD.MainThread.VerifyAccess();
				return bookmarks;
			}
		}
		
		public IEnumerable<SDBookmark> GetBookmarks(FileName fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			
			SD.MainThread.VerifyAccess();
			return bookmarks.Where(b => b.FileName == fileName);
		}
		
		public void AddMark(SDBookmark bookmark)
		{
			SD.MainThread.VerifyAccess();
			if (bookmark == null) return;
			if (bookmarks.Contains(bookmark)) return;
			if (bookmarks.Exists(b => IsEqualBookmark(b, bookmark))) return;
			bookmarks.Add(bookmark);
			OnAdded(new BookmarkEventArgs(bookmark));
		}
		
		public void AddMark(SDBookmark bookmark, IDocument document, int line)
		{
			int lineStartOffset = document.GetLineByNumber(line).Offset;
			int column = 1 + DocumentUtilities.GetWhitespaceAfter(document, lineStartOffset).Length;
			bookmark.Location = new TextLocation(line, column);
			bookmark.FileName = FileName.Create(document.FileName);
			AddMark(bookmark);
		}
		
		static bool IsEqualBookmark(SDBookmark a, SDBookmark b)
		{
			if (a == b)
				return true;
			if (a == null || b == null)
				return false;
			if (a.GetType() != b.GetType())
				return false;
			if (a.FileName != b.FileName)
				return false;
			return a.LineNumber == b.LineNumber;
		}
		
		public void RemoveMark(SDBookmark bookmark)
		{
			SD.MainThread.VerifyAccess();
			bookmarks.Remove(bookmark);
			OnRemoved(new BookmarkEventArgs(bookmark));
		}
		
		public void Clear()
		{
			SD.MainThread.VerifyAccess();
			while (bookmarks.Count > 0) {
				SDBookmark b = bookmarks[bookmarks.Count - 1];
				bookmarks.RemoveAt(bookmarks.Count - 1);
				OnRemoved(new BookmarkEventArgs(b));
			}
		}
		
		void OnRemoved(BookmarkEventArgs e)
		{
			if (BookmarkRemoved != null) {
				BookmarkRemoved(null, e);
			}
		}
		
		void OnAdded(BookmarkEventArgs e)
		{
			if (BookmarkAdded != null) {
				BookmarkAdded(null, e);
			}
		}
		
		public IEnumerable<SDBookmark> GetProjectBookmarks(ICSharpCode.SharpDevelop.Project.IProject project)
		{
			SD.MainThread.VerifyAccess();
			List<SDBookmark> projectBookmarks = new List<SDBookmark>();
			foreach (SDBookmark mark in bookmarks) {
				// Only return those bookmarks which belong to the specified project.
				if (mark.IsSaved && mark.FileName != null && project.IsFileInProject(mark.FileName)) {
					projectBookmarks.Add(mark);
				}
			}
			return projectBookmarks;
		}
		
		public bool RemoveBookmarkAt(FileName fileName, int line, Predicate<SDBookmark> predicate = null)
		{
			foreach (SDBookmark bookmark in GetBookmarks(fileName)) {
				if (bookmark.CanToggle && bookmark.LineNumber == line) {
					if (predicate == null || predicate(bookmark)) {
						RemoveMark(bookmark);
						return true;
					}
				}
			}
			return false;
		}
		
		public void RemoveAll(Predicate<SDBookmark> match)
		{
			if (match == null)
				throw new ArgumentNullException("Predicate is null!");
			SD.MainThread.VerifyAccess();
			
			for(int index = bookmarks.Count - 1; index >= 0; --index){
				SDBookmark bookmark = bookmarks[index];
				if(match(bookmark)) {
					bookmarks.RemoveAt(index);
					OnRemoved(new BookmarkEventArgs(bookmark));
				}
			}
		}
		
		public event EventHandler<BookmarkEventArgs> BookmarkRemoved;
		public event EventHandler<BookmarkEventArgs> BookmarkAdded;
	}
}
