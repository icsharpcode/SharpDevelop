// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Debugger.AddIn.Pads.Controls
{
	public partial class SimpleListViewControl : UserControl
	{
		public event EventHandler ItemActivated;
		
		private ObservableCollection<ExpandoObject> itemCollection = new ObservableCollection<ExpandoObject>();
		
		public SimpleListViewControl()
		{
			InitializeComponent();
			
			ItemsListView.MouseDoubleClick += new MouseButtonEventHandler(ItemsListView_MouseDoubleClick);
		}
		
		public ObservableCollection<ExpandoObject> ItemCollection { 
			get { return itemCollection; }
		}
		
		public IList<ExpandoObject> SelectedItems {
			get { 
				var result = new List<ExpandoObject>();
				foreach (var item in ItemsListView.SelectedItems)
					result.Add((ExpandoObject)item);
				
				return result;
			}
		}

		public void ClearColumns()
		{
			((GridView)this.ItemsListView.View).Columns.Clear();
		}
		
		public void AddColumn(string header, Binding binding, double width)
		{
			GridViewColumn column = new GridViewColumn();
			column.Width = width;
			column.DisplayMemberBinding = binding;
			column.Header = header;
			((GridView)this.ItemsListView.View).Columns.Add(column);
		}
		
		void ItemsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var handler = ItemActivated;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
	}
}