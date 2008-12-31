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
using SharpDevelop.XamlDesigner.Controls;

namespace SharpDevelop.XamlDesigner.PropertyGrid.Editors
{
	public partial class ObjectEditor : UserControl
	{
		public ObjectEditor()
		{
			InitializeComponent();
		}

		PropertyNode PropertyNode
		{
			get { return DataContext as PropertyNode; }
		}

		void uxNewButton_Click(object sender, RoutedEventArgs e)
		{
			var type = ChooseClassDialog.Show(
				PropertyNode.Context.Project.GetAvailableTypes(), 
				PropertyNode.MemberId.ValueType);

			if (type != null) {
				PropertyNode.Value = Activator.CreateInstance(type);
			}
		}
	}
}
