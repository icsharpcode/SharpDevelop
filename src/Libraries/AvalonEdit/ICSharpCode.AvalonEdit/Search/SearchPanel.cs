// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
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
	public class SearchPanel : Control
	{
		TextArea textArea;
		SearchResultBackgroundRenderer renderer;
		SearchResult currentResult;
		FoldingManager foldingManager;
		TextBox searchTextBox;
		SearchPanelAdorner adorner;
		
		public static readonly DependencyProperty UseRegexProperty =
			DependencyProperty.Register("UseRegex", typeof(bool), typeof(SearchPanel),
			                            new FrameworkPropertyMetadata(false, SearchPatternChangedCallback));
		
		public bool UseRegex {
			get { return (bool)GetValue(UseRegexProperty); }
			set { SetValue(UseRegexProperty, value); }
		}
		
		public static readonly DependencyProperty MatchCaseProperty =
			DependencyProperty.Register("MatchCase", typeof(bool), typeof(SearchPanel),
			                            new FrameworkPropertyMetadata(false, SearchPatternChangedCallback));
		
		public bool MatchCase {
			get { return (bool)GetValue(MatchCaseProperty); }
			set { SetValue(MatchCaseProperty, value); }
		}
		
		public static readonly DependencyProperty WholeWordsProperty =
			DependencyProperty.Register("WholeWords", typeof(bool), typeof(SearchPanel),
			                            new FrameworkPropertyMetadata(false, SearchPatternChangedCallback));
		
		public bool WholeWords {
			get { return (bool)GetValue(WholeWordsProperty); }
			set { SetValue(WholeWordsProperty, value); }
		}
		
		public static readonly DependencyProperty SearchPatternProperty =
			DependencyProperty.Register("SearchPattern", typeof(string), typeof(SearchPanel),
			                            new FrameworkPropertyMetadata("", SearchPatternChangedCallback));
		
		public string SearchPattern {
			get { return (string)GetValue(SearchPatternProperty); }
			set { SetValue(SearchPatternProperty, value); }
		}
		
		static SearchPanel()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchPanel), new FrameworkPropertyMetadata(typeof(SearchPanel)));
		}
		
		ISearchStrategy strategy;
		
		static void SearchPatternChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			SearchPanel panel = d as SearchPanel;
			if (panel != null) {
				panel.ValidateSearchText();
				panel.UpdateSearch();
			}
		}

		void UpdateSearch()
		{
			messageView.IsOpen = false;
			strategy = SearchStrategyFactory.Create(SearchPattern ?? "", !MatchCase, WholeWords, UseRegex ? SearchMode.RegEx : SearchMode.Normal);
			DoSearch(true);
		}
		
		/// <summary>
		/// Creates a new SearchPanel.
		/// </summary>
		public SearchPanel()
		{
		}
		
		public void Attach(TextArea textArea)
		{
			if (textArea == null)
				throw new ArgumentNullException("textArea");
			this.textArea = textArea;
			var layer = AdornerLayer.GetAdornerLayer(textArea);
			adorner = new SearchPanelAdorner(textArea, this);
			if (layer != null)
				layer.Add(adorner);
			DataContext = this;
			
			foldingManager = textArea.GetService(typeof(FoldingManager)) as FoldingManager;
			
			renderer = new SearchResultBackgroundRenderer();
			textArea.TextView.BackgroundRenderers.Add(renderer);
			textArea.Document.TextChanged += delegate { DoSearch(false); };
			KeyDown += SearchLayerKeyDown;
			
			this.CommandBindings.Add(new CommandBinding(SearchCommands.FindNext, (sender, e) => FindNext()));
			this.CommandBindings.Add(new CommandBinding(SearchCommands.FindPrevious, (sender, e) => FindPrevious()));
			this.CommandBindings.Add(new CommandBinding(SearchCommands.CloseSearchPanel, (sender, e) => Close()));
		}
		
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			searchTextBox = Template.FindName("PART_searchTextBox", this) as TextBox;
		}
		
		void ValidateSearchText()
		{
			if (searchTextBox == null)
				return;
			var be = searchTextBox.GetBindingExpression(TextBox.TextProperty);
			try {
				Validation.ClearInvalid(be);
				UpdateSearch();
			} catch (SearchPatternException ex) {
				var ve = new ValidationError(be.ParentBinding.ValidationRules[0], be, ex.Message, ex);
				Validation.MarkInvalid(be, ve);
			}
		}
		
		/// <summary>
		/// Reactivates the SearchPanel by setting the focus on the search box and selecting all text.
		/// </summary>
		public void Reactivate()
		{
			if (searchTextBox == null)
				return;
			searchTextBox.Focus();
			searchTextBox.SelectAll();
		}
		
		/// <summary>
		/// Moves to the next occurrence in the file.
		/// </summary>
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
		
		/// <summary>
		/// Moves to the previous occurrence in the file.
		/// </summary>
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
		
		ToolTip messageView = new ToolTip { Placement = PlacementMode.Bottom };

		void DoSearch(bool changeSelection)
		{
			renderer.CurrentResults.Clear();
			currentResult = null;
			if (!string.IsNullOrEmpty(SearchPattern)) {
				int offset = textArea.Caret.Offset;
				if (changeSelection) {
					textArea.Selection = SimpleSelection.Empty;
				}
				foreach (SearchResult result in strategy.FindAll(textArea.Document, 0, textArea.Document.TextLength)) {
					if (currentResult == null && result.StartOffset >= offset) {
						currentResult = result;
						if (changeSelection) {
							SetResult(result);
						}
					}
					renderer.CurrentResults.Add(result);
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
			textArea.Caret.BringCaretToView();
		}
		
		void SearchLayerKeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key) {
				case Key.Enter:
					e.Handled = true;
					messageView.IsOpen = false;
					if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
						FindPrevious();
					else
						FindNext();
					if (searchTextBox != null) {
						var error = Validation.GetErrors(searchTextBox).FirstOrDefault();
						if (error != null) {
							messageView.Content = "Error: " + error.ErrorContent;
							messageView.PlacementTarget = searchTextBox;
							messageView.IsOpen = true;
						}
					}
					break;
				case Key.Escape:
					e.Handled = true;
					Close();
					break;
			}
		}
		
		/// <summary>
		/// Closes the SearchPanel.
		/// </summary>
		public void Close()
		{
			var layer = AdornerLayer.GetAdornerLayer(textArea);
			if (layer != null)
				layer.Remove(adorner);
			textArea.TextView.BackgroundRenderers.Remove(renderer);
			messageView.IsOpen = false;
		}
	}
	
	class SearchPanelAdorner : Adorner
	{
		SearchPanel panel;
		
		public SearchPanelAdorner(TextArea textArea, SearchPanel panel)
			: base(textArea)
		{
			this.panel = panel;
			AddVisualChild(panel);
		}
		
		protected override int VisualChildrenCount {
			get { return 1; }
		}

		protected override Visual GetVisualChild(int index)
		{
			if (index != 0)
				throw new ArgumentOutOfRangeException();
			return panel;
		}
		
		protected override Size ArrangeOverride(Size finalSize)
		{
			panel.Arrange(new Rect(new Point(0, 0), finalSize));
			return new Size(panel.ActualWidth, panel.ActualHeight);
		}
	}
}