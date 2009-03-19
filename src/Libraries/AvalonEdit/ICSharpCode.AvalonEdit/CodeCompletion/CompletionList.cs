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
			get { return listBox; }
		}
		
		/// <summary>
		/// Gets the scroll viewer used in this list box.
		/// </summary>
		public ScrollViewer ScrollViewer {
			get { return listBox != null ? listBox.scrollViewer : null; }
		}
		
		ObservableCollection<ICompletionData> completionData = new ObservableCollection<ICompletionData>();
		
		/// <summary>
		/// Gets/Sets the completion data.
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
					SelectIndex(listBox.SelectedIndex + 1);
					break;
				case Key.Up:
					e.Handled = true;
					SelectIndex(listBox.SelectedIndex - 1);
					break;
				case Key.PageDown:
					e.Handled = true;
					SelectIndex(listBox.SelectedIndex + listBox.VisibleItemCount);
					break;
				case Key.PageUp:
					e.Handled = true;
					SelectIndex(listBox.SelectedIndex - listBox.VisibleItemCount);
					break;
				case Key.Home:
					e.Handled = true;
					SelectIndex(0);
					break;
				case Key.End:
					e.Handled = true;
					SelectIndex(listBox.Items.Count - 1);
					break;
			}
		}
		
		void SelectIndex(int offset)
		{
			if (offset >= listBox.Items.Count)
				offset = listBox.Items.Count - 1;
			if (offset < 0)
				offset = 0;
			listBox.SelectedIndex = offset;
			listBox.ScrollIntoView(listBox.SelectedItem);
		}
	}
}
