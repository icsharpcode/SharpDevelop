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
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Editor.Bookmarks;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.SharpDevelop.Editor.Bookmarks
{
	public sealed class BookmarkPad : BookmarkPadBase
	{
		public BookmarkPad()
		{
			ToolBar toolbar = ToolBarService.CreateToolBar((UIElement)this.Control, this, "/SharpDevelop/Pads/BookmarkPad/Toolbar");
			this.control.Children.Add(toolbar);
		}
		
		protected override bool ShowBookmarkInThisPad(SDBookmark bookmark)
		{
			return bookmark.ShowInPad(this);
		}
	}
	
	public abstract class BookmarkPadBase : AbstractPadContent
	{
		protected BookmarkPadContent control;
		
		public override object Control {
			get { return this.control; }
		}
		
		public ListView ListView {
			get { return this.control.listView; }
		}
		
		public ItemCollection Items {
			get { return this.control.listView.Items; }
		}
		
		public SDBookmark SelectedItem {
			get { return (SDBookmark)this.control.listView.SelectedItem; }
		}
		
		public IEnumerable<SDBookmark> SelectedItems {
			get { return this.control.listView.SelectedItems.OfType<SDBookmark>(); }
		}
		
		protected BookmarkPadBase()
		{
			this.control = new BookmarkPadContent();
			this.control.InitializeComponent();
			
			SD.BookmarkManager.BookmarkAdded   += BookmarkManagerAdded;
			SD.BookmarkManager.BookmarkRemoved += BookmarkManagerRemoved;
			
			foreach (SDBookmark bookmark in SD.BookmarkManager.Bookmarks) {
				if (ShowBookmarkInThisPad(bookmark)) {
					this.Items.Add(bookmark);
				}
			}
			
			this.control.listView.MouseDoubleClick += delegate {
				SDBookmark bm = this.control.listView.SelectedItem as SDBookmark;
				if (bm != null)
					OnItemActivated(bm);
			};
			
			this.control.listView.KeyDown += delegate(object sender, System.Windows.Input.KeyEventArgs e) {
				var selectedItems = this.SelectedItems.ToList();
				if (!selectedItems.Any())
					return;
				switch (e.Key) {
					case System.Windows.Input.Key.Delete:
						foreach (var selectedItem in selectedItems) {
							SD.BookmarkManager.RemoveMark(selectedItem);
						}
						break;
				}
			};
		}
		
		public override void Dispose()
		{
			SD.BookmarkManager.BookmarkAdded   -= BookmarkManagerAdded;
			SD.BookmarkManager.BookmarkRemoved -= BookmarkManagerRemoved;
		}
		
		protected abstract bool ShowBookmarkInThisPad(SDBookmark mark);
		
		protected virtual void OnItemActivated(SDBookmark bm)
		{
			FileService.JumpToFilePosition(bm.FileName, bm.LineNumber, 1);
		}
		
		void BookmarkManagerAdded(object sender, BookmarkEventArgs e)
		{
			if (ShowBookmarkInThisPad(e.Bookmark)) {
				this.Items.Add(e.Bookmark);
			}
		}
		
		void BookmarkManagerRemoved(object sender, BookmarkEventArgs e)
		{
			this.Items.Remove(e.Bookmark);
		}
	}
}
