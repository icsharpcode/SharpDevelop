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
		ISearchResult CreateSearchResult(string title, IEnumerable<SearchResultMatch> matches);
		
		ISearchResult CreateSearchResult(string title, IObservable<SearchedFile> matches);
	}
}
