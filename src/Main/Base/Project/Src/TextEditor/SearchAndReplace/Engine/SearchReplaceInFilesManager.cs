// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using ICSharpCode.SharpDevelop.Gui;

using ICSharpCode.Core;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace SearchAndReplace
{
	public static class SearchReplaceInFilesManager
	{
		static Search find               = new Search();
		
		static string    currentFileName = String.Empty;
		static List<SearchAllFinishedEventArgs> lastSearches = new List<SearchAllFinishedEventArgs> ();
		
		public static List<SearchAllFinishedEventArgs> LastSearches {
			get {
				return lastSearches;
			}
		}
		
		static SearchReplaceInFilesManager()
		{
			find.TextIteratorBuilder = new ForwardTextIteratorBuilder();
		}
		
		static void SetSearchOptions()
		{
			find.SearchStrategy   = SearchReplaceUtilities.CreateSearchStrategy(SearchOptions.SearchStrategyType);
			find.DocumentIterator = SearchReplaceUtilities.CreateDocumentIterator(SearchOptions.DocumentIteratorType);
		}
		
		static bool InitializeSearchInFiles()
		{
			SetSearchOptions();
			
			find.Reset();
			find.SearchStrategy.CompilePattern();
			
			currentFileName = String.Empty;
			return true;
		}
		
		static void FinishSearchInFiles(List<SearchResult> results)
		{
			PadDescriptor searchResultPanel = WorkbenchSingleton.Workbench.GetPad(typeof(SearchResultPanel));
			if (searchResultPanel != null) {
				searchResultPanel.BringPadToFront();
				SearchResultPanel.Instance.ShowSearchResults(SearchOptions.FindPattern, results);
			} else {
				MessageService.ShowError("SearchResultPanel can't be created.");
			}
		}
		
		public static void ReplaceAll()
		{
			if (!InitializeSearchInFiles()) {
				return;
			}
			
			List<SearchResult> results = new List<SearchResult>();
			
			while (true) {
				SearchResult result = find.FindNext();
				if (result == null) {
					break;
				}
				
				find.Replace(result.Offset, 
				             result.Length, 
				             result.TransformReplacePattern(SearchOptions.ReplacePattern));
				
				results.Add(result);
			}
			
			FinishSearchInFiles(results);
		}
		
		public static void FindAll()
		{
			if (!InitializeSearchInFiles()) {
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
			OnSearchAllFinished(new SearchAllFinishedEventArgs(SearchOptions.FindPattern, results));
		}
			
		static void OnSearchAllFinished(SearchAllFinishedEventArgs e)
		{
			lastSearches.Add(e);
			if (SearchAllFinished != null) {
				SearchAllFinished(null, e);
			}
		}
		
		public static event SearchAllFinishedEventHandler SearchAllFinished;
	}
}
