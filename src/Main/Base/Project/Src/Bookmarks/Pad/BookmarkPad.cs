// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Bookmarks.Pad.Controls;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Bookmarks
{
	public sealed class BookmarkPad : BookmarkPadBase
	{
		static BookmarkPad instance;
		
		public static BookmarkPad Instance {
			get {
				if (instance == null) {
					WorkbenchSingleton.Workbench.GetPad(typeof(BookmarkPad)).CreatePad();
				}
				return instance;
			}
		}
		
		protected override ToolBar CreateToolBar()
		{
			ToolBar toolbar = ToolBarService.CreateToolBar(myPanel, this, "/SharpDevelop/Pads/BookmarkPad/Toolbar");
			toolbar.SetValue(Grid.RowProperty, 0);
			return toolbar;
		}
		
		protected override void CreateColumns() { }
		
		public BookmarkPad()
		{
			instance = this;
			myPanel.Children.Add(CreateToolBar());
			listView.HideColumns(2, 0);
		}
	}
	
	public abstract class BookmarkPadBase : AbstractPadContent
	{
		protected Grid  myPanel  = new Grid();
		protected ListViewPad listView = new ListViewPad();
		
		public override object Control {
			get {
				return myPanel;
			}
		}
		
		public ListViewPadItemModel CurrentItem {
			get {
				return listView.CurrentItem;
			}
		}
		
		protected abstract ToolBar CreateToolBar();
		
		protected abstract void CreateColumns();
		
		protected BookmarkPadBase()
		{
			myPanel.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			myPanel.RowDefinitions.Add(new RowDefinition());
			listView.SetValue(Grid.RowProperty, 1);
			myPanel.Children.Add(listView);
			
			BookmarkManager.Added   += BookmarkManagerAdded;
			BookmarkManager.Removed += BookmarkManagerRemoved;
			
			foreach (SDBookmark mark in BookmarkManager.Bookmarks) {
				AddMark(mark);
			}
			
			listView.ItemActivated += new EventHandler(OnItemActivated);
		}
		
		public IEnumerable<ListViewPadItemModel> AllItems {
			get {
				foreach (var item in listView.ItemCollection) {
					yield return item;
				}
			}
		}
		
		public ListViewPadItemModel NextItem {
			get {
				return this.listView.NextItem;
			}
		}
		
		public ListViewPadItemModel PreviousItem {
			get {
				return this.listView.PreviousItem;
			}
		}
		
		public void EnableDisableAll()
		{
			bool isOneChecked = false;
			foreach (var node in AllItems) {
				if (node.IsChecked) {
					isOneChecked = true;
					break;
				}
			}
			foreach (var node in AllItems)
				node.IsChecked = !isOneChecked;
		}
		
		public void SelectItem(ListViewPadItemModel model)
		{
			listView.CurrentItem = model;
		}
		
		public override void Dispose()
		{
			BookmarkManager.Added   -= BookmarkManagerAdded;
			BookmarkManager.Removed -= BookmarkManagerRemoved;
		}
		
		void AddMark(SDBookmark mark)
		{
			if (!ShowBookmarkInThisPad(mark))
				return;
			
			var model = new ListViewPadItemModel(mark);
			model.PropertyChanged += OnModelPropertyChanged;
			listView.Add(model);
		}
		
		protected virtual bool ShowBookmarkInThisPad(SDBookmark mark)
		{
			return mark.IsVisibleInBookmarkPad && !(mark is BreakpointBookmark);
		}
		
		protected virtual void OnItemActivated(object sender, EventArgs e)
		{
			var node = CurrentItem;
			if (node != null) {
				SDBookmark mark = node.Mark as SDBookmark;
				if (mark != null) {
					FileService.JumpToFilePosition(mark.FileName, mark.LineNumber, 1);
				}
			}
		}
		
		void BookmarkManagerAdded(object sender, BookmarkEventArgs e)
		{
			AddMark(e.Bookmark);
		}
		
		void BookmarkManagerRemoved(object sender, BookmarkEventArgs e)
		{
			if (ShowBookmarkInThisPad(e.Bookmark)) {
				var model = listView.Remove(e.Bookmark);
				if (model != null)
					model.PropertyChanged -= OnModelPropertyChanged;
			}
		}
		
		void OnModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			var model = sender as ListViewPadItemModel;
			if (e.PropertyName == "IsChecked") {
				if (model.Mark is BreakpointBookmark) {
					var bpm = model.Mark as BreakpointBookmark;
					bpm.IsEnabled = model.IsChecked;
					if (model.IsChecked) {
						model.Image = string.IsNullOrEmpty(model.Condition) ? BreakpointBookmark.BreakpointImage.ImageSource : BreakpointBookmark.BreakpointConditionalImage.ImageSource;
					} else {
						model.Image = BreakpointBookmark.DisabledBreakpointImage.ImageSource;
					}
				}
			}
		}
	}
}