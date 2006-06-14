
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
		
		
		private void OnSectionPrinting (object sender,SectionPrintingEventArgs e) {
//			System.Console.WriteLine("");
//			System.Console.WriteLine("Begin Print <{0}> with  <{1}> Items ",e.Section.Name,
//			                         e.Section.Items.Count);
		}
		
		private void OnSectionPrinted (object sender,SectionPrintingEventArgs e) {
//			System.Console.WriteLine("Section Printed <{0}> ",e.Section.Name);
			
		}
		
		private void AddSectionEvents () {
			base.CurrentSection.SectionPrinting += new EventHandler<SectionPrintingEventArgs>(OnSectionPrinting);
			base.CurrentSection.SectionPrinted += new EventHandler<SectionPrintingEventArgs>(OnSectionPrinted);
		}
		
		private void RemoveSectionEvents () {
			base.CurrentSection.SectionPrinting -= new EventHandler<SectionPrintingEventArgs>(OnSectionPrinting);
			base.CurrentSection.SectionPrinted -= new EventHandler<SectionPrintingEventArgs>(OnSectionPrinted);
		}
		
		#region overrides
		
		#region Draw the different report Sections
		
		private PointF DoReportHeader (ReportPageEventArgs rpea){
			System.Console.WriteLine("DoReportHeader");
			PointF endAt = base.MeasureReportHeader (rpea);

			this.AddSectionEvents();
			base.RenderSection (base.CurrentSection,rpea);
			base.DoItems(rpea);
			this.RemoveSectionEvents();
			
//			if (base.CheckPageBreakAfter()) {
//				base.PageBreak(rpea);
//				base.CurrentSection.PageBreakAfter = false;
//				return new PointF();
//			}
			return endAt;
		}
		
		
		private PointF DoPageHeader (PointF startAt,ReportPageEventArgs rpea){
			PointF endAt = base.MeasurePageHeader (startAt,rpea);
			this.AddSectionEvents();
			base.RenderSection (base.CurrentSection,rpea);
			base.DoItems(rpea);
			this.RemoveSectionEvents();
			return endAt;
		}
		
		
		private void DoPageEnd (ReportPageEventArgs rpea){
			System.Console.WriteLine("DoPageEnd");
			base.PrintPageEnd(this,rpea);
			base.MeasurePageEnd (rpea);
			
			this.AddSectionEvents();
			base.RenderSection (base.CurrentSection,rpea);
			base.DoItems(rpea);
			this.RemoveSectionEvents();
			
		}
		
		//TODO how should we handle ReportFooter, print it on an seperate page ????
		private void  DoReportFooter (PointF startAt,ReportPageEventArgs rpea){
			base.MeasureReportFooter(rpea);
			this.AddSectionEvents();
			base.RenderSection (base.CurrentSection,rpea);
			base.DoItems(rpea);
			this.RemoveSectionEvents();
		}
		#endregion

		#region test
		
		protected override void BuildReportHeader (object sender, ReportPageEventArgs e) {
			base.BuildReportHeader (sender,e);
			this.currentPoint = DoReportHeader (e);
		}
		
		protected override void BuildPageHeader (object sender, ReportPageEventArgs e) {
			base.BuildPageHeader (sender,e);
			this.currentPoint = DoPageHeader (this.currentPoint,e);
			base.DetailStart = new Point ((int)currentPoint.X,(int)currentPoint.Y +1);
		}
		
		#endregion
		
		
		#region overrides
		
		
		protected override void ReportQueryPage(object sender, QueryPageSettingsEventArgs qpea) {
			base.ReportQueryPage (sender,qpea);
		}
		
		protected override void ReportBegin(object sender, PrintEventArgs pea) {
			System.Console.WriteLine("ReportBegin (BeginPrint)");
			base.ReportBegin (sender,pea);
			base.DataManager.ListChanged += new EventHandler<ListChangedEventArgs> (OnListChanged);
			dataNavigator = base.DataManager.GetNavigator;
			dataNavigator.ListChanged += new EventHandler<ListChangedEventArgs> (OnListChanged);
			dataNavigator.Reset();
			base.DataNavigator = dataNavigator;
		}
		
		protected override void PrintBodyStart(object sender, ReportPageEventArgs rpea) {
			Rectangle sectionRect;
			bool firstOnPage = true;
			
			base.PrintBodyStart (sender,rpea);
			this.currentPoint = new PointF (base.CurrentSection.Location.X,
			                                this.DetailStart.Y);
			
			base.CurrentSection.SectionOffset = (int)this.DetailStart.Y + base.Gap;
			
//			base.DebugRectangle(rpea,base.DetailRectangle(rpea));
			// no loop if there is no data
			if (! this.dataNavigator.HasMoreData ) {
				rpea.PrintPageEventArgs.HasMorePages = false;
				return;
			}
			
			// first element
			if (this.dataNavigator.MoveNext()){
				do {
					
					this.dataNavigator.Fill (base.CurrentSection.Items);

					base.RenderSection (base.CurrentSection,rpea);
					
					if (!firstOnPage) {
						base.CurrentSection.SectionOffset = base.CurrentSection.SectionOffset + base.CurrentSection.Size.Height  + 2 * base.Gap;
					
					}
					
					
					base.FitSectionToItems (base.CurrentSection,rpea);
					
					sectionRect = new Rectangle (rpea.PrintPageEventArgs.MarginBounds.Left,
					                             base.CurrentSection.SectionOffset,
					                             rpea.PrintPageEventArgs.MarginBounds.Width,
					                             base.CurrentSection.Size.Height);
					
					if (!base.DetailRectangle(rpea).Contains(sectionRect)) {
						base.PageBreak(rpea);
						return;
					}
					
					int i = base.DoItems(rpea);
					this.currentPoint = new PointF (base.CurrentSection.Location.X, i);
					firstOnPage = false;

					if (this.dataNavigator.CurrentRow < this.dataNavigator.Count -1) {
						if (base.CurrentSection.PageBreakAfter) {
							base.PageBreak(rpea);;
							return;
						}
					}                            
				}
				while (this.dataNavigator.MoveNext());
			}
			
			DoReportFooter (new PointF(0,base.CurrentSection.SectionOffset + base.CurrentSection.Size.Height),
			                rpea);
			
			rpea.PrintPageEventArgs.HasMorePages = false;
		}
		
		
		protected override void PrintBodyEnd(object sender, ReportPageEventArgs rpea) {
//			System.Console.WriteLine("PrintBodyEnd ????");
			base.PrintBodyEnd (sender,rpea);
		}
		
		
		protected override void PrintPageEnd(object sender, ReportPageEventArgs rpea) {
			System.Console.WriteLine("Page End");
			this.DoPageEnd (rpea);
		}
		
		protected override void ReportEnd(object sender, PrintEventArgs e){
			base.ReportEnd(sender, e);
		}
		
		#endregion
		
		public override string ToString() {
			base.ToString();
			return "RenderDataReport";
		}
		#endregion

		
	}
}
