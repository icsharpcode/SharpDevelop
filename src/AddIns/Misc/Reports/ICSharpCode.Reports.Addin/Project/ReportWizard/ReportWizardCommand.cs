/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 17.05.2008
 * Zeit: 17:22
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Reports.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Reports.Addin.ReportWizard
{
	/// <summary>
	/// Description of GeneratorCommand
	/// </summary>
	public class ReportWizardCommand : AbstractMenuCommand
	{
		private const string WizardPath = "/ReportGenerator/ReportGeneratorWizard";
		private OpenedFile file;
		private ReportModel reportModel;
		private IReportGenerator reportGenerator;
		private Properties customizer = new Properties();
		private ReportStructure reportStructure;
		private bool canceled;
		
		
		public ReportWizardCommand(OpenedFile file)
		{
			this.file = file;
		}
		
		
		public override void Run()
		{
			reportStructure = new ReportStructure();
			customizer.Set("Generator", reportStructure);
			customizer.Set("ReportLayout",GlobalEnums.ReportLayout.ListLayout);
			
			if (GlobalValues.IsValidPrinter() == true) {
				
				using (WizardDialog wizard = new WizardDialog("Report Wizard", customizer, WizardPath)) {
					if (wizard.ShowDialog() == DialogResult.OK) {
						reportModel = reportStructure.CreateAndFillReportModel ();
						CreateReportFromModel(reportModel);
					}
					else{
						this.canceled = true;
					}
				}
			} else {
				MessageService.ShowError(ResourceService.GetString("Sharpreport.Error.NoPrinter"));
			}
		}
		
		
		private void CreateReportFromModel (ReportModel model)
		{
			reportGenerator = GeneratorFactory.Create (model,customizer);
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
