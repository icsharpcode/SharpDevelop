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
using SharpDevelop.XamlDesigner.Dom;

namespace SharpDevelop.XamlDesigner.PropertyGrid.Editors
{
	public partial class CollectionEditor : UserControl
	{
		public CollectionEditor()
		{
			InitializeComponent();
		}

		PropertyNode PropertyNode
		{
			get { return DataContext as PropertyNode; }
		}

		void uxItemsButton_Click(object sender, RoutedEventArgs e)
		{
			//CollectionEditorDialog.Show(PropertyModel. DesignContext.TestCollection.Property("Children").Collection);
		}
	}
}
