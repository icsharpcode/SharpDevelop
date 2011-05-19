/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 03.05.2011
 * Time: 19:34
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Data;
using System.Drawing;
using System.Windows.Documents;

using ICSharpCode.Reports.Addin.Commands;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.BaseClasses;
using ICSharpCode.Reports.Core.Exporter;
using ICSharpCode.Reports.Core.Exporter.ExportRenderer;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Reports.Core.WpfReportViewer;

namespace ICSharpCode.Reports.Addin.Project.WPF
{
	/// <summary>
	/// Description of WPFReportPreview.
	/// </summary>
	public class WPFReportPreview: AbstractSecondaryViewContent
	{
		ReportDesignerLoader designerLoader;
		
		//DocumentViewer viewer = new DocumentViewer();
		IWpfReportViewer viewer = new WpfReportViewer();
		
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
			var c = new CollectParametersCommand(model);
			c.Run();
			switch (model.DataModel)
			{
					case GlobalEnums.PushPullModel.FormSheet : {
						viewer.RunReport(model,(ReportParameters)null);
						break;
					}
					case GlobalEnums.PushPullModel.PullData:{
						viewer.RunReport(model,(ReportParameters)null);
						break;
					}
				case GlobalEnums.PushPullModel.PushData:
					{
						DataSetFromXsdCommand cmd = new DataSetFromXsdCommand();
						cmd.Run();
						System.Data.DataSet ds = cmd.DataSet;
						viewer.RunReport(model,ds.Tables[0],(ReportParameters)null);
						break;
					}
				default:
					throw new InvalidReportModelException();
			}

//			http://www.xs4all.nl/~wrb/Articles/Article_WPFButtonXPS_01.htm
			
			FixedDocumentRenderer renderer =  FixedDocumentRenderer.CreateInstance(model.ReportSettings,viewer.Pages);
			
			renderer.Start();
			renderer.RenderOutput();
			renderer.End();	
			
			viewer.Document = renderer.Document;
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
