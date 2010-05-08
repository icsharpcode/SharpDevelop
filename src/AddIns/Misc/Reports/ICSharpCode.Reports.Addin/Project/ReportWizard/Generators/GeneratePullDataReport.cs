/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 29.08.2008
 * Zeit: 18:16
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using ICSharpCode.Core;
using ICSharpCode.Reports.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.Reports.Addin.ReportWizard
{
	/// <summary>
	/// Description of GeneratePullDataReport.
	/// </summary>
	internal class GeneratePullDataReport: AbstractReportGenerator
	{
		private ReportStructure reportStructure;
		
		public GeneratePullDataReport(ReportModel reportModel,	                             
		                              Properties properties):base(reportModel,properties)
		                              	
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}

			
			if (base.ReportModel.ReportSettings.DataModel != GlobalEnums.PushPullModel.PullData) {
				throw new ArgumentException ("Wrong DataModel in GeneratePullReport");
			}
			reportStructure = (ReportStructure)properties.Get("Generator");

			base.AvailableFieldsCollection.AddRange(reportStructure.AvailableFieldsCollection);
			base.ReportItemCollection.AddRange(reportStructure.ReportItemCollection);
			base.SqlQueryParameters.AddRange(reportStructure.SqlQueryParameters);
		}
		
		
		
		public override void GenerateReport()
		{
			base.ReportModel.ReportSettings.ReportType = GlobalEnums.ReportType.DataReport;
			base.ReportModel.ReportSettings.DataModel = GlobalEnums.PushPullModel.PullData;
			base.ReportModel.ReportSettings.AvailableFieldsCollection.AddRange(reportStructure.AvailableFieldsCollection);
			base.ReportModel.ReportSettings.ParameterCollection.AddRange(reportStructure.SqlQueryParameters);
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
