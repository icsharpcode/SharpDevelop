using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.Bookmarks
{
	/// <summary>
	/// Description of BookmarkManager.
	/// </summary>
	public static class BookmarkManager
	{
		static List<SDBookmark> bookmarks = new List<SDBookmark>();
		
		public static List<SDBookmark> Bookmarks {
			get {
				return bookmarks;
			}
		}
		
		public static List<SDBookmark> GetBookmarks(string fileName)
		{
			List<SDBookmark> marks = new List<SDBookmark>();
			
			foreach (SDBookmark mark in bookmarks) {
				if (mark.FileName == null) continue;
				if (FileUtility.IsEqualFile(mark.FileName, fileName)) {
					marks.Add(mark);
				}
			}
			
			return marks;
		}
		
		public static void AddMark(SDBookmark bookmark)
		{
			if (bookmarks.Contains(bookmark)) return;
			bookmarks.Add(bookmark);
			OnAdded(new BookmarkEventArgs(bookmark));
		}
		
		public static void RemoveMark(SDBookmark bookmark)
		{
			bookmarks.Remove(bookmark);
			OnRemoved(new BookmarkEventArgs(bookmark));
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
