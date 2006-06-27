
//
// SharpDevelop ReportEditor
//
// Copyright (C) 2005 Peter Forstmeier
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Peter Forstmeier (Peter.Forstmeier@t-online.de)

using System;

using System.Data;
using System.Drawing;
using System.ComponentModel;
using System.Globalization;
using System.Drawing.Printing;

using SharpReportCore;

/// <summary>
/// Renderer for DataReports
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 13.12.2004 11:07:59
/// </remarks>
/// 

namespace SharpReportCore {
	public class RenderDataReport : AbstractDataRenderer {

		private PointF currentPoint;
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
		
		private PointF DoReportHeader (ReportPageEventArgs rpea){
			PointF endAt = base.MeasureReportHeader (rpea);
			base.RenderSection (rpea);
			base.DoItems(rpea);
			
			return endAt;
		}
		
		
		private PointF DoPageHeader (PointF startAt,ReportPageEventArgs rpea){
			PointF endAt = base.MeasurePageHeader (startAt,rpea);
			base.RenderSection (rpea);
			base.DoItems(rpea);
			return endAt;
		}
		
		
		private void DoPageEnd (ReportPageEventArgs rpea){
//			System.Console.WriteLine("DataRenderer:DoPageEnd");
			base.MeasurePageEnd (rpea);
			base.RenderSection (rpea);
			base.DoItems(rpea);
		}
		
		//TODO how should we handle ReportFooter, print it on an seperate page ????
		private void  DoReportFooter (PointF startAt,ReportPageEventArgs rpea){
//			System.Console.WriteLine("DoReportFooter");
			base.MeasureReportFooter(rpea);
			base.RenderSection (rpea);
			base.DoItems(rpea);
		}
		
		#endregion

		#region test
		
		protected override void PrintReportHeader (object sender, ReportPageEventArgs e) {
			base.PrintReportHeader (sender,e);
			this.currentPoint = DoReportHeader (e);
			base.RemoveSectionEvents();
		}
		
		protected override void PrintPageHeader (object sender, ReportPageEventArgs e) {
			base.PrintPageHeader (sender,e);
			this.currentPoint = DoPageHeader (this.currentPoint,e);
			base.DetailStart = new Point ((int)currentPoint.X,(int)currentPoint.Y +1);
			base.RemoveSectionEvents();
		}
		
		protected override void PrintPageEnd(object sender, ReportPageEventArgs rpea) {
//			System.Console.WriteLine("DataRenderer:PrintPageEnd");
			base.PrintPageEnd(sender,rpea);
			this.DoPageEnd (rpea);
			base.RemoveSectionEvents();
		}
		
		protected override void PrintReportFooter(object sender, ReportPageEventArgs rpea){
//			System.Console.WriteLine("DataRenderer:PrintReportFooter");
			base.PrintReportFooter(sender, rpea);
			DoReportFooter (new PointF(0,base.CurrentSection.SectionOffset + base.CurrentSection.Size.Height),
			                rpea);
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
//			System.Console.WriteLine("");
//			System.Console.WriteLine("ReportBegin (BeginPrint)");
			base.ReportBegin (sender,pea);
			base.DataManager.ListChanged += new EventHandler<ListChangedEventArgs> (OnListChanged);
			dataNavigator = base.DataManager.GetNavigator;
			dataNavigator.ListChanged += new EventHandler<ListChangedEventArgs> (OnListChanged);
			dataNavigator.Reset();
			base.DataNavigator = dataNavigator;
		}
		
		
		
		protected override void BodyStart(object sender, ReportPageEventArgs rpea) {
//			System.Console.WriteLine("DataRenderer:PrintBodyStart");
			base.BodyStart (sender,rpea);
			this.currentPoint = new PointF (base.CurrentSection.Location.X,
			                                this.DetailStart.Y);
			
			base.CurrentSection.SectionOffset = (int)this.DetailStart.Y + AbstractRenderer.Gap;
		}
		
		
		protected override void PrintDetail(object sender, ReportPageEventArgs rpea){
			Rectangle sectionRect;
			bool firstOnPage = true;
//			System.Console.WriteLine("RenderDataReport:PrintDetail");
			base.PrintDetail(sender, rpea);
			
//			base.DebugRectangle(rpea,base.DetailRectangle(rpea));
			// no loop if there is no data
			if (! this.dataNavigator.HasMoreData ) {
				rpea.PrintPageEventArgs.HasMorePages = false;
				return;
			}
			
			// first element
			if (rpea.PageNumber == 1) {
				this.dataNavigator.MoveNext();
			}
			
			do {
				
				this.dataNavigator.Fill (base.CurrentSection.Items);

				base.RenderSection (rpea);
				
				if (!firstOnPage) {
					base.CurrentSection.SectionOffset = base.CurrentSection.SectionOffset + base.CurrentSection.Size.Height  + 2 * AbstractRenderer.Gap;
					
				}
				
				
				base.FitSectionToItems (base.CurrentSection,rpea);
				
				sectionRect = new Rectangle (rpea.PrintPageEventArgs.MarginBounds.Left,
				                             base.CurrentSection.SectionOffset,
				                             rpea.PrintPageEventArgs.MarginBounds.Width,
				                             base.CurrentSection.Size.Height);
				
				if (!base.DetailRectangle(rpea).Contains(sectionRect)) {
					AbstractRenderer.PageBreak(rpea);
					return;
				}
				
				int i = base.DoItems(rpea);
				this.currentPoint = new PointF (base.CurrentSection.Location.X, i);
				firstOnPage = false;

				if (this.dataNavigator.CurrentRow < this.dataNavigator.Count -1) {
					if (base.CurrentSection.PageBreakAfter) {
						AbstractRenderer.PageBreak(rpea);;
						return;
					}
				}
			}
			while (this.dataNavigator.MoveNext());
			this.RemoveSectionEvents();
		}
		
		protected override void OnBodyEnd(object sender, ReportPageEventArgs rpea) {
//			System.Console.WriteLine("PrintBodyEnd ????");
			base.OnBodyEnd (sender,rpea);
//			DoReportFooter (new PointF(0,base.CurrentSection.SectionOffset + base.CurrentSection.Size.Height),
//			                rpea);
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
