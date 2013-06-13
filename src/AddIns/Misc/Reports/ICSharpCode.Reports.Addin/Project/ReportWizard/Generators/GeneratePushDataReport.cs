// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.SharpDevelop;

/// <summary>
/// This class is used to generate PushDataReports
/// (Reports, that are feed with an DataSet etc)
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 08.09.2005 10:10:19
/// </remarks>
	
namespace ICSharpCode.Reports.Addin.ReportWizard 
{
	public class GeneratePushDataReport : AbstractReportGenerator
	{

		/// <summary>
		/// Default constructor - initializes all fields to default values
		/// </summary>
		public GeneratePushDataReport(ReportModel reportModel,ReportStructure reportStructure):base(reportModel,reportStructure){		                              		                      
			base.UpdateGenerator();
			base.UpdateModel();
		}
		
		
		public override void GenerateReport()
		{
			base.ReportModel.ReportSettings.ReportType = GlobalEnums.ReportType.DataReport;
			base.ReportModel.ReportSettings.DataModel = GlobalEnums.PushPullModel.PushData;
			base.GenerateReport();

			GlobalEnums.ReportLayout reportLayout = ReportStructure.ReportLayout;
			var layout = LayoutFactory.CreateGenerator(reportLayout,base.ReportModel,base.ReportItemCollection);
			
			layout.CreateReportHeader();
			layout.CreatePageHeader();
			layout.CreateDataSection(base.ReportModel.DetailSection);
			layout.CreatePageFooter();
			base.WriteToXml();
		}
	}
}
