
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
//	public class RenderDataReport : AbstractRenderer {
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
			System.Console.WriteLine("List Changed sender <{0}> reason <{1}>",
			                         sender.ToString(),
			                         e.ListChangedType);
		}
		
		#region overrides
		

		protected override void ReportQueryPage(object sender, QueryPageSettingsEventArgs e) {
			base.ReportQueryPage (sender,e);
		}
			
		protected override void ReportBegin(object sender, ReportPageEventArgs e) {
			base.ReportBegin (sender,e);
		
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
			//Draw ReportHeader
			currentPoint = base.DrawReportHeader (rpea);		
			if (base.CurrentSection.PageBreakAfter) {
				base.PageBreak(rpea,base.CurrentSection);
				base.CurrentSection.PageBreakAfter = false;
				return;
			}
			
			//Draw Pageheader
			currentPoint = base.DrawPageHeader (currentPoint,rpea);
			base.DetailStart = new Point ((int)currentPoint.X,(int)currentPoint.Y);
		}
		
		protected override void PrintBodyStart(object sender, ReportPageEventArgs e) {
			Rectangle sectionRect;
			Rectangle detailRect;
			
			base.PrintBodyStart (sender,e);
			base.SectionInUse = Convert.ToInt16(GlobalEnums.enmSection.ReportDetail,CultureInfo.InvariantCulture);
			
			BaseSection section = base.CurrentSection;
			
			section.SectionOffset = (int)this.currentPoint.Y + base.Gap;
			
			detailRect = base.DetailRectangle (e);
			
			//this is only for the first record, zhe other details will be calculated
			sectionRect = new Rectangle (e.PrintPageEventArgs.MarginBounds.Left,
			                             section.SectionOffset,
			                             e.PrintPageEventArgs.MarginBounds.Width,
			                             section.Size.Height);
//			DebugRectangle (e,detailRect);
			
			// no loop if there is no data
			System.Console.WriteLine("Navi hasMore {0}",this.dataNavigator.HasMoreData);
			if (! this.dataNavigator.HasMoreData ) {
				e.PrintPageEventArgs.HasMorePages = false;
				return;
			}

			while (this.dataNavigator.MoveNext()) {
				this.dataNavigator.Fill (base.CurrentSection.Items);	
				base.RenderSection (section,e);
				
				section.SectionOffset = section.SectionOffset + section.Size.Height  + 2 * base.Gap;
				
				base.FitSectionToItems (base.CurrentSection,e);
				
				sectionRect = new Rectangle (e.PrintPageEventArgs.MarginBounds.Left,
				                             section.SectionOffset,
				                             e.PrintPageEventArgs.MarginBounds.Width,
				                             section.Size.Height);
				
				if (!detailRect.Contains(sectionRect)) {
					base.PageBreak(e,section);
					return;
				}	
				
			}
			
			e.PrintPageEventArgs.HasMorePages = false;

			//Did we have a pagebreak 
			if (base.CurrentSection.PageBreakAfter) {
				base.PageBreak(e,section);
				base.CurrentSection.PageBreakAfter = false;
				return;
			}
		}
		
		
		protected override void PrintBodyEnd(object sender, ReportPageEventArgs e) {
			base.PrintBodyEnd (sender,e);
		}
		
		
		protected override void PrintPageEnd(object sender, ReportPageEventArgs e) {
			base.SectionInUse = Convert.ToInt16(GlobalEnums.enmSection.ReportPageFooter,
			                                   CultureInfo.InvariantCulture);
			base.PrintPageEnd (sender,e);
			base.DetailEnds = new Point (0,base.CurrentSection.SectionOffset);
		}
	
		public override string ToString() {
			base.ToString();
			return "RenderDataReport";
		}
		#endregion

		
	}
}
