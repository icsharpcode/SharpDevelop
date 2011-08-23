// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Debugging;

namespace ICSharpCode.SharpDevelop.Bookmarks.Pad.Controls
{
	/// <summary>
	/// ListViewPad inside WPF pads.
	/// </summary>
	public sealed partial class ListViewPad : UserControl
	{
		ObservableCollection<ListViewPadItemModel> itemCollection = new ObservableCollection<ListViewPadItemModel>();
		
		public ObservableCollection<ListViewPadItemModel> ItemCollection {
			get { return itemCollection; } 
		}
		
		public event EventHandler ItemActivated;
		
		public ListViewPad()
		{
			InitializeComponent();
			
			this.MyListView.PreviewMouseDoubleClick += new MouseButtonEventHandler(ListViewPad_PreviewMouseDoubleClick);
			this.MyListView.KeyDown += new KeyEventHandler(ListViewPad_KeyDown);
		}
		
		public ListViewPadItemModel CurrentItem {
			get {
				if (MyListView.SelectedItem == null && MyListView.Items.Count > 0)
					this.MyListView.SelectedItem	= MyListView.Items[0];
				
				return MyListView.SelectedItem as ListViewPadItemModel;
			}
			set {
				if (value == null) return;

				this.MyListView.SelectedItem = value;
			}
		}
		
		public ListViewPadItemModel NextItem {
			get {
				bool found = false;
				foreach (var line in ItemCollection) {
					if (found)
						return line;
					if (line == CurrentItem)
						found = true;
				}
				
				return null;
			}
		}
		
		public ListViewPadItemModel PreviousItem {
			get {
				bool found = false;
				ListViewPadItemModel prev = null;
				foreach (var line in ItemCollection) {
					if (found)
						return prev;
					if (line == CurrentItem) {
						found = true;
					}
					else {
						prev = line;
					}
				}
				
				return prev;
			}
		}
		
		public void Add(ListViewPadItemModel item)
		{
			if (item == null) return;
			ItemCollection.Add(item);
		}
		
		public ListViewPadItemModel Remove(SDBookmark bookmark)
		{
			if (bookmark is CurrentLineBookmark)
				return null;
			
			foreach (var model in itemCollection) {
				SDBookmark currentBookmark = model.Mark as SDBookmark;
				
				if (bookmark.FileName == currentBookmark.FileName &&
				    bookmark.LineNumber == currentBookmark.LineNumber) {
					ItemCollection.Remove(model);
					return model;
				}
			}
			
			return null;
		}
		
		public void AddColumn(string header, DataTemplate cellTemplate)
		{
			GridViewColumn column = new GridViewColumn();
			column.Header = header;
			column.CellTemplate = cellTemplate;
			((GridView)this.MyListView.View).Columns.Add(column);
		}
		
		/// <summary>
		/// Indexes from end to start.
		/// </summary>
		/// <param name="columnIndex"></param>
		public void HideColumns(params int[] columnIndexes)
		{
			foreach(int i in columnIndexes)
				((GridView)MyListView.View).Columns.RemoveAt(i);
		}
		
		private void ListViewPad_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var handler = ItemActivated;
			
			if (handler != null)
				ItemActivated(this, EventArgs.Empty);	
		}
		
		private void ListViewPad_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape) {
				this.MyListView.UnselectAll();
				e.Handled = true;
			}
		}
	}
	
	public sealed class ListViewPadItemModel : INotifyPropertyChanged
	{
		bool isChecked;
		object tag;
		string language;
		string condition;
		ImageSource imageSource;
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		public ListViewPadItemModel(SDBookmark mark)
		{
			if (mark is BreakpointBookmark) {
				isChecked = ((BreakpointBookmark)mark).IsEnabled;
				condition = ((BreakpointBookmark)mark).Condition;
				language = ((BreakpointBookmark)mark).ScriptLanguage;
			}

			imageSource = mark.Image.ImageSource;
			
			Location = GetLocation(mark);
			Mark = mark;
			tag = this;
		}
		
		public bool IsChecked {
			get {
				return isChecked;
			}
			set {
				if (value != isChecked)
				{
				    isChecked = value;
				    NotifyPropertyChanged("IsChecked");
				}
			}
		}
		
		public SDBookmark Mark {
			get; set;
		}
		
		public ImageSource Image {
			get { return imageSource; }
			set {
				imageSource = value;
				NotifyPropertyChanged("Image");
			}
		}
		
		public string Location {
			get; private set;
		}
		
		public string Language {
			get { return language; }
			set {
				language = value;
				NotifyPropertyChanged("Language");
			}
		}
		
		public string Condition {
			get { return condition; }
			set {
				condition = value;
				NotifyPropertyChanged("Condition");
			}
		}
		
		public object Tag {
			get { return tag;}
			set {
				tag = value;
				NotifyPropertyChanged("Tag");
			}
		}
		
		public override string ToString()
		{
			return Location;
		}
		
		private string GetLocation(SDBookmark bookmark)
		{
			return string.Format(StringParser.Parse("${res:MainWindow.Windows.BookmarkPad.LineText}"),
			                                        Path.GetFileName(bookmark.FileName), bookmark.LineNumber);
		}
		
		private void NotifyPropertyChanged(string property)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(property));
			}
		}
	}
}