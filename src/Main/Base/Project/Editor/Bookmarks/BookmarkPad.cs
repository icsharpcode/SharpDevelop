// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			return bookmark.IsVisibleInBookmarkPad && !(bookmark is BreakpointBookmark);
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
				SDBookmark bm = this.control.listView.SelectedItem as SDBookmark;
				if (bm == null) return;
				switch (e.Key) {
					case System.Windows.Input.Key.Delete:
						SD.BookmarkManager.RemoveMark(bm);
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