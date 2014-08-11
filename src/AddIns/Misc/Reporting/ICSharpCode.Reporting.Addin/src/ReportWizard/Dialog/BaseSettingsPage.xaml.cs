/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 08.07.2014
 * Time: 20:26
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ICSharpCode.Reporting.Globals;
using ICSharpCode.SharpDevelop;
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
			_DataModel.SelectedItem = PushPullModel.FormSheet;
			_ReportType.SelectedItem = ReportType.FormSheet;
			this.context = new PageOneContext();
			this._DataModel.SelectedItem = PushPullModel.FormSheet;
			_image.Source = WizardHelper.GetWizardIcon();
		}
    
		
		public IWizardContext Context {
			get{
				UpdateContext();
				return context;}
		}
		
		public int PageNumber {
			get {return 1;}
		}
		
		
		void UpdateContext(){

			context.DataModel = (PushPullModel) _DataModel.SelectedItem;
			context.ReportType = (ReportType) _ReportType.SelectedItem;
			context.ReportName = this._ReportName.Text;
			context.FileName = this._Filename.Text;
			context.Legal = _Legal.IsChecked == true;
			context.Landscape = _Landscape.IsChecked == true;
		}
		
		
		void _DataModel_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e){
			var cbo = (ComboBox) sender;
			
			if (((PushPullModel)cbo.SelectedItem) == PushPullModel.FormSheet) {
				this.CanFinish = true;
				this.CanSelectNextPage = false;
				this._ReportType.SelectedItem = ReportType.FormSheet;
			} else {
				this._ReportType.SelectedItem = ReportType.DataReport;
				this.CanFinish = false;
				this.CanSelectNextPage = true;
			}
		}
	}
}