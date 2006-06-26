//
// SharpDevelop ReportEditor
//
// Copyright (C) 2005 Peter Forstmeier
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
	
		private StandartFormatter standartFormatter;
		private bool cancel;		
		
		public event EventHandler<SectionRenderEventArgs> SectionRendering;
		public event EventHandler<SectionRenderEventArgs> SectionRendered;
		
		protected AbstractRenderer(ReportModel model){
			if (model == null) {
				throw new MissingModelException();
			}
			this.reportSettings = model.ReportSettings;
			this.sections = model.SectionCollection;
			Init();
			standartFormatter = new StandartFormatter();
		}
		
		public virtual void SetupRenderer () {
			this.cancel = false;
		}
		
		void Init() {
			reportDocument = new SharpReportCore.ReportDocument();
			reportDocument.DocumentName = reportSettings.ReportName;
			
			// Events from ReportDocument
			reportDocument.QueryPageSettings += new QueryPageSettingsEventHandler (ReportQueryPage);
			reportDocument.BeginPrint += new PrintEventHandler(ReportBegin);
			reportDocument.EndPrint += new PrintEventHandler(ReportEnd);
			
			// homemade events
			reportDocument.PrintPageBodyStart += new EventHandler<ReportPageEventArgs> (BodyStart);
			
			reportDocument.PrintPageBodyEnd += new EventHandler<ReportPageEventArgs> (OnBodyEnd);

			//
			reportDocument.RenderReportHeader += new EventHandler<ReportPageEventArgs> (PrintReportHeader);
			reportDocument.RenderPageHeader += new EventHandler<ReportPageEventArgs> (PrintPageHeader);
			reportDocument.RenderDetails += new EventHandler<ReportPageEventArgs> (PrintDetail);
			reportDocument.RenderPageEnd += new EventHandler<ReportPageEventArgs> (PrintPageEnd);
			reportDocument.RenderReportEnd += new EventHandler<ReportPageEventArgs> (PrintReportFooter);
		}
		#region Event handling for SectionRendering
		
		protected void AddSectionEvents () {
			this.CurrentSection.SectionPrinting += new EventHandler<SectionEventArgs>(OnSectionPrinting);
			this.CurrentSection.SectionPrinted += new EventHandler<SectionEventArgs>(OnSectionPrinted);
		}
		
		
		protected void RemoveSectionEvents () {
			this.CurrentSection.SectionPrinting -= new EventHandler<SectionEventArgs>(OnSectionPrinting);
			this.CurrentSection.SectionPrinted -= new EventHandler<SectionEventArgs>(OnSectionPrinted);
		}
		
		
		private  void OnSectionPrinting (object sender,SectionEventArgs e) {
			if (this.SectionRendering != null) {
				SectionRenderEventArgs ea = new SectionRenderEventArgs (e.Section,
				                                                        this.reportDocument.PageNumber,0,
				                                                        (GlobalEnums.enmSection)this.sectionInUse);
				this.SectionRendering(this,ea);
			} 
		}
		
		
		private void OnSectionPrinted (object sender,SectionEventArgs e) {
			if (this.SectionRendered != null) {
				SectionRenderEventArgs ea = new SectionRenderEventArgs (e.Section,
				                                                        this.reportDocument.PageNumber,0,
				                                                        (GlobalEnums.enmSection)this.sectionInUse);
				this.SectionRendered(this,ea);
			}
		}
		
		#endregion
		
		protected virtual void PrintReportHeader (object sender, ReportPageEventArgs e) {
			SectionInUse = Convert.ToInt16(GlobalEnums.enmSection.ReportHeader,
			                               CultureInfo.InvariantCulture);
			this.AddSectionEvents();
		}
		
		protected virtual void PrintPageHeader (object sender, ReportPageEventArgs e) {
			SectionInUse = Convert.ToInt16(GlobalEnums.enmSection.ReportPageHeader,
			                               CultureInfo.InvariantCulture);
			this.AddSectionEvents();
		}
		
		protected virtual void  PrintDetail (object sender,ReportPageEventArgs rpea) {
			SectionInUse = Convert.ToInt16(GlobalEnums.enmSection.ReportDetail,
			                               CultureInfo.InvariantCulture);
			this.AddSectionEvents();
//			System.Console.WriteLine("\tAbstract - PrintDetail");
		}
		
		protected virtual void  PrintPageEnd (object sender,ReportPageEventArgs rpea) {	
			SectionInUse = Convert.ToInt16(GlobalEnums.enmSection.ReportPageFooter,
			                               CultureInfo.InvariantCulture);
//			System.Console.WriteLine("\tAbstract:PrintPageEnd");
			this.AddSectionEvents();

		}
		protected virtual void  PrintReportFooter (object sender,ReportPageEventArgs rpea) {	
			SectionInUse = Convert.ToInt16(GlobalEnums.enmSection.ReportFooter,
			                               CultureInfo.InvariantCulture);
//			System.Console.WriteLine("\tAbstract:PrintReportFooter");
			this.AddSectionEvents();
		}
		
		protected static void PageBreak(ReportPageEventArgs pea) {
			if (pea == null) {
				throw new ArgumentNullException("pea");
			}
			pea.PrintPageEventArgs.HasMorePages = true;
			pea.ForceNewPage = true;
		}
		
		protected bool CheckPageBreakAfter () {
			if (this.CurrentSection.PageBreakAfter) {
				return true;
			}
			return false;
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
		
		protected static void DebugRectangle (ReportPageEventArgs rpea,Rectangle rectangle) {
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}
			rpea.PrintPageEventArgs.Graphics.DrawRectangle (Pens.Black,rectangle);
		}
		
		/// <summary>
		/// Calculates the rectangle wich can be used by Detail
		/// </summary>
		/// <returns></returns>
		protected Rectangle DetailRectangle (ReportPageEventArgs rpea) {
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}
			sectionInUse = Convert.ToInt16(GlobalEnums.enmSection.ReportDetail,
			                               CultureInfo.InvariantCulture);
			
			Rectangle rect = new Rectangle (rpea.PrintPageEventArgs.MarginBounds.Left,
			                               this.detailStart.Y ,
			                                rpea.PrintPageEventArgs.MarginBounds.Width,
			                                detailEnds.Y - detailStart.Y - (3 * gap));
			return rect;
		}
		
		protected PointF MeasureReportHeader (ReportPageEventArgs rpea) {
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}
			PointF endAt = new PointF();
			if (rpea.PageNumber == 1) {
				sectionInUse = Convert.ToInt16(GlobalEnums.enmSection.ReportHeader,
				                               CultureInfo.InvariantCulture);
				if (this.CurrentSection.Items.Count > 0) {
					this.CurrentSection.SectionOffset = reportSettings.DefaultMargins.Top;
					FitSectionToItems (this.CurrentSection,rpea);
					endAt = new PointF (0,
			                   reportSettings.DefaultMargins.Top + this.CurrentSection.Size.Height + Gap);
				} else {
					endAt = new PointF(0,reportSettings.DefaultMargins.Top);
				}
				
			}
			return endAt;
		}
		
		
		///</summary>
		/// <param name="startAt">Section start at this PointF</param>
		/// <param name="e">ReportPageEventArgs</param>
		protected PointF MeasurePageHeader (PointF startat,ReportPageEventArgs rpea) {
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}
			sectionInUse = Convert.ToInt16(GlobalEnums.enmSection.ReportPageHeader,
			                               CultureInfo.InvariantCulture);

			if (rpea.PageNumber == 1) {
				this.CurrentSection.SectionOffset = (int)startat.Y + Gap;
			} else {
				this.CurrentSection.SectionOffset = reportSettings.DefaultMargins.Top;
			}

			FitSectionToItems (this.CurrentSection,rpea);
			return new PointF (0,
			                   this.CurrentSection.SectionOffset + this.CurrentSection.Size.Height + Gap);
		}
		
		protected PointF  MeasurePageEnd (ReportPageEventArgs e) {
			sectionInUse = Convert.ToInt16(GlobalEnums.enmSection.ReportPageFooter,
			                               CultureInfo.InvariantCulture);
			this.CurrentSection.SectionOffset = reportSettings.PageSettings.Bounds.Height - reportSettings.DefaultMargins.Top - reportSettings.DefaultMargins.Bottom;
			FitSectionToItems (this.CurrentSection,e);
			this.DetailEnds = new Point (0,this.CurrentSection.SectionOffset);
			return new PointF(0,this.CurrentSection.SectionOffset);
		}
		
		
		protected PointF MeasureReportFooter (ReportPageEventArgs e) {
			sectionInUse = Convert.ToInt16(GlobalEnums.enmSection.ReportFooter,
			                                   CultureInfo.InvariantCulture);
			FitSectionToItems (this.CurrentSection,e);
			return new PointF(0,this.CurrentSection.SectionOffset + this.CurrentSection.Size.Height);
		}
		
		protected virtual int RenderSection (ReportPageEventArgs rpea) {
			Point drawPoint	= new Point(0,0);
			
			if (this.CurrentSection.Visible){
				this.CurrentSection.Render (rpea);
				
				foreach (BaseReportItem item in this.CurrentSection.Items) {
					if (item.Parent == null) {
						item.Parent = this.CurrentSection;
					}
					item.SectionOffset = this.CurrentSection.SectionOffset;
					this.DrawSingleItem (rpea,item);
					
					drawPoint.Y = this.CurrentSection.SectionOffset + this.CurrentSection.Size.Height;
					rpea.LocationAfterDraw = new PointF (rpea.LocationAfterDraw.X,this.CurrentSection.SectionOffset + this.CurrentSection.Size.Height);
					
				}
				
				if ((this.CurrentSection.CanGrow == false)&& (this.CurrentSection.CanShrink == false)) {
					return this.CurrentSection.Size.Height;
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
			BaseDataItem baseDataItem = sender as BaseDataItem;
			
			if (baseDataItem != null) {
				if (!String.IsNullOrEmpty(baseDataItem.FormatString)) {
					
					rpea.FormatedValue = standartFormatter.FormatItem (baseDataItem);
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
				AbstractRenderer.AdjustSection (section,rpea);
			} else {
				AbstractRenderer.AdjustItems (section,rpea);
				
			}
		}
		
		private static void AdjustItems (BaseSection section,ReportPageEventArgs e){

			int toFit = section.Size.Height;
			foreach (BaseReportItem rItem in section.Items) {
				if (!AbstractRenderer.CheckItemInSection (section,rItem,e)){
					
					rItem.Size = new Size (rItem.Size.Width,
					                       toFit - rItem.Location.Y);
					
				}
			}
		}
		
		private static void AdjustSection (BaseSection section,ReportPageEventArgs e){
			
			foreach (BaseReportItem rItem in section.Items) {
				if (!AbstractRenderer.CheckItemInSection (section,rItem,e)){
					
					SizeF size = AbstractRenderer.MeasureReportItem (rItem,e);
					
					section.Size = new Size (section.Size.Width,
					                         Convert.ToInt32(rItem.Location.Y + size.Height));
			
				}
			}
		}
		
		
		private static bool CheckItemInSection (BaseSection section,BaseReportItem item ,ReportPageEventArgs e) {
			Rectangle secRect = new Rectangle (0,0,section.Size.Width,section.Size.Height);
			SizeF size = AbstractRenderer.MeasureReportItem(item,e);
			Rectangle itemRect = new Rectangle (item.Location.X,
			                                    item.Location.Y,
			                                    (int)size.Width,
			                                    (int)size.Height);
			if (secRect.Contains(itemRect)) {
				return true;
			}
			return false;
		}
		
		private static SizeF MeasureReportItem(IItemRenderer item,
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
		
				sizeF = e.PrintPageEventArgs.Graphics.MeasureString(str,
				                                                    myItem.Font,
				                                                    myItem.Size.Width,
				                                                    myItem.StringFormat);
			} else {
				sizeF = new SizeF (item.Size.Width,item.Size.Height);
			}
		
			return sizeF;
		}
		#endregion
		
		
		#region virtuals
		
		protected virtual void ReportQueryPage (object sender,QueryPageSettingsEventArgs qpea) {
			qpea.PageSettings.Margins = reportSettings.DefaultMargins;
		}
		
		
		protected virtual void  ReportBegin (object sender,PrintEventArgs pea) {
//			System.Console.WriteLine("\tAbstract - ReportBegin");
		}
		
		
		protected virtual void  BodyStart (object sender,ReportPageEventArgs rpea) {
//			System.Console.WriteLine("\tAbstract - PrintBodyStart");
			this.SectionInUse = Convert.ToInt16(GlobalEnums.enmSection.ReportDetail,
			                                    CultureInfo.InvariantCulture);
			
		}
		
		
	
		protected virtual void  OnBodyEnd (object sender,ReportPageEventArgs rpea) {
//			System.Console.WriteLine("\tAbstarct - PrintBodyEnd");
		this.SectionInUse = Convert.ToInt16(GlobalEnums.enmSection.ReportFooter,
			                                    CultureInfo.InvariantCulture);
		}
		
		protected virtual void  ReportEnd (object sender,PrintEventArgs e) {
//			System.Console.WriteLine("\tAbstract - ReportEnd");
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
		
		protected static int Gap {
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
		public  void Dispose(){
			System.Console.WriteLine("base:Dispose()");
			if (this.reportDocument != null) {
				this.reportDocument.Dispose();
			}
		}
		#endregion
	}
}
