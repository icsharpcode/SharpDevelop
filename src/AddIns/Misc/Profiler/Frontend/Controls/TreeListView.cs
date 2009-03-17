using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Linq;

namespace ICSharpCode.Profiler.Controls
{
	public class TreeListView : ListView
	{		
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (this.SelectedItem is CallTreeNodeViewModel) {
				CallTreeNodeViewModel item = this.SelectedItem as CallTreeNodeViewModel;
				if (e.Key == Key.Right) {
					if (item.IsExpanded) {
						if (item.Children.Count > 0)
							this.SelectedItem = item.Children.First();
					} else {
						item.IsExpanded = true;
					}
					e.Handled = true;
				} else if (e.Key == Key.Left) {
					if (item.IsExpanded) {
						item.IsExpanded = false;
					} else {
						this.SelectedItem = item.Parent;
					}
					e.Handled = true;
				}
			}
			
			if (!e.Handled) {
				base.OnKeyDown(e);
			}
		}
	}
}