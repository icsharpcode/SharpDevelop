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
	public class SectionBounds{
		Rectangle reportHeaderRectangle;
		Rectangle pageHeaderRectangle;
		Rectangle detailRectangle;
		Rectangle pageFooterRectangle;
		Rectangle reportFooterRectangle;
		
		bool firstpage;
		
		public SectionBounds(bool firstPage){
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
		
		public bool Firstpage {
			get {
				return firstpage;
			}
		}
		
	}
}
