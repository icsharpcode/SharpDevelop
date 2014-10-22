/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 08.07.2014
 * Time: 19:47
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Core;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.Reporting.Addin.ReportWizard.Dialog;
using ICSharpCode.Reporting.Addin.ReportWizard.ViewModels;

namespace ICSharpCode.Reporting.Addin.ReportWizard
{
	/// <summary>
	/// Description of ReportWizardCommand.
	/// </summary>
	class ReportWizardCommand: AbstractMenuCommand
	{
		
		public override void Run()
		{
			var wizardViewModel = new ReportWizardContext();
			var reportWizard = new ICSharpCode.Reporting.Addin.ReportWizard.Dialog.ReportWizard(wizardViewModel);
			
			reportWizard.ShowDialog();
			if (reportWizard.DialogResult.HasValue && reportWizard.DialogResult.Value){
				   	LoggingService.Info("ReportWizard - CreateReport");   
				var rg = new ReportGenerator();
				
				rg.Generate(wizardViewModel);
				ReportModel = rg.ReportModel;
				} else {
				Canceled = true;
			}
		}
		
		public bool Canceled {get; private set;}
		
		public IReportModel ReportModel {get;private set;}
	}
}
