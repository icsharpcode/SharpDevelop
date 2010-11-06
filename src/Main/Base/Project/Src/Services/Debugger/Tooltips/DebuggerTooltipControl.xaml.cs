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
		private readonly double ChildPopupOpenXOffet = 16;
		private readonly double ChildPopupOpenYOffet = 15;
		private readonly int InitialItemsCount = 12;
		private readonly int VisibleItemsCount = 11;
		
		bool showPinControl;
		PinCloseControl pinCloseControl;

		public DebuggerTooltipControl(bool showPinControl = false)
		{
			InitializeComponent();
			
			this.showPinControl = showPinControl;
			
			// show pin close control
			if (this.showPinControl) {
				dataGrid.Columns[5].Visibility = Visibility.Collapsed;
				
				pinCloseControl = new PinCloseControl(this);
				pinCloseControl.Visibility = Visibility.Visible;
				PinControlCanvas.Visibility = Visibility.Visible;
				PinControlCanvas.Children.Add(pinCloseControl);
			}
			else {
				PinControlCanvas.Visibility = Visibility.Collapsed;
			}
		}

		public DebuggerTooltipControl(ITreeNode node)
			: this(new ITreeNode[] { node })
		{
		}

		public DebuggerTooltipControl(IEnumerable<ITreeNode> nodes)
			: this()
		{
			this.ItemsSource = nodes;
		}

		public DebuggerTooltipControl(DebuggerTooltipControl parentControl, bool showPinControl = false)
			: this(showPinControl)
		{
			this.parentControl = parentControl;
		}

		public event RoutedEventHandler Closed;
		protected void OnClosed()
		{
			if (this.Closed != null) {
				this.Closed(this, new RoutedEventArgs());
			}
		}

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
				this.dataGrid.AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(handleScroll));

				if (this.lazyGrid.ItemsSourceTotalCount != null) {
					// hide up/down buttons if too few items
					btnUp.Visibility = btnDown.Visibility =
						this.lazyGrid.ItemsSourceTotalCount.Value <= VisibleItemsCount ? Visibility.Collapsed : Visibility.Visible;
				}
			}
		}

		/// <inheritdoc/>
		public bool ShowAsPopup
		{
			get
			{
				return true;
			}
		}
		
		public string Comment {
			get { return CommentTextBox.Text; }
			set { CommentTextBox.Text = value; }
		}

		/// <summary>
		/// Position within the document
		/// </summary>
		public Location LogicalPosition { get; set; }
		
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

		DebuggerPopup childPopup { get; set; }
		DebuggerTooltipControl parentControl { get; set; }
		internal DebuggerPopup containingPopup { get; set; }

		bool isChildExpanded
		{
			get
			{
				return this.childPopup != null && this.childPopup.IsOpen;
			}
		}

		private ToggleButton expandedButton;

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
				}
				if (this.containingPopup != null) {
					this.containingPopup.IsLeaf = false;
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
		
		void TextBox_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape) {
				dataGrid.Focus();
				return;
			}

			if (e.Key == Key.Enter) {
				dataGrid.Focus();
				// set new value
				var textBox = (TextBox)sender;				
				var newValue = textBox.Text;
				var node = ((FrameworkElement)sender).DataContext as ITreeNode;
				SaveNewValue(node, textBox.Text);
			}
		}
		
		void TextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			// set new value
			var textBox = (TextBox)sender;
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
		
		void PinButton_Checked(object sender, RoutedEventArgs e)
		{
			ITextEditorProvider provider = WorkbenchSingleton.Workbench.ActiveContent as ITextEditorProvider;
			ToggleButton button = (ToggleButton)sender;
			
			if (provider != null) {
				ITextEditor editor = provider.TextEditor;
				if (!string.IsNullOrEmpty(editor.FileName)) {
					
					var pin = new PinBookmark(editor.FileName, LogicalPosition);
					if (!BookmarkManager.Bookmarks.Contains(pin)) {
						// show pinned DebuggerPopup
						if (pin.Popup == null) {
							pin.Popup = new DebuggerPopup(this, true);
							pin.Popup.Placement = PlacementMode.Absolute;
							Rect rect = new Rect(this.DesiredSize);
							var point = this.PointToScreen(rect.TopRight);
							pin.Popup.HorizontalOffset = point.X + 150;
							pin.Popup.VerticalOffset = point.Y - 20;
							pin.Popup.Open();
						}
						pin.Nodes.Add(button.DataContext as ITreeNode);
						
						BookmarkManager.ToggleBookmark(
							editor, 
							LogicalPosition.Line, 
							b => b.CanToggle && b is PinBookmark,
							location => pin);
					}
					else
					{
						pin.Nodes.Add(button.DataContext as ITreeNode);
					}
				}
			}
		}
		
		void PinButton_Unchecked(object sender, RoutedEventArgs e)
		{
			if (!showPinControl)
				return;
			
			ITextEditorProvider provider = WorkbenchSingleton.Workbench.ActiveContent as ITextEditorProvider;
			if (provider != null) {
				ITextEditor editor = provider.TextEditor;
				if (!string.IsNullOrEmpty(editor.FileName)) {
					// remove from pinned DebuggerPopup
					var pin = new PinBookmark(editor.FileName, LogicalPosition);
					if (!BookmarkManager.Bookmarks.Contains(pin)) 
						return;
					ToggleButton button = (ToggleButton)sender;
					pin.Nodes.Remove(button.DataContext as ITreeNode);
				}
			}
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
	}
}