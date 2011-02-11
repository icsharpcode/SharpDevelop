// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.SharpDevelop;

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
