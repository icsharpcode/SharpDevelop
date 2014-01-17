// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Threading;

namespace ICSharpCode.SharpDevelop.Editor.Search
{
	/// <summary>
	/// Creates ISearchResult for a set of results.
	/// </summary>
	public interface ISearchResultFactory
	{
		/// <summary>
		/// Creates an <see cref="ISearchResult"/> object from a list of matches.
		/// </summary>
		/// <param name="title">The title of the search.</param>
		/// <param name="matches">The list of matches. CreateSearchResult() will enumerate once through the IEnumerable in order to retrieve the search results.</param>
		ISearchResult CreateSearchResult(string title, IEnumerable<SearchResultMatch> matches);
		
		/// <summary>
		/// Creates an <see cref="ISearchResult"/> object for a background search operation.
		/// </summary>
		/// <param name="title">The title of the search.</param>
		/// <param name="matches">The background search operation. CreateSearchResult() will subscribe to the observable in order to retrieve the search results.</param>
		ISearchResult CreateSearchResult(string title, IObservable<SearchedFile> matches);
	}
}
