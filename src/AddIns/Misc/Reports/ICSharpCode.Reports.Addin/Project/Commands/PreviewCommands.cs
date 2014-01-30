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
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Reports.Addin.Commands
{
	
	public class AbstractPreviewCommand :AbstractCommand
	{
		
		public AbstractPreviewCommand(ReportModel model, ICSharpCode.Reports.Core.ReportViewer.PreviewControl reportViewer)
		{
			if (model == null) {
				throw new  ArgumentNullException ("model");
			}
			if (reportViewer == null) {
				throw new ArgumentNullException("reportViewer");
			}
			
			this.Model = model;
			this.ReportViewer = reportViewer;
		}
		
		
		public override void Run()
		{
			CollectParametersCommand cmd = new CollectParametersCommand(Model.ReportSettings);
			cmd.Run();
		}
		
		
		public ICSharpCode.Reports.Core.ReportViewer.PreviewControl ReportViewer {get;private set;}
		
		
		public ReportModel Model {get;private set;}
		
	}
	
	
	public class FormSheetToReportViewerCommand:AbstractPreviewCommand
	{
		public FormSheetToReportViewerCommand(ReportModel model,
		                                     ICSharpCode.Reports.Core.ReportViewer.PreviewControl reportViewer):base (model,reportViewer)
		{
		}
		
		public override void Run()
		{
			base.Run();
			base.ReportViewer.RunReport (base.Model,(ReportParameters)null);
		}
	}
	
	
	public class PullModelToReportViewerCommand:AbstractPreviewCommand
	{
		
		public PullModelToReportViewerCommand(ReportModel model, ICSharpCode.Reports.Core.ReportViewer.PreviewControl reportViewer):base (model,reportViewer)
		{
		}
		
		public override void Run()
		{
			base.Run();
			SD.StatusBar.SetMessage("Connect...");
			base.ReportViewer.RunReport(base.Model,(ReportParameters)null);
		}
	}
	
	
	public class PushModelToReportViewerCommand:AbstractPreviewCommand
	{
		public PushModelToReportViewerCommand(ReportModel model, ICSharpCode.Reports.Core.ReportViewer.PreviewControl reportViewer):base (model,reportViewer)
		{
		}
		
		public override void Run()
		{
			base.Run();
			DataSetFromXsdCommand cmd = new DataSetFromXsdCommand();
			cmd.Run();
			System.Data.DataSet ds = cmd.DataSet;
			SD.StatusBar.SetMessage("Connect...");
			base.ReportViewer.RunReport(base.Model,ds.Tables[0],null);
		}
		
	}
}
