/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 07.07.2014
 * Time: 20:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows;
using Xceed.Wpf.Toolkit;
using ICSharpCode.Reporting.Addin.ReportWizard.ViewModels;
using System.Linq;

namespace ICSharpCode.Reporting.Addin.ReportWizard.Dialog
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class ReportWizard : Window
	{
		readonly ReportWizardContext context;
		readonly BaseSettingsPage baseSettingsPage;
		
		public ReportWizard(ReportWizardContext context)
		{
			InitializeComponent();
			this.context = context;
			baseSettingsPage = new BaseSettingsPage();
			_wizard.Items.Insert(1,baseSettingsPage);
		}
		
		
		void _wizard_Next(object sender, Xceed.Wpf.Toolkit.Core.CancelRoutedEventArgs e)
		{
		}
	
		
		void _wizard_PageChanged(object sender, RoutedEventArgs e)
		{
	
		}
	
		void _wizard_Finish(object sender, RoutedEventArgs e)
		{
			context.PageOneContext = baseSettingsPage.Context;
		
		}
		
	}
}