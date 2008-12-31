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
	public partial class CollectionEditorDialog : Window
	{
		CollectionEditorDialog()
		{
			InitializeComponent();
		}

		CollectionEditorDialogModel Model { get; set; }

		public static bool Show(DesignItemCollection collection)
		{
			var dialog = new CollectionEditorDialog();
			dialog.Model = new CollectionEditorDialogModel(collection);
			dialog.DataContext = dialog.Model;
			return dialog.ShowDialog().GetValueOrDefault();
		}

		void uxOkButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}

		void uxChooseTypeButton_Click(object sender, RoutedEventArgs e)
		{
			Model.SelectedType = ChooseClassDialog.Show(
				Model.Collection.Context.Project.GetAvailableTypes(), null);
		}

		void uxUpButton_Click(object sender, RoutedEventArgs e)
		{
			Model.MoveUp();
		}

		void uxAddButton_Click(object sender, RoutedEventArgs e)
		{
			Model.Add();
		}

		void uxDownButton_Click(object sender, RoutedEventArgs e)
		{
			Model.MoveDown();
		}

		void uxRemoveButton_Click(object sender, RoutedEventArgs e)
		{
			Model.Remove();
		}
	}
}
