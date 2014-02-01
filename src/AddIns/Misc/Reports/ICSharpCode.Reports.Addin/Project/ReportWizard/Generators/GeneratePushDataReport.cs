// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
