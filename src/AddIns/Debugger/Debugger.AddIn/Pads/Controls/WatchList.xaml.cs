// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Debugger.AddIn.TreeModel;
using ICSharpCode.SharpDevelop.Debugging;

namespace Debugger.AddIn.Pads.Controls
{
	public partial class WatchList : UserControl
	{
		private ObservableCollection<TreeNode> items = new ObservableCollection<TreeNode>();
		
		public WatchList()
		{
			InitializeComponent(); 
		}
		
		void TextBox_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter && e.Key != Key.Escape) 
			{
				e.Handled = true;
				return;
			}
				
			if (e.Key == Key.Enter) {				
				if(SelectedNode is ExpressionNode) {
					var node = (ExpressionNode)SelectedNode;
					node.SetText(((TextBox)sender).Text);
				}
			}
			if (e.Key == Key.Enter || e.Key == Key.Escape) {
				for (int i = 0; i < MyList.Items.Count; i++) {
                TreeViewItem child = (TreeViewItem)MyList.ItemContainerGenerator.ContainerFromIndex(i);
                child.IsSelected = false;
				}
			}
		}

		public ObservableCollection<TreeNode> WatchItems { get { return items; } }
		
		public TreeNode SelectedNode {
			get {
				return this.MyList.SelectedItem as TreeNode;
			}
		}
	}
}