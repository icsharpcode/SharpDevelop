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
	/// This class generates a plain Formsheet
	/// </summary>
	public class GenerateFormSheetReport:AbstractReportGenerator
	{
		public GenerateFormSheetReport(Properties customizer,
		                              ReportModel reportModel):base(customizer,reportModel){

			if (customizer == null) {
				throw new ArgumentException("customizer");
			}
			if (reportModel == null) {
				throw new ArgumentException("reportModel");
			}
			if (base.ReportModel.ReportSettings.DataModel != GlobalEnums.PushPullModelEnum.FormSheet) {
				throw new ArgumentException ("Wrong DataModel in GeneratePullDataReport");
			}
			base.ReportItemCollection.Clear();
		}
		
		public override void GenerateReport() {
			base.ReportModel.ReportSettings.ReportType = GlobalEnums.ReportTypeEnum.FormSheet;
			base.ReportModel.ReportSettings.DataModel = GlobalEnums.PushPullModelEnum.FormSheet;
			base.GenerateReport();	
			base.AdjustAllNames();
		}
	}
}
