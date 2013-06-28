// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace ICSharpCode.Profiler.Controls
{
	public class TreeListView : ListView
	{		
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (SelectedItem is CallTreeNodeViewModel) {
				CallTreeNodeViewModel item = SelectedItem as CallTreeNodeViewModel;
				if (e.Key == Key.Right) {
					if (item.IsExpanded) {
						if (item.Children.Count > 0)
							SelectedItem = item.Children.First();
					} else {
						item.IsExpanded = true;
					}
					e.Handled = true;
				} else if (e.Key == Key.Left) {
					if (item.IsExpanded) {
						item.IsExpanded = false;
					} else {
						SelectedItem = item.Parent;
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
