/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 03.05.2011
 * Time: 19:34
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Documents;

using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.BaseClasses;
using ICSharpCode.Reports.Core.Exporter;
using ICSharpCode.Reports.Core.Exporter.ExportRenderer;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Reports.Addin.Project.WPF
{
	/// <summary>
	/// Description of WPFReportPreview.
	/// </summary>
	public class WPFReportPreview: AbstractSecondaryViewContent
	{
		ReportDesignerLoader designerLoader;
		DocumentViewer viewer = new DocumentViewer();
		
		public WPFReportPreview(ReportDesignerLoader loader,IViewContent content):base(content)
		{
			this.designerLoader = loader;
			base.TabPageText = "Wpf View";
		}
		
		protected override void LoadFromPrimary()
		{
			Console.WriteLine("WPFReportPreview - LoadFromPrimary");
			ReportModel model = designerLoader.CreateRenderableModel();
			
			IReportCreator reportCreator = FormPageBuilder.CreateInstance(model);
//			reportCreator.SectionRendering += new EventHandler<SectionRenderEventArgs>(PushPrinting);
//			reportCreator.PageCreated += OnPageCreated;
			reportCreator.BuildExportList();
			
			var pages = reportCreator.Pages;
			
//			var pages = CreateTestPage();
			
			Console.WriteLine("WPFReportPreview - Create FixedDocumentRenderer");
		
			
			FixedDocumentRenderer renderer =  FixedDocumentRenderer.CreateInstance(model.ReportSettings,pages);
			
			renderer.Start();
			renderer.RenderOutput();
			renderer.End();	
//			Console.WriteLine("WPFReportPreview - document to viewer");
			IDocumentPaginatorSource document = renderer.Document;
			viewer.Document = renderer.Document;
		}
		
		
		PagesCollection CreateTestPage()
		{
			var pages = new PagesCollection();
			ExporterPage page = ExporterPage.CreateInstance(new SectionBounds(new ReportSettings(),false),1);
			page.PageNumber = 1;
			
			TextStyleDecorator decorator1 = new TextStyleDecorator()
			{
				Location = new System.Drawing.Point (10,10),
				Font = GlobalValues.DefaultFont,
				ForeColor = Color.Black,
				BackColor = Color.White
					
			};
			
			
			page.Items.Add(new ExportText (decorator1,false)
			               {
			               	Text = "hello world"
			               		
			               });
			
			
			TextStyleDecorator decorator2 = new TextStyleDecorator()
			{
				Location = new System.Drawing.Point (20,20),
				Font = GlobalValues.DefaultFont,
				ForeColor = Color.Black,
				BackColor = Color.White
					
			};
			page.Items.Add(new ExportText (decorator2,false)
			               {
			               	Text = "My First PdfPrintout"
			               		
			               });
			
			
			pages.Add(page);
			return pages;
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
