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
using System.Drawing;
using System.Drawing.Printing;

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
		private void DoReportHeader (ReportPageEventArgs rpea){
			base.RenderSection (rpea);
		}
		
		private void DoPageHeader (PointF startAt,ReportPageEventArgs rpea){
			this.CurrentSection.SectionOffset = base.Page.PageHeaderRectangle.Location.Y;
			base.RenderSection (rpea);
		}
		
		private void DoPageEnd (ReportPageEventArgs rpea){
			
			base.PrintPageEnd(this,rpea);
			this.CurrentSection.SectionOffset = base.Page.PageFooterRectangle.Location.Y;
			base.RenderSection (rpea);
		}
		
		
		
		//TODO how should we handle ReportFooter, print it on an seperate page ????
		private void  DoReportFooter (PointF startAt,ReportPageEventArgs rpea){
			this.CurrentSection.SectionOffset = base.Page.ReportFooterRectangle.Location.Y;
			base.RenderSection (rpea);
			this.RemoveSectionEvents();
		}
		
		#endregion
		
		#region print all the sections
		
		protected override void PrintReportHeader (object sender, ReportPageEventArgs e) {
			base.PrintReportHeader (sender,e);
			DoReportHeader (e);
		}
		
		protected override void PrintPageHeader (object sender, ReportPageEventArgs e) {
			base.PrintPageHeader (sender,e);
			DoPageHeader (this.currentPoint,e);
		}
		/// <summary>
		/// Detail Section
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void BodyStart (object sender,ReportPageEventArgs rpea) {
//			System.Console.WriteLine("BodyStart");
			base.BodyStart (sender,rpea);
			this.currentPoint = new PointF (base.CurrentSection.Location.X,
			                                base.page.DetailStart.Y);

		}
		
		protected override void PrintDetail(object sender, ReportPageEventArgs rpea){
			base.PrintDetail(sender, rpea);
			this.CurrentSection.SectionOffset = base.Page.PageHeaderRectangle.Bottom;
			base.RenderSection(rpea);
			base.RemoveSectionEvents();
		}
		
		protected override void PrintReportFooter(object sender, ReportPageEventArgs rpea){
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
			this.DoPageEnd (rpea);
		}
		#endregion
		
		
		#region event's
		
		
		
		
		
		protected override void BodyEnd (object sender,ReportPageEventArgs rpea) {
			
			base.BodyEnd (sender,rpea);
			this.DoReportFooter (new PointF(0,base.CurrentSection.SectionOffset + base.CurrentSection.Size.Height),
			                     rpea);
		}
		
		
		protected override void ReportEnd(object sender, PrintEventArgs e){
			base.ReportEnd(sender, e);
		}
		
		#endregion
		
	}
}
