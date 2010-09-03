// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace ICSharpCode.SharpDevelop.Bookmarks
{
	/// <summary>
	/// The bookmark margin.
	/// </summary>
	public interface IBookmarkMargin
	{
		/// <summary>
		/// Gets the list of bookmarks.
		/// </summary>
		IList<IBookmark> Bookmarks { get; }
		
		/// <summary>
		/// Redraws the bookmark margin. Bookmarks need to call this method when the Image changes.
		/// </summary>
		void Redraw();
	}
	
	/// <summary>
	/// Represents a bookmark in the bookmark margin.
	/// </summary>
	public interface IBookmark
	{
		/// <summary>
		/// Gets the line number of the bookmark.
		/// </summary>
		int LineNumber { get; }
		
		/// <summary>
		/// Gets the image.
		/// </summary>
		IImage Image { get; }
		
		/// <summary>
		/// Handles the mouse down event.
		/// </summary>
		void MouseDown(MouseButtonEventArgs e);
	}
}
