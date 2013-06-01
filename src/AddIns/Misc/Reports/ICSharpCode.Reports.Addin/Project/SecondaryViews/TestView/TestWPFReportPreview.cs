/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 03.05.2011
 * Time: 19:34
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.ObjectModel;
using System.Xml;

using ICSharpCode.Reporting;
using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.Exporter;
using ICSharpCode.Reporting.Globals;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.WpfReportViewer;
using ICSharpCode.Reporting.Xml;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

//using ICSharpCode.Reports.Addin.Commands;
//using ICSharpCode.Reports.Core;
//using ICSharpCode.Reports.Core.Exporter.ExportRenderer;
//using ICSharpCode.Reports.Core.Globals;
//using ICSharpCode.Reports.Core.WpfReportViewer;




namespace ICSharpCode.Reports.Addin.SecondaryViews
{
	/// <summary>
	/// Description of WPFReportPreview.
	/// </summary>
	public class TestWPFReportPreview: AbstractSecondaryViewContent
	{
		ReportDesignerLoader designerLoader;
		
		ICSharpCode.Reporting.WpfReportViewer.IWpfReportViewer viewer;
		
//		IExportRunner exportRunner = new ExportRunner();
		
		public TestWPFReportPreview(ReportDesignerLoader loader,IViewContent content):base(content)
		{
			this.designerLoader = loader;
			viewer = new ICSharpCode.Reporting.WpfReportViewer.WpfReportViewer();
			base.TabPageText = "TestWpf View";
			Pages = new Collection<Page>();
		}
		
		
		public Collection<Page> Pages{get;private set;}
		
		
		protected override void LoadFromPrimary()
		{
			Pages.Clear();
			var xmDoc = designerLoader.CreateXmlModel();
			var modulLoader = new ModelLoader();
			ReportModel model = (ReportModel)modulLoader.Load(xmDoc.DocumentElement);
			
			var reportingFactory = new ReportingFactory();
			var reportCreator = reportingFactory.ReportCreator(model);
			reportCreator.BuildExportList();
			
			PreviewViewModel previewViewModel = new PreviewViewModel (model.ReportSettings,reportCreator.Pages);
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
