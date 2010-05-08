/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 05.04.2008
 * Zeit: 10:37
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Globalization;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Reports.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.Reports.Core.ReportViewer;
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
	
	
	public class AsyncFormsSheetPreviewCommand:AbstractPreviewCommand
	{
		public AsyncFormsSheetPreviewCommand(ReportModel model,
		                                     ICSharpCode.Reports.Core.ReportViewer.PreviewControl reportViewer):base (model,reportViewer)
		{
		}
		
		public override void Run()
		{
			base.Run();
			base.ReportViewer.RunReport (base.Model,(ReportParameters)null);
		}
	}
	
	
	public class AsyncPullModelPreviewCommand:AbstractPreviewCommand
	{
		
		public AsyncPullModelPreviewCommand(ReportModel model, ICSharpCode.Reports.Core.ReportViewer.PreviewControl reportViewer):base (model,reportViewer)
		{
		}
		
		public override void Run()
		{
			base.Run();
			ICSharpCode.SharpDevelop.StatusBarService.SetMessage("Connect...");
			base.ReportViewer.RunReport(base.Model,(ReportParameters)null);
		}
	}
	
	
	public class AsyncPushModelPreviewCommand:AbstractPreviewCommand
	{
		public AsyncPushModelPreviewCommand(ReportModel model, ICSharpCode.Reports.Core.ReportViewer.PreviewControl reportViewer):base (model,reportViewer)
		{
		}
		
		public override void Run()
		{
			base.Run();
			DataSetFromXsdCommand cmd = new DataSetFromXsdCommand();
			cmd.Run();
			System.Data.DataSet ds = cmd.DataSet;
			ICSharpCode.SharpDevelop.StatusBarService.SetMessage("Connect...");
			base.ReportViewer.RunReport(base.Model,ds.Tables[0],null);
		}
		
	}
}
