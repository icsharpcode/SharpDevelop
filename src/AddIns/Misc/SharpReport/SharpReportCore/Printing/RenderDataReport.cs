using System;
using System.Drawing;
using System.Drawing.Printing;
using System.ComponentModel;

/// <summary>
/// Renderer for DataReports
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 13.12.2004 11:07:59
/// </remarks>
namespace SharpReportCore {
	public class RenderDataReport : AbstractDataRenderer {

//		private PointF currentPoint;
		private DataNavigator dataNavigator;

		
		public RenderDataReport(ReportModel model,DataManager dataManager):base (model,dataManager){
//			base.DataManager.GroupChanged += new EventHandler<GroupChangedEventArgs>(OnGroupChanged);
//			base.DataManager.GroupChanging += new EventHandler <EventArgs> (OnGroupChanging);

			
		}
		

		void OnGroupChanged (object sender,GroupChangedEventArgs e) {
			
			System.Console.WriteLine("OnGroupChanged");
		}
		
		void OnGroupChanging (object sender, EventArgs e) {
			
			System.Console.WriteLine("OnGroupChanging");
		}
		
		private void OnListChanged (object sender,System.ComponentModel.ListChangedEventArgs e) {
//			System.Console.WriteLine("List Changed sender <{0}> reason <{1}>",
//			                         sender.ToString(),
//			                         e.ListChangedType);
		}
		
		#region overrides
		
		#region Draw the different report Sections
		
		private void DoReportHeader (ReportPageEventArgs rpea){			
			base.RenderSection (rpea);
			base.DoItems(rpea);
		}
		
		private void DoPageHeader (ReportPageEventArgs rpea){
			this.CurrentSection.SectionOffset = base.SectionBounds.PageHeaderRectangle.Location.Y;
			base.RenderSection (rpea);
			base.DoItems(rpea);
		}
		
		
		
		private void DoPageEnd (ReportPageEventArgs rpea){
			this.CurrentSection.SectionOffset = base.SectionBounds.PageFooterRectangle.Location.Y;
			base.RenderSection (rpea);
			base.DoItems(rpea);
		}
		
		private void  DoReportFooter (ReportPageEventArgs rpea){
			this.CurrentSection.SectionOffset = (int)rpea.LocationAfterDraw.Y;
			base.RenderSection (rpea);
			base.DoItems(rpea);
		}
		
		#endregion

		#region test
		
		protected override void PrintReportHeader (object sender, ReportPageEventArgs rpea) {
			base.PrintReportHeader (sender,rpea);
			DoReportHeader (rpea);
			base.RemoveSectionEvents();
		}
		
		protected override void PrintPageHeader (object sender, ReportPageEventArgs rpea) {
			base.PrintPageHeader (sender,rpea);
			DoPageHeader(rpea);
			base.RemoveSectionEvents();
		}
		
		protected override void PrintPageEnd(object sender, ReportPageEventArgs rpea) {
//			System.Console.WriteLine("DataRenderer:PrintPageEnd");
			base.PrintPageEnd(sender,rpea);
			this.DoPageEnd (rpea);
			base.RemoveSectionEvents();
			
		}
		
		protected override void PrintReportFooter(object sender, ReportPageEventArgs rpea){
//			DebugFooterRectangle(rpea);
			this.CurrentSection.SectionOffset = (int)rpea.LocationAfterDraw.Y;
			base.PrintReportFooter(sender, rpea);
			this.DoReportFooter(rpea);
			base.RemoveSectionEvents();
		}
		
		protected override void ReportEnd(object sender, PrintEventArgs e){
//			System.Console.WriteLine("DataRenderer:ReportEnd");
			base.ReportEnd(sender, e);
		}
		
		#endregion
		
		
		#region overrides
		
		
		protected override void ReportQueryPage(object sender, QueryPageSettingsEventArgs qpea) {
			base.ReportQueryPage (sender,qpea);
		}
		
		protected override void ReportBegin(object sender, PrintEventArgs pea) {

			base.ReportBegin (sender,pea);
			base.DataManager.ListChanged += new EventHandler<ListChangedEventArgs> (OnListChanged);
			dataNavigator = base.DataManager.GetNavigator;
			dataNavigator.ListChanged += new EventHandler<ListChangedEventArgs> (OnListChanged);
			dataNavigator.Reset();
			base.DataNavigator = dataNavigator;
		}
		
		
		
		protected override void BodyStart(object sender, ReportPageEventArgs rpea) {
			base.BodyStart (sender,rpea);
//			System.Console.WriteLine("BodyStart with {0}",this.dataNavigator.Count);
			
			if (this.dataNavigator.Count == 0){
				this.ReportDocument.ReportHasData = false;
				PrintNoDataMessage(rpea.PrintPageEventArgs);
			
			}	
			base.CurrentSection.SectionOffset = (int)base.SectionBounds.DetailStart.Y + AbstractRenderer.Gap;
		}
		
		
		protected override void PrintDetail(object sender, ReportPageEventArgs rpea){
			Rectangle sectionRect;
			bool firstOnPage = true;
//			System.Console.WriteLine("PrintDetail");
			base.PrintDetail(sender, rpea);
		
			// no loop if there is no data
			if (! this.dataNavigator.HasMoreData ) {
				rpea.PrintPageEventArgs.HasMorePages = false;
				return;
			}
			
			// first element
			if (this.ReportDocument.PageNumber ==1) {
				this.dataNavigator.MoveNext();
			}

			do {
				this.dataNavigator.Fill (base.CurrentSection.Items);
				base.RenderSection (rpea);
				
				if (!firstOnPage) {
					base.CurrentSection.SectionOffset = base.CurrentSection.SectionOffset + base.CurrentSection.Size.Height  + 2 * AbstractRenderer.Gap;
					
				}
				
				MeasurementService.FitSectionToItems (base.CurrentSection,rpea.PrintPageEventArgs.Graphics);
				
				sectionRect = new Rectangle (rpea.PrintPageEventArgs.MarginBounds.Left,
				                             base.CurrentSection.SectionOffset,
				                             rpea.PrintPageEventArgs.MarginBounds.Width,
				                             base.CurrentSection.Size.Height);
				
				if (!base.SectionBounds.DetailArea.Contains(sectionRect)) {
					AbstractRenderer.PageBreak(rpea);
					System.Console.WriteLine("DataRenderer:RemoveEvents reason <PageBreak>");
					this.RemoveSectionEvents();
					return;
				}
				
				base.DoItems(rpea);
				firstOnPage = false;

				if (this.dataNavigator.CurrentRow < this.dataNavigator.Count -1) {
					if (base.CurrentSection.PageBreakAfter) {
						AbstractRenderer.PageBreak(rpea);
						System.Console.WriteLine("DataRenderer:RemoveEvents reason <PageBreakAfter>");
						this.RemoveSectionEvents();
			
						return;
					}
				}
			}
			while (this.dataNavigator.MoveNext());

			this.ReportDocument.DetailsDone = true;
			
			// test for reportfooter
			if (!base.IsRoomForFooter (rpea.LocationAfterDraw)) {
				AbstractRenderer.PageBreak(rpea);
			}

		}
		
		protected override void BodyEnd(object sender, ReportPageEventArgs rpea) {
			base.BodyEnd (sender,rpea);
			System.Console.WriteLine("\tRemoveEvents reason <finish>");
			base.RemoveSectionEvents();

			rpea.PrintPageEventArgs.HasMorePages = false;
		}
		
		
		
		
		#endregion
		
		public override string ToString() {
			base.ToString();
			return "RenderDataReport";
		}
		#endregion

		
	}
}
