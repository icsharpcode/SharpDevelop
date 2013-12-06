// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ICSharpCode.WpfDesign.PropertyGrid;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	public partial class RightClickContextMenu
	{
		private DesignItem designItem;
		
		public RightClickContextMenu(DesignItem designItem)
		{
			this.designItem = designItem;
			
			InitializeComponent();
		}

		void Click_BringToFront(object sender, RoutedEventArgs e)
		{
			var collection = this.designItem.ParentProperty.CollectionElements;
			collection.Remove(this.designItem);
			collection.Add(this.designItem);
		}

		void Click_SendToBack(object sender, RoutedEventArgs e)
		{
			var collection = this.designItem.ParentProperty.CollectionElements;
			collection.Remove(this.designItem);
			collection.Insert(0, this.designItem);
		}
		
		void Click_Backward(object sender, RoutedEventArgs e)
		{
			var collection = this.designItem.ParentProperty.CollectionElements;
			var idx = collection.IndexOf(this.designItem);
			collection.RemoveAt(idx);
			collection.Insert((--idx < 0 ? 0: idx), this.designItem);
		}

		void Click_Forward(object sender, RoutedEventArgs e)
		{
			var collection = this.designItem.ParentProperty.CollectionElements;
			var idx = collection.IndexOf(this.designItem);
			collection.RemoveAt(idx);
			var cnt = collection.Count;
			collection.Insert((++idx > cnt ? cnt : idx), this.designItem);
		}
	}
}
