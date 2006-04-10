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
		
		#region event's
		protected override  void ReportQueryPage (object sender,QueryPageSettingsEventArgs e) {
			base.ReportQueryPage (sender,e);
		}
		
		
		protected override void ReportBegin (object sender,ReportPageEventArgs e) {
			base.ReportBegin (sender,e);
		}
		
		/// <summary>
		/// ReportHeader and if PageHeader
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// 
		protected override void  BeginPrintPage (object sender,ReportPageEventArgs rpea) {
			base.BeginPrintPage (sender,rpea);
			//Draw ReportHeader
			
			currentPoint = base.MeasureReportHeader(rpea);
			base.RenderSection (base.CurrentSection,rpea);
			if (base.CurrentSection.PageBreakAfter) {
				base.PageBreak(rpea,base.CurrentSection);
				base.CurrentSection.PageBreakAfter = false;
				return;
			}
			
			//Draw Pageheader
			currentPoint = base.MeasurePageHeader(currentPoint,rpea);
			base.RenderSection (base.CurrentSection,rpea);
			base.DetailStart = new Point ((int)currentPoint.X,(int)currentPoint.Y);
		}
		
		
		/// <summary>
		/// Detail Section
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void PrintBodyStart (object sender,ReportPageEventArgs e) {
			base.PrintBodyStart (sender,e);
			
			BaseSection section = base.CurrentSection;
			section.SectionOffset = (int)this.currentPoint.Y + base.Gap;
			
			Rectangle detailRect = base.DetailRectangle (e);
			FitSectionToItems (section,e);
			base.RenderSection (section,e);
//			DebugRectangle (e,detailRect);
		}
		
		
		protected override void PrintBodyEnd (object sender,ReportPageEventArgs e) {
			base.PrintBodyEnd (sender,e);
		}
		
		
		/// <summary>
		/// PageFooter and, if LastPage ReportFooter
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void PrintPageEnd (object sender,ReportPageEventArgs e) {
		
			//PageFooter
			base.SectionInUse = Convert.ToInt16(GlobalEnums.enmSection.ReportPageFooter,
			                                   CultureInfo.InvariantCulture);
			base.PrintPageEnd (sender,e);
			
			
			Rectangle r = new Rectangle (e.PrintPageEventArgs.MarginBounds.Left,
			                                base.CurrentSection.SectionOffset ,
			                                e.PrintPageEventArgs.MarginBounds.Width,
			                                base.CurrentSection.Size.Height);
//			DebugRectangle (e,r);
			
			int off = base.CurrentSection.SectionOffset + base.CurrentSection.Size.Height + base.Gap;
			//ReportFooter
			base.SectionInUse = Convert.ToInt16(GlobalEnums.enmSection.ReportFooter,
			                                   CultureInfo.InvariantCulture);
			BaseSection section = base.CurrentSection;
	
			section.SectionOffset = off;
			FitSectionToItems (section,e);
			Rectangle rr = new Rectangle (e.PrintPageEventArgs.MarginBounds.Left,
			                                base.CurrentSection.SectionOffset ,
			                                e.PrintPageEventArgs.MarginBounds.Width,
			                                base.CurrentSection.Size.Height);
			
			base.RenderSection (section,e);
//			DebugRectangle (e,rr);
		}
		#endregion
		
	}
}
