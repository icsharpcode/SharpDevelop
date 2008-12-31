using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.Specialized;

namespace SharpDevelop.XamlDesigner.Controls
{
	public class CollectionListener : ItemsControl
	{
		public CollectionListener(object source, string bindingPath)
		{
			SetBinding(ItemsSourceProperty, new Binding(bindingPath) { Source = source });
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
		{
			base.OnItemsChanged(e);
			if (CollectionChanged != null) {
				CollectionChanged(this, e);
			}
		}
	}
}
