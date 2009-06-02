// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Editor.Search
{
	/// <summary>
	/// Creates ISearchResult for a set of results.
	/// </summary>
	public interface ISearchResultFactory
	{
		ISearchResult CreateSearchResult(string title, IEnumerable<SearchResultMatch> matches);
	}
}
