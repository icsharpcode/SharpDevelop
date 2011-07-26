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
	/// Description of GeneratorFactory.
	/// </summary>
	public static class LayoutFactory
	{
		
		public static AbstractLayout CreateGenerator (GlobalEnums.ReportLayout reportLayout,
		                                              ReportModel model,
		                                              ReportItemCollection items)
		{
			AbstractLayout layout = null;
			switch (reportLayout) {
				case GlobalEnums.ReportLayout.ListLayout:
					layout = new ListLayout(model,items);
					break;
				case GlobalEnums.ReportLayout.TableLayout:
					layout = new TableLayout(model,items);
					break;
			}
			return layout;
		}
	}
	
	public static class GeneratorFactory
	{

		public static IReportGenerator Create (ReportModel model,		                                     
		                                       Properties customizer)
		{	
			IReportGenerator reportGenerator = null;
				switch (model.DataModel) {
				case GlobalEnums.PushPullModel.PullData:
					reportGenerator = new GeneratePullDataReport(model,customizer);
					
					break;
				case GlobalEnums.PushPullModel.PushData:
					reportGenerator = new GeneratePushDataReport(model,customizer);
					break;
				case GlobalEnums.PushPullModel.FormSheet:
					reportGenerator = new GenerateFormSheetReport (model,customizer);
					break;
			}
			return reportGenerator;
		}
	}
}
