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
using System.Windows.Shapes;
using SharpDevelop.XamlDesigner.Controls;
using SharpDevelop.XamlDesigner.Dom;

namespace SharpDevelop.XamlDesigner.PropertyGrid
{
	public partial class ChooseClassDialog : Window
	{
		ChooseClassDialog()
		{
			InitializeComponent();
		}
		
		public static Type Show(IEnumerable<Type> types, Type baseClass)
		{
			var model = new ChooseClassDialogModel(types);
			model.BaseClass = baseClass;

			var dialog = new ChooseClassDialog();
			dialog.DataContext = model;

			if (dialog.ShowDialog().GetValueOrDefault()) {
				return model.SelectedClass;
			}
			return null;
		}

		void uxList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.GetDataContext() is Type) {
				Commit();
			}
		}

		void uxOkButton_Click(object sender, RoutedEventArgs e)
		{
			Commit();
		}

		void Commit()
		{
			DialogResult = true;
			Close();
		}
	}
}
