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

namespace SharpDevelop.XamlDesigner.PropertyGrid
{
	public partial class BindingDialog : Window
	{
		BindingDialog()
		{
			InitializeComponent();
		}

		public static void Show()
		{
			var dialog = new BindingDialog();
			dialog.ShowDialog();
		}

		void uxOkButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}
	}
}
