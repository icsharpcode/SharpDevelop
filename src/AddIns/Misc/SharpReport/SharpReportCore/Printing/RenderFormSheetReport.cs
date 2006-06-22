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
			
			PointF endAt = base.MeasurePageHeader (startAt,rpea);
			base.RenderSection (rpea);
			return endAt;
		}
		
		private void DoPageEnd (ReportPageEventArgs rpea){
			base.PrintPageEnd(this,rpea);
			base.MeasurePageEnd (rpea);
			base.RenderSection (rpea);

		}
		
		//TODO how should we handle ReportFooter, print it on an seperate page ????
		private void  DoReportFooter (PointF startAt,ReportPageEventArgs rpea){
			base.MeasureReportFooter(rpea);
			base.RenderSection (rpea);
			this.RemoveSectionEvents();
		}
		
		#endregion
		
		#region test
		
		protected override void PrintReportHeader (object sender, ReportPageEventArgs e) {
			base.PrintReportHeader (sender,e);
			this.currentPoint = DoReportHeader (e);
		}
		
		protected override void PrintPageHeader (object sender, ReportPageEventArgs e) {
			base.PrintPageHeader (sender,e);
			this.currentPoint = DoPageHeader (this.currentPoint,e);
			base.DetailStart = new Point ((int)currentPoint.X,(int)currentPoint.Y);
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
		/// <summary>
		/// Print the PageFooter 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void PrintPageEnd(object sender, ReportPageEventArgs rpea) {
			this.DoPageEnd (rpea);
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
