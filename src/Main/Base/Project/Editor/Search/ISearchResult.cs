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
		/// Notifies the search that it is no longer active (another search result will be activated in the search results pad).
		/// GetControl() will be called if the search result is activated again.
		/// </summary>
		void OnDeactivate();
		
		/// <summary>
		/// Gets the items for the toolbar that are visible only for this search result.
		/// </summary>
		IList GetToolbarItems();
	}
}
