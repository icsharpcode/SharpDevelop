/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 16.08.2014
 * Time: 16:43
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
using Xceed.Wpf.Toolkit;

namespace ICSharpCode.Reporting.Addin.ReportWizard.Dialog
{
	/// <summary>
	/// Interaction logic for PullModelPage.xaml
	/// </summary>
	public partial class PullModelPage : WizardPage,IHasContext
	{
		public PullModelPage()
		{
			InitializeComponent();
		}
		
		
		#region IHasContext implementation
		public ICSharpCode.Reporting.Addin.ReportWizard.ViewModels.IWizardContext Context {
			get {
				throw new NotImplementedException();
			}
		}
		public WizardPageType ReportPageType {
			get { return WizardPageType.PullModelPage;}
		}
		
		#endregion
	}
}