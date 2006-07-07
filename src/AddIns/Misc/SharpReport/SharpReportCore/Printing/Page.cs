/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 30.06.2006
 * Time: 17:12
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;

namespace SharpReportCore{
	/// <summary>
	/// Description of Page.
	/// </summary>
	public class Page{
		Rectangle reportHeaderRectangle;
		Rectangle pageHeaderRectangle;
		Rectangle detailRectangle;
		Rectangle pageFooterRectangle;
		Rectangle reportFooterRectangle;
		
		bool firstpage;
		
		public Page(bool firstPage){
			this.firstpage = firstPage;
		}
		
		
		public Rectangle ReportHeaderRectangle {
			get {
				return reportHeaderRectangle;
			}
			set {
				reportHeaderRectangle = value;
			}
		}
		
		public Rectangle PageHeaderRectangle {
			get {
				return pageHeaderRectangle;
			}
			set {
				pageHeaderRectangle = value;
			}
		}
		
		public Rectangle DetailRectangle {
			get {
				return detailRectangle;
			}
			set {
				detailRectangle = value;
			}
		}
		
		
		public Rectangle PageFooterRectangle {
			get {
				return pageFooterRectangle;
			}
			set {
				pageFooterRectangle = value;
			}
		}
		
		public Rectangle ReportFooterRectangle {
			get {
				return reportFooterRectangle;
			}
			set {
				reportFooterRectangle = value;
			}
		}
		
		public Point DetailStart{
			get {
				return new Point(this.pageHeaderRectangle.Left,
				                 this.pageHeaderRectangle.Bottom);
			}
		}
		
		public Point DetailEnds{
			get {
			return new Point(this.pageFooterRectangle.Left,this.pageFooterRectangle.Top);
			}
		}
		/*
		/// <summary>
		/// Calculates the rectangle wich can be used by Detail
		/// </summary>
		/// <returns></returns>
		
		protected Rectangle old_DetailRectangle (ReportPageEventArgs rpea) {
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}
			
			sectionInUse = Convert.ToInt16(GlobalEnums.enmSection.ReportDetail,
			                               CultureInfo.InvariantCulture);
			Rectangle rect = new Rectangle (rpea.PrintPageEventArgs.MarginBounds.Left,
			                               this.page.DetailStart.Y ,
			                                rpea.PrintPageEventArgs.MarginBounds.Width,
			                                page.DetailEnds.Y - this.page.DetailStart.Y - (3 * gap));
			System.Console.WriteLine("Page DetRec {0} base DetRec {1}",page.DetailArea,rect);
			return rect;
		}
		*/
			/*
		protected int CalculateDrawAreaHeight(ReportPageEventArgs rpea){
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}
			int to = rpea.PrintPageEventArgs.MarginBounds.Height ;
			if (this.reportDocument.PageNumber == 1){
				to -= sections[Convert.ToInt16(GlobalEnums.enmSection.ReportHeader,CultureInfo.InvariantCulture)].Size.Height;
			}
			
			to -= sections[Convert.ToInt16(GlobalEnums.enmSection.ReportPageHeader,CultureInfo.InvariantCulture)].Size.Height;
			
			to -= sections[Convert.ToInt16(GlobalEnums.enmSection.ReportPageFooter,CultureInfo.InvariantCulture)].Size.Height;
			return to;
		}
		*/
		/// <summary>
		/// This rectangle starts directly after PageHeader and ends bevore PageFooter
		/// </summary>
		public Rectangle DetailArea {
			get {
				return new Rectangle (this.DetailStart.X,
				                       this.DetailStart.Y,
				                      this.pageHeaderRectangle.Width,
				                      (this.pageFooterRectangle.Top -1) - (this.pageHeaderRectangle.Bottom + 1));
			}
		
		}
		
	}
}
