// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core.BaseClasses
{
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
		Rectangle DetailArea {get;}
		Size PageSize {get;set;}
		bool Landscape{get;}
		Point Offset {get;set;}
	}
	
	
	public class SectionBounds :ISectionBounds
	{
		Rectangle reportHeaderRectangle;
		Rectangle pageHeaderRectangle;
		Rectangle pageFooterRectangle;
		Rectangle reportFooterRectangle;
		Rectangle marginBounds;
		
		bool firstPage;
		bool landscape;
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
		
		public void CalculatePageBounds (IReportModel reportModel)
		{
			MeasureReportHeader(reportModel.ReportHeader);
			MeasurePageHeader(reportModel.PageHeader);
			MeasurePageFooter(reportModel.PageFooter);
			MeasureReportFooter(reportModel.ReportFooter);
		}
			
		
		public void MeasureReportHeader (BaseSection section)
		{
			if (section == null) {
				throw new ArgumentNullException("section");
			}
			
			Size size = Size.Empty;
			section.SectionOffset = this.printableArea.Location.Y;
			if (this.firstPage)
			{
				size = CalculateSize(section);
			} 
			else
			{
				size = new Size(this.marginBounds.Width,0);
			}
			this.reportHeaderRectangle = new Rectangle(this.printableArea.Location,size);
		}

		
		Size CalculateSize(BaseSection section)
		{
			Size size = Size.Empty;
			if (section.Items.Count > 0) {
				size = new Size(this.marginBounds.Width, section.Size.Height);
			} else {
				size = new Size(this.marginBounds.Width, 0);
			}
			return size;
		}
		
		
		
		public void MeasurePageHeader (BaseSection section)
		{
			if (section == null) {
				throw new ArgumentNullException("section");
			}
			
			section.SectionOffset = this.reportHeaderRectangle.Bottom + GlobalValues.GapBetweenContainer;
			Size s = CalculateSize(section);
			
			this.pageHeaderRectangle =  new Rectangle (this.reportHeaderRectangle.Left,
			                                           section.SectionOffset,
			                                           this.marginBounds.Width,
			                                           s.Height);
		}
		
		
		
		public void  MeasurePageFooter (IReportItem section)
		{
			if (section == null) {
				throw new ArgumentNullException("section");
			}
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
			this.reportFooterRectangle =  new Rectangle (this.printableArea.Left,
			                                             this.PageFooterRectangle.Top - section.Size.Height - GlobalValues.GapBetweenContainer,
			                                             this.marginBounds.Width,
			                                             section.Size.Height);
			section.SectionOffset = this.reportFooterRectangle.Top;
		}
		
		#endregion
		
		#region Properties
		
		public Rectangle MarginBounds
		{
			get {
				return this.marginBounds;
			}
		}
		
		
		public Rectangle ReportHeaderRectangle
		{
			get {
				return reportHeaderRectangle;
			}
		}
		
		
		public Rectangle PageHeaderRectangle
		{
			get {
				return pageHeaderRectangle;
			}
		}
		
		
		public Rectangle PageFooterRectangle
		{
			get {
				return pageFooterRectangle;
			}
		}
		
		
		public Rectangle ReportFooterRectangle
		{
			get {
				return reportFooterRectangle;
			}
			set {
				reportFooterRectangle = value;
			}
		}
		
		
		/// <summary>
		/// This rectangle starts directly after PageHeader and ends bevore PageFooter
		/// </summary>
		
		public Rectangle DetailArea
		{
			get {
				return new Rectangle (new Point (pageHeaderRectangle.X,pageHeaderRectangle.Bottom + GlobalValues.GapBetweenContainer),
				                      new Size(pageFooterRectangle.Location.X,pageFooterRectangle.Top - GlobalValues.GapBetweenContainer));
			}
		}
		
		
		public Rectangle DetailSectionRectangle {get;set;}
		
		
		public bool Landscape
		{get {return this.landscape;}}
		
		public Size PageSize {get;set;}
		
		public Point Offset {get;set;}
		
		#endregion
	}
}
