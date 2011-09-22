// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
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
		
		public SearchPanel(TextArea textArea)
		{
			if (textArea == null)
				throw new ArgumentNullException("textArea");
			this.textArea = textArea;
			
			InitializeComponent();
			
			textArea.TextView.Layers.Add(this);
			
			renderer = new SearchResultBackgroundRenderer();
			textArea.TextView.BackgroundRenderers.Add(renderer);
			textArea.Document.TextChanged += delegate { DoSearch(false); };
			
			Dispatcher.Invoke(DispatcherPriority.Input, (Action)(() => searchTextBox.Focus()));
		}

		void SearchTextBoxTextChanged(object sender, TextChangedEventArgs e)
		{
			DoSearch(true);
		}

		void DoSearch(bool changeSelection)
		{
			renderer.CurrentResults.Clear();
			if (!string.IsNullOrEmpty(searchTextBox.Text)) {

				currentResult = null;
				int offset = textArea.Caret.Offset;
				foreach (var result in FindAll(searchTextBox.Text, textArea.Document)) {
					if (currentResult == null && result.StartOffset >= offset) {
						currentResult = result;
						if (changeSelection) {
							textArea.Caret.Offset = currentResult.StartOffset;
							textArea.Selection = new SimpleSelection(currentResult.StartOffset, currentResult.EndOffset);
						}
					}
					renderer.CurrentResults.Add(result);
				}
			}
			textArea.TextView.InvalidateLayer(KnownLayer.Selection);
		}
		
		IEnumerable<SearchResult> FindAll(string search, TextDocument document)
		{
			SearchResult lastResult = FindNext(search, 0, document);
			while (lastResult != null) {
				yield return lastResult;
				lastResult = FindNext(search, lastResult.StartOffset + lastResult.Length, document);
			}
		}
		
		SearchResult FindNext(string search, int index, TextDocument document)
		{
			int result = document.Text.IndexOf(search, index, StringComparison.OrdinalIgnoreCase);
			if (result > -1)
				return new SearchResult { StartOffset = result, Length = search.Length };
			return null;
		}
		
		SearchResult FindPrev(string search, int index, TextDocument document)
		{
			int result = document.GetText(0, index).LastIndexOf(search, StringComparison.OrdinalIgnoreCase);
			if (result > -1)
				return new SearchResult { StartOffset = result, Length = search.Length };
			return null;
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
		
		void PrevClick(object sender, RoutedEventArgs e)
		{
			if (currentResult != null) {
				var result = FindPrev(searchTextBox.Text, currentResult.StartOffset, textArea.Document);
				if (result != null) {
					currentResult = result;
					textArea.Selection = new SimpleSelection(currentResult.StartOffset, currentResult.EndOffset);
				}
			}
		}
		
		void NextClick(object sender, RoutedEventArgs e)
		{
			if (currentResult != null) {
				var result = FindNext(searchTextBox.Text, currentResult.EndOffset, textArea.Document);
				if (result != null) {
					currentResult = result;
					textArea.Selection = new SimpleSelection(currentResult.StartOffset, currentResult.EndOffset);
				}
			}
		}
	}
}