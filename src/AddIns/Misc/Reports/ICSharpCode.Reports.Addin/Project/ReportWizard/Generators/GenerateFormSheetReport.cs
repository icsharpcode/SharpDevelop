// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.Reports.Core;

namespace ICSharpCode.Reports.Addin.ReportWizard
{
	/// <summary>
	/// This class generates a plain Formsheet
	/// </summary>
	public class GenerateFormSheetReport:GeneratePlainReport
	{
		public GenerateFormSheetReport(ReportModel reportModel,                              
		                               Properties customizer):base(reportModel,customizer)
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
//			base.AdjustAllNames();
			base.WriteToXml();
		}
	}
}
