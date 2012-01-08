// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Controls;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public partial class AddServiceReferenceDialog : Window
	{
		public AddServiceReferenceDialog()
		{
			InitializeComponent();
		}
		
		void okButtonClick(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
			Close();
		}
		
		void cancelButtonClick(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			Close();
		}
		
		void Cbo_LostFocus(object sender, RoutedEventArgs e)
		{
			var comboBox = (ComboBox)sender;
			if (comboBox.SelectedItem != null)
				return;
			var newItem = comboBox.Text;
			comboBox.SelectedItem = newItem;
		}
		
		void Tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (e.NewValue != null) {
				ServiceItem myItem = (ServiceItem)e.NewValue;
				var dc = (AddServiceReferenceViewModel)tree.DataContext;
				dc.ServiceItem = myItem;
			}
		}
	}
}