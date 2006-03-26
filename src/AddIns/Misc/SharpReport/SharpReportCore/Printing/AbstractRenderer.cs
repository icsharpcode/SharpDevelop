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
using System.Globalization;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Printing;
	
using SharpReportCore;

	/// <summary>
	/// Base Class for Rendering Reports
	/// 
	/// </summary>
	/// <remarks>
	/// 	created by - Forstmeier Peter
	/// 	created on - 13.12.2004 09:55:16
	/// </remarks>
	/// 
namespace SharpReportCore {
	public abstract class AbstractRenderer : object,IDisposable {
		private const int gap = 1;
		
		private ReportDocument reportDocument;
		private ReportSectionCollection sections;
		private ReportSettings reportSettings;
		
		private int sectionInUse;
		
		private Point detailStart;
		private Point detailEnds;
	
		private DefaultFormatter defaultFormatter;
		private bool cancel;
		
		
		protected AbstractRenderer(ReportModel model){
			if (model == null) {
				throw new MissingModelException();
			}
			this.reportSettings = model.ReportSettings;
			this.sections = model.SectionCollection;
			Init();
			defaultFormatter = new DefaultFormatter();
		}
		
		public virtual void SetupRenderer () {
			this.cancel = false;
		}
		
		void Init() {
			reportDocument = new SharpReportCore.ReportDocument();
			
			reportDocument.QueryPage += new QueryPageSettingsEventHandler (ReportQueryPage);
			
			reportDocument.ReportBegin += new EventHandler<ReportPageEventArgs> (ReportBegin);
			reportDocument.PrintPageBegin +=  new EventHandler<ReportPageEventArgs>(BeginPrintPage);
			reportDocument.PrintPageBodyStart += new EventHandler<ReportPageEventArgs> (PrintBodyStart);
			reportDocument.PrintPageBodyEnd += new EventHandler<ReportPageEventArgs> (PrintBodyEnd);
			reportDocument.PrintPageEnd += new EventHandler<ReportPageEventArgs> (PrintPageEnd);
			reportDocument.DocumentName = reportSettings.ReportName;
		}
		
		protected void PageBreak(ReportPageEventArgs pea, BaseSection sec) {
			pea.PrintPageEventArgs.HasMorePages = true;
			pea.ForceNewPage = true;
			sec.PageBreakBefore = false;
		}

		
		protected int CalculateDrawAreaHeight(ReportPageEventArgs rpea){
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}
			int to = rpea.PrintPageEventArgs.MarginBounds.Height ;

			if (rpea.PageNumber ==1) {
				to -= sections[Convert.ToInt16(GlobalEnums.enmSection.ReportHeader,CultureInfo.InvariantCulture)].Size.Height;
			}
			
			to -= sections[Convert.ToInt16(GlobalEnums.enmSection.ReportPageHeader,CultureInfo.InvariantCulture)].Size.Height;
			
			to -= sections[Convert.ToInt16(GlobalEnums.enmSection.ReportPageFooter,CultureInfo.InvariantCulture)].Size.Height;
			return to;
		}
		
		///<summary>
		/// Use this function to draw controlling rectangles
		/// </summary>	
		
		protected void DebugRectangle (ReportPageEventArgs rpea,Rectangle rectangle) {
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}
			
			System.Console.WriteLine("Debug Rectangle {0}",rectangle);
			rpea.PrintPageEventArgs.Graphics.DrawRectangle (Pens.Black,rectangle);
	
		}
		
		/// <summary>
		/// Calculates the rectangle wich can be used by Detail
		/// </summary>
		/// <returns></returns>
		protected Rectangle DetailRectangle (ReportPageEventArgs e) {
			sectionInUse = Convert.ToInt16(GlobalEnums.enmSection.ReportDetail,CultureInfo.InvariantCulture);
			
			Rectangle rect = new Rectangle (e.PrintPageEventArgs.MarginBounds.Left,
			                                CurrentSection.SectionOffset ,
			                                e.PrintPageEventArgs.MarginBounds.Width,
			                                detailEnds.Y - detailStart.Y - (3 * gap));
			return rect;
		}
		
		
		///<summary>
		/// Prints the ReportHader printjob is the same in all Types of reportz
		///</summary>
		/// <param name="e">ReportpageEventArgs</param>
		/// 
		protected PointF DrawReportHeader (ReportPageEventArgs e) {
			float offset = 0;
			BaseSection section = null;
			if (e.PageNumber == 1) {
				sectionInUse = Convert.ToInt16(GlobalEnums.enmSection.ReportHeader,CultureInfo.InvariantCulture);
				
				section = CurrentSection;
				section.SectionOffset = reportSettings.DefaultMargins.Top;
				FitSectionToItems (section,e);
				offset =  RenderSection (section,e);
			}
			return new PointF (0,offset + reportSettings.DefaultMargins.Top + Gap);
		}
		
		
		
		///<summary>
		/// Prints the PageHeader printjob is the same in all Types of reportz
		///</summary>
		/// <param name="startAt">Section start at this PointF</param>
		/// <param name="e">ReportPageEventArgs</param>
		protected PointF DrawPageHeader (PointF startat,ReportPageEventArgs e) {
			float offset = 0F;
			BaseSection section = null;
			sectionInUse = Convert.ToInt16(GlobalEnums.enmSection.ReportPageHeader,CultureInfo.InvariantCulture);
			section = CurrentSection;

			if (e.PageNumber == 1) {
				section.SectionOffset = (int)startat.Y + Gap;
			} else {
				section.SectionOffset = reportSettings.DefaultMargins.Top;
			}

			FitSectionToItems (section,e);
			offset = RenderSection (section,e);
			return new PointF (0,section.SectionOffset + offset + Gap);	
		}
		
		
		
		protected virtual int RenderSection (BaseSection section,ReportPageEventArgs rpea) {
			
			Point drawPoint	= new Point(0,0);
			if (section.Visible){
				section.Render (rpea);
				
				foreach (BaseReportItem item in section.Items) {
					if (item.Parent == null) {
						item.Parent = section;
					}
					item.SectionOffset = section.SectionOffset;
					this.DrawSingleItem (rpea,item);
					
					drawPoint.Y = section.SectionOffset + section.Size.Height;
					rpea.LocationAfterDraw = new PointF (rpea.LocationAfterDraw.X,section.SectionOffset + section.Size.Height);
					
				}
				
				if ((section.CanGrow == false)&& (section.CanShrink == false)) {
					return section.Size.Height;
				}
				
				return drawPoint.Y;
			}
			return drawPoint.Y;
		}
		
		
		protected void DrawSingleItem (ReportPageEventArgs rpea,BaseReportItem item){
			item.SuspendLayout();
			item.FormatOutput -= new EventHandler<FormatOutputEventArgs> (FormatBaseReportItem);
			item.FormatOutput += new EventHandler<FormatOutputEventArgs> (FormatBaseReportItem);
			item.Render(rpea);
			item.ResumeLayout();
		}
	
		// Called by FormatOutPutEvent of the BaseReportItem
		void FormatBaseReportItem (object sender, FormatOutputEventArgs rpea) {
			System.Console.WriteLine("FormatBaseReportItem");
			BaseDataItem baseDataItem = sender as BaseDataItem;
			if (baseDataItem != null) {
				if (!String.IsNullOrEmpty(baseDataItem.FormatString)) {
					rpea.FormatedValue = defaultFormatter.FormatItem (baseDataItem);
					System.Console.WriteLine("\tFormated Value = {0}",rpea.FormatedValue);
				} else {
					rpea.FormatedValue = rpea.ValueToFormat;
				}
			}
		}
		
		
		#region privates
		protected void FitSectionToItems (BaseSection section,ReportPageEventArgs rpea){
			if (section == null) {
				throw new ArgumentNullException("section");
			}
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}
			Rectangle orgRect = new Rectangle (rpea.PrintPageEventArgs.MarginBounds.Left,
			                                   section.SectionOffset,
			                                   rpea.PrintPageEventArgs.MarginBounds.Width,
			                                   section.Size.Height);
			
			if ((section.CanGrow == true)||(section.CanShrink == true))  {
				AdjustSection (section,rpea);
			} else {
				AdjustItems (section,rpea);
				
			}
		}
		
		private void AdjustItems (BaseSection section,ReportPageEventArgs e){

			int toFit = section.Size.Height;
			foreach (BaseReportItem rItem in section.Items) {
				if (!CheckItemInSection (section,rItem,e)){
					
					rItem.Size = new Size (rItem.Size.Width,
					                       toFit - rItem.Location.Y);
					
				}
			}
			
		}
		
		private void AdjustSection (BaseSection section,ReportPageEventArgs e){
			
			foreach (BaseReportItem rItem in section.Items) {
				if (!CheckItemInSection (section,rItem,e)){
					
					SizeF size = MeasureReportItem (rItem,e);
					
					section.Size = new Size (section.Size.Width,
					                         Convert.ToInt32(rItem.Location.Y + size.Height));
			
				}
			}
		}
		
		
		private bool CheckItemInSection (BaseSection section,BaseReportItem item ,ReportPageEventArgs e) {
			Rectangle secRect = new Rectangle (0,0,section.Size.Width,section.Size.Height);
			SizeF size = MeasureReportItem(item,e);
			Rectangle itemRect = new Rectangle (item.Location.X,
			                                    item.Location.Y,
			                                    (int)size.Width,
			                                    (int)size.Height);
			if (secRect.Contains(itemRect)) {
				return true;
			}
			return false;
		}
		
		private  SizeF MeasureReportItem(IItemRenderer item,
		                                                    ReportPageEventArgs e) {
			SizeF sizeF = new SizeF ();
			BaseTextItem myItem = item as BaseTextItem;
			if (myItem != null) {
				string str = String.Empty;
				if (item is BaseTextItem) {
					BaseTextItem it = item as BaseTextItem;
					str = it.Text;
				} else if(item is BaseDataItem) {
					BaseDataItem it = item as BaseDataItem;
					str = it.DbValue;
				}
		// TODO need a much better way		
				sizeF = e.PrintPageEventArgs.Graphics.MeasureString(str,
				                                                    myItem.Font,
				                                                    myItem.Size.Width,
				                                                    GlobalValues.StandartStringFormat());
			} else {
				sizeF = new SizeF (item.Size.Width,item.Size.Height);
			}
		
			return sizeF;
		}
		#endregion
		
		#region virtuals
		
		protected virtual void ReportQueryPage (object sender,QueryPageSettingsEventArgs e) {
			e.PageSettings.Margins = reportSettings.DefaultMargins;
		}
		
		
		protected virtual void  ReportBegin (object sender,ReportPageEventArgs e) {
		}
		
		protected virtual void  BeginPrintPage (object sender,ReportPageEventArgs e) {
			SectionInUse = Convert.ToInt16(GlobalEnums.enmSection.ReportPageHeader,CultureInfo.InvariantCulture);
		}
		
		protected virtual void  PrintBodyStart (object sender,ReportPageEventArgs e) {
			
			
		}
		
		protected virtual void  PrintBodyEnd (object sender,ReportPageEventArgs e) {
			
		}
		
		protected virtual void  PrintPageEnd (object sender,ReportPageEventArgs e) {
			BaseSection section = null;
			section = CurrentSection;
			section.SectionOffset = reportSettings.PageSettings.Bounds.Height - reportSettings.DefaultMargins.Top - reportSettings.DefaultMargins.Bottom;
			FitSectionToItems (section,e);
			RenderSection (section,e);
		}
		
		#endregion
		
	
		#region property's
		public ReportDocument ReportDocument {
			get {
				return reportDocument;
			}
		}
		public ReportSettings ReportSettings {
			get {
				return reportSettings;
			}
		}
		public ReportSectionCollection Sections {
			get {
				return sections;
			}
		}
		
		public bool Cancel {
			get {
				return cancel;
			}
			set {
				cancel = value;
			}
		}
		
		
		protected int SectionInUse {
			get {
				return sectionInUse;
			}
			set {
				sectionInUse = value;
			}
		}
		
		protected BaseSection CurrentSection {
			get {
				return (BaseSection)sections[sectionInUse];
			}
		}
		
		protected int Gap {
			get {
				return gap;
			}
		}
		protected Point DetailEnds {
			get {
				return detailEnds;
			}
			set {
				detailEnds = value;
			}
		}
		
		protected Point DetailStart {
			get {
				return detailStart;
			}
			set {
				detailStart = value;
			}
		}
		#endregion
	
		#region IDispoable
		public virtual void Dispose(){
			if (this.reportDocument != null) {
				this.reportDocument.Dispose();
			}
		}
		#endregion
	}
}
