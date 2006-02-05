/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 27.01.2005
 * Time: 10:55
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */


	/// <summary>
	/// Description of GeneratorCommands.
	/// </summary>
using System;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

using SharpReport;
using SharpReportCore;

namespace ReportGenerator{
	
	/// <summary>
	/// This Class Create a new Report
	/// </summary>
	public class CreateReport : AbstractMenuCommand {
		const string WizardPath = "/ReportGenerator/ReportGeneratorWizard";
		
		private ReportModel reportModel;

		private Properties customizer = new Properties();
		
		public CreateReport() {
			
		}
		
		public CreateReport(ReportModel reportModel){
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			this.reportModel = reportModel;
		}
		
		public override void Run(){
			ReportGenerator gen = new ReportGenerator();
			if (GlobalValues.IsValidPrinter() == true) {
				customizer.Set("Generator", gen);
				customizer.Set("Language",  ".XSD");
				using (WizardDialog wizard = new WizardDialog("Report Wizard", customizer, WizardPath)) {
					if (wizard.ShowDialog() == DialogResult.OK) {
						
						try {
							gen.FillReportModel (reportModel);
							DoCreate(reportModel);
						} catch (Exception) {
							throw;
						}
					} else {
						throw new SharpReportException("Chancel");
					}
				}
			} else {
				MessageService.ShowError("We need at least one installed Printer to run SharpReport");
			}
		}
		
		void DoCreate (ReportModel model) {
			GlobalEnums.enmPushPullModel dataModel;
			dataModel = model.DataModel;
			switch (dataModel) {
				case GlobalEnums.enmPushPullModel.PullData:
					GeneratePullDataReport generatePullDataReport = new GeneratePullDataReport(customizer,model);
					generatePullDataReport.GenerateReport();
					break;
				case GlobalEnums.enmPushPullModel.PushData:
					GeneratePushDataReport generatePushDataReport = new GeneratePushDataReport(customizer,model);
					generatePushDataReport.GenerateReport();
					break;
				case GlobalEnums.enmPushPullModel.FormSheet:
					model.ReportSettings.ReportType = GlobalEnums.enmReportType.FormSheet;
					break;
			}
		}
		
		
		public class WriteXsdComplete : AbstractMenuCommand {
			public override void Run() {
				ResultPanel resultPanel = base.Owner as ResultPanel;
				if (resultPanel != null) {
					resultPanel.SaveXsdFile (false);
				}
				
			}
		}
		
		public class WriteXsdSchema : AbstractMenuCommand {
			public override void Run() {
				ResultPanel resultPanel = base.Owner as ResultPanel;
				if (resultPanel != null) {
					resultPanel.SaveXsdFile (true);
				}
				
			}
		}
	}
}
