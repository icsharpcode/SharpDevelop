/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 05.05.2011
 * Time: 20:27
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.Reports.Core.WPF;

namespace ICSharpCode.Reports.Core.Exporter.ExportRenderer
{
	/// <summary>
	/// Description of FixedDocumentRenderer.
	/// </summary>
	public class FixedDocumentRenderer:BaseExportRenderer
	{
		private PagesCollection pages;
		private ReportSettings reportSettings;
		private FixedDocument document ;
		private FixedDocumentCreator docCreator;
	
//		http://www.nbdtech.com/Blog/archive/2009/04/20/wpf-printing-part-2-the-fixed-document.aspx
		
	
		public static FixedDocumentRenderer CreateInstance (ReportSettings reportSettings,PagesCollection pages)
		{
			var instance = new FixedDocumentRenderer(reportSettings,pages);
			return instance;
		}
		
		private FixedDocumentRenderer(ReportSettings reportSettings,PagesCollection pages):base(pages)
		{
			this.pages  = pages;
			this.reportSettings = reportSettings;
			this.docCreator = new FixedDocumentCreator();
			
		}
		
		
		
		#region overrides
		
		public override void Start()
		{
			base.Start();
			Console.WriteLine("FixedDocumentRenderer - Start");
			document = new FixedDocument();
			

			// point 	 595x842
			
			// 827/1169

//			A4 paper is 210mm x 297mm 
//8.2 inch x 11.6 inch 
//1240 px x 1754 px
/*
		iTextSharp uses a default of 72 pixels per inch. 
			792 would be 11", or the height of a standard Letter size paper." +
			595 would be 8.264",
		which is the standard width of A4 size paper.
			Using 595 x 792 as the page size would be a cheap and dirty way 
			to ensure that you could print on either A4 or Letter 
			without anything getting cut off. –
*/			
			PrintDialog printDialog = new PrintDialog();
			var w = printDialog.PrintableAreaHeight;
			var h = printDialog.PrintableAreaWidth;
			Console.WriteLine(new System.Windows.Size(w,h));

			docCreator.PageSize = new System.Windows.Size(reportSettings.PageSize.Width,reportSettings.PageSize.Height);
			document.DocumentPaginator.PageSize = docCreator.PageSize;
			
		}
		
	
		
		public override void RenderOutput()
		{
			base.RenderOutput();
			Console.WriteLine("FixedDocumentRenderer - RenderOutput");
			
			foreach (var page in pages)
			{
				FixedPage fixedPage = docCreator.CreatePage (page);
				docCreator.ArrangePage(document.DocumentPaginator.PageSize,fixedPage);
				AddPageToDocument(document,fixedPage);
			}
			
//http://www.ericsink.com/wpf3d/B_Printing.html
//			http://www.switchonthecode.com/tutorials/wpf-printing-part-2-pagination
		
			
//			http://stackoverflow.com/questions/3671724/wpf-flowdocument-page-break-positioning
//		http://wpf.2000things.com/tag/drawingvisual/
//http://wpf.2000things.com/2011/03/25/256-use-a-fixeddocument-to-display-content-at-fixed-locations/

//http://wpf.2000things.com/2011/03/25/256-use-a-fixeddocument-to-display-content-at-fixed-locations/
//http://www.neodynamic.com/ND/FaqsTipsTricks.aspx?tabid=66&prodid=0&sid=99

//http://www.eggheadcafe.com/tutorials/aspnet/9cbb4841-8677-49e9-a3a8-46031e699b2e/wpf-printing-and-print-pr.aspx
//http://www.eggheadcafe.com/tutorials/aspnet/9cbb4841-8677-49e9-a3a8-46031e699b2e/wpf-printing-and-print-pr.aspx
//
//http://www.eggheadcafe.com/tutorials/aspnet/22ac97f3-4a3d-4fee-a411-e456f77f6a90/wpf-report-engine-part-3.aspx
			
			Document = document;
		}
		
	
		
		void AddPageToDocument(FixedDocument fixedDocument,FixedPage page)
		{
			PageContent pageContent = new PageContent();
			((IAddChild)pageContent).AddChild(page);
			fixedDocument.Pages.Add(pageContent);
		}
		
		
		public override void End()
		{
			base.End();
			Console.WriteLine("FixedDocumentRenderer - End");		
		}
			
		public IDocumentPaginatorSource Document {get;private set;}
		
		#endregion
	}
}
