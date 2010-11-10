// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

using ICSharpCode.AvalonEdit;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace Services.Debugger.Tooltips
{
	public partial class PinDebuggerControl : UserControl
	{
		private const int InitialItemsCount = 12;		
		private const double MINIMUM_OPACITY = .3d;
				
		public PinDebuggerControl()
		{
			InitializeComponent();
			
			Loaded += OnLoaded;
			this.PinCloseControl.Closed += PinCloseControl_Closed;
			this.PinCloseControl.ShowingComment += PinCloseControl_ShowingComment;
			this.PinCloseControl.PinningChanged += PinCloseControl_PinningChanged;
			this.Focusable = true;
			BookmarkManager.Removed += BookmarkManager_Removed;
		}
		
		#region Properties
		
		public PinBookmark Mark { get; set; }
		
		private LazyItemsControl<ITreeNode> lazyGrid;

		private IEnumerable<ITreeNode> itemsSource;
		public IEnumerable<ITreeNode> ItemsSource
		{
			get { return this.itemsSource; }
			set
			{
				this.itemsSource = value;
				this.lazyGrid = new LazyItemsControl<ITreeNode>(this.dataGrid, InitialItemsCount);
				lazyGrid.ItemsSource = new VirtualizingIEnumerable<ITreeNode>(value);
			}
		}
		
		#endregion
		
		public void Open()
		{
			Pin();			
		}
		
		public void Close()
		{
			Unpin();
		}

		void PinCloseControl_Closed(object sender, EventArgs e)
		{
			BookmarkManager.RemoveMark(Mark);
			
			Unpin();
		}
		
		void PinCloseControl_PinningChanged(object sender, EventArgs e)
		{
			if (this.PinCloseControl.IsChecked) {
				BookmarkManager.RemoveMark(Mark);
			}
			else {
				if(BookmarkManager.Bookmarks.Contains(Mark))
					BookmarkManager.RemoveMark(Mark);
			
				BookmarkManager.AddMark(Mark);
			}
		}
		
		void PinCloseControl_ShowingComment(object sender, ShowingCommentEventArgs e)
		{
			ShowComment(e.ShowComment);
		}

		void BookmarkManager_Removed(object sender, BookmarkEventArgs e)
		{
			// if the bookmark was removed from pressing the button, return
			if (this.PinCloseControl.IsChecked)
				return;
			
			if (e.Bookmark is PinBookmark) {
				var pin = (PinBookmark)e.Bookmark;
				if (pin.Location == Mark.Location && pin.FileName == Mark.FileName) {
					Close();
				}							
			}
		}

		void OnLoaded(object sender, RoutedEventArgs e)
		{
			this.CommentTextBox.Text = Mark.Comment;
		}			
		
		void Pin()
		{
			var provider = WorkbenchSingleton.Workbench.ActiveContent as ITextEditorProvider;
				if(provider != null) {
					var editor = (TextEditor)provider.TextEditor.GetService(typeof(TextEditor));
					editor.TextArea.PinningLayer.Pin(this);
				}
		}
		
		void Unpin()
		{
			var provider = WorkbenchSingleton.Workbench.ActiveContent as ITextEditorProvider;
				if(provider != null) {
					var editor = (TextEditor)provider.TextEditor.GetService(typeof(TextEditor));
					editor.TextArea.PinningLayer.Unpin(this);
				}
		}
		
		#region Comment
		
		void ShowComment(bool show)
		{
			if(show && BorderComment.Height != 0)
				return;
			if(!show && BorderComment.Height != 40)
				return;
			
			DoubleAnimation animation = new DoubleAnimation(); 
			animation.From = show ? 0 : 40;
			animation.To = show ? 40 : 0;
			
			animation.Duration = new Duration(TimeSpan.FromMilliseconds(300));
			animation.SetValue(Storyboard.TargetProperty, BorderComment);
			animation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath(Border.HeightProperty));
			                   
			Storyboard board = new Storyboard();
			board.Children.Add(animation);			
			board.Begin(this);
		}
		
		void CommentTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			Mark.Comment = this.CommentTextBox.Text;
		}
		
		#endregion
				
		protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
		{
			Opacity = 1d;
			Cursor = Cursors.Arrow;
			base.OnMouseEnter(e);
		}
		
		protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
		{
			Opacity = MINIMUM_OPACITY;
			Cursor = Cursors.IBeam;
			base.OnMouseLeave(e);
		}
	}
}