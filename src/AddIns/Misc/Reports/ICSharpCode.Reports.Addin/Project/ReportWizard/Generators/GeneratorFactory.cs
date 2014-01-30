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

		public static IReportGenerator Create (ReportModel model,ReportStructure reportStructure)			                                     
		                                       
		{	
			IReportGenerator reportGenerator = null;
				switch (model.DataModel) {
				case GlobalEnums.PushPullModel.PullData:
					reportGenerator = new GeneratePullDataReport(model,reportStructure);
					break;
				case GlobalEnums.PushPullModel.PushData:
					reportGenerator = new GeneratePushDataReport(model,reportStructure);
					break;
				case GlobalEnums.PushPullModel.FormSheet:
					reportGenerator = new GenerateFormSheetReport (model,reportStructure);
					break;
			}
			return reportGenerator;
		}
	}
}
