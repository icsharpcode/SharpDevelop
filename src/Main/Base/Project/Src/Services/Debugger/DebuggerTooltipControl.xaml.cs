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
					btnUp.Visibility = btnDown.Visibility =
						this.lazyGrid.ItemsSourceTotalCount.Value <= 10 ? Visibility.Collapsed : Visibility.Visible;
				}
			}
			get
			{
				return this.itemsSource;
			}
		}
		
		/// <summary>
		/// Determines whether DebuggerTooltipControl should be displayed in WPF Popup.
		/// </summary>
		public bool ShowAsPopup {
			get {
				return true;
			}
		}
		
		/// <summary>
		/// Closes the debugger Popup containing this control. Also closes all child Popups.
		/// </summary>
		/// <returns></returns>
		public bool Close()
		{
			throw new NotImplementedException();
		}
		
		private void btnExpander_Click(object sender, RoutedEventArgs e)
		{
			var clickedButton = (ToggleButton)e.OriginalSource;
			Point buttonPos = clickedButton.PointToScreen(new Point(0, 0));

			if (clickedButton.IsChecked.GetValueOrDefault(false))
			{
				// open child Popup
			}
			else
			{
				// close child Popups
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