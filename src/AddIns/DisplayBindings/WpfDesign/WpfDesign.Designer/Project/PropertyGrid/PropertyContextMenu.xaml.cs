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

namespace ICSharpCode.WpfDesign.Designer.PropertyGrid
{
	public partial class PropertyContextMenu
	{
		public PropertyContextMenu()
		{
			InitializeComponent();
		}

		public PropertyNode PropertyNode {
			get { return DataContext as PropertyNode; }
		}

		void Click_Reset(object sender, RoutedEventArgs e)
		{
			PropertyNode.Reset();
		}

		void Click_Binding(object sender, RoutedEventArgs e)
		{
			PropertyNode.CreateBinding();
		}

		void Click_CustomExpression(object sender, RoutedEventArgs e)
		{
		}

		void Click_ConvertToLocalValue(object sender, RoutedEventArgs e)
		{
		}

		void Click_SaveAsResource(object sender, RoutedEventArgs e)
		{
		}
	}
}
