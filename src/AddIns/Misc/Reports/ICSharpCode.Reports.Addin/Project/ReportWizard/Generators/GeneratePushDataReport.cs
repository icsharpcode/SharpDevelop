// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.Reports.Core;
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
		private ReportStructure reportStructure;
		/// <summary>
		/// Default constructor - initializes all fields to default values
		/// </summary>
		public GeneratePushDataReport(ReportModel reportModel,		                              
		                              Properties properties):base(reportModel,properties)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}

			if (base.ReportModel.ReportSettings.DataModel != GlobalEnums.PushPullModel.PushData) {
				throw new ArgumentException ("Wrong DataModel in GeneratePushReport");
			}
			
			reportStructure = (ReportStructure)properties.Get("Generator");

			base.AvailableFieldsCollection.AddRange(reportStructure.AvailableFieldsCollection);
			base.ReportItemCollection.AddRange(reportStructure.ReportItemCollection);
		}
		
		
		public override void GenerateReport()
		{
			base.ReportModel.ReportSettings.ReportType = GlobalEnums.ReportType.DataReport;
			base.ReportModel.ReportSettings.DataModel = GlobalEnums.PushPullModel.PushData;
			base.ReportModel.ReportSettings.AvailableFieldsCollection.AddRange(reportStructure.AvailableFieldsCollection);
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

