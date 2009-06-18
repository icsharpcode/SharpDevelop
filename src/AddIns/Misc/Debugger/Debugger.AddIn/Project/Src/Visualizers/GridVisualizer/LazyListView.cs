// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Windows.Controls;

namespace Debugger.AddIn.Visualizers.GridVisualizer
{

	public class LazyListView<T>
	{
		private ListView listView;
		
		public LazyListView(ListView wrappedListView)
		{
			if (wrappedListView == null)
				throw new ArgumentNullException("wrappedListView");
			
			this.listView = wrappedListView;
			this.listView.AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(handleListViewScroll));
		}
		
		private VirtualizingIEnumerable<T> itemsSource;
		
		public VirtualizingIEnumerable<T> ItemsSource
		{
			get
			{
				return itemsSource;
			}
			set 
			{
				this.itemsSource = value;
				this.itemsSource.RequestNextItems(14);
				this.listView.ItemsSource = value;
			}
		}
		
		private void handleListViewScroll(object sender, ScrollChangedEventArgs e)
		{
			if (e.VerticalChange > 0)
			{
				// scrolled to bottom
				if (e.VerticalOffset == this.itemsSource.Count - e.ViewportHeight)
				{
					this.itemsSource.RequestNextItems(1);
				}
			}
		}
	}
}
