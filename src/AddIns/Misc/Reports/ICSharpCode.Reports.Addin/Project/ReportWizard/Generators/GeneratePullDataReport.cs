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
	/// Description of GeneratePullDataReport.
	/// </summary>
	internal class GeneratePullDataReport: AbstractReportGenerator
	{
		
		
		public GeneratePullDataReport(ReportModel reportModel,	                             
		                              Properties properties):base(reportModel,properties)
		                              	
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}

			base.UpdateGenerator();
			base.UpdateModel();
			base.SqlQueryParameters.AddRange(base.ReportStructure.SqlQueryParameters);
		}
		
		
		
		public override void GenerateReport()
		{
			base.ReportModel.ReportSettings.ReportType = GlobalEnums.ReportType.DataReport;
			base.ReportModel.ReportSettings.DataModel = GlobalEnums.PushPullModel.PullData;
			base.GenerateReport();
		
			GlobalEnums.ReportLayout reportLayout = (GlobalEnums.ReportLayout)base.Properties.Get("ReportLayout");
			AbstractLayout layout = LayoutFactory.CreateGenerator(reportLayout,base.ReportModel,base.ReportItemCollection);
			layout.CreateReportHeader();
			layout.CreatePageHeader();
			layout.CreateDataSection(base.ReportModel.DetailSection);
			layout.CreatePageFooter();
			base.WriteToXml();
		}
	}
}
