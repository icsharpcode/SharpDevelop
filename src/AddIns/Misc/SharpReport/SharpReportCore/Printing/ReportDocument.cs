


using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Printing;
	
using SharpReportCore;
/// <summary>
/// Derived from PrintDocument to have more control about printing
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 21.11.2004 14:47:20
/// </remarks>
	
namespace SharpReportCore {
	
	public class ReportDocument : PrintDocument {
		
		public event QueryPageSettingsEventHandler QueryPage;

		public event EventHandler<ReportPageEventArgs> ReportBegin;
		public event EventHandler<ReportPageEventArgs> PrintPageBegin;
		public event EventHandler<ReportPageEventArgs> PrintPageBodyStart;
		public event EventHandler<ReportPageEventArgs> PrintPageBodyEnd;
		public event EventHandler<ReportPageEventArgs> PrintPageEnd;
		public event EventHandler<ReportPageEventArgs> ReportEnd;
		
		
		int pageNr;
		
		
		public ReportDocument():base() {
			System.Console.WriteLine("ReportDocument Constructor");
			base.BeginPrint += new PrintEventHandler (ReportDocumentBeginPrint);
			
			base.PrintPage += new PrintPageEventHandler (ReportDocumentPrintPage);
			
			base.EndPrint += new PrintEventHandler (ReportDocumentEndPrint);
			base.QueryPageSettings += new QueryPageSettingsEventHandler (ReportDocumentQueryPage);
		}
		
		void GeneratePage (SharpReportCore.ReportPageEventArgs page) {		
			System.Console.WriteLine("\tGeneratePage");
			if (PrintPageBegin != null) {
				PrintPageBegin (this,page);
			}
			
			if (page.ForceNewPage == true) {
				page.PrintPageEventArgs.HasMorePages = true;
				return;
			}
			// print PageFooter before DetailSection
			//so it's much easyer to calculate size of DetailSection
			if (PrintPageEnd != null) {
				PrintPageEnd (this,page);
			}
			
			
			if (PrintPageBodyStart != null) {
				PrintPageBodyStart (this,page);
			}
			
			if (PrintPageBodyEnd != null) {
				PrintPageBodyEnd (this,page);
			}
		}
		
		#region events
		//this events are also used by PrintPreviewControl
		public  void ReportDocumentBeginPrint (object sender,PrintEventArgs e) {
			System.Console.WriteLine("\tReportDocument BeginPrint");
			pageNr = 0;
		}
		
		public void ReportDocumentQueryPage (object sender, QueryPageSettingsEventArgs e) {
//			System.Console.WriteLine("\tReportDocument QueryPage");
			if (QueryPage != null) {
				QueryPage (this,e);
			}
		}
		
		public void ReportDocumentPrintPage (object sender, PrintPageEventArgs e) {
//			System.Console.WriteLine("\tReportDocument PrintPage");
			pageNr ++;
			ReportPageEventArgs pea = new ReportPageEventArgs (e,pageNr,false,new PointF (0,0));
			
			if (pageNr == 1) {
				if (ReportBegin != null) {
					ReportBegin (this,pea);
				}
			}
			
			GeneratePage (pea);
			
			if ((pea.ForceNewPage == true) && (pea.PrintPageEventArgs.HasMorePages == true)) {
				pea.ForceNewPage = false;
			    	return;
			}
			if (pea.PrintPageEventArgs.HasMorePages == false) {
				if (ReportEnd != null) {
					ReportEnd (this,pea);
				}
			}
			
		}
		public void  ReportDocumentEndPrint (object sender,PrintEventArgs e) {
//			System.Console.WriteLine("\tReportDocument EndPrint");
			pageNr = 0;
			if (ReportEnd != null) {
				ReportEnd (this,null);
			}
			
		}
		#endregion
		
		
	}
}
