// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.SharpDevelop.Editor;

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

		public DebuggerTooltipControl()
		{
			InitializeComponent();
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

		public DebuggerTooltipControl(DebuggerTooltipControl parentControl)
			: this()
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
					this.childPopup = new DebuggerPopup(this);
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
	}
}
