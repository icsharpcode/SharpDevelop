/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 08.07.2014
 * Time: 19:47
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using System.Text;
using ICSharpCode.Core;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Templates;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.Reporting.Addin.Factory;
using ICSharpCode.Reporting.Addin.ReportWizard.Dialog;
using ICSharpCode.Reporting.Addin.ReportWizard.ViewModels;

namespace ICSharpCode.Reporting.Addin.ReportWizard
{
	/// <summary>
	/// This command is invoked by EmptyReport.xfg.
	/// </summary>
	class ReportWizardCommand: SimpleCommand
	{
		
		public override void Execute(object parameter)
		{
			if (parameter == null)
				throw new ArgumentNullException("parameter");
			FileTemplateResult result = (FileTemplateResult)parameter;
			var openedFile = result.NewOpenedFiles.Single();
			var wizardViewModel = new ReportWizardContext();
			var reportWizard = new ICSharpCode.Reporting.Addin.ReportWizard.Dialog.ReportWizard(wizardViewModel);
			
			reportWizard.ShowDialog();
			if (reportWizard.DialogResult.HasValue && reportWizard.DialogResult.Value){
				   	LoggingService.Info("ReportWizard - CreateReport");   
				var rg = new ReportGenerator();
				
				rg.Generate(wizardViewModel);
				string xml = CreateFormSheetFromModel.ToXml(rg.ReportModel).ToString();
				openedFile.SetData(Encoding.UTF8.GetBytes(xml));
				if (!openedFile.IsUntitled)
					openedFile.SaveToDisk();
			} else {
				LoggingService.Info("ReportWizard canceled");
				// HACK: cancel opening the file by clearing the file list
				openedFile.CloseIfAllViewsClosed();
				result.NewOpenedFiles.Clear();
				result.NewFiles.Clear();
			}
		}
	}
}
