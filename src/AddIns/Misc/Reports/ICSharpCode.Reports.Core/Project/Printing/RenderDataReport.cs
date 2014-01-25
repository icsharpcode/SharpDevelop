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
using System.Drawing.Printing;

using ICSharpCode.Reports.Core.BaseClasses.Printing;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.Reports.Core.Interfaces;

/// <summary>
/// Renderer for DataReports
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 13.12.2004 11:07:59
/// </remarks>
namespace ICSharpCode.Reports.Core {
	
	
	
	public class RenderDataReport : AbstractDataRenderer {

		private DataNavigator dataNavigator;

		
		public RenderDataReport(IReportModel model,
		                        IDataManager dataManager,
		                        ReportDocument reportDocument,
		                        ILayouter layout):base (model,dataManager,reportDocument,layout)
		{
		}
		
	
		private void ReportHeader (ReportPageEventArgs rpea)
		{
			base.RenderSection (rpea);
			base.RenderItems(rpea);
		}
		
		
		private void PageHeader (ReportPageEventArgs rpea)
		{
			base.RenderSection (rpea);
			base.RenderItems(rpea);
		}
		
		
		
		private void PageFooter (ReportPageEventArgs rpea)
		{
			this.CurrentSection.SectionOffset = base.SectionBounds.PageFooterRectangle.Location.Y;
			base.RenderSection (rpea);
			base.RenderItems(rpea);
		}
		
		
		private void  ReportFooter (ReportPageEventArgs rpea)
		{
			this.CurrentSection.SectionOffset = (int)rpea.LocationAfterDraw.Y;
			base.RenderSection (rpea);
			base.RenderItems(rpea);
		}
	
		#region overrides
		
		internal override void PrintReportHeader (object sender, ReportPageEventArgs rpea) 
		{
			base.PrintReportHeader (sender,rpea);
			if (base.CurrentSection.Items.Count > 0) {
				this.ReportHeader (rpea);
			}
			base.RemoveSectionEvents();
		}
		
		
		internal override void PrintPageHeader (object sender, ReportPageEventArgs rpea)
		{
			base.PrintPageHeader (sender,rpea);
			if (base.CurrentSection.Items.Count > 0) {
				this.PageHeader(rpea);
				this.SectionBounds.MeasurePageHeader(base.CurrentSection);
			}
			base.RemoveSectionEvents();
		}
		
		
		internal override void PrintPageFooter(object sender, ReportPageEventArgs rpea) 
		{
			base.PrintPageFooter(sender,rpea);
			this.PageFooter (rpea);
			base.RemoveSectionEvents();
		}
		
		
		internal override void PrintReportFooter(object sender, ReportPageEventArgs rpea)
		{
			this.CurrentSection.SectionOffset = (int)rpea.LocationAfterDraw.Y;
			base.PrintReportFooter(sender, rpea);
			this.ReportFooter(rpea);
			base.RemoveSectionEvents();
		}
		
		
		internal override void ReportEnd(object sender, PrintEventArgs e)
		{
			base.ReportEnd(sender, e);
		}
		
		
		internal override void ReportQueryPage(object sender, QueryPageSettingsEventArgs qpea) 
		{
			qpea.PageSettings.Landscape = base.ReportSettings.Landscape;
			base.ReportQueryPage (sender,qpea);
		}
		
		
		internal override void ReportBegin(object sender, PrintEventArgs pea) 
		{
			base.ReportBegin (sender,pea);
			this.dataNavigator = base.DataNavigator;
			dataNavigator.Reset();
			base.DataNavigator = dataNavigator;
		}
		
		
		
		internal override void BodyStart(object sender, ReportPageEventArgs rpea)
		{
			base.BodyStart (sender,rpea);
			if (this.dataNavigator.Count == 0){
				this.ReportDocument.ReportHasData = false;
				PrintNoDataMessage(rpea.PrintPageEventArgs);
			}	
			base.CurrentSection.SectionOffset = base.SectionBounds.DetailArea.Top + GlobalValues.GapBetweenContainer;
		}
		
		
		
		internal override void PrintDetail(object sender, ReportPageEventArgs rpea)
		{
			base.PrintDetail(sender,rpea);
			this.PrintDetailInternal (rpea,base.DataNavigator);
		}
		
		
		private void PrintDetailInternal (ReportPageEventArgs rpea,DataNavigator nav)
		{
			Rectangle sectionRect;
			
			base.PrintDetail(null, rpea);
			
			// no loop if there is no data
			if (! nav.HasMoreData ) {
				rpea.PrintPageEventArgs.HasMorePages = false;
				return;
			}
		
			ITableContainer tableContainer = base.CurrentSection.Items[0] as ITableContainer;
			
			// branch to render table's etc
			if (tableContainer != null)
			{
				
				tableContainer.DataNavigator = nav;
				tableContainer.Parent = base.CurrentSection;
				if (rpea.SinglePage.PageNumber == 1) {
					tableContainer.StartLayoutAt(base.Sections[2]);
				} else {
					tableContainer.StartLayoutAt(base.Sections[0]);
				}
				
				
				base.RenderTable (base.CurrentSection,tableContainer,rpea);
				
				this.ReportDocument.DetailsDone = true;
			}
			
			else
			{
				
				// first element
				if (base.SinglePage.PageNumber ==1) {
					this.dataNavigator.MoveNext();
				}
				
				do {
					ISimpleContainer simpleContainer = base.CurrentSection.Items[0] as ISimpleContainer;
					
					if (simpleContainer != null) {
						nav.Fill (simpleContainer.Items);
					} else {
						nav.Fill (base.CurrentSection.Items);
					}
					
					sectionRect = new Rectangle (rpea.PrintPageEventArgs.MarginBounds.Left,
					                             base.CurrentSection.SectionOffset,
					                             rpea.PrintPageEventArgs.MarginBounds.Width,
					                             base.CurrentSection.Size.Height);

					
					if (PrintHelper.IsPageFull(sectionRect,base.SectionBounds)) {
						PerformPageBreak(rpea);
						return;
					}
					
					Point currentPosition = base.RenderItems(rpea);
					
					
					if (nav.CurrentRow < nav.Count -1) {
						if (base.CurrentSection.PageBreakAfter) {
							PerformPageBreak(rpea);
							return;
						}
					}

					base.CurrentSection.SectionOffset = currentPosition.Y;
					
					rpea.LocationAfterDraw = new Point (rpea.LocationAfterDraw.X,
					                                    currentPosition.Y);
				}
				
				while (nav.MoveNext());
				this.ReportDocument.DetailsDone = true;
			}
			
			rpea.LocationAfterDraw = new Point(rpea.LocationAfterDraw.X,rpea.LocationAfterDraw.Y + 5);
			if (!PrintHelper.IsRoomForFooter (base.SectionBounds,rpea.LocationAfterDraw)) {
				AbstractRenderer.PageBreak(rpea);
			}
		}
		
		
		private void PerformPageBreak (ReportPageEventArgs rpea)
		{
			AbstractRenderer.PageBreak(rpea);
						this.RemoveSectionEvents();
		}
		
		internal override void BodyEnd(object sender, ReportPageEventArgs rpea) 
		{
			base.BodyEnd (sender,rpea);
			base.RemoveSectionEvents();
			rpea.PrintPageEventArgs.HasMorePages = false;
		}
		
		#endregion
	}
}
