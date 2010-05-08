// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Drawing.Printing;
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
		
		
		
		private void PageEnd (ReportPageEventArgs rpea)
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
		
		
		internal override void PrintPageEnd(object sender, ReportPageEventArgs rpea) 
		{
			base.PrintPageEnd(sender,rpea);
			this.PageEnd (rpea);
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
			base.CurrentSection.SectionOffset = (int)base.SectionBounds.DetailStart.Y + GlobalValues.GapBetweenContainer;
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
			if (tableContainer != null) {
				tableContainer.DataNavigator = nav;
				tableContainer.Parent = base.CurrentSection;
				tableContainer.ExpressionEvaluatorFacade = base.ExpressionFassade;
				if (rpea.SinglePage.PageNumber == 1) {
					tableContainer.StartLayoutAt(base.Sections[2]);
				} else {
					tableContainer.StartLayoutAt(base.Sections[0]);
				}
				
				tableContainer.RenderTable(base.CurrentSection,this.SectionBounds,rpea,this.Layout);
				this.ReportDocument.DetailsDone = true;
			}
			
			else {
				
				// first element
				if (base.SinglePage.PageNumber ==1) {
					this.dataNavigator.MoveNext();
				}
				
				do {
					ISimpleContainer i = base.CurrentSection.Items[0] as ISimpleContainer;
					if (i != null) {
						nav.Fill (i.Items);
					} else {
						nav.Fill (base.CurrentSection.Items);
					}
					
					sectionRect = new Rectangle (rpea.PrintPageEventArgs.MarginBounds.Left,
					                             base.CurrentSection.SectionOffset,
					                             rpea.PrintPageEventArgs.MarginBounds.Width,
					                             base.CurrentSection.Size.Height);

					
					if (PrintHelper.IsPageFull(sectionRect,base.SectionBounds)) {
						AbstractRenderer.PageBreak(rpea);
						this.RemoveSectionEvents();
						return;
					}
					
					base.RenderItems(rpea);
					
					
					if (nav.CurrentRow < nav.Count -1) {
						if (base.CurrentSection.PageBreakAfter) {
							AbstractRenderer.PageBreak(rpea);
							this.RemoveSectionEvents();
							
							return;
						}
					}

					Rectangle r = new Rectangle (rpea.PrintPageEventArgs.MarginBounds.Left,
					                             base.CurrentSection.SectionOffset,
					                             rpea.PrintPageEventArgs.MarginBounds.Width,
					                             base.CurrentSection.Size.Height);
					
					
//					if (this.dataNavigator.CurrentRow % 2 == 0) {
//					PrintHelper.DebugRectangle(rpea.PrintPageEventArgs.Graphics,r);
//					}
					
					base.CurrentSection.SectionOffset = r.Bottom;
					rpea.LocationAfterDraw = new Point (rpea.LocationAfterDraw.X,
					                                    sectionRect.Bottom);
					
					rpea.LocationAfterDraw = new Point (rpea.LocationAfterDraw.X,
					                                    r.Bottom);
				}
				
				while (nav.MoveNext());
				this.ReportDocument.DetailsDone = true;
			}
			
			rpea.LocationAfterDraw = new Point(rpea.LocationAfterDraw.X,rpea.LocationAfterDraw.Y + 5);
			if (!PrintHelper.IsRoomForFooter (base.SectionBounds,rpea.LocationAfterDraw)) {
				AbstractRenderer.PageBreak(rpea);
			}
		}
		
		
		internal override void PrintDetail(object sender, ReportPageEventArgs rpea)
		{
			this.PrintDetailInternal (rpea,base.DataNavigator);
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


