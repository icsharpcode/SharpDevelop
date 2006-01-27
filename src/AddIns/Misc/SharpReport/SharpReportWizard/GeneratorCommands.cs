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
using System.Drawing.Printing;

using System.Data;
using System.Data.OleDb;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

using SharpReport;
using SharpReportCore;

using SharpReport.Designer;

using SharpQuery.SchemaClass;
using SharpQuery.Collections;
using System.Diagnostics;

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
			this.reportModel = reportModel;
		}
		
		public override void Run(){
			ReportGenerator gen = new ReportGenerator();
			if (GlobalValues.IsValidPrinter() == true) {
				customizer.Set("Generator", gen);
				customizer.Set("Language",  ".XSD");
				using (WizardDialog wizard = new WizardDialog("Report Wizard", customizer, WizardPath)) {
					if (wizard.ShowDialog() == DialogResult.OK) {
						Debug.Assert (reportModel != null,"No report model");
						try {
							gen.FillReportModel (reportModel);
							DoCreate(reportModel);
						} catch (Exception e) {
							MessageService.ShowError (e,e.Message);
							return;
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
					GeneratePullReport (model);
					break;
				case GlobalEnums.enmPushPullModel.PushData:
					GeneratePushReport (model);
					break;
				case GlobalEnums.enmPushPullModel.FormSheet:
					GenerateFormSheet (model);
					break;
			}
		}
		
		
		/// <summary>
		/// Generate a report
		/// Pull - Report fill's data be themselve
		/// </summary>
		/// <param name="model">ReportModel</param>
		
		void GeneratePullReport (ReportModel model) {
			
			try {
				GeneratePullDataReport generator = new GeneratePullDataReport(customizer,model);
				if (generator != null) {
					generator.GenerateReport();
				} else {
					throw new NullReferenceException ("GeneratePullDataReport");
				}
			} catch (Exception e) {
				throw e;
			}
		}
		
		/// <summary>
		/// Push Model Report 
		/// Report is created by an .Xsd File 
		/// </summary>
		/// <param name="model">ReportModel</param> 
		void GeneratePushReport (ReportModel model) {
			try {
				GeneratePushDataReport generator = new GeneratePushDataReport(customizer,model);
				if (generator != null) {
					generator.GenerateReport();
				} else {
					throw new NullReferenceException ("GeneratePullDataReport");
				}
			} catch (Exception e) {
				throw e;
			}
		}
		
		
		
		void GenerateFormSheet (ReportModel model) {
			if (model.ReportSettings.DataModel != GlobalEnums.enmPushPullModel.FormSheet) {
				throw new ArgumentException ("Wrong DataModel in GenerateFormSheet");
			}
			
			try {
				model.ReportSettings.ReportType = GlobalEnums.enmReportType.FormSheet;
			} catch (Exception e) {
				throw e;
			}
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
