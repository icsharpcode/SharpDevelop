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
		
		void UrlComboBoxLoaded(object sender, RoutedEventArgs e)
		{
			TextBox textBox = UrlComboBox.Template.FindName("PART_EditableTextBox", UrlComboBox) as TextBox;
			textBox.Focus();
		}
		
		void okButtonClick(object sender, RoutedEventArgs e)
		{
			if (CanAddServiceReference()) {
				this.DialogResult = true;
				Close();
			}
		}
		
		bool CanAddServiceReference()
		{
			var dc = (AddServiceReferenceViewModel)DataContext;
			return dc.CanAddServiceReference();
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
