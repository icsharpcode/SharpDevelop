/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 07.02.2006
 * Time: 15:32
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using ICSharpCode.Core;

using SharpReportCore;

namespace ReportGenerator
{
	/// <summary>
	/// Description of GenerateFormSheetReport.
	/// </summary>
	public class GenerateFormSheetReport:AbstractReportGenerator
	{
		public GenerateFormSheetReport(Properties customizer,
		                              ReportModel reportModel):base(customizer,reportModel){

			if (base.ReportModel.ReportSettings.DataModel != GlobalEnums.enmPushPullModel.FormSheet) {
				throw new ArgumentException ("Wrong DataModel in GeneratePullDataReport");
			}
		}
		
		public override void GenerateReport() {
			base.ReportModel.ReportSettings.ReportType = GlobalEnums.enmReportType.FormSheet;
			base.ReportModel.ReportSettings.DataModel = GlobalEnums.enmPushPullModel.FormSheet;
			base.GenerateReport();	
			base.AdjustAll();
		}
	}
}
