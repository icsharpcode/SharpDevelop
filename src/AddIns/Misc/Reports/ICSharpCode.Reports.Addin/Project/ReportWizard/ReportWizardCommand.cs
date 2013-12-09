// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.Reports.Addin.ReportWizard
{
	/// <summary>
	/// Description of GeneratorCommand
	/// </summary>
	public class ReportWizardCommand : AbstractMenuCommand
	{
		private const string WizardPath = "/ReportGenerator/ReportGeneratorWizard";
		private readonly OpenedFile file;
		private ReportModel reportModel;
		private IReportGenerator reportGenerator;
//		private Properties customizer = new Properties();
		private ReportStructure reportStructure;
		private bool canceled;
		
		
		public ReportWizardCommand(OpenedFile file)
		{
			this.file = file;
		}
		
		
		public override void Run()
		{
			reportStructure = new ReportStructure();
			if (GlobalValues.IsValidPrinter() == true) {
				
				using (WizardDialog wizard = new WizardDialog("Report Wizard", reportStructure, WizardPath)) {
					if (wizard.ShowDialog() == DialogResult.OK) {
						reportModel = reportStructure.CreateAndFillReportModel ();
						CreateReportFromModel(reportModel,reportStructure);
					}
					else{
						this.canceled = true;
					}
				}
			} else {
				MessageService.ShowError(ResourceService.GetString("Sharpreport.Error.NoPrinter"));
			}
		}
		
		
		private void CreateReportFromModel (ReportModel model,ReportStructure reportStructure)
		{
			reportGenerator = GeneratorFactory.Create (model,reportStructure);
			file.MakeDirty();
			reportGenerator.GenerateReport();
		}
		
		
		public bool Canceled
		{
			get { return canceled; }
		}
		
		
		public MemoryStream GeneratedReport 
		{
			get { return reportGenerator.Generated; }
		}
	}
}
