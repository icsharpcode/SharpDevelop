// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using Services.Debugger.Tooltips;

namespace ICSharpCode.SharpDevelop.Debugging
{
	/// <summary>
	/// Default Control used as content of SharpDevelop debugger tooltips.
	/// </summary>
	public partial class DebuggerTooltipControl : UserControl, ITooltip
	{
		public const double MINIMUM_OPACITY = .3d;
		
		private readonly double ChildPopupOpenXOffet = 16;
		private readonly double ChildPopupOpenYOffet = 15;
		private readonly int InitialItemsCount = 12;
		private readonly int VisibleItemsCount = 11;		
		private readonly ITextEditor editor;
		
		#region Contructors

		public DebuggerTooltipControl(bool showPinControl = false)
		{
			InitializeComponent();
			
			this.showPinControl = showPinControl;
			
			ITextEditorProvider provider = WorkbenchSingleton.Workbench.ActiveContent as ITextEditorProvider;			
			if (provider != null) 
				editor = provider.TextEditor;
			
			// show pin close control
			if (this.showPinControl) {
				dataGrid.Columns[5].Visibility = Visibility.Collapsed;
				
				pinCloseControl = new PinCloseControl(this);
				pinCloseControl.Visibility = Visibility.Visible;
				PinControlCanvas.Visibility = Visibility.Visible;
				PinControlCanvas.Children.Add(pinCloseControl);
				this.Opacity = MINIMUM_OPACITY;
			}
			else {
				PinControlCanvas.Visibility = Visibility.Collapsed;
			}
			
			Loaded += new RoutedEventHandler(OnLoaded);
		}

		public DebuggerTooltipControl(ITreeNode node)
			: this(new ITreeNode[] { node })
		{
		}

		public DebuggerTooltipControl(IEnumerable<ITreeNode> nodes)
			: this()
		{
			this.itemsSource = nodes;
		}

		public DebuggerTooltipControl(DebuggerTooltipControl parentControl, bool showPinControl = false)
			: this(showPinControl)
		{
			this.parentControl = parentControl;
		}
		
		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			// verify if at the line of the root there's a pin bookmark
			var pin = BookmarkManager.Bookmarks.Find(
							b => b is PinBookmark &&
							b.Location.Line == LogicalPosition.Line &&
							b.FileName == editor.FileName) as PinBookmark;
			
			if (pin != null) {
				foreach (var node in this.itemsSource) {
					if (pin.ContainsNode(node))
						node.IsChecked = true;
				}
			}
			
			SetItemsSource(this.itemsSource);
		}
		
		#endregion
		
		private bool showPinControl;
		internal PinCloseControl pinCloseControl;
		private DebuggerPopup childPopup { get; set; }
		private DebuggerTooltipControl parentControl { get; set; }
		internal DebuggerPopup containingPopup { get; set; }
		
		#region Source
		
		private LazyItemsControl<ITreeNode> lazyGrid;
		private IEnumerable<ITreeNode> itemsSource;
		
		public IEnumerable<ITreeNode> ItemSource { get { return itemsSource; } }
		
		public void SetItemsSource(IEnumerable<ITreeNode> source)
		{
			this.itemsSource = source;
				
			this.lazyGrid = new LazyItemsControl<ITreeNode>(this.dataGrid, InitialItemsCount);
			lazyGrid.ItemsSource = new VirtualizingIEnumerable<ITreeNode>(source);
			this.dataGrid.AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(handleScroll));

			if (this.lazyGrid.ItemsSourceTotalCount != null) {
				// hide up/down buttons if too few items
				btnUp.Visibility = btnDown.Visibility =
					this.lazyGrid.ItemsSourceTotalCount.Value <= VisibleItemsCount ? Visibility.Collapsed : Visibility.Visible;
			}
		}
		
		void DebuggerService_DebugStarted(object sender, EventArgs e)
		{
//			// refresh values
//			ToolTipRequestEventArgs args = new ToolTipRequestEventArgs(editor);
//			args.LogicalPosition = this.LogicalPosition;
//			DebuggerService.HandleToolTipRequest(args);
//			
//			if (args.ContentToShow is ITooltip) {
//				var source = ((ITooltip)args.ContentToShow).ItemSource;
//				itemsSource = source;
//				lazyGrid.ItemsSource = new VirtualizingIEnumerable<ITreeNode>(source);
//			}
		}
			
		#endregion

		/// <inheritdoc/>
		public bool ShowAsPopup { get { return true; } }	

		/// <summary>
		/// Position within the document
		/// </summary>
		public Location LogicalPosition { get; set; }		

		#region Expander

		bool isChildExpanded
		{
			get
			{
				return this.childPopup != null && this.childPopup.IsOpen;
			}
		}

		private ToggleButton expandedButton;
		
		private void btnExpander_Click(object sender, RoutedEventArgs e)
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
					this.childPopup = new DebuggerPopup(this, showPinControl);
					this.childPopup.Placement = PlacementMode.Absolute;
					this.childPopup.contentControl.LogicalPosition = LogicalPosition;
				}
				if (this.containingPopup != null) {
					this.containingPopup.IsLeaf = false;
				}
				this.childPopup.IsLeaf = true;
				this.childPopup.HorizontalOffset = buttonPos.X + ChildPopupOpenXOffet;
				this.childPopup.VerticalOffset = buttonPos.Y + ChildPopupOpenYOffet;
				this.childPopup.SetItemsSource(clickedNode.ChildNodes);
				this.childPopup.Open();
			} else {
				CloseChildPopups();
			}
		}
		
		#endregion
		
		#region Close
		
		/// <inheritdoc/>
		public bool Close(bool mouseClick)
		{
			if (mouseClick || (!mouseClick && !isChildExpanded)) {
				CloseChildPopups();
				return true;
			} else {
				return false;
			}
		}

		/// <summary>
		/// Closes the child popup of this control, if it exists.
		/// </summary>
		public void CloseChildPopups()
		{
			if (this.expandedButton != null) {
				this.expandedButton.IsChecked = false;
				this.expandedButton = null;
				// nice simple example of indirect recursion
				this.childPopup.CloseSelfAndChildren();
			}
		}

		public void CloseOnLostFocus()
		{
			// when we close, parent becomes leaf
			if (this.containingPopup != null) {
				this.containingPopup.IsLeaf = true;
			}
			if (!this.IsMouseOver) {
				if (this.containingPopup != null) {
					this.containingPopup.IsOpen = false;
					this.containingPopup.IsLeaf = false;
				}
				if (this.parentControl != null) {
					this.parentControl.CloseOnLostFocus();
				}
				OnClosed();
			} else {
				// leaf closed because of click inside this control - stop the closing chain
				if (this.expandedButton != null && !this.expandedButton.IsMouseOver) {
					this.expandedButton.IsChecked = false;
					this.expandedButton = null;
				}
			}
		}
		
		public event RoutedEventHandler Closed;
		protected void OnClosed()
		{
			if (this.Closed != null) {
				this.Closed(this, new RoutedEventArgs());
			}
		}

		#endregion
		
		#region Scrolling

		private void handleScroll(object sender, ScrollChangedEventArgs e)
		{
			btnUp.IsEnabled = !this.lazyGrid.IsScrolledToStart;
			btnDown.IsEnabled = !this.lazyGrid.IsScrolledToEnd;
		}

		void BtnUp_Click(object sender, RoutedEventArgs e)
		{
			this.lazyGrid.ScrollViewer.ScrollUp(1);
		}

		void BtnDown_Click(object sender, RoutedEventArgs e)
		{
			this.lazyGrid.ScrollViewer.ScrollDown(1);
		}
		
		#endregion
		
		#region Edit value in tooltip
		
		void TextBox_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape) {
				dataGrid.Focus();
				return;
			}

			if (e.Key == Key.Enter) {
				dataGrid.Focus();
				// set new value
				var textBox = (TextBox)e.OriginalSource;				
				var newValue = textBox.Text;
				var node = ((FrameworkElement)sender).DataContext as ITreeNode;
				SaveNewValue(node, textBox.Text);
			}
		}
		
		void TextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			var textBox = (TextBox)e.OriginalSource;				
			var newValue = textBox.Text;
			var node = ((FrameworkElement)sender).DataContext as ITreeNode;
			SaveNewValue(node, textBox.Text);
		}
		
		void SaveNewValue(ITreeNode node, string newValue)
		{
			if(node != null && node.SetText(newValue)) {				
				// show adorner
				var adornerLayer = AdornerLayer.GetAdornerLayer(dataGrid);
				var adorners = adornerLayer.GetAdorners(dataGrid);
				if (adorners != null && adorners.Length != 0)
					adornerLayer.Remove(adorners[0]);
				SavedAdorner adorner = new SavedAdorner(dataGrid);						
				adornerLayer.Add(adorner);	
			}
		}
		
		#endregion
		
		#region Pining checked/unchecked
		
		void PinButton_Checked(object sender, RoutedEventArgs e)
		{
			var node = (ITreeNode)(((ToggleButton)(e.OriginalSource)).DataContext);
			
			if (!string.IsNullOrEmpty(editor.FileName)) {
				
				// verify if at the line of the root there's a pin bookmark
				var pin = BookmarkManager.Bookmarks.Find(
								b => b is PinBookmark &&
								b.LineNumber == LogicalPosition.Line &&
								b.FileName == editor.FileName) as PinBookmark;
				
				if (pin == null) {
					pin = new PinBookmark(editor.FileName, LogicalPosition);
					// show pinned DebuggerPopup
					if (pin.Popup == null) {
						pin.Popup = new DebuggerPopup(null, true);
						pin.Popup.contentControl.LogicalPosition = LogicalPosition;
						pin.Popup.contentControl.pinCloseControl.Mark = pin;
						Rect rect = new Rect(this.DesiredSize);
						var point = this.PointToScreen(rect.TopRight);
						pin.Popup.HorizontalOffset = 500;
						pin.Popup.VerticalOffset = point.Y - 150;
						pin.SavedPopupPosition = new Point { X = pin.Popup.HorizontalOffset, Y = pin.Popup.VerticalOffset };
						pin.Nodes.Add(node);
					}					
					
					// do actions
					pin.Popup.Open();
					BookmarkManager.AddMark(pin);
				}
				else
				{
					if (!pin.ContainsNode(node))
						pin.Nodes.Add(node);
				}
			}			
		}
		
		void PinButton_Unchecked(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrEmpty(editor.FileName)) {
				// remove from pinned DebuggerPopup
				var pin = BookmarkManager.Bookmarks.Find(
										b => b is PinBookmark &&
										b.LineNumber == LogicalPosition.Line &&
										b.FileName == editor.FileName) as PinBookmark;
				if (pin == null) return;
				
				ToggleButton button = (ToggleButton)e.OriginalSource;
				pin.RemoveNode((ITreeNode)button.DataContext);
				
				// remove if no more data pins are available
				if (pin.Nodes.Count == 0) {
					pin.Popup.CloseSelfAndChildren();
					
					BookmarkManager.RemoveMark(pin);
				}
			}
		}
		
		#endregion
		
		#region Comment
		
		public event EventHandler CommentChanged;
		
		public string Comment {
			get { return CommentTextBox.Text; }
			set { CommentTextBox.Text = value; }
		}
		
		public void ShowComment(bool show)
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
			var handler = CommentChanged;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		
		#endregion
		
		#region Saved Adorner
		
		class SavedAdorner : Adorner
		{	
			public SavedAdorner(UIElement adornedElement) : base(adornedElement) 
			{
				Loaded += delegate { Show(); };
			}
			
			protected override void OnRender(DrawingContext drawingContext)
			{
				Rect adornedElementRect = new Rect(this.AdornedElement.DesiredSize);
				
				// Some arbitrary drawing implements.
				var formatedText = new FormattedText(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Debugging.SavedString}"),
				                                     CultureInfo.CurrentCulture, 
				                                     FlowDirection.LeftToRight,
				                                     new Typeface(new FontFamily("Arial"), 
				                                                  FontStyles.Normal, 
				                                                  FontWeights.Black,
				                                                  FontStretches.Expanded),
				                                     8d,
				                                     Brushes.Black);
				
				
				drawingContext.DrawText(formatedText, 
				                        new Point(adornedElementRect.TopRight.X - formatedText.Width - 2,
				                                  adornedElementRect.TopRight.Y));
			}
			
			private void Show()
			{
				DoubleAnimation animation = new DoubleAnimation(); 
				animation.From = 1;
				animation.To = 0;				
				
				animation.Duration = new Duration(TimeSpan.FromSeconds(2));
				animation.SetValue(Storyboard.TargetProperty, this);
				animation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath(Rectangle.OpacityProperty));
				                   
				Storyboard board = new Storyboard();
				board.Children.Add(animation);			
				
				board.Begin(this);
			}
		}
		
		#endregion
	}
}