using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace Bookmark
{
	/// <summary>
	/// Description of BookmarkManager.
	/// </summary>
	public static class BookmarkManager
	{
		static List<Bookmark> bookmarks = new List<Bookmark>();
		
		public static List<Bookmark> Bookmarks {
			get {
				return bookmarks;
			}
		}
		
		public static List<Bookmark> GetBookmarks(string fileName)
		{
			List<Bookmark> marks = new List<Bookmark>();
			
			foreach (Bookmark mark in bookmarks) {
				if (FileUtility.IsEqualFile(mark.FileName, fileName)) {
					marks.Add(mark);
				}
			}
			
			return marks;
		}
		
		public static void AddMark(string fileName, ICSharpCode.TextEditor.Document.Bookmark bookmark)
		{
			Bookmark mark = new Bookmark(fileName, bookmark);
			bookmarks.Add(mark);
			OnAdded(new BookmarkEventArgs(mark));
		}
		
		public static void RemoveMark(string fileName, ICSharpCode.TextEditor.Document.Bookmark bookmark)
		{
			for (int i = 0; i < bookmarks.Count; ) {
				if (FileUtility.IsEqualFile(bookmarks[i].FileName, fileName) && bookmarks[i].LineNumber == bookmark.LineNumber) {
					OnRemoved(new BookmarkEventArgs(bookmarks[i]));
					bookmarks.RemoveAt(i);
				} else {
					++i;
				}
			}
		}
		
		static void OnRemoved(BookmarkEventArgs e) 
		{
			if (Removed != null) {
				Removed(null, e);
			}
		}
		
		
		static void OnAdded(BookmarkEventArgs e) 
		{
			if (Added != null) {
				Added(null, e);
			}
		}
		
		public static event BookmarkEventHandler Removed;
		public static event BookmarkEventHandler Added;
	}
}
