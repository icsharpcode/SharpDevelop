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
using System.Drawing.Printing;

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
		int gap;
		
		PageSettings pageSettings;

		public SectionBounds(PageSettings pageSettings,bool firstpage,int gap){
			this.firstpage = firstpage;
			this.pageSettings = pageSettings;
			this.gap = gap;
		}
		
		public Rectangle MeasureReportHeader (BaseSection section,Graphics graphics) {
			if (section == null) {
				throw new ArgumentNullException("section");
			}
			
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			
			if (this.gap == 0) {
				gap = 1;
			}
			
			Point top = new Point(this.pageSettings.Margins.Left,this.pageSettings.Margins.Top);
			Size size = Size.Empty;

			if (section.Items.Count > 0) {
				section.SectionOffset = this.pageSettings.Bounds.Top;
				MeasurementService.FitSectionToItems (section,graphics);
				size = new Size(this.MarginBounds.Width,section.Size.Height + gap);
				
			} else {
				size = new Size(this.MarginBounds.Width,0);
			}
			return new Rectangle(top,size);
		}
		
		
		public void MeasurePageHeader (BaseSection section,Rectangle startAfter,Graphics graphics,int pageNr) {
			
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			
			if (pageNr == 1){
				section.SectionOffset = (int)startAfter.Top + this.gap;
			} else {
				section.SectionOffset = this.pageSettings.Margins.Top;
			}

			MeasurementService.FitSectionToItems (section,graphics);
			this.pageHeaderRectangle =  new Rectangle (startAfter.Left,
			                                             startAfter.Bottom + this.gap,
			                                             this.MarginBounds.Width,
			                                             section.Size.Height + this.gap);

		}
		
		
		public void  MeasurePageFooter (BaseSection section,Graphics graphics) {
			
			section.SectionOffset = this.pageSettings.Bounds.Height - this.pageSettings.Margins.Top - this.pageSettings.Margins.Bottom;
			MeasurementService.FitSectionToItems (section,graphics);
			this.pageFooterRectangle =  new Rectangle(this.pageSettings.Margins.Left,
			                                          section.SectionOffset,
			                                          this.MarginBounds.Width,
			                                          section.Size.Height);
		}
		
		
		public Rectangle MeasureReportFooter (BaseSection section,Graphics graphics) {
			
			MeasurementService.FitSectionToItems (section,graphics);
			return new Rectangle (this.pageSettings.Margins.Left,
			                      section.SectionOffset,
			                      this.MarginBounds.Width,
			                      section.Size.Height);                   
		}
		
		#region Properties
	
		public Rectangle MarginBounds  {
			get {
				Rectangle r = new Rectangle(this.pageSettings.Margins.Left,
				                            this.pageSettings.Margins.Top,
				                            this.pageSettings.Bounds.Width - this.pageSettings.Margins.Left - this.pageSettings.Margins.Right,
				                            this.pageSettings.Bounds.Height - this.pageSettings.Margins.Top - this.pageSettings.Margins.Bottom);
				return r;
			}
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
		
		#endregion
	}
}
