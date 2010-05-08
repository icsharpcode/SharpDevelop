/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 23.05.2008
 * Zeit: 22:16
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.Reports.Core;

namespace ICSharpCode.Reports.Addin.ReportWizard
{
	/// <summary>
	/// Description of GeneratePlainReport.
	/// </summary>
	public class GeneratePlainReport:AbstractReportGenerator
	{
		
		public GeneratePlainReport(ReportModel reportModel,Properties customizer):base(reportModel,customizer)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			
			if (base.ReportModel.ReportSettings.DataModel != GlobalEnums.PushPullModel.FormSheet) {
				throw new InvalidReportModelException();
			}
		}
		
		
		public override void GenerateReport()
		{
			base.GenerateReport();
			base.ReportModel.ReportSettings.ReportType = GlobalEnums.ReportType.FormSheet;
			base.ReportModel.ReportSettings.DataModel = GlobalEnums.PushPullModel.FormSheet;
			base.ReportModel.ReportSettings.CommandType = System.Data.CommandType.Text;
			base.WriteToXml();
		}
	}
}
