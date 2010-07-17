// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Drawing.Printing;

namespace ICSharpCode.Reports.Core.Exporter.ExportRenderer{
	/// <summary>
	/// Description of PrintRenderer.
	/// </summary>
	public class PrintRenderer:BaseExportRenderer,IDisposable{
		PrintDocument doc;
		int currentPage;
		PrinterSettings printerSettings;
		
		#region Constructor
		/// <summary>
		/// Send's output to the StandartPrinter
		/// </summary>
		/// <param name="pages"></param>
		/// <returns>PrintRenderer</returns>
		public static PrintRenderer CreateInstance(PagesCollection pages)
		{
			if (pages == null) {
				throw new ArgumentNullException("pages");
			}
			PrintRenderer instance = new PrintRenderer(pages);
			return instance;
		}
		
		public static PrintRenderer CreateInstance(PagesCollection pages,
		                                           PrinterSettings printerSettings)
		{
			if (pages == null) {
				throw new ArgumentNullException("pages");
			}
			PrintRenderer instance = new PrintRenderer(pages);
			instance.PrinterSettings = printerSettings;
			return instance;
		}
		
		
		private PrintRenderer(PagesCollection pages):base(pages)
		{
			doc = new PrintDocument();
//			doc.QueryPageSettings += new QueryPageSettingsEventHandler(OnQueryPage);
			doc.BeginPrint += new PrintEventHandler(OnBeginPrint);
			doc.EndPrint += new PrintEventHandler(OnEndPrint);
			doc.PrintPage += new PrintPageEventHandler(OnPrintPage);
		}
		
		#endregion
		

		
		private void OnBeginPrint (object sender,PrintEventArgs e) 
		{
			this.currentPage = 0;
		}
		
		
		private void OnPrintPage (object sender,PrintPageEventArgs e) 
		{
			BaseExportRenderer.DrawItems(e.Graphics,this.Pages[currentPage].Items);
			if (this.currentPage < this.Pages.Count -1) {
				this.currentPage ++;
				e.HasMorePages = true;
				return;
			}
			e.HasMorePages = false;
		}
		
		
		private void OnEndPrint (object sender,PrintEventArgs e)
		{
		}
		
		
		public override void Start()
		{
			base.Start();
		}
		
		
		public override void RenderOutput()
		{
			base.RenderOutput();
			if (this.printerSettings != null) {
				doc.PrinterSettings = this.printerSettings;
			}
			doc.Print();
		}
		
		public override void End()
		{
			base.End();
		}
		
		
		internal PrinterSettings PrinterSettings 
		{
			set { printerSettings = value; }
		}
		
		#region IDisposable
		
		public  void Dispose()
		{
			Dispose(true);
		}
		
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				// free managed resources
				if (this.doc != null)
				{
					doc.Dispose();
					doc = null;
				}
			}
		}

		#endregion
		
	}
}
