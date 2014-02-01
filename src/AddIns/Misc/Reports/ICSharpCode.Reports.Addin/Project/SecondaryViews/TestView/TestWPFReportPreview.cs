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
using System.Collections.ObjectModel;
using System.Xml;

using ICSharpCode.Reporting;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;
using ICSharpCode.Reporting.WpfReportViewer;
using ICSharpCode.Reporting.Xml;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.Reports.Addin.SecondaryViews
{
	/// <summary>
	/// Description of WPFReportPreview.
	/// </summary>
	public class TestWPFReportPreview: AbstractSecondaryViewContent
	{
		readonly ReportDesignerLoader designerLoader;
		
		IWpfReportViewer viewer;
		
//		IExportRunner exportRunner = new ExportRunner();
		
		public TestWPFReportPreview(ReportDesignerLoader loader,IViewContent content):base(content)
		{
			this.designerLoader = loader;
			viewer = new ICSharpCode.Reporting.WpfReportViewer.WpfReportViewer();
			base.TabPageText = "TestWpf View";
			Pages = new Collection<ExportPage>();
		}
		
		
		public Collection<ExportPage> Pages{get;private set;}
		
		
		protected override void LoadFromPrimary()
		{
			Pages.Clear();
			var xmDoc = designerLoader.CreateXmlModel();
			var modulLoader = new ModelLoader();
			var reportModel = (ReportModel)modulLoader.Load(xmDoc.DocumentElement);
			
			var reportingFactory = new ReportingFactory();
			var reportCreator = reportingFactory.ReportCreator(reportModel);
			var previewViewModel = new PreviewViewModel (reportModel.ReportSettings,reportCreator.Pages);
	reportCreator.BuildExportList();		
			var p = new PreviewViewModel (reportModel.ReportSettings,reportCreator.Pages);
			viewer.SetBinding(previewViewModel);
			//Missing
//			var reportCreator = reportingFactory.ReportCreator(model);
//			if (reportCreator == null){
//				SD.MessageService.ShowWarning(String.Format("Cannot run {0} from Designer",
//				                                            GlobalEnums.PushPullModel.PushData.ToString()));
//				return;
//			}
//			reportCreator.BuildExportList();
//			
//			PreviewViewModel previewViewModel = new PreviewViewModel (model.ReportSettings,reportCreator.Pages);
//			viewer.SetBinding(previewViewModel);
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
