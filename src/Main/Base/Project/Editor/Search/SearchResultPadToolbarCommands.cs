// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;

using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;

namespace ICSharpCode.SharpDevelop.Editor.Search
{
	public class LastSearchResultsBuilder : IMenuItemBuilder
	{
		public ICollection BuildItems(Codon codon, object owner)
		{
			List<object> items = new List<object>();
			foreach (ISearchResult searchResult in SearchResultsPad.Instance.LastSearches) {
				MenuItem menuItem = new MenuItem();
				menuItem.Header = searchResult.Text;
				// copy in local variable so that lambda refers to correct loop iteration
				ISearchResult searchResultCopy = searchResult;
				menuItem.Click += (sender, e) => SearchResultsPad.Instance.ShowSearchResults(searchResultCopy);
				items.Add(menuItem);
			}
			return items;
		}
	}
	
	public class ClearSearchResultsList : AbstractMenuCommand
	{
		public override void Run()
		{
			SearchResultsPad.Instance.ClearLastSearchesList();
		}
	}
}
