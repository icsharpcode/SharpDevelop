// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.SharpDevelop.Editor.Bookmarks
{
	/// <summary>
	/// The bookmark margin.
	/// </summary>
	[DocumentService]
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
		
		event EventHandler RedrawRequested;
	}
}
