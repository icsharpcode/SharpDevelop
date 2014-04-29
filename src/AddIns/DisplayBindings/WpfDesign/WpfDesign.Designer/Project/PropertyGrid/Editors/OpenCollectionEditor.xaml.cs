using System;
using System.Collections;
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
//using Xceed.Wpf.Toolkit;

namespace ICSharpCode.WpfDesign.Designer.PropertyGrid.Editors
{
	[TypeEditor(typeof(ICollection))]
	public partial class OpenCollectionEditor : UserControl
	{
		public OpenCollectionEditor()
		{
			InitializeComponent();
		}
		
		void open_Click(object sender, RoutedEventArgs e)
		{
			var node = this.DataContext as PropertyNode;
			
			var editor = new FlatCollectionEditor();
			editor.LoadItemsCollection(node.FirstProperty);
			editor.ShowDialog();
		}
	}
}