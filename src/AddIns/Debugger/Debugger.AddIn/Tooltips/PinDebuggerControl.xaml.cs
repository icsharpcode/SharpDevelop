// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn.Tooltips
{
	public partial class PinDebuggerControl : UserControl, IPinDebuggerControl
	{
		private const double ChildPopupOpenXOffet = 16;
		private const double ChildPopupOpenYOffet = 15;
		private const int InitialItemsCount = 12;
		private const double MINIMUM_OPACITY = .3d;
		
		private WindowsDebugger currentDebugger;
		private DebuggerPopup childPopup;
		private LazyItemsControl<ITreeNode> lazyExpandersGrid;
		private LazyItemsControl<ITreeNode> lazyGrid;
		private LazyItemsControl<ITreeNode> lazyImagesGrid;
		private IEnumerable<ITreeNode> itemsSource;
		
		public PinDebuggerControl()
		{
			InitializeComponent();
			
			if (!DebuggerService.IsDebuggerStarted)
				Opacity = MINIMUM_OPACITY;
			this.PinCloseControl.Opacity = 0;
			
			Loaded += OnLoaded;
			this.PinCloseControl.Closed += PinCloseControl_Closed;
			this.PinCloseControl.ShowingComment += PinCloseControl_ShowingComment;
			this.PinCloseControl.PinningChanged += PinCloseControl_PinningChanged;
			
			BookmarkManager.Removed += OnBookmarkRemoved;
			
			currentDebugger = (WindowsDebugger)DebuggerService.CurrentDebugger;
			
			currentDebugger.DebugStopped += OnDebugStopped;
			currentDebugger.ProcessSelected += OnProcessSelected;
			
			if (currentDebugger.DebuggedProcess != null)
				currentDebugger.DebuggedProcess.Paused += OnDebuggedProcessPaused;
		}
		
		#region Properties
		
		public PinBookmark Mark { get; set; }
		
		public IEnumerable<ITreeNode> ItemsSource
		{
			get { return this.itemsSource; }
			set {
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
		
		/// <summary>
		/// Relative position of the pin with respect to the screen.
		/// </summary>
		public Point Location { get; set; }
		
		#endregion
		
		#region Main operations
		
		public void Open()
		{
			Pin();
		}
		
		public void Close()
		{
			CloseChildPopups();
			Unpin();
			
			BookmarkManager.Removed -= OnBookmarkRemoved;
			if (currentDebugger != null) {
				currentDebugger.DebugStopped -= OnDebugStopped;
				currentDebugger.ProcessSelected -= OnProcessSelected;
				currentDebugger = null;
			}
		}
		
		void Pin()
		{
			var provider = WorkbenchSingleton.Workbench.ActiveContent as ITextEditorProvider;
			if(provider != null) {
				var pinLayer = PinningBinding.GetPinlayer(provider.TextEditor);
				if (pinLayer != null)
					pinLayer.Pin(this);
			}
		}
		
		void Unpin()
		{
			var provider = WorkbenchSingleton.Workbench.ActiveContent as ITextEditorProvider;
			if(provider != null) {
				var pinLayer = PinningBinding.GetPinlayer(provider.TextEditor);
				if (pinLayer != null)
					pinLayer.Unpin(this);
			}
		}
		
		#endregion
		
		#region Debugger events
		
		void OnDebugStopped(object sender, EventArgs e)
		{
			if (currentDebugger.DebuggedProcess != null)
				currentDebugger.DebuggedProcess.Paused -= OnDebuggedProcessPaused;
		}

		void OnProcessSelected(object sender, ProcessEventArgs e)
		{
			Opacity = 1d;
			if (currentDebugger.DebuggedProcess != null)
				currentDebugger.DebuggedProcess.Paused += OnDebuggedProcessPaused;
		}
		
		void OnDebuggedProcessPaused(object sender, ProcessEventArgs e)
		{
			//var nodes = new StackFrameNode(e.Process.SelectedStackFrame).ChildNodes;
			
//			if (!lazyGrid.ItemsSource.ContainsNode(node))
//				return;
			// TODO : find the current expression so we don't update every pin
//			var observable = new List<ITreeNode>();
//
//			foreach (var node in lazyGrid.ItemsSource) {
//				var resultNode = currentDebugger.GetNode(node.FullName);
//				// HACK for updating the pins in tooltip
//				observable.Add(resultNode);
//			}
//
//			// update UI
//			var newSource = new VirtualizingIEnumerable<ITreeNode>(observable);
//			lazyGrid.ItemsSource = newSource;
//			lazyExpandersGrid.ItemsSource = newSource;
		}
		
		#endregion
		
		#region Expand button
		
		private ToggleButton expandedButton;

		/// <summary>
		/// Closes the child popup of this control, if it exists.
		/// </summary>
		void CloseChildPopups()
		{
			if (this.expandedButton != null) {
				this.expandedButton = null;
				// nice simple example of indirect recursion
				this.childPopup.CloseSelfAndChildren();
			}
		}
		
		void BtnExpander_Checked(object sender, RoutedEventArgs e)
		{
			if (!DebuggerService.IsDebuggerStarted)
				return;
			
			var clickedButton = (ToggleButton)e.OriginalSource;
			var clickedNode = (ITreeNode)clickedButton.DataContext;
			// use device independent units, because child popup Left/Top are in independent units
			Point buttonPos = clickedButton.PointToScreen(new Point(0, 0)).TransformFromDevice(clickedButton);

			if (clickedButton.IsChecked.GetValueOrDefault(false)) {
				
				this.expandedButton = clickedButton;
				
				// open child Popup
				if (this.childPopup == null) {
					this.childPopup = new DebuggerPopup(null, ICSharpCode.NRefactory.Location.Empty, false);
					this.childPopup.PlacementTarget = this;
					this.childPopup.Closed += new EventHandler(PinDebuggerControl_Closed);
					this.childPopup.Placement = PlacementMode.Absolute;
				}
				
				this.childPopup.IsLeaf = true;
				this.childPopup.HorizontalOffset = buttonPos.X + ChildPopupOpenXOffet;
				this.childPopup.VerticalOffset = buttonPos.Y + ChildPopupOpenYOffet;
				if (clickedNode.ChildNodes != null) {
					this.childPopup.ItemsSource = clickedNode.ChildNodes;
					this.childPopup.Open();
				}
			} else {
				
			}
		}

		void PinDebuggerControl_Closed(object sender, EventArgs e)
		{
			if (expandedButton != null && expandedButton.IsChecked.GetValueOrDefault(false))
				expandedButton.IsChecked = false;
		}
		
		void BtnExpander_Unchecked(object sender, RoutedEventArgs e)
		{
			CloseChildPopups();
		}
		
		#endregion
		
		#region PinCloseControl
		
		void PinCloseControl_Closed(object sender, EventArgs e)
		{
			BookmarkManager.RemoveMark(Mark);
			Close();
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
		
		void AnimateCloseControl(bool show)
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

		#endregion
		
		void OnBookmarkRemoved(object sender, BookmarkEventArgs e)
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

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			this.CommentTextBox.Text = Mark.Comment;
		}
		
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (!DebuggerService.IsDebuggerStarted)
				return;
			
			// refresh content
			ITreeNode node = ((FrameworkElement)e.OriginalSource).DataContext as ITreeNode;
			
			var resultNode = currentDebugger.GetNode(node.FullName, node.ImageName);
			if (resultNode == null)
				return;
			// HACK for updating the pins in tooltip
			var observable = new ObservableCollection<ITreeNode>();
			var source = lazyGrid.ItemsSource;
			source.ForEach(item => {
			               	if (item.CompareTo(node) == 0)
			               		observable.Add(resultNode);
			               	else
			               		observable.Add(item);
			               });
			
			Mark.Nodes = observable;
			// update UI
			var newSource = new VirtualizingIEnumerable<ITreeNode>(observable);
			lazyGrid.ItemsSource = newSource;
			lazyExpandersGrid.ItemsSource = newSource;
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
		
		#region Overrides
		
		protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
		{
			AnimateCloseControl(true);
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
			if (DebuggerService.IsDebuggerStarted)
				Opacity = 1;
			else
				Opacity = MINIMUM_OPACITY;
			
			AnimateCloseControl(false);
			
			Cursor = Cursors.IBeam;
			base.OnMouseLeave(e);
		}
		
		#endregion
	}
}