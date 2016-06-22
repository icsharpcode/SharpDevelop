// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

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
					SD.Workbench.GetPad(typeof(SearchResultsPad)).CreatePad();
				}
				return instance;
			}
		}
		
		Grid contentPanel;
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
			defaultToolbarItems = ToolBarService.CreateToolBarItems(contentPanel, this, "/SharpDevelop/Pads/SearchResultPad/Toolbar");
			foreach (object toolBarItem in defaultToolbarItems) {
				toolBar.Items.Add(toolBarItem);
			}
			
			contentPlaceholder = new ContentPresenter();
			contentPanel = new Grid {
				Children = { toolBar, contentPlaceholder }
			};
			
			contentPanel.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			contentPanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

			Grid.SetRow(contentPlaceholder, 1);
		}
		
		public override object Control {
			get {
				return contentPanel;
			}
		}
		
		ISearchResult activeSearchResult;
		List<ISearchResult> lastSearches = new List<ISearchResult>();
		
		public IEnumerable<ISearchResult> LastSearches {
			get { return lastSearches; }
		}
		
		public void ClearLastSearchesList()
		{
			lastSearches.Clear();
			if (activeSearchResult != null) {
				activeSearchResult.OnDeactivate();
				activeSearchResult = null;
			}
			SD.WinForms.SetContent(contentPlaceholder, null);
			// clear search pad toolbar items when ClearLastSearchesList() is called
			toolBar.Items.Clear();
			foreach (object toolBarItem in defaultToolbarItems) {
				toolBar.Items.Add(toolBarItem);
			}
		}
		
		/// <summary>
		/// Shows a search in the search results pad.
		/// The previously shown search will be stored in the list of past searches.
		/// </summary>
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
			
			if (activeSearchResult != result) {
				if (activeSearchResult != null) {
					activeSearchResult.OnDeactivate();
				}
				activeSearchResult = result;
			}
			SD.WinForms.SetContent(contentPlaceholder, result.GetControl());
			
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
			
			SearchResultsShown(this, EventArgs.Empty);
		}
		
		/// <summary>
		/// Shows a search in the search results pad.
		/// The previously shown search will be stored in the list of past searches.
		/// </summary>
		/// <param name="title">The title of the search.</param>
		/// <param name="matches">The list of matches. ShowSearchResults() will enumerate once through the IEnumerable in order to retrieve the search results.</param>
		public void ShowSearchResults(string title, IEnumerable<SearchResultMatch> matches)
		{
			ShowSearchResults(CreateSearchResult(title, matches));
		}
		
		/// <summary>
		/// Performs a background search in the search results pad.
		/// The previously shown search will be stored in the list of past searches.
		/// </summary>
		/// <param name="title">The title of the search.</param>
		/// <param name="matches">The background search operation. ShowSearchResults() will subscribe to the observable in order to retrieve the search results.</param>
		public void ShowSearchResults(string title, IObservable<SearchedFile> matches)
		{
			ShowSearchResults(CreateSearchResult(title, matches));
		}
		
		public event EventHandler SearchResultsShown = delegate {};
		
		/// <inheritdoc cref="ISearchResultFactory.CreateSearchResult(string,IEnumerable{SearchResultMatch})"/>
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
		
		
		/// <inheritdoc cref="ISearchResultFactory.CreateSearchResult(string,IObservable{SearchResultMatch})"/>
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
		
		public static RichText CreateInlineBuilder(TextLocation startPosition, TextLocation endPosition, IDocument document, IHighlighter highlighter)
		{
			if (startPosition.Line >= 1 && startPosition.Line <= document.LineCount) {
				var highlightedLine = highlighter.HighlightLine(startPosition.Line);
				var documentLine = highlightedLine.DocumentLine;
				var inlineBuilder = highlightedLine.ToRichTextModel();
				// reset bold/italics
				inlineBuilder.SetFontWeight(0, documentLine.Length, FontWeights.Normal);
				inlineBuilder.SetFontStyle(0, documentLine.Length, FontStyles.Normal);
				
				// now highlight the match in bold
				if (startPosition.Column >= 1) {
					if (endPosition.Line == startPosition.Line && endPosition.Column > startPosition.Column) {
						// subtract one from the column to get the offset inside the line's text
						int startOffset = startPosition.Column - 1;
						int endOffset = Math.Min(documentLine.Length, endPosition.Column - 1);
						inlineBuilder.SetFontWeight(startOffset, endOffset - startOffset, FontWeights.Bold);
					}
				}
				return new RichText(document.GetText(documentLine), inlineBuilder);
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
			
			public void OnDeactivate()
			{
			}
		}
	}
}
