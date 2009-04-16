// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.NRefactory;
using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.SharpDevelop.Bookmarks
{
	/// <summary>
	/// Static class that maintains the list of bookmarks and breakpoints.
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
				if (FileUtility.IsEqualFileName(mark.FileName, fileName)) {
					marks.Add(mark);
				}
			}
			
			return marks;
		}
		
		public static void AddMark(SDBookmark bookmark)
		{
			if (bookmark == null) return;
			if (bookmarks.Contains(bookmark)) return;
			if (bookmarks.Exists(b => IsEqualBookmark(b, bookmark))) return;
			bookmarks.Add(bookmark);
			OnAdded(new BookmarkEventArgs(bookmark));
		}
		
		static bool IsEqualBookmark(SDBookmark a, SDBookmark b)
		{
			if (a == b)
				return true;
			if (a == null || b == null)
				return false;
			if (a.GetType() != b.GetType())
				return false;
			if (!FileUtility.IsEqualFileName(a.FileName, b.FileName))
				return false;
			return a.LineNumber == b.LineNumber;
		}
		
		public static void RemoveMark(SDBookmark bookmark)
		{
			bookmarks.Remove(bookmark);
			OnRemoved(new BookmarkEventArgs(bookmark));
		}
		
		public static void Clear()
		{
			while (bookmarks.Count > 0) {
				SDBookmark b = bookmarks[bookmarks.Count - 1];
				bookmarks.RemoveAt(bookmarks.Count - 1);
				OnRemoved(new BookmarkEventArgs(b));
			}
		}
		
		internal static void Initialize()
		{
			Project.ProjectService.SolutionClosing += delegate { Clear(); };
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
		
		public static List<SDBookmark> GetProjectBookmarks(ICSharpCode.SharpDevelop.Project.IProject project)
		{
			List<SDBookmark> projectBookmarks = new List<SDBookmark>();
			foreach (SDBookmark mark in bookmarks) {
				if (mark.IsSaved && mark.FileName != null && project.IsFileInProject(mark.FileName)) {
					projectBookmarks.Add(mark);
				}
			}
			return projectBookmarks;
		}
		
		public static void ToggleBookmark(ITextEditor editor, int line,
		                                  Predicate<SDBookmark> canToggle,
		                                  Func<Location, SDBookmark> bookmarkFactory)
		{
			foreach (SDBookmark bookmark in GetBookmarks(editor.FileName)) {
				if (canToggle(bookmark) && bookmark.LineNumber == line) {
					BookmarkManager.RemoveMark(bookmark);
					return;
				}
			}
			// no bookmark at that line: create a new bookmark
			int lineStartOffset = editor.Document.GetLine(line).Offset;
			int column = 1 + DocumentUtilitites.GetIndentation(editor.Document, lineStartOffset).Length;
			BookmarkManager.AddMark(bookmarkFactory(new Location(column, line)));
		}
		
		public static event BookmarkEventHandler Removed;
		public static event BookmarkEventHandler Added;
	}
}
