/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 05.08.2014
 * Time: 20:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Xceed.Wpf.Toolkit;

namespace ICSharpCode.Reporting.Addin.ReportWizard.Dialog
{
	/// <summary>
	/// Interaction logic for WelcomePage.xaml
	/// </summary>
	public partial class WelcomePage : WizardPage
	{
		public WelcomePage()
		{
			InitializeComponent();
			_image.Source = WizardHelper.GetWizardIcon();
		}
	}
}