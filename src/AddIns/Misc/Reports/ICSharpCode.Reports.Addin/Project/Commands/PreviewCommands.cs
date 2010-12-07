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
		ReportModel model;
		ICSharpCode.Reports.Core.ReportViewer.PreviewControl reportViewer;
		
		public AbstractPreviewCommand(ReportModel model, ICSharpCode.Reports.Core.ReportViewer.PreviewControl reportViewer)
		{
			if (model == null) {
				throw new  ArgumentNullException ("model");
			}
			if (reportViewer == null) {
				throw new ArgumentNullException("reportViewer");
			}
			
			this.model = model;
			this.reportViewer = reportViewer;
		}
		
		
		public override void Run()
		{
			CollectParametersCommand sql = new CollectParametersCommand(model);
			sql.Run();
		}
		
		
		public ICSharpCode.Reports.Core.ReportViewer.PreviewControl ReportViewer {
			get { return reportViewer; }
		}
		
		public ReportModel Model {
			get { return this.model; }
		}
	}
	
	
	public class FormsSheetPreviewCommand:AbstractPreviewCommand
	{
		public FormsSheetPreviewCommand(ReportModel model,
		                                     ICSharpCode.Reports.Core.ReportViewer.PreviewControl reportViewer):base (model,reportViewer)
		{
		}
		
		public override void Run()
		{
			base.Run();
			base.ReportViewer.RunReport (base.Model,(ReportParameters)null);
		}
	}
	
	
	public class PullModelPreviewCommand:AbstractPreviewCommand
	{
		
		public PullModelPreviewCommand(ReportModel model, ICSharpCode.Reports.Core.ReportViewer.PreviewControl reportViewer):base (model,reportViewer)
		{
		}
		
		public override void Run()
		{
			base.Run();
			WorkbenchSingleton.StatusBar.SetMessage("Connect...");
			base.ReportViewer.RunReport(base.Model,(ReportParameters)null);
		}
	}
	
	
	public class PushModelPreviewCommand:AbstractPreviewCommand
	{
		public PushModelPreviewCommand(ReportModel model, ICSharpCode.Reports.Core.ReportViewer.PreviewControl reportViewer):base (model,reportViewer)
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
