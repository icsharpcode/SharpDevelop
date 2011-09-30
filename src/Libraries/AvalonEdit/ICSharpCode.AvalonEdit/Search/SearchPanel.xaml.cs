// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.Search
{
	/// <summary>
	/// Interaction logic for SearchPanel.xaml
	/// </summary>
	public partial class SearchPanel : UserControl
	{
		TextArea textArea;
		SearchResultBackgroundRenderer renderer;
		SearchResult currentResult;
		FoldingManager foldingManager;
		
		public bool UseRegex { get { return useRegex.IsChecked == true; } }
		public bool MatchCase { get { return matchCase.IsChecked == true; } }
		public bool WholeWords { get { return wholeWords.IsChecked == true; } }
		
		public SearchPanel(TextArea textArea)
		{
			if (textArea == null)
				throw new ArgumentNullException("textArea");
			this.textArea = textArea;
			
			InitializeComponent();
			
			textArea.TextView.Layers.Add(this);
			foldingManager = textArea.GetService(typeof(FoldingManager)) as FoldingManager;
			
			renderer = new SearchResultBackgroundRenderer();
			textArea.TextView.BackgroundRenderers.Add(renderer);
			textArea.Document.TextChanged += delegate { DoSearch(false); };
			this.Loaded += delegate { searchTextBox.Focus(); };
			
			useRegex.Checked += delegate { DoSearch(true); };
			matchCase.Checked += delegate { DoSearch(true); };
			wholeWords.Checked += delegate { DoSearch(true); };
			
			useRegex.Unchecked += delegate { DoSearch(true); };
			matchCase.Unchecked += delegate { DoSearch(true); };
			wholeWords.Unchecked += delegate { DoSearch(true); };
		}

		void SearchTextBoxTextChanged(object sender, TextChangedEventArgs e)
		{
			DoSearch(true);
		}
		
		/// <summary>
		/// Reactivates the SearchPanel by setting the focus on the search box and selecting all text.
		/// </summary>
		public void Reactivate()
		{
			searchTextBox.Focus();
			searchTextBox.SelectAll();
		}
		
		public void FindNext()
		{
			SearchResult result = null;
			if (currentResult != null)
				result = renderer.CurrentResults.GetNextSegment(currentResult);
			if (result == null)
				result = renderer.CurrentResults.FirstSegment;
			if (result != null) {
				currentResult = result;
				SetResult(result);
			}
		}
		
		public void FindPrevious()
		{
			SearchResult result = null;
			if (currentResult != null)
				result = renderer.CurrentResults.GetPreviousSegment(currentResult);
			if (result == null)
				result = renderer.CurrentResults.LastSegment;
			if (result != null) {
				currentResult = result;
				SetResult(result);
			}
		}

		void DoSearch(bool changeSelection)
		{
			renderer.CurrentResults.Clear();
			currentResult = null;
			messageView.Visibility = Visibility.Collapsed;
			if (!string.IsNullOrEmpty(searchTextBox.Text)) {
				try {
					ISearchStrategy strategy = DefaultSearchStrategy.Create(searchTextBox.Text, !MatchCase, UseRegex, WholeWords);
					int offset = textArea.Caret.Offset;
					if (changeSelection) {
						textArea.Selection = SimpleSelection.Empty;
					}
					foreach (SearchResult result in strategy.FindAll(textArea.Document)) {
						if (currentResult == null && result.StartOffset >= offset) {
							currentResult = result;
							if (changeSelection) {
								SetResult(result);
							}
						}
						renderer.CurrentResults.Add(result);
					}
				} catch (SearchPatternException ex) {
					messageView.Text = "Error: " + ex.Message;
					messageView.Visibility = Visibility.Visible;
				}
			}
			textArea.TextView.InvalidateLayer(KnownLayer.Selection);
		}

		void SetResult(SearchResult result)
		{
			textArea.Caret.Offset = currentResult.StartOffset;
			textArea.Selection = new SimpleSelection(currentResult.StartOffset, currentResult.EndOffset);
			if (foldingManager != null) {
				foreach (var folding in foldingManager.GetFoldingsContaining(result.StartOffset))
					folding.IsFolded = false;
			}
		}
		
		void SearchLayerKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape) {
				CloseClick(sender, e);
			}
		}
		
		void CloseClick(object sender, RoutedEventArgs e)
		{
			textArea.TextView.Layers.Remove(this);
			textArea.TextView.BackgroundRenderers.Remove(renderer);
		}
	}
}