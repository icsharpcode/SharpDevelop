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

namespace ICSharpCode.TreeView.Demo
{
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();

			treeView1.Root = new FolderNode("c:\\");
			//treeView1.ShowRoot = false;
			//treeView1.SelectionChanged += new SelectionChangedEventHandler(treeView1_SelectionChanged);

			treeView2.Root = new FolderNode("c:\\");
			//treeView2.ShowRootExpander = true;
			//treeView2.ShowRoot = false;
		}

		//void treeView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
		//{
		//    //Debug.WriteLine(treeView1.SelectedItems.Count);

		//    StringBuilder sb = new StringBuilder();
		//    foreach (var item in SharpTreeNode.ActiveNodes) {
		//        sb.Append(item.ToString() + "; ");
		//    }
		//    Debug.WriteLine(sb.ToString());
		//}

		public static Image LoadIcon(string name)
		{
			var frame = BitmapFrame.Create(new Uri("pack://application:,,,/Images/" + name, UriKind.Absolute));
			Image result = new Image();
			result.Source = frame;
			return result;
		}
	}
}
