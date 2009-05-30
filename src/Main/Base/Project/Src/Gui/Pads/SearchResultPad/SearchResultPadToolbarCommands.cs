// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using System.Windows.Controls;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class LastSearchResultsBuilder : IMenuItemBuilder
	{
		public ICollection BuildItems(Codon codon, object owner)
		{
			List<object> items = new List<object>();
			foreach (ISearchResult searchResult in SearchResultPad.Instance.LastSearches) {
				MenuItem menuItem = new MenuItem();
				menuItem.Header = searchResult.Text;
				// copy in local variable so that lambda refers to correct loop iteration
				ISearchResult searchResultCopy = searchResult;
				menuItem.Click += (sender, e) => SearchResultPad.Instance.ShowSearchResults(searchResultCopy);
				items.Add(menuItem);
			}
			return items;
		}
	}
	
	public class ClearSearchResultsList : AbstractMenuCommand
	{
		public override void Run()
		{
			SearchResultPad.Instance.ClearLastSearchesList();
		}
	}
}
