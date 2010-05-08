// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;

namespace ICSharpCode.Reports.Core{
	/// <summary>
	/// Description of Page.
	/// </summary>
	
	public interface ISectionBounds
	{
		Rectangle MarginBounds {get;}
		Rectangle ReportHeaderRectangle{get;} 
		Rectangle PageHeaderRectangle {get;}
		Rectangle PageFooterRectangle {get;}
		Rectangle ReportFooterRectangle {get;set;}
		Rectangle DetailSectionRectangle{get;set;}
		Point DetailStart {get;}
		Point DetailEnds {get;}
		Rectangle DetailArea {get;}
		Size PageSize {get;set;}
		int Gap {get;}
		bool Landscape{get;}
	}
	
	
	public class SectionBounds :ISectionBounds
	{
		Rectangle reportHeaderRectangle;
		Rectangle pageHeaderRectangle;
		Rectangle pageFooterRectangle;
		Rectangle reportFooterRectangle;
		Rectangle marginBounds;
		Rectangle detailArea;
		
		
		bool firstPage;
		bool landscape;
		int gap = 1;
		Rectangle printableArea;
		
		#region Constructor
	
		
		public SectionBounds (ReportSettings reportSettings,bool firstPage)
		{
			if (reportSettings == null) {
				throw new ArgumentNullException("reportSettings");
			}
			
			this.firstPage = firstPage;
			this.landscape = reportSettings.Landscape;
			this.PageSize = reportSettings.PageSize;
			
			this.printableArea = new Rectangle(reportSettings.LeftMargin,reportSettings.TopMargin,
			                                   reportSettings.PageSize.Width - reportSettings.RightMargin,
			                                   reportSettings.PageSize.Height - reportSettings.BottomMargin);
		
			this.marginBounds = new Rectangle(reportSettings.LeftMargin,
			                                  reportSettings.TopMargin,
			                                  reportSettings.PageSize.Width - reportSettings.LeftMargin - reportSettings.RightMargin,
			                                  reportSettings.PageSize.Height - reportSettings.TopMargin - reportSettings.BottomMargin);
		}
		
		#endregion
		
		#region Measurement
		
		public void MeasureReportHeader (BaseSection section)
		{
			if (section == null) {
				throw new ArgumentNullException("section");
			}
			
			Size size = Size.Empty;
			section.SectionOffset = this.printableArea.Location.Y;
			if (this.firstPage) {
				if (section.Items.Count > 0) {
					size = new Size(this.marginBounds.Width,section.Size.Height + gap);
				} else {
					size = new Size(this.marginBounds.Width,0);
				}
			} else {
				size = new Size(this.marginBounds.Width,0);
			}
			this.reportHeaderRectangle = new Rectangle(this.printableArea.Location,size);
		}
		
		
		public void MeasurePageHeader (IReportItem section)
		{
			if (section == null) {
				throw new ArgumentNullException("section");
			}
			
			section.SectionOffset = this.reportHeaderRectangle.Bottom + this.gap;
			this.pageHeaderRectangle =  new Rectangle (this.reportHeaderRectangle.Left,
			                                           section.SectionOffset,
			                                           this.marginBounds.Width,
			                                           section.Size.Height + this.gap);
		}
		
		
		//Test
		public void  MeasurePageFooter (IReportItem section) 
		{
			if (section == null) {
				throw new ArgumentNullException("section");
			}
			/*
			section.SectionOffset = this.printableArea.Bottom - section.Size.Height;
			this.pageFooterRectangle =  new Rectangle(this.printableArea.Location.X,
			                                          section.SectionOffset,
			                                          this.marginBounds.Width,
			                                          section.Size.Height);
			*/
			this.pageFooterRectangle =  new Rectangle(this.printableArea.Location.X,
			                                          this.marginBounds.Bottom  - section.Size.Height,
			                                          this.marginBounds.Width,
			                                          section.Size.Height);
			                                         
		}
		
		//Test
		public void MeasureReportFooter (IReportItem section)
		{
			if (section == null) {
				throw new ArgumentNullException("section");
			}
			
			// The reportFooter is set On Top of PageFooter
			this.reportFooterRectangle =  new Rectangle (this.printableArea.Left,
			                                             this.PageFooterRectangle.Top - section.Size.Height - this.gap,
			                                             this.marginBounds.Width,
			                                             section.Size.Height);
			section.SectionOffset = this.reportFooterRectangle.Top;
		}
		
		
		//Test
		public void MeasureDetailArea ()
		{
			this.detailArea =  new Rectangle (this.DetailStart.X,
			                                 this.DetailStart.Y,
			                                 this.pageHeaderRectangle.Width,
			                                 (this.pageFooterRectangle.Top -1) - (this.pageHeaderRectangle.Bottom + 1));
		}
		
		
		#endregion
		
		#region Properties
		
		public Rectangle MarginBounds
		{
			get {
				return this.marginBounds;
			}
		}
		
		
		//Test
		public Rectangle ReportHeaderRectangle
		{
			get {
				return reportHeaderRectangle;
			}
		}
		
		
		//Test
		public Rectangle PageHeaderRectangle 
		{
			get {
				return pageHeaderRectangle;
			}
		}
		
		
		//Test
		public Rectangle PageFooterRectangle 
		{
			get {
				return pageFooterRectangle;
			}
		}
		
		
		//Test
		public Rectangle ReportFooterRectangle 
		{
			get {
				return reportFooterRectangle;
			}
			set {
				reportFooterRectangle = value;
			}
		}
		
		//Test
		public Point DetailStart
		{
			get {
				return new Point(this.pageHeaderRectangle.Left,
				                 this.pageHeaderRectangle.Bottom + this.Gap);
			}
		}
		
		
		//Test
		public Point DetailEnds
		{
			get {
				return new Point(this.pageFooterRectangle.Left,this.pageFooterRectangle.Top - this.Gap);
			}
		}
		
		
		/// <summary>
		/// This rectangle starts directly after PageHeader and ends bevore PageFooter
		/// </summary>
		
		//Test
		public Rectangle DetailArea
		{
			get {
				return this.detailArea;
			}
		}
		
		/// <summary>
		/// gap between two Sections
		/// </summary>
		
		
		public int Gap
		{
			get {
				return gap;
			}
		}
		
		public Rectangle DetailSectionRectangle {get;set;}
		
		
		public bool Landscape
		{get {return this.landscape;}}
		
		public Size PageSize {get;set;}
		
		#endregion
	}
}
