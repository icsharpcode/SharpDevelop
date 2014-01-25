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
