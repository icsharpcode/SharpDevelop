/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 24.03.2014
 * Time: 20:02
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Core;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.WpfReportViewer;
using ICSharpCode.Reporting.Xml;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.Reporting.Addin.DesignerBinding;

namespace ICSharpCode.Reporting.Addin.Views
{
	/// <summary>
	/// Description of the view content
	/// </summary>
	public class WpfPreview : AbstractSecondaryViewContent
	{
		ReportDesignerLoader designerLoader;
		
		ICSharpCode.Reporting.WpfReportViewer.WpfReportViewer viewer = new ICSharpCode.Reporting.WpfReportViewer.WpfReportViewer();
		
		public WpfPreview(ReportDesignerLoader loader,IViewContent content):base(content)
		{
			LoggingService.Info("Create WpfPreview");
			this.designerLoader = loader;
			TabPageText = "WpfPreview";
		}
		
		protected override void LoadFromPrimary()
		{
			LoggingService.Info("LoadFrompromary");
		
			var xml = designerLoader.SerializeModel();
			var modelLoader = new ModelLoader();
			var reportmodel = modelLoader.Load(xml.DocumentElement) as ReportModel;
			var reportingFactory = new ReportingFactory();
			var reportCreator = reportingFactory.ReportCreator(reportmodel);
			reportCreator.BuildExportList();		
			var previewViewModel = new PreviewViewModel (reportingFactory.ReportModel.ReportSettings,reportCreator.Pages);
			viewer.SetBinding(previewViewModel);
		}
		/// <summary>
		/// The <see cref="System.Windows.Forms.Control"/> representing the view
		/// </summary>
		public override object Control {
			get {
				return viewer;
			}
		}
		
		protected override void SaveToPrimary()
		{
			LoggingService.Info("WpfPreview:SaveToPrimary is not implemented");
		}
	}
	
}
