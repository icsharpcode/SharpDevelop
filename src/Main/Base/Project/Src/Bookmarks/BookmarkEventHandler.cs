// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Bookmarks
{
	public delegate void BookmarkEventHandler(object sender, BookmarkEventArgs e);
	
	/// <summary>
	/// Description of BookmarkEventHandler.
	/// </summary>
	public class BookmarkEventArgs : EventArgs
	{
		SDBookmark bookmark;
		
		public SDBookmark Bookmark {
			get {
				return bookmark;
			}
		}
		
		public BookmarkEventArgs(SDBookmark bookmark)
		{
			this.bookmark = bookmark;
		}
	}
}
