// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Editor.Bookmarks
{
	/// <summary>
	/// Manages the list of bookmarks in the current solution.
	/// </summary>
	[SDService]
	public interface IBookmarkManager
	{
		/// <summary>
		/// Gets the collection of bookmarks.
		/// </summary>
		IReadOnlyCollection<SDBookmark> Bookmarks { get; }
		
		event EventHandler<BookmarkEventArgs> BookmarkAdded;
		event EventHandler<BookmarkEventArgs> BookmarkRemoved;
		
		IEnumerable<SDBookmark> GetBookmarks(FileName fileName);
		
		IEnumerable<SDBookmark> GetProjectBookmarks(IProject project);
		
		/// <summary>
		/// Adds the specified bookmark.
		/// The FileName and Location properties must be set before calling this method.
		/// </summary>
		void AddMark(SDBookmark bookmark);
		
		/// <summary>
		/// Adds the specified bookmark.
		/// This method sets the FileName/Location properties.
		/// </summary>
		void AddMark(SDBookmark bookmark, IDocument document, int line);
		
		/// <summary>
		/// Removes the specified bookmark.
		/// </summary>
		void RemoveMark(SDBookmark bookmark);
		
		/// <summary>
		/// Removes a bookmark in the specified file and line.
		/// </summary>
		/// <returns>True if a bookmark was removed, null if no bookmark matching the specified criteria was found</returns>
		bool RemoveBookmarkAt(FileName fileName, int line, Predicate<SDBookmark> predicate = null);
		
		void RemoveAll(Predicate<SDBookmark> predicate);
		
		/// <summary>
		/// Deletes all bookmarks
		/// </summary>
		void Clear();
	}
}
