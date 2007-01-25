// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

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
		static List<SearchAllFinishedEventArgs> lastSearches = new List<SearchAllFinishedEventArgs> ();
		
		public static List<SearchAllFinishedEventArgs> LastSearches {
			get {
				return lastSearches;
			}
		}
		
		static SearchInFilesManager()
		{
			find.TextIteratorBuilder = new ForwardTextIteratorBuilder();
		}
		
		static void SetSearchOptions()
		{
			find.SearchStrategy   = SearchReplaceUtilities.CreateSearchStrategy(SearchOptions.SearchStrategyType);
			find.DocumentIterator = SearchReplaceUtilities.CreateDocumentIterator(SearchOptions.DocumentIteratorType);
		}
		
		static bool InitializeSearchInFiles(IProgressMonitor monitor)
		{
			SetSearchOptions();
			
			find.Reset();
			if (!find.SearchStrategy.CompilePattern(monitor))
				return false;
			
			currentFileName = String.Empty;
			return true;
		}
		
		static void FinishSearchInFiles(List<SearchResult> results)
		{
			ShowSearchResults(SearchOptions.FindPattern, results);
		}
		
		public static void ShowSearchResults(string pattern, List<SearchResult> results)
		{
			SearchAndReplace.SearchAllFinishedEventArgs e =
				new SearchAllFinishedEventArgs(pattern, results);
			OnSearchAllFinished(e);

			PadDescriptor searchResultPanel = WorkbenchSingleton.Workbench.GetPad(typeof(SearchResultPanel));
			if (searchResultPanel != null) {
				searchResultPanel.BringPadToFront();
				SearchResultPanel.Instance.ShowSearchResults(pattern, results);
			} else {
				MessageService.ShowError("SearchResultPanel can't be created.");
			}
		}
		
		public static void FindAll(IProgressMonitor monitor)
		{
			if (!InitializeSearchInFiles(monitor)) {
				return;
			}
			
			List<SearchResult> results = new List<SearchResult>();
			while (true) {
				SearchResult result = find.FindNext();
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
			
			List<SearchResult> results = new List<SearchResult>();
			while (true) {
				SearchResult result = find.FindNext(offset, length);
				if (result == null) {
					break;
				}
				results.Add(result);
			}
			FinishSearchInFiles(results);
		}
		
		static void OnSearchAllFinished(SearchAllFinishedEventArgs e)
		{
			lastSearches.Insert(0, e);
			if (SearchAllFinished != null) {
				SearchAllFinished(null, e);
			}
		}
		
		public static event SearchAllFinishedEventHandler SearchAllFinished;
	}
}
