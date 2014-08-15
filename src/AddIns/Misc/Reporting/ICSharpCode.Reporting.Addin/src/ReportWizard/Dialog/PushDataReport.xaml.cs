/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 07.08.2014
 * Time: 20:13
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using ICSharpCode.Reporting.BaseClasses;
using Xceed.Wpf.Toolkit;
using ICSharpCode.Reporting.Addin.ReportWizard.ViewModels;

namespace ICSharpCode.Reporting.Addin.ReportWizard.Dialog
{
	/// <summary>
	/// Interaction logic for PushDataReport.xaml
	/// </summary>
	public partial class PushDataReport : WizardPage,IHasContext
	{
		List<AbstractColumn> items;
		PushModelContext context;
		
		public PushDataReport()
		{
			InitializeComponent();
			items = new List<AbstractColumn>();
			_DataGrid.ItemsSource = items;
			this.context = new PushModelContext();
			var data = new AbstractColumn("MyColumn",typeof(string));
			items.Add(data);
		}

		
		void UpdateContext()
		{
			context.Items = (List<AbstractColumn>)_DataGrid.ItemsSource;
		}
		
		
		#region IHasContext implementation

		public IWizardContext Context {
			get {
				UpdateContext();
				return context;
			}
		}

		public WizardPageType ReportPageType {
			get {return WizardPageType.PushModelPage;}	
		}

		#endregion
	}
}