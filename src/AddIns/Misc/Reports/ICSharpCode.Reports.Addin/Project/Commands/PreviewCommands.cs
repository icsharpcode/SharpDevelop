// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.Reports.Core;
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
			CollectParametersCommand cmd = new CollectParametersCommand(Model);
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
			WorkbenchSingleton.StatusBar.SetMessage("Connect...");
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
			WorkbenchSingleton.StatusBar.SetMessage("Connect...");
			base.ReportViewer.RunReport(base.Model,ds.Tables[0],null);
		}
		
	}
}
