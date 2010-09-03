// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using ICSharpCode.XamlBinding.PowerToys.Commands;

namespace ICSharpCode.XamlBinding.PowerToys.Dialogs
{
	/// <summary>
	/// Interaction logic for ExtractPropertiesAsStyleDialog.xaml
	/// </summary>
	public partial class ExtractPropertiesAsStyleDialog : Window
	{
		IList<PropertyEntry> items;
		
		public ExtractPropertiesAsStyleDialog(IList<PropertyEntry> items)
		{
			InitializeComponent();
			this.lvwProperties.ItemsSource = this.items = items;
		}
		
		void BtnOKClick(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
		}
		
		void BtnCancelClick(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
		}
		
		void ChkSelectedClick(object sender, RoutedEventArgs e)
		{
			btnOK.IsEnabled = items.Any(item => item.Selected);
		}
		
		public string StyleName {
			get { return txtStyleName.Text; }
		}
	}
}
