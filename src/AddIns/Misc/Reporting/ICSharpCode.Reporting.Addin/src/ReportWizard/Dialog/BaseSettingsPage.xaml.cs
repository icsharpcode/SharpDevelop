/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 08.07.2014
 * Time: 20:26
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows;
using Xceed.Wpf.Toolkit;
using ICSharpCode.Reporting.Addin.ReportWizard.ViewModels;

namespace ICSharpCode.Reporting.Addin.ReportWizard.Dialog{

	/// <summary>
	/// Interaction logic for PageOne.xamlWizardPage,IHasContext
	/// </summary>
	public partial class BaseSettingsPage : WizardPage,IHasContext
	{
		PageOneContext context;
		
		public BaseSettingsPage()
		{
			InitializeComponent();
			this.context = new PageOneContext(); 
		}
		
		
		public IWizardContext Context {
			get{
				UpdateContext();
				return context;}
		}
		
		public int PageNumber {
			get {return 1;}
		}
		
		
		void  WizardPage_Enter(object sender, RoutedEventArgs e)
		{
			
			     Console.Write("Create Context for PageOne");
		}
		
		void WizardPage_Leave(object sender, RoutedEventArgs e)
		{
//			NewMethod();
		}

		void UpdateContext()
		{
//			context.FormSheet = this._FormSheet.IsChecked == true;
//			context.PushModel = this._PushModel.IsChecked == true;
			context.ReportName = this._ReportName.Text;
			context.FileName = this._Filename.Text;
			context.Legal = _Legal.IsChecked == true;
			;
			context.Landscape = _Landscape.IsChecked == true;
		}
	}
}