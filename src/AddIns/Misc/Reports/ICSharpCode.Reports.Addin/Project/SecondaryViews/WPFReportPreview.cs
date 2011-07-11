/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 03.05.2011
 * Time: 19:34
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Reports.Addin.Commands;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Exporter.ExportRenderer;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.Reports.Core.WpfReportViewer;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Reports.Addin.SecondaryViews
{
	/// <summary>
	/// Description of WPFReportPreview.
	/// </summary>
	public class WPFReportPreview: AbstractSecondaryViewContent
	{
		ReportDesignerLoader designerLoader;
		
		IWpfReportViewer viewer = new WpfReportViewer();
		IExportRunner exportRunner = new ExportRunner();
		
		public WPFReportPreview(ReportDesignerLoader loader,IViewContent content):base(content)
		{
			this.designerLoader = loader;
			base.TabPageText = "Wpf View";
			Pages = new PagesCollection();
		}
		
		
		public PagesCollection Pages{get;private set;}
	
		
		protected override void LoadFromPrimary()
		{
			Pages.Clear();
			ReportModel model = designerLoader.CreateRenderableModel();
			var collectCmd = new CollectParametersCommand(model);
			collectCmd.Run();
			switch (model.DataModel)
			{
					case GlobalEnums.PushPullModel.FormSheet :
					{
						exportRunner.RunReport(model,(ReportParameters)null);
						break;
					}
					case GlobalEnums.PushPullModel.PullData:
					{
						exportRunner.RunReport(model,(ReportParameters)null);
						break;
					}
					case GlobalEnums.PushPullModel.PushData:
					{
						var cmd = new DataSetFromXsdCommand();
						cmd.Run();
						System.Data.DataSet ds = cmd.DataSet;
						exportRunner.RunReport(model,ds.Tables[0],(ReportParameters)null);
						break;
					}
				default:
					throw new InvalidReportModelException();
			}
			PreviewViewModel previewViewModel = new PreviewViewModel (model.ReportSettings,exportRunner.Pages);
			viewer.SetBinding(previewViewModel);
		}
		
		
		protected override void SaveToPrimary()
		{
//			throw new NotImplementedException();
		}
		
		
		public override object Control {
			get {
				return viewer;
			}
		}
	}
}
