


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
		
		public event EventHandler<ReportPageEventArgs> PrintPageBodyStart;
	
		public event EventHandler<ReportPageEventArgs> PrintPageBodyEnd;
		
		
		
		public event EventHandler<ReportPageEventArgs> RenderReportHeader;
		public event EventHandler<ReportPageEventArgs> RenderPageHeader;
		public event EventHandler<ReportPageEventArgs> RenderDetails;
		public event EventHandler<ReportPageEventArgs> RenderPageEnd;
		public event EventHandler<ReportPageEventArgs> RenderReportEnd;
		int pageNumber;
		
		public ReportDocument():base() {
			
		}
		
		
		#region overriede's
		
		protected override void OnQueryPageSettings(QueryPageSettingsEventArgs e){
			base.OnQueryPageSettings(e);
		}
		
		protected override void OnBeginPrint(PrintEventArgs e){
			base.OnBeginPrint(e);
			pageNumber = 0;
		}
		
		protected override void OnPrintPage(PrintPageEventArgs e){
			base.OnPrintPage(e);
			pageNumber ++;

			ReportPageEventArgs pea = new ReportPageEventArgs (e,pageNumber,
			                                                   false,new PointF (0,0));
			GeneratePage (pea);
			

			if (pea.PrintPageEventArgs.HasMorePages == false) {
				if (this.RenderReportEnd != null) {
					this.RenderReportEnd(this,pea);
				}
//				this.OnEndPrint (new PrintEventArgs());
			}
		}
		
		protected override void OnEndPrint(PrintEventArgs e){
			base.OnEndPrint(e);
		}
		
		#endregion
		
		private void GeneratePage (SharpReportCore.ReportPageEventArgs page) {
			
			if (this.pageNumber == 1) {
				if (this.RenderReportHeader != null) {
					this.RenderReportHeader(this,page);
				}
			}
			
			if (this.RenderPageHeader != null) {
				this.RenderPageHeader (this,page);
			}
			
			
			// print PageFooter before DetailSection
			//so it's much easyer to calculate size of DetailSection
			if (RenderPageEnd != null) {
				RenderPageEnd (this,page);
			}
			
			
			if (PrintPageBodyStart != null) {
				PrintPageBodyStart (this,page);
			}
			
			if (this.RenderDetails != null) {
				this.RenderDetails(this,page);
			}

			if (page.PrintPageEventArgs.HasMorePages == false) {
				if (PrintPageBodyEnd != null) {
					PrintPageBodyEnd (this,page);
				}
			}
			
		}
	
	
		
		#region Property's
		
		public int PageNumber {
			get {
				return pageNumber;
			}
		}
		
		#endregion
	}
}
