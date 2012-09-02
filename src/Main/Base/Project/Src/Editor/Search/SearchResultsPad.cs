// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Editor.Search
{
	/// <summary>
	/// Pad that displays search results.
	/// </summary>
	public class SearchResultsPad : AbstractPadContent
	{
		static SearchResultsPad instance;
		
		public static SearchResultsPad Instance {
			get {
				if (instance == null) {
					WorkbenchSingleton.Workbench.GetPad(typeof(SearchResultsPad)).CreatePad();
				}
				return instance;
			}
		}
		
		DockPanel dockPanel;
		ToolBar toolBar;
		ContentPresenter contentPlaceholder;
		IList defaultToolbarItems;
		
		public SearchResultsPad()
		{
			if (instance != null)
				throw new InvalidOperationException("Cannot create multiple instances");
			instance = this;
			toolBar = new ToolBar();
			ToolBarTray.SetIsLocked(toolBar, true);
			defaultToolbarItems = ToolBarService.CreateToolBarItems(dockPanel, this, "/SharpDevelop/Pads/SearchResultPad/Toolbar");
			foreach (object toolBarItem in defaultToolbarItems) {
				toolBar.Items.Add(toolBarItem);
			}
			
			DockPanel.SetDock(toolBar, Dock.Top);
			contentPlaceholder = new ContentPresenter();
			dockPanel = new DockPanel {
				Children = { toolBar, contentPlaceholder }
			};
		}
		
		public override object Control {
			get {
				return dockPanel;
			}
		}
		
		List<ISearchResult> lastSearches = new List<ISearchResult>();
		
		public IEnumerable<ISearchResult> LastSearches {
			get { return lastSearches; }
		}
		
		public void ClearLastSearchesList()
		{
			lastSearches.Clear();
			contentPlaceholder.SetContent(null);
		}
		
		public void ShowSearchResults(ISearchResult result)
		{
			if (result == null)
				throw new ArgumentNullException("result");
			
			// move result to top of last searches
			lastSearches.Remove(result);
			lastSearches.Insert(0, result);
			
			// limit list of last searches to 15 entries
			while (lastSearches.Count > 15)
				lastSearches.RemoveAt(15);
			
			contentPlaceholder.SetContent(result.GetControl());
			
			toolBar.Items.Clear();
			foreach (object toolBarItem in defaultToolbarItems) {
				toolBar.Items.Add(toolBarItem);
			}
			IList additionalToolbarItems = result.GetToolbarItems();
			if (additionalToolbarItems != null) {
				toolBar.Items.Add(new Separator());
				foreach (object toolBarItem in additionalToolbarItems) {
					toolBar.Items.Add(toolBarItem);
				}
			}
			
			SearchResultsShown.RaiseEvent(this, EventArgs.Empty);
		}
		
		public void ShowSearchResults(string title, IEnumerable<SearchResultMatch> matches)
		{
			ShowSearchResults(CreateSearchResult(title, matches));
		}
		
		public void ShowSearchResults(string title, IObservable<SearchedFile> matches)
		{
			ShowSearchResults(CreateSearchResult(title, matches));
		}
		
		public event EventHandler SearchResultsShown;
		
		public static ISearchResult CreateSearchResult(string title, IEnumerable<SearchResultMatch> matches)
		{
			if (title == null)
				throw new ArgumentNullException("title");
			if (matches == null)
				throw new ArgumentNullException("matches");
			foreach (ISearchResultFactory factory in AddInTree.BuildItems<ISearchResultFactory>("/SharpDevelop/Pads/SearchResultPad/Factories", null, false)) {
				ISearchResult result = factory.CreateSearchResult(title, matches);
				if (result != null)
					return result;
			}
			return new DummySearchResult { Text = title };
		}
		
		
		public static ISearchResult CreateSearchResult(string title, IObservable<SearchedFile> matches)
		{
			if (title == null)
				throw new ArgumentNullException("title");
			if (matches == null)
				throw new ArgumentNullException("matches");
			foreach (ISearchResultFactory factory in AddInTree.BuildItems<ISearchResultFactory>("/SharpDevelop/Pads/SearchResultPad/Factories", null, false)) {
				ISearchResult result = factory.CreateSearchResult(title, matches);
				if (result != null)
					return result;
			}
			return new DummySearchResult { Text = title };
		}
		
		public static HighlightedInlineBuilder CreateInlineBuilder(Location startPosition, Location endPosition, TextDocument document, ISyntaxHighlighter highlighter)
		{
			if (startPosition.Line >= 1 && startPosition.Line <= document.LineCount) {
				HighlightedInlineBuilder inlineBuilder;
				if (highlighter != null) {
					inlineBuilder = highlighter.BuildInlines(startPosition.Line);
				} else {
					inlineBuilder = new HighlightedInlineBuilder(document.GetText(document.GetLineByNumber(startPosition.Line)));
				}
				
				// now highlight the match in bold
				if (startPosition.Column >= 1) {
					if (endPosition.Line == startPosition.Line && endPosition.Column > startPosition.Column) {
						// subtract one from the column to get the offset inside the line's text
						int startOffset = startPosition.Column - 1;
						int endOffset = Math.Min(inlineBuilder.Text.Length, endPosition.Column - 1);
						inlineBuilder.SetFontWeight(startOffset, endOffset - startOffset, FontWeights.Bold);
					}
				}
				return inlineBuilder;
			}
			return null;
		}
		
		sealed class DummySearchResult : ISearchResult
		{
			public string Text {
				get; set;
			}
			
			public object GetControl()
			{
				return "Could not find ISearchResultFactory. Did you disable the search+replace addin?";
			}
			
			public IList GetToolbarItems()
			{
				return null;
			}
		}
	}
}
