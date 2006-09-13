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

		
		#region Constructor
		
		public RenderFormSheetReport (ReportModel model):base( model){
		                             
		}
		
		#endregion
		
		#region print all the sections
		protected override void ReportQueryPage(object sender, QueryPageSettingsEventArgs qpea) {
			base.ReportQueryPage (sender,qpea);
		}
		
		protected override void ReportBegin(object sender, PrintEventArgs pea) {
			base.ReportBegin (sender,pea);
		}
		
		protected override void PrintReportHeader (object sender, ReportPageEventArgs rpea) {
			base.PrintReportHeader (sender,rpea);
			base.RenderSection (rpea);
		}
		
		protected override void PrintPageHeader (object sender, ReportPageEventArgs rpea) {
			base.PrintPageHeader (sender,rpea);
			this.CurrentSection.SectionOffset = base.SectionBounds.PageHeaderRectangle.Location.Y;
			base.RenderSection (rpea);
		}
		
		/// <summary>
		/// Detail Section
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void BodyStart (object sender,ReportPageEventArgs rpea) {
			base.BodyStart (sender,rpea);
		}
		
		protected override void PrintDetail(object sender, ReportPageEventArgs rpea){
			base.PrintDetail(sender, rpea);
			this.CurrentSection.SectionOffset = base.SectionBounds.PageHeaderRectangle.Bottom;
			base.RenderSection(rpea);
			base.RemoveSectionEvents();
			base.ReportDocument.DetailsDone = true;
			
			// test for reportfooter
			if (!base.IsRoomForFooter (rpea.LocationAfterDraw)) {
				AbstractRenderer.PageBreak(rpea);
			}
		}
		
		protected override void PrintReportFooter(object sender, ReportPageEventArgs rpea){
			base.PrintReportFooter(sender, rpea);
			this.CurrentSection.SectionOffset = (int)rpea.LocationAfterDraw.Y;
			base.RenderSection(rpea);
			base.RemoveSectionEvents();
		}
		
		/// <summary>
		/// Print the PageFooter 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		
		protected override void PrintPageEnd(object sender, ReportPageEventArgs rpea) {
			base.PrintPageEnd(this,rpea);
			this.CurrentSection.SectionOffset = base.SectionBounds.PageFooterRectangle.Location.Y;
			base.RenderSection (rpea);
		}
		#endregion
		
		
		#region event's
		
		
		protected override void BodyEnd (object sender,ReportPageEventArgs rpea) {
	
//			System.Console.WriteLine("");
//			System.Console.WriteLine("BodyEnd ");

			base.BodyEnd (sender,rpea);
//			System.Console.WriteLine("\tRemoveEvents reason <finish>");
//			base.RemoveSectionEvents();
			rpea.PrintPageEventArgs.HasMorePages = false;
		}
		
		
		protected override void ReportEnd(object sender, PrintEventArgs e){
			base.ReportEnd(sender, e);
		}
		
		#endregion
		
	}
}
