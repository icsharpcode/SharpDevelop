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
using ICSharpCode.Reporting.Globals;
using Xceed.Wpf.Toolkit;
using ICSharpCode.Reporting.Addin.ReportWizard.Dialog;
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
		
		public ReportWizard(ReportWizardContext context)
		{
			InitializeComponent();
			this.context = context;
		}
		
	
		void _wizard_Finish(object sender, RoutedEventArgs e)
		{
			foreach (WizardPage element in _wizard.Items) {
				var hc = element as IHasContext;
				if (hc != null) {
					UpdateContext(hc);
				}
			}
		}
  
		
		void UpdateContext(IHasContext hc)
		{
			switch (hc.ReportPageType) {
					case WizardPageType.BaseSettingsPage:{
						context.PageOneContext = hc.Context;
						break;
					}
					
					case WizardPageType.PushModelPage: {
						context.PushModelContext = hc.Context;
						break;
					}
			}
		}
		
		
		void _wizard_Next(object sender, Xceed.Wpf.Toolkit.Core.CancelRoutedEventArgs e)
		{
			var current = this._wizard.CurrentPage;
			if (current.Name.Equals("BaseSettingsPage")) {
				var hasContext = current as IHasContext;
				if (hasContext != null) {
					var pushPullModel = ((PageOneContext)hasContext.Context).DataModel;
					switch (pushPullModel) {
							case PushPullModel.PushData: {
								current.NextPage = (WizardPage)_wizard.Items[2];
								break;
							}
							
							case PushPullModel.PullData: {
								current.NextPage = (WizardPage)_wizard.Items[3];
								break;
							}
							
							case PushPullModel.FormSheet: {
								break;
							}
					}
				}
			}
		}
	}
}