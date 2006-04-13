
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
			base.DataManager.GroupChanged += new EventHandler<GroupChangedEventArgs>(OnGroupChanged);
			base.DataManager.GroupChanging += new EventHandler <EventArgs> (OnGroupChanging);
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
			System.Console.WriteLine("");
			System.Console.WriteLine("Begin Print <{0}> with  <{1}> Items ",e.Section.Name,
			                         e.Section.Items.Count);
		}
		
		private void OnSectionPrinted (object sender,SectionPrintingEventArgs e) {
			System.Console.WriteLine("Section Printed <{0}> ",e.Section.Name);
			
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
			PointF endAt = base.MeasureReportHeader (rpea);

			this.AddSectionEvents();
			base.RenderSection (base.CurrentSection,rpea);
			this.RemoveSectionEvents();
			
			if (base.CurrentSection.PageBreakAfter) {
				base.PageBreak(rpea,base.CurrentSection);
				base.CurrentSection.PageBreakAfter = false;
				return new PointF();
			}
			return endAt;
		}
		
		
		private PointF DoPageHeader (PointF startAt,ReportPageEventArgs rpea){
			
			PointF endAt = base.MeasurePageHeader (startAt,rpea);

			this.AddSectionEvents();
			base.RenderSection (base.CurrentSection,rpea);
			this.RemoveSectionEvents();
			return endAt;
		}
		
		
		private void DoPageEnd (ReportPageEventArgs rpea){
			System.Console.WriteLine("\tDoPageEnd");
			base.PrintPageEnd(this,rpea);
			base.MeasurePageEnd (rpea);
			
			this.AddSectionEvents();
			base.RenderSection (base.CurrentSection,rpea);
			this.RemoveSectionEvents();
			
		}
		//TODO how should we handle ReportFooter, print it on an seperate page ????
		private void  DoReportFooter (PointF startAt,ReportPageEventArgs rpea){
			base.MeasureReportFooter(rpea);

			this.AddSectionEvents();
			base.RenderSection (base.CurrentSection,rpea);
			this.RemoveSectionEvents();
		}
		#endregion

		protected override void ReportQueryPage(object sender, QueryPageSettingsEventArgs qpea) {
			base.ReportQueryPage (sender,qpea);
		}
			
		protected override void ReportBegin(object sender, ReportPageEventArgs rpea) {
			base.ReportBegin (sender,rpea);
		
			base.DataManager.ListChanged += new EventHandler<ListChangedEventArgs> (OnListChanged);
			dataNavigator = base.DataManager.GetNavigator;
			dataNavigator.ListChanged += new EventHandler<ListChangedEventArgs> (OnListChanged);
			dataNavigator.Reset();
			base.DataNavigator = dataNavigator;
		}
		
		protected override void BeginPrintPage(object sender, ReportPageEventArgs rpea) {
			
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}
			
			base.BeginPrintPage (sender,rpea);
			
			if (rpea.PageNumber == 1) {
				//Draw ReportHeader
				this.currentPoint = DoReportHeader (rpea);
			}
			
			//Draw Pageheader
			this.currentPoint = DoPageHeader (this.currentPoint,rpea);
			
			base.DetailStart = new Point ((int)currentPoint.X,(int)currentPoint.Y);
		
		}
		
	
			
		protected override void PrintBodyStart(object sender, ReportPageEventArgs rpea) {
			Rectangle sectionRect;
			Rectangle detailRect;

			base.PrintBodyStart (sender,rpea);

			BaseSection section = base.CurrentSection;
			
			section.SectionOffset = (int)this.currentPoint.Y + base.Gap;
			
			detailRect = base.DetailRectangle (rpea);
			
			//this is only for the first record, zhe other details will be calculated
			sectionRect = new Rectangle (rpea.PrintPageEventArgs.MarginBounds.Left,
			                             section.SectionOffset,
			                             rpea.PrintPageEventArgs.MarginBounds.Width,
			                             section.Size.Height);
//			DebugRectangle (e,detailRect);
			
			// no loop if there is no data

			if (! this.dataNavigator.HasMoreData ) {
				rpea.PrintPageEventArgs.HasMorePages = false;
				return;
			}

			while (this.dataNavigator.MoveNext()) {
				this.dataNavigator.Fill (base.CurrentSection.Items);	
				base.RenderSection (section,rpea);
				
				section.SectionOffset = section.SectionOffset + section.Size.Height  + 2 * base.Gap;
				
				base.FitSectionToItems (base.CurrentSection,rpea);
				
				sectionRect = new Rectangle (rpea.PrintPageEventArgs.MarginBounds.Left,
				                             section.SectionOffset,
				                             rpea.PrintPageEventArgs.MarginBounds.Width,
				                             section.Size.Height);
				
				if (!detailRect.Contains(sectionRect)) {
					base.PageBreak(rpea,section);
					return;
				}	
				
			}
			
			DoReportFooter (new PointF(0,section.SectionOffset + section.Size.Height),
			                rpea);
				
			rpea.PrintPageEventArgs.HasMorePages = false;

			//Did we have a pagebreak 
			if (base.CurrentSection.PageBreakAfter) {
				base.PageBreak(rpea,section);
				base.CurrentSection.PageBreakAfter = false;
				return;
			}
		}
		
		
		protected override void PrintBodyEnd(object sender, ReportPageEventArgs rpea) {
//			System.Console.WriteLine("PrintBodyEnd");
			base.PrintBodyEnd (sender,rpea);
		}
		
		
		protected override void PrintPageEnd(object sender, ReportPageEventArgs rpea) {
			this.DoPageEnd (rpea);
		}
	
		public override string ToString() {
			base.ToString();
			return "RenderDataReport";
		}
		#endregion

		
	}
}
