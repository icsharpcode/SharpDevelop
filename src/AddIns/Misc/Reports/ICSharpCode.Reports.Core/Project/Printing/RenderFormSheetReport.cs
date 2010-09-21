// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing.Printing;
using ICSharpCode.Reports.Core.BaseClasses.Printing;
using ICSharpCode.Reports.Core.Interfaces;

	/// <summary>
	/// Runs the Report
	/// </summary>
	/// <remarks>
	/// 	created by - Forstmeier Helmut
	/// 	created on - 21.11.2004 17:14:48
	/// </remarks>
	/// 
namespace ICSharpCode.Reports.Core {
	public class RenderFormSheetReport : AbstractRenderer {

		#region Constructor
		
		public RenderFormSheetReport (IReportModel model,
		                              ReportDocument reportDcoument,
		                             ILayouter layout):base( model,reportDcoument,layout)
		{
		}
		
		#endregion
		
		#region print all the sections
		internal override void ReportQueryPage(object sender, QueryPageSettingsEventArgs qpea) {
			base.ReportQueryPage (sender,qpea);
		}
		
	
		internal override void PrintReportHeader (object sender, ReportPageEventArgs rpea) {
			base.PrintReportHeader (sender,rpea);
			base.RenderSection (rpea);
		}
		
		internal override void PrintPageHeader (object sender, ReportPageEventArgs rpea) {
			base.PrintPageHeader (sender,rpea);
			this.CurrentSection.SectionOffset = base.SectionBounds.PageHeaderRectangle.Location.Y;
			base.RenderSection (rpea);
		}
		
		/// <summary>
		/// Detail Section
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		internal override void BodyStart (object sender,ReportPageEventArgs rpea) {
			base.BodyStart (sender,rpea);
		}
		
		internal override void PrintDetail(object sender, ReportPageEventArgs rpea){
			base.PrintDetail(sender, rpea);
			this.CurrentSection.SectionOffset = base.SectionBounds.PageHeaderRectangle.Bottom;
			base.RenderSection(rpea);
			base.RemoveSectionEvents();
			base.ReportDocument.DetailsDone = true;
			
			// test for reportfooter
			if (!PrintHelper.IsRoomForFooter (base.SectionBounds,rpea.LocationAfterDraw)) {
				AbstractRenderer.PageBreak(rpea);
			}
		}
		
		internal override void PrintReportFooter(object sender, ReportPageEventArgs rpea){
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
		
		internal override void PrintPageFooter(object sender, ReportPageEventArgs rpea) {
			base.PrintPageFooter(this,rpea);
			this.CurrentSection.SectionOffset = base.SectionBounds.PageFooterRectangle.Location.Y;
			base.RenderSection (rpea);
		}
		#endregion
		
		
		#region event's
		
		
		internal override void BodyEnd (object sender,ReportPageEventArgs rpea) {
			base.BodyEnd (sender,rpea);
			rpea.PrintPageEventArgs.HasMorePages = false;
		}
		
		/*
		internal override void ReportEnd(object sender, PrintEventArgs e){
			base.ReportEnd(sender, e);
		}
		*/
		#endregion
		
	}
}
