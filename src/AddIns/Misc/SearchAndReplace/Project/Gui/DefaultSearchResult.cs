// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor.Search;
using ICSharpCode.SharpDevelop.Gui;

namespace SearchAndReplace
{
	/// <summary>
	/// Implements ISearchResult and provides the ResultsTreeView.
	/// </summary>
	public class DefaultSearchResult : ISearchResult
	{
		IList<SearchResultMatch> matches;
		SearchRootNode rootNode;
		
		public DefaultSearchResult(string pattern, IEnumerable<SearchResultMatch> matches)
		{
			if (pattern == null)
				throw new ArgumentNullException("pattern");
			if (matches == null)
				throw new ArgumentNullException("matches");
			this.matches = matches.ToArray();
			rootNode = new SearchRootNode(pattern, this.matches);
		}
		
		public string Text {
			get {
				return rootNode.GetPatternString() + " (" + SearchRootNode.GetOccurrencesString(rootNode.Occurrences) + ")";
			}
		}
		
		static ResultsTreeView resultsTreeViewInstance;
		
		public object GetControl()
		{
			WorkbenchSingleton.AssertMainThread();
			if (resultsTreeViewInstance == null)
				resultsTreeViewInstance = new ResultsTreeView();
			resultsTreeViewInstance.ItemsSource = new object[] { rootNode };
			return resultsTreeViewInstance;
		}
		
		public IList GetToolbarItems()
		{
			return null;
		}
	}
	
	public class DefaultSearchResultFactory : ISearchResultFactory
	{
		public ISearchResult CreateSearchResult(string pattern, IEnumerable<SearchResultMatch> matches)
		{
			return new DefaultSearchResult(pattern, matches);
		}
	}
}
