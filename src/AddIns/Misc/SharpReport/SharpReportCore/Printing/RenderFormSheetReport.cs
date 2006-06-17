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
using System.Globalization;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Xml;
	
using SharpReportCore;

	/// <summary>
	/// Runs the Report
	/// </summary>
	/// <remarks>
	/// 	created by - Forstmeier Helmut
	/// 	created on - 21.11.2004 17:14:48
	/// </remarks>
	/// 
namespace SharpReportCore {
	public class RenderFormSheetReport : AbstractRenderer {

		private PointF currentPoint = new PointF (0,0);

		public RenderFormSheetReport (ReportModel model):base( model){
		                             
		}
		
		private void OnSectionPrinting (object sender,SectionPrintingEventArgs e) {
			System.Console.WriteLine("");
			System.Console.WriteLine("Begin Print <{0}> with  <{1}> Items ",e.Section.Name,
			                         e.Section.Items.Count);
		}
		
		private void OnSectionPrinted (object sender,SectionPrintingEventArgs e) {
			System.Console.WriteLine("Section Printed {0} ",e.Section.Name);
			
		}
		
		private void AddSectionEvents () {
			base.CurrentSection.SectionPrinting += new EventHandler<SectionPrintingEventArgs>(OnSectionPrinting);
			base.CurrentSection.SectionPrinted += new EventHandler<SectionPrintingEventArgs>(OnSectionPrinted);
		}
		
		private void RemoveSectionEvents () {
			base.CurrentSection.SectionPrinting -= new EventHandler<SectionPrintingEventArgs>(OnSectionPrinting);
			base.CurrentSection.SectionPrinted -= new EventHandler<SectionPrintingEventArgs>(OnSectionPrinted);
		}
		
		
		
		#region Draw the different report Sections
		private PointF DoReportHeader (ReportPageEventArgs rpea){
			PointF endAt = base.MeasureReportHeader (rpea);

			this.AddSectionEvents();
			base.RenderSection (base.CurrentSection,rpea);
			this.RemoveSectionEvents();
			
			if (base.CheckPageBreakAfter()) {
				AbstractRenderer.PageBreak(rpea);
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
		
		#region test
		
		protected override void BuildReportHeader (object sender, ReportPageEventArgs e) {
			System.Console.WriteLine("!!!!");
			System.Console.WriteLine("testReportStart");
			base.BuildReportHeader (sender,e);
			this.currentPoint = DoReportHeader (e);
			System.Console.WriteLine("");
		}
		
		protected override void BuildPageHeader (object sender, ReportPageEventArgs e) {
			
			System.Console.WriteLine("!!!!");
			System.Console.WriteLine("testPrintheader");
			base.BuildPageHeader (sender,e);
			this.currentPoint = DoPageHeader (this.currentPoint,e);
			base.DetailStart = new Point ((int)currentPoint.X,(int)currentPoint.Y);
			System.Console.WriteLine("");
		}
		
		#endregion
		
		#region event's
//		protected override  void ReportQueryPage (object sender,QueryPageSettingsEventArgs qpea) {
//			base.ReportQueryPage (sender,qpea);
//		}
		
		
//		protected override void ReportBegin (object sender,PrintEventArgs rpea) {
//			base.ReportBegin (sender,rpea);
//		}
		
		
		/// <summary>
		/// Detail Section
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void PrintBodyStart (object sender,ReportPageEventArgs rpea) {
			base.PrintBodyStart (sender,rpea);
			
//			BaseSection section = base.CurrentSection;
			base.CurrentSection.SectionOffset = (int)this.currentPoint.Y + AbstractRenderer.Gap;
			
			
			FitSectionToItems (base.CurrentSection,rpea);
			
			this.AddSectionEvents();
			base.RenderSection (base.CurrentSection,rpea);
			this.RemoveSectionEvents();
		}
		/// <summary>
		/// Print the PageFooter 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void PrintPageEnd(object sender, ReportPageEventArgs rpea) {
			this.DoPageEnd (rpea);
		}
		
		
		
		protected override void PrintBodyEnd (object sender,ReportPageEventArgs rpea) {
			
			base.PrintBodyEnd (sender,rpea);
			this.DoReportFooter (new PointF(0,base.CurrentSection.SectionOffset + base.CurrentSection.Size.Height),
			                     rpea);
		}
		
		
		protected override void ReportEnd(object sender, PrintEventArgs e){
			base.ReportEnd(sender, e);
		}
		
		#endregion
		
	}
}
