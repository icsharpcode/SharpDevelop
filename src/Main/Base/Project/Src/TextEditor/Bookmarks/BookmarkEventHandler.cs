/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 27.12.2004
 * Time: 21:35
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

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
