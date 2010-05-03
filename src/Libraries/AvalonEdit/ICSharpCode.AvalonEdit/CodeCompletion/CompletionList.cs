// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Linq;

namespace ICSharpCode.AvalonEdit.CodeCompletion
{
	/// <summary>
	/// The listbox used inside the CompletionWindow, contains CompletionListBox.
	/// </summary>
	public class CompletionList : Control
	{
		static CompletionList()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(CompletionList),
			                                         new FrameworkPropertyMetadata(typeof(CompletionList)));
		}
		
		bool isFiltering = true;
		/// <summary>
		/// If true, the CompletionList is filtered to show only matching items. Also enables search by substring.
		/// If false, enables the old behavior: no filtering, search by string.StartsWith.
		/// </summary>
		public bool IsFiltering {
			get { return isFiltering; }
			set { isFiltering = value; }
		}
		
		/// <summary>
		/// Is raised when the completion list indicates that the user has chosen
		/// an entry to be completed.
		/// </summary>
		public event EventHandler InsertionRequested;
		
		/// <summary>
		/// Raises the InsertionRequested event.
		/// </summary>
		public void RequestInsertion(EventArgs e)
		{
			if (InsertionRequested != null)
				InsertionRequested(this, e);
		}
		
		CompletionListBox listBox;
		
		/// <inheritdoc/>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			
			listBox = GetTemplateChild("PART_ListBox") as CompletionListBox;
			if (listBox != null) {
				listBox.ItemsSource = completionData;
			}
		}
		
		/// <summary>
		/// Gets the list box.
		/// </summary>
		public CompletionListBox ListBox {
			get {
				if (listBox == null)
					ApplyTemplate();
				return listBox;
			}
		}
		
		/// <summary>
		/// Gets the scroll viewer used in this list box.
		/// </summary>
		public ScrollViewer ScrollViewer {
			get { return listBox != null ? listBox.scrollViewer : null; }
		}
		
		ObservableCollection<ICompletionData> completionData = new ObservableCollection<ICompletionData>();
		
		/// <summary>
		/// Gets the list to which completion data can be added.
		/// </summary>
		public IList<ICompletionData> CompletionData {
			get { return completionData; }
		}
		
		/// <inheritdoc/>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (!e.Handled) {
				HandleKey(e);
			}
		}
		
		/// <summary>
		/// Handles a key press. Used to let the completion list handle key presses while the
		/// focus is still on the text editor.
		/// </summary>
		public void HandleKey(KeyEventArgs e)
		{
			if (listBox == null)
				return;
			
			// We have to do some key handling manually, because the default doesn't work with
			// our simulated events.
			// Also, the default PageUp/PageDown implementation changes the focus, so we avoid it.
			switch (e.Key) {
				case Key.Down:
					e.Handled = true;
					listBox.SelectIndex(listBox.SelectedIndex + 1);
					break;
				case Key.Up:
					e.Handled = true;
					listBox.SelectIndex(listBox.SelectedIndex - 1);
					break;
				case Key.PageDown:
					e.Handled = true;
					listBox.SelectIndex(listBox.SelectedIndex + listBox.VisibleItemCount);
					break;
				case Key.PageUp:
					e.Handled = true;
					listBox.SelectIndex(listBox.SelectedIndex - listBox.VisibleItemCount);
					break;
				case Key.Home:
					e.Handled = true;
					listBox.SelectIndex(0);
					break;
				case Key.End:
					e.Handled = true;
					listBox.SelectIndex(listBox.Items.Count - 1);
					break;
				case Key.Tab:
				case Key.Enter:
					e.Handled = true;
					RequestInsertion(e);
					break;
			}
		}
		
		/// <inheritdoc/>
		protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
		{
			base.OnMouseDoubleClick(e);
			if (e.ChangedButton == MouseButton.Left) {
				e.Handled = true;
				RequestInsertion(e);
			}
		}
		
		/// <summary>
		/// Gets/Sets the selected item.
		/// </summary>
		public ICompletionData SelectedItem {
			get {
				return (listBox != null ? listBox.SelectedItem : null) as ICompletionData;
			}
			set {
				if (listBox == null && value != null)
					ApplyTemplate();
				listBox.SelectedItem = value;
			}
		}
		
		/// <summary>
		/// Occurs when the SelectedItem property changes.
		/// </summary>
		public event SelectionChangedEventHandler SelectionChanged {
			add { AddHandler(Selector.SelectionChangedEvent, value); }
			remove { RemoveHandler(Selector.SelectionChangedEvent, value); }
		}
		
		/// <summary>
		/// Selects the best match, and filter the items if turned on using <see cref="IsFiltering" />.
		/// </summary>
		public void SelectItem(string text)
		{
			if (listBox == null)
				ApplyTemplate();
			
			if (this.IsFiltering) {
				FilterMatchingItems(text);
			}
			else {
				SelectItemWithStart(text);
			}
		}
		
		/// <summary>
		/// Filters CompletionList items to show only those matching given text, and selects the best match.
		/// </summary>
		void FilterMatchingItems(string text)
		{
			var itemsWithQualities =
				from item in this.completionData
				select new { Item = item, Quality = GetMatchQuality(item.Text, text) };
			var matchingItems = itemsWithQualities.Where(item => item.Quality > 0);
			
			var listBoxItems = new ObservableCollection<ICompletionData>();
			int bestIndex = -1;
			int bestQuality = -1;
			double bestPriority = 0;
			int i = 0;
			foreach (var matchingItem in matchingItems) {
				double priority = matchingItem.Item.Priority;
				int quality = matchingItem.Quality;
				if (quality > bestQuality || (quality == bestQuality && priority > bestPriority)) {
					bestIndex = i;
					bestPriority = priority;
					bestQuality = quality;
				}
				listBoxItems.Add(matchingItem.Item);
				i++;
			}
			listBox.ItemsSource = listBoxItems;
			SelectIndexCentered(bestIndex);
		}
		
		/// <summary>
		/// Selects the item that starts with the specified text.
		/// </summary>
		void SelectItemWithStart(string startText)
		{
			if (string.IsNullOrEmpty(startText))
				return;
			
			int selectedItem = listBox.SelectedIndex;
			
			int bestIndex = -1;
			int bestQuality = -1;
			double bestPriority = 0;
			for (int i = 0; i < completionData.Count; ++i) {
				int quality = GetMatchQuality(completionData[i].Text, startText);
				if (quality < 0)
					continue;
				
				double priority = completionData[i].Priority;
				bool useThisItem;
				if (bestQuality < quality) {
					useThisItem = true;
				} else {
					if (bestIndex == selectedItem) {
						// martin.konicek: I'm not sure what this does. This item could have higher priority, why not select it?
						useThisItem = false;
					} else if (i == selectedItem) {
						useThisItem = bestQuality == quality;
					} else {
						useThisItem = bestQuality == quality && bestPriority < priority;
					}
				}
				if (useThisItem) {
					bestIndex = i;
					bestPriority = priority;
					bestQuality = quality;
				}
			}
			SelectIndexCentered(bestIndex);
		}

		void SelectIndexCentered(int bestIndex)
		{
			if (bestIndex < 0) {
				listBox.ClearSelection();
			} else {
				int firstItem = listBox.FirstVisibleItem;
				if (bestIndex < firstItem || firstItem + listBox.VisibleItemCount <= bestIndex) {
					// CenterViewOn does nothing as CompletionListBox.ScrollViewer is null
					listBox.CenterViewOn(bestIndex);
					listBox.SelectIndex(bestIndex);
				} else {
					listBox.SelectIndex(bestIndex);
				}
			}
		}

		int GetMatchQuality(string itemText, string query)
		{
			// Qualities:
			//		-1 = no match
			//		1 = match CamelCase
			//		2 = match sustring
			// 		3 = match substring case sensitive
			//		4 = match CamelCase when length of query is 1 or 2 characters
			//		5 = match start
			// 		6 = match start case sensitive
			// 		7 = full match
			//  	8 = full match case sensitive
			if (query == itemText)
				return 8;
			if (string.Equals(itemText, query, StringComparison.OrdinalIgnoreCase))
				return 7;
			
			if (itemText.StartsWith(query))
				return 6;
			if (itemText.StartsWith(query, StringComparison.OrdinalIgnoreCase))
				return 5;
			
			if (query.Length <= 2 && CamelCaseMatch(itemText, query))
				return 4;
			
			// search by substring, if filtering (i.e. new behavior) turned on
			if (IsFiltering && query.Length > 1 && itemText.Contains(query))
				return 3;
			if (IsFiltering && query.Length > 1 && itemText.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
				return 2;
			
			if (CamelCaseMatch(itemText, query))
				return 1;
			
			return -1;
		}
		
		bool CamelCaseMatch(string text, string query)
		{
			int i = 0;
			foreach (char upper in text.Where(c => char.IsUpper(c))) {
				if (i > query.Length - 1)
					return false;	// return true here for CamelCase partial match (CQ match CodeQualityAnalysis)
				if (char.ToUpper(query[i]) != upper)
					return false;
				i++;
			}
			// query must have the same length as how many there are uppercase characters in text
			if (i == query.Length)
				return true;
			return false;
		}
	}
}
