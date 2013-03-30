// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.Reports.Addin.ReportWizard
{
	/// <summary>
	/// This class generates a plain Formsheet
	/// </summary>
	public class GenerateFormSheetReport:GeneratePlainReport
	{
		public GenerateFormSheetReport(ReportModel reportModel,ReportStructure reportStructure ):base(reportModel,reportStructure)                                                      
		{
		                             	
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			
			if (base.ReportModel.ReportSettings.DataModel != GlobalEnums.PushPullModel.FormSheet) {
				throw new InvalidReportModelException();
			}
		}
		
		
		public override void GenerateReport() {
			base.GenerateReport();
			ListLayout layout = new ListLayout(base.ReportModel,null);
			layout.CreateReportHeader();
			layout.CreatePageFooter();
			base.WriteToXml();
		}
	}
}
