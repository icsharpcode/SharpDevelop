// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace Services.Debugger.Tooltips
{
	public partial class PinDebuggerControl : UserControl
	{
		private const double ChildPopupOpenXOffet = 16;
		private const double ChildPopupOpenYOffet = 15;
		private const int InitialItemsCount = 12;
		private const double MINIMUM_OPACITY = .3d;
		
		private DebuggerPopup childPopup;
		
		public PinDebuggerControl()
		{
			InitializeComponent();
			
			if (!DebuggerService.IsDebuggerRunning)
				Opacity = MINIMUM_OPACITY;
			PinCloseControl.Opacity = 0;
			
			Loaded += OnLoaded;
			this.PinCloseControl.Closed += PinCloseControl_Closed;
			this.PinCloseControl.ShowingComment += PinCloseControl_ShowingComment;
			this.PinCloseControl.PinningChanged += PinCloseControl_PinningChanged;
			BookmarkManager.Removed += BookmarkManager_Removed;
			DebuggerService.DebugStarted += delegate { Opacity = 1d; };
		}
		
		#region Properties
		
		public PinBookmark Mark { get; set; }
		
		private LazyItemsControl<ITreeNode> lazyExpandersGrid;
		private LazyItemsControl<ITreeNode> lazyGrid;
		private LazyItemsControl<ITreeNode> lazyImagesGrid;

		private IEnumerable<ITreeNode> itemsSource;
		public IEnumerable<ITreeNode> ItemsSource
		{
			set
			{
				itemsSource = value;
				var items = new VirtualizingIEnumerable<ITreeNode>(value);
				lazyExpandersGrid = new LazyItemsControl<ITreeNode>(this.ExpandersGrid, InitialItemsCount);
				lazyExpandersGrid.ItemsSource = items;
				
				lazyGrid = new LazyItemsControl<ITreeNode>(this.dataGrid, InitialItemsCount);
				lazyGrid.ItemsSource = items;
				
				lazyImagesGrid = new LazyItemsControl<ITreeNode>(this.ImagesGrid, InitialItemsCount);
				lazyImagesGrid.ItemsSource = items;
			}
		}
		
		public Point Location { get; set; }
		
		#endregion
		
		public void Open()
		{
			Pin();
		}
		
		public void Close()
		{
			Unpin();
		}
		
		private ToggleButton expandedButton;

		/// <summary>
		/// Closes the child popup of this control, if it exists.
		/// </summary>
		void CloseChildPopups()
		{
			if (this.expandedButton != null) {
				this.expandedButton.IsChecked = false;
				this.expandedButton = null;
				// nice simple example of indirect recursion
				this.childPopup.CloseSelfAndChildren();
			}
		}

		void btnExpander_Click(object sender, RoutedEventArgs e)
		{
			var clickedButton = (ToggleButton)e.OriginalSource;
			var clickedNode = (ITreeNode)clickedButton.DataContext;
			// use device independent units, because child popup Left/Top are in independent units
			Point buttonPos = clickedButton.PointToScreen(new Point(0, 0)).TransformFromDevice(clickedButton);

			if (clickedButton.IsChecked.GetValueOrDefault(false)) {
				CloseChildPopups();
				this.expandedButton = clickedButton;

				// open child Popup
				if (this.childPopup == null) {
					this.childPopup = new DebuggerPopup(null, false);
					this.childPopup.PlacementTarget = this;
					this.childPopup.Placement = PlacementMode.Absolute;
				}
				
				this.childPopup.IsLeaf = true;
				this.childPopup.HorizontalOffset = buttonPos.X + ChildPopupOpenXOffet;
				this.childPopup.VerticalOffset = buttonPos.Y + ChildPopupOpenYOffet;
				this.childPopup.ItemsSource = clickedNode.ChildNodes;
				this.childPopup.Open();
			} else {
				CloseChildPopups();
			}
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
				PinningBinding.GetPinlayer(provider.TextEditor).Pin(this);
			}
		}
		
		void Unpin()
		{
			var provider = WorkbenchSingleton.Workbench.ActiveContent as ITextEditorProvider;
			if(provider != null) {
				PinningBinding.GetPinlayer(provider.TextEditor).Unpin(this);
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
		
		void AnimateCloseControls(bool show)
		{
			DoubleAnimation animation = new DoubleAnimation();
			animation.From = show ? 0 : 1;
			animation.To = show ? 1 : 0;
			animation.BeginTime = new TimeSpan(0, 0, show ? 0 : 1);
			animation.Duration = new Duration(TimeSpan.FromMilliseconds(500));
			animation.SetValue(Storyboard.TargetProperty, this.PinCloseControl);
			animation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath(Rectangle.OpacityProperty));
			
			Storyboard board = new Storyboard();
			board.Children.Add(animation);
			
			board.Begin(this);
		}
		
		void RefreshContentImage_MouseDown(object sender, MouseButtonEventArgs e)
		{
			// refresh content			
			ITreeNode node = ((Image)sender).DataContext as ITreeNode;
			
			if (!DebuggerService.IsDebuggerRunning)
				return;
			
			
		}
		
		protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
		{
			AnimateCloseControls(true);			
			Opacity = 1d;
			Cursor = Cursors.Arrow;
			base.OnMouseEnter(e);
		}
		
		protected override void OnMouseMove(MouseEventArgs e)
		{
			Opacity = 1d;
			Cursor = Cursors.Arrow;
			base.OnMouseMove(e);
		}
		
		protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
		{
			if (DebuggerService.IsDebuggerRunning) 
				Opacity = 1;
			else
				Opacity = MINIMUM_OPACITY;
			
			AnimateCloseControls(false);
				
			Cursor = Cursors.IBeam;
			base.OnMouseLeave(e);
		}
	}
}