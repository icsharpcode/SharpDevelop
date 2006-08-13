


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
		///<summary>
		/// Fired just before the DetailSection ist printed
		/// </summary>
		/// 
		public event EventHandler<ReportPageEventArgs> BodyStart;
		
		/// <summary>
		/// Fired if all Details 8data) are printed
		/// </summary>
		public event EventHandler<ReportPageEventArgs> BodyEnd;
		
		
		
		public event EventHandler<ReportPageEventArgs> RenderReportHeader;
		public event EventHandler<ReportPageEventArgs> RenderPageHeader;
		public event EventHandler<ReportPageEventArgs> RenderDetails;
		public event EventHandler<ReportPageEventArgs> RenderPageEnd;
		public event EventHandler<ReportPageEventArgs> RenderReportEnd;
		
		int pageNumber;
		bool detailsDone;
		bool reportHasData;
		public ReportDocument():base() {
			this.reportHasData = true;
		}
		
		
		#region Overrides
		
		protected override void OnQueryPageSettings(QueryPageSettingsEventArgs e){
			base.OnQueryPageSettings(e);
		}
		
		protected override void OnBeginPrint(PrintEventArgs e){
			base.OnBeginPrint(e);
			pageNumber = 0;
		}
		
		protected override void OnPrintPage(PrintPageEventArgs e){
			pageNumber ++;
			base.OnPrintPage(e);
			
			
			ReportPageEventArgs pea = new ReportPageEventArgs (e,pageNumber,
			                                                   false,Point.Empty);
			
			
			// ReportHeader only on first page
			if (this.pageNumber == 1) {
				if (this.RenderReportHeader != null) {
					this.RenderReportHeader(this,pea);
				}
			}
			
			// allway draw PageHeader
			
			if (this.RenderPageHeader != null) {
				this.RenderPageHeader (this,pea);
			}
			
			// Details

			if (BodyStart != null) {
				BodyStart (this,pea);
			}
			System.Console.WriteLine("ReportDocumnet:'NoData' = {0}",this.reportHasData);
			
			if (this.reportHasData == true) {
				if (this.RenderDetails != null) {
					this.RenderDetails(this,pea);
				}
				
				
				if (pea.ForceNewPage) {
					if (RenderPageEnd != null) {
						RenderPageEnd (this,pea);
						return;
					}
					pea.ForceNewPage = false;
					return;
				}
				
				if (BodyEnd != null) {
					BodyEnd (this,pea);
				}
			}
			
			
			// ReportFooter
			if (this.detailsDone) {
				this.RenderReportEnd(this,pea);
				
				if (pea.ForceNewPage) {
					e.HasMorePages = true;
					pea.ForceNewPage = false;

				}
			}

			
			//PageFooter
			if (RenderPageEnd != null) {
				RenderPageEnd (this,pea);
				e.HasMorePages = false;
				return;
			}
		}
		
		
		protected override void OnEndPrint(PrintEventArgs e){
			base.OnEndPrint(e);
		}
		
		#endregion
		
		
		#region Property's
		
		public int PageNumber {
			get {
				return pageNumber;
			}
		}
		
		public bool DetailsDone {
			set {
				detailsDone = value;
			}
		}
		
		public bool ReportHasData {
			set {
				reportHasData = value;
				this.detailsDone = true;
			}
		}
		
		#endregion
	}
}
