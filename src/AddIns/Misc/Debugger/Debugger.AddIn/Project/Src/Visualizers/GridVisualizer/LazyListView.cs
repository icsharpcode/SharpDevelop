// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Debugger.AddIn.Visualizers.GridVisualizer
{
	/// <summary>
	/// ListView that takes VirtualizingIEnumerable as source, 
	/// and requests additional items from the source as needed when scrolling.
	/// </summary>
	public class LazyItemsControl<T>
	{
		private ItemsControl itemsControl;
		
		public LazyItemsControl(ItemsControl wrappedItemsControl)
		{
			if (wrappedItemsControl == null)
				throw new ArgumentNullException("wrappedListView");
			
			this.itemsControl = wrappedItemsControl;
			this.itemsControl.AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(handleListViewScroll));
		}
		
		
		/// <summary> The collection that underlying ItemsControl sees. </summary>
		private VirtualizingIEnumerable<T> itemsSourceVirtualized;
		
		private IEnumerable<T> itemsSource;
		/// <summary>
		/// IEnumerable providing all the items. LazyItemsControl pulls next items from this source when scrolled to bottom.
		/// </summary>
		public IEnumerable<T> ItemsSource
		{
			get
			{
				return itemsSource;
			}
			set 
			{
				this.itemsSource = value;
				// virtualize IEnumerable and use it as ItemsControl source 
				// -> ItemsControl sees only a few items, more are added when scrolled to bottom
				this.itemsSourceVirtualized = new VirtualizingIEnumerable<T>(value);
				this.itemsSourceVirtualized.AddNextItems(25);
				this.itemsControl.ItemsSource = this.itemsSourceVirtualized;
			}
		}
		
		private void handleListViewScroll(object sender, ScrollChangedEventArgs e)
		{
			if (e.VerticalChange > 0)
			{
				// scrolled to bottom
				if (e.VerticalOffset == this.itemsSourceVirtualized.Count - e.ViewportHeight)
				{
					this.itemsSourceVirtualized.AddNextItems(1);
				}
			}
		}
	}
}
