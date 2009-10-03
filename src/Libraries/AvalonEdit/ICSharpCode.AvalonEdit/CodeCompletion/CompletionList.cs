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

namespace ICSharpCode.AvalonEdit.CodeCompletion
{
	/// <summary>
	/// The listbox used inside the CompletionWindow.
	/// </summary>
	public class CompletionList : Control
	{
		static CompletionList()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(CompletionList),
			                                         new FrameworkPropertyMetadata(typeof(CompletionList)));
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
		/// Selects the item that starts with the specified text.
		/// </summary>
		public void SelectItemWithStart(string startText)
		{
			if (string.IsNullOrEmpty(startText))
				return;
			if (listBox == null)
				ApplyTemplate();
			int selectedItem = listBox.SelectedIndex;
			
			int bestIndex = -1;
			int bestQuality = -1;
			// Qualities: 0 = match start
			//            1 = match start case sensitive
			//            2 = full match
			//            3 = full match case sensitive
			double bestPriority = 0;
			for (int i = 0; i < completionData.Count; ++i) {
				string itemText = completionData[i].Text;
				if (itemText.StartsWith(startText, StringComparison.OrdinalIgnoreCase)) {
					double priority = 0; //completionData[i].Priority;
					int quality;
					if (string.Equals(itemText, startText, StringComparison.OrdinalIgnoreCase)) {
						if (startText == itemText)
							quality = 3;
						else
							quality = 2;
					} else if (itemText.StartsWith(startText, StringComparison.Ordinal)) {
						quality = 1;
					} else {
						quality = 0;
					}
					bool useThisItem;
					if (bestQuality < quality) {
						useThisItem = true;
					} else {
						if (bestIndex == selectedItem) {
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
			}
			if (bestIndex < 0) {
				listBox.ClearSelection();
			} else {
				int firstItem = listBox.FirstVisibleItem;
				if (bestIndex < firstItem || firstItem + listBox.VisibleItemCount <= bestIndex) {
					listBox.CenterViewOn(bestIndex);
					listBox.SelectIndex(bestIndex);
				} else {
					listBox.SelectIndex(bestIndex);
				}
			}
		}
	}
}
