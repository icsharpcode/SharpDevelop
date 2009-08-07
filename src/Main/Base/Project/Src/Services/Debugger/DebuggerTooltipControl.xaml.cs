// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
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
		
		private LazyItemsControl<ITreeNode> lazyGrid;
		
		private IEnumerable<ITreeNode> itemsSource;
		public IEnumerable<ITreeNode> ItemsSource
		{
			set
			{
				this.itemsSource = value;
				this.lazyGrid = new LazyItemsControl<ITreeNode>(this.dataGrid, 12);
				lazyGrid.ItemsSource = new VirtualizingIEnumerable<ITreeNode>(value);
				this.dataGrid.AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(handleScroll));
				
				if (this.lazyGrid.ItemsSourceTotalCount != null)
				{
					// hide up, down buttons if too few items
					btnUp.Visibility = btnDown.Visibility =
						this.lazyGrid.ItemsSourceTotalCount.Value <= 10 ? Visibility.Collapsed : Visibility.Visible;
				}
			}
			get
			{
				return this.itemsSource;
			}
		}
		
		/// <inheritdoc/>
		public bool ShowAsPopup {
			get {
				return true;
			}
		}
		
		/// <summary>
		/// When child popup is expanded, returns false. Otherwise true.
		/// </summary>
		public bool AllowsClose {
			get {
				return !isChildExpanded;
			}
		}
		
		/// <inheritdoc/>
		public bool Close(bool mouseClick)
		{
			if (mouseClick || (!mouseClick && !isChildExpanded))
			{
				CloseChildPopup();
				return true;
			} 
			else
			{
				return false;
			}
		}
		
		DebuggerPopup childPopup;
		bool isChildExpanded
		{
			get {
				return this.childPopup != null && this.childPopup.IsOpen;
			}
		}
		
		/// <summary>
		/// Closes the child popup of this control, if it exists.
		/// </summary>
		public void CloseChildPopup()
		{
			if (this.childPopup != null)
			{
				this.childPopup.Close();
			}
		}
		
		internal Popup containingPopup;
		
		private void btnExpander_Click(object sender, RoutedEventArgs e)
		{
			var clickedButton = (ToggleButton)e.OriginalSource;
			var clickedNode = (ITreeNode)clickedButton.DataContext;
			Point buttonPos = clickedButton.PointToScreen(new Point(0, 0));

			if (clickedButton.IsChecked.GetValueOrDefault(false))
			{
				CloseChildPopup();
				
				// open child Popup
				if (this.childPopup == null)
				{
					this.childPopup = new DebuggerPopup();
					this.childPopup.Placement = PlacementMode.Absolute;
					this.childPopup.StaysOpen = true;
				}
				if (this.containingPopup != null)
				{
					this.containingPopup.StaysOpen = true;
				}
				// last popup is always StaysOpen = false, therefore focused
				this.childPopup.StaysOpen = false;
				this.childPopup.HorizontalOffset = buttonPos.X + 15;
				this.childPopup.VerticalOffset = buttonPos.Y + 15;
				this.childPopup.ItemsSource = clickedNode.ChildNodes;
				this.childPopup.UpdateLayout();
				this.childPopup.Open();
			}
			else
			{
				CloseChildPopup();
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