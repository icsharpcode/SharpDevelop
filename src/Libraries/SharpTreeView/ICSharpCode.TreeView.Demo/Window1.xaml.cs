// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
