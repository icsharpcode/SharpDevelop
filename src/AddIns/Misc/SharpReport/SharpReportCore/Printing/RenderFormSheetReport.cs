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
		
		
		#region Draw the different report Sections
		private PointF DoReportHeader (ReportPageEventArgs rpea){
			System.Console.WriteLine("\t ReportHeader");
			PointF endAt = base.MeasureReportHeader (rpea);
			base.RenderSection (rpea);

			if (base.CheckPageBreakAfter()) {
				AbstractRenderer.PageBreak(rpea);
				base.CurrentSection.PageBreakAfter = false;
				return new PointF();
			}
			return endAt;
		}
		
		private PointF DoPageHeader (PointF startAt,ReportPageEventArgs rpea){
System.Console.WriteLine("\t PageHeader");
			PointF endAt = base.MeasurePageHeader (startAt,rpea);
			base.RenderSection (rpea);
			return endAt;
		}
		
		private void DoPageEnd (ReportPageEventArgs rpea){
			
			base.PrintPageEnd(this,rpea);
			base.MeasurePageFooter (rpea);
			base.RenderSection (rpea);
		}
		
		
		
		//TODO how should we handle ReportFooter, print it on an seperate page ????
		private void  DoReportFooter (PointF startAt,ReportPageEventArgs rpea){
			System.Console.WriteLine("\t ReportFooter");
			base.MeasureReportFooter(rpea);
			base.RenderSection (rpea);
			this.RemoveSectionEvents();
		}
		
		#endregion
		
		#region print all the sections
		
		protected override void PrintReportHeader (object sender, ReportPageEventArgs e) {
			System.Console.WriteLine("PRINT REPORTHEADER");
			base.PrintReportHeader (sender,e);
			this.currentPoint = DoReportHeader (e);
		}
		
		protected override void PrintPageHeader (object sender, ReportPageEventArgs e) {
			System.Console.WriteLine("PRINT PAGEHEDER");
			base.PrintPageHeader (sender,e);
			this.currentPoint = DoPageHeader (this.currentPoint,e);
			base.DetailStart = new Point ((int)currentPoint.X,(int)currentPoint.Y);
		}
		
		protected override void PrintDetail(object sender, ReportPageEventArgs rpea){
			base.PrintDetail(sender, rpea);
			System.Console.WriteLine("PRINT DETAIL");
			base.RenderSection(rpea);
			base.RemoveSectionEvents();
		}
		
		protected override void PrintReportFooter(object sender, ReportPageEventArgs rpea){
			System.Console.WriteLine("PRINT REPORTFOOTER");
			base.PrintReportFooter(sender, rpea);
			base.RenderSection(rpea);
			base.RemoveSectionEvents();
		}
		
		/// <summary>
		/// Print the PageFooter 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void PrintPageEnd(object sender, ReportPageEventArgs rpea) {
			System.Console.WriteLine("PRINTPAGEEND");
			this.DoPageEnd (rpea);
		}
		#endregion
		
		
		#region event's
		
		
		/// <summary>
		/// Detail Section
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void BodyStart (object sender,ReportPageEventArgs rpea) {
			base.BodyStart (sender,rpea);
			base.CurrentSection.SectionOffset = (int)this.currentPoint.Y + AbstractRenderer.Gap;
			
			FitSectionToItems (base.CurrentSection,rpea);
			base.RenderSection (rpea);

		}
		
		
		protected override void OnBodyEnd (object sender,ReportPageEventArgs rpea) {
			
			base.OnBodyEnd (sender,rpea);
			this.DoReportFooter (new PointF(0,base.CurrentSection.SectionOffset + base.CurrentSection.Size.Height),
			                     rpea);
		}
		
		
		protected override void ReportEnd(object sender, PrintEventArgs e){
			base.ReportEnd(sender, e);
		}
		
		#endregion
		
	}
}
