/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 02.11.2011
 * Time: 19:49
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	/// <summary>
	/// Interaction logic for AdvancedServiceDialog.xaml
	/// </summary>
	public partial class AdvancedServiceDialog : Window
	{
		public AdvancedServiceDialog()
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
	}
}