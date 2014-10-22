/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 16.07.2014
 * Time: 20:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Reporting.Addin.ReportWizard.ViewModels;

namespace ICSharpCode.Reporting.Addin.ReportWizard.Dialog
{
	/// <summary>
	/// Description of IHasContext.
	/// </summary>
	public interface IHasContext
	{
		IWizardContext Context {get;}
		
		WizardPageType ReportPageType {get;}
	}
}
