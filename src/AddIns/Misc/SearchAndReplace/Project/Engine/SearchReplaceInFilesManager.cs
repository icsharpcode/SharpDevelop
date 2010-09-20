// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Editor.Search;
using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace SearchAndReplace
{
	public static class SearchInFilesManager
	{
		static Search find               = new Search();
		
		static string    currentFileName = String.Empty;
		
		static SearchInFilesManager()
		{
			find.TextIteratorBuilder = new ForwardTextIteratorBuilder();
		}
		
		static void SetSearchOptions(IProgressMonitor monitor)
		{
			find.SearchStrategy   = SearchReplaceUtilities.CreateSearchStrategy(SearchOptions.SearchStrategyType);
			find.DocumentIterator = SearchReplaceUtilities.CreateDocumentIterator(SearchOptions.DocumentIteratorType, monitor);
		}
		
		static bool InitializeSearchInFiles(IProgressMonitor monitor)
		{
			SetSearchOptions(monitor);
			
			find.Reset();
			if (!find.SearchStrategy.CompilePattern(monitor))
				return false;
			
			currentFileName = String.Empty;
			return true;
		}
		
		static void FinishSearchInFiles(List<SearchResultMatch> results)
		{
			ShowSearchResults(SearchOptions.FindPattern, results);
		}
		
		public static void ShowSearchResults(string pattern, List<SearchResultMatch> results)
		{
			string title = StringParser.Parse("${res:MainWindow.Windows.SearchResultPanel.OccurrencesOf}",
			                                  new string[,] {{ "Pattern", pattern }});
			SearchResultsPad.Instance.ShowSearchResults(title, results);
			SearchResultsPad.Instance.BringToFront();
		}
		
		public static void FindAll(IProgressMonitor monitor)
		{
			if (!InitializeSearchInFiles(monitor)) {
				return;
			}
			
			List<SearchResultMatch> results = new List<SearchResultMatch>();
			while (true) {
				SearchResultMatch result = find.FindNext(monitor);
				if (result == null) {
					break;
				}
				results.Add(result);
			}
			FinishSearchInFiles(results);
		}
		
		public static void FindAll(int offset, int length, IProgressMonitor monitor)
		{
			if (!InitializeSearchInFiles(monitor)) {
				return;
			}
			
			List<SearchResultMatch> results = new List<SearchResultMatch>();
			while (true) {
				SearchResultMatch result = find.FindNext(offset, length);
				if (result == null) {
					break;
				}
				results.Add(result);
			}
			FinishSearchInFiles(results);
		}
	}
}
