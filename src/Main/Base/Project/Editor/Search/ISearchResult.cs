// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;

namespace ICSharpCode.SharpDevelop.Editor.Search
{
	/// <summary>
	/// Represents the content of the search results pad.
	/// </summary>
	public interface ISearchResult
	{
		/// <summary>
		/// Gets the title of the search result.
		/// </summary>
		string Text { get; }
		
		/// <summary>
		/// Retrieves the UI Element for displaying the search results.
		/// It is valid to create a new UI Element on each call or to reuse an existing one.
		/// 
		/// This method will only be called on the active search result in the 'Search Results' pad, so
		/// it is valid to use a control for multiple search result instances and exchange that control's data when
		/// this method is called.
		/// </summary>
		object GetControl();
		
		/// <summary>
		/// Gets the items for the toolbar that are visible only for this search result.
		/// </summary>
		IList GetToolbarItems();
	}
}
