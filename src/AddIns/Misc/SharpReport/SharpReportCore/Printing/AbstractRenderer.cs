//
// SharpDevelop ReportEditor
//
// Copyright (C) 2005 Peter Forstmeier
//

// Peter Forstmeier (Peter.Forstmeier@t-online.de)


using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;

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
	
		private bool cancel;		
		
		public event EventHandler<SectionRenderEventArgs> Rendering;
		public event EventHandler<SectionRenderEventArgs> SectionRendered;
		
		public Page page;
		
		protected AbstractRenderer(ReportModel model){
			if (model == null) {
				throw new MissingModelException();
			}
			this.reportSettings = model.ReportSettings;
			this.sections = model.SectionCollection;
			Init();
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
			reportDocument.PrintPage += new PrintPageEventHandler(ReportPageStart);
			reportDocument.EndPrint += new PrintEventHandler(ReportEnd);

			// homemade events
			reportDocument.BodyStart += new EventHandler<ReportPageEventArgs> (BodyStart);
			
			reportDocument.BodyEnd += new EventHandler<ReportPageEventArgs> (BodyEnd);

			//
			reportDocument.RenderReportHeader += new EventHandler<ReportPageEventArgs> (PrintReportHeader);
			reportDocument.RenderPageHeader += new EventHandler<ReportPageEventArgs> (PrintPageHeader);
			reportDocument.RenderDetails += new EventHandler<ReportPageEventArgs> (PrintDetail);
			reportDocument.RenderPageEnd += new EventHandler<ReportPageEventArgs> (PrintPageEnd);
			reportDocument.RenderReportEnd += new EventHandler<ReportPageEventArgs> (PrintReportFooter);
		}
		
		#region Event handling for SectionRendering
		
		protected void AddSectionEvents () {
			System.Console.WriteLine("AddSectionEvents for <{0}>",this.CurrentSection.Name);
			this.CurrentSection.SectionPrinting += new EventHandler<SectionEventArgs>(OnSectionPrinting);
			this.CurrentSection.SectionPrinted += new EventHandler<SectionEventArgs>(OnSectionPrinted);
		}
		
		
		protected void RemoveSectionEvents () {
			System.Console.WriteLine("RemoveSectionEvents for <{0}>",this.CurrentSection.Name);
			System.Console.WriteLine("");
			this.CurrentSection.SectionPrinting -= new EventHandler<SectionEventArgs>(OnSectionPrinting);
			this.CurrentSection.SectionPrinted -= new EventHandler<SectionEventArgs>(OnSectionPrinted);
		}
		
		
		private  void OnSectionPrinting (object sender,SectionEventArgs e) {
			if (this.Rendering != null) {
				SectionRenderEventArgs ea = new SectionRenderEventArgs (e.Section,
				                                                        this.reportDocument.PageNumber,0,
				                                                        (GlobalEnums.enmSection)this.sectionInUse);
				this.Rendering(this,ea);
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
		
		#region SharpReport Events
		
		protected virtual void PrintReportHeader (object sender, ReportPageEventArgs rpea) {
			SectionInUse = Convert.ToInt16(GlobalEnums.enmSection.ReportHeader,
			                               CultureInfo.InvariantCulture);
			this.AddSectionEvents();
		}
		
		protected virtual void PrintPageHeader (object sender, ReportPageEventArgs rpea) {
			SectionInUse = Convert.ToInt16(GlobalEnums.enmSection.ReportPageHeader,
			                               CultureInfo.InvariantCulture);
			this.AddSectionEvents();
		}
		
		
		protected virtual void  BodyStart (object sender,ReportPageEventArgs rpea) {
//			System.Console.WriteLine("\tAbstract - PrintBodyStart");
			this.SectionInUse = Convert.ToInt16(GlobalEnums.enmSection.ReportDetail,
			                                    CultureInfo.InvariantCulture);
			
		}
	
		
		protected virtual void  PrintDetail (object sender,ReportPageEventArgs rpea) {
			SectionInUse = Convert.ToInt16(GlobalEnums.enmSection.ReportDetail,
			                               CultureInfo.InvariantCulture);
			this.AddSectionEvents();
		}
		
		
		protected virtual void  BodyEnd (object sender,ReportPageEventArgs rpea) {
//			System.Console.WriteLine("\tAbstarct - PrintBodyEnd");
			this.SectionInUse = Convert.ToInt16(GlobalEnums.enmSection.ReportFooter,
			                                    CultureInfo.InvariantCulture);
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
		
		#endregion
		
		protected static void PageBreak(ReportPageEventArgs pea) {
			if (pea == null) {
				throw new ArgumentNullException("pea");
			}
			pea.PrintPageEventArgs.HasMorePages = true;
			pea.ForceNewPage = true;
		}
		
		#region Debugg Code
		///<summary>
		/// Use this function to draw controlling rectangles
		/// For debugging only
		/// </summary>	
		
		public static void DebugRectangle (PrintPageEventArgs rpea,Rectangle rectangle) {
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}
			rpea.Graphics.DrawRectangle (Pens.Black,rectangle);
		}
		
		public void DebugFooterRectangle (ReportPageEventArgs rpea) {
			Rectangle r =  new Rectangle( this.page.ReportFooterRectangle.Left,
			                             rpea.LocationAfterDraw.Y,
			                             this.page.ReportFooterRectangle.Width,
			                             this.page.ReportFooterRectangle.Height);
			
			Rectangle s = new Rectangle (this.page.ReportFooterRectangle.Left,
			                             rpea.LocationAfterDraw.Y,
			                             
			                             this.page.ReportFooterRectangle.Width,
			                             this.page.PageFooterRectangle.Top - rpea.LocationAfterDraw.Y -1);
			
			AbstractRenderer.DebugRectangle(rpea.PrintPageEventArgs,r);
			AbstractRenderer.DebugRectangle(rpea.PrintPageEventArgs,s);
		}
		
		#endregion
		
		protected Rectangle MeasureReportHeader (PrintPageEventArgs ppea) {
			if (ppea == null) {
				throw new ArgumentNullException("ppea");
			}
			
			Rectangle rect = new Rectangle();
			if (this.reportDocument.PageNumber == 1) {

				sectionInUse = Convert.ToInt16(GlobalEnums.enmSection.ReportHeader,
				                               CultureInfo.InvariantCulture);

				if (this.CurrentSection.Items.Count > 0) {
					this.CurrentSection.SectionOffset = reportSettings.DefaultMargins.Top;
					FitSectionToItems (this.CurrentSection,ppea);

					rect = new Rectangle(reportSettings.DefaultMargins.Left,
					                     reportSettings.DefaultMargins.Top,
					                     ppea.MarginBounds.Width,
					                     this.CurrentSection.Size.Height + Gap);
				} else {

					rect = new Rectangle (reportSettings.DefaultMargins.Left,
					                      reportSettings.DefaultMargins.Top,
					                      ppea.MarginBounds.Width,
					                      0);
				}
				
			}
			return rect;
		}
		
		
		///</summary>
		/// <param name="startAt">Section start at this PointF</param>
		/// <param name="e">ReportPageEventArgs</param>
		private Rectangle MeasurePageHeader (Rectangle startAfter,PrintPageEventArgs rpea) {
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}
			
			sectionInUse = Convert.ToInt16(GlobalEnums.enmSection.ReportPageHeader,
			                               CultureInfo.InvariantCulture);

			if (this.reportDocument.PageNumber == 1){
				this.CurrentSection.SectionOffset = (int)startAfter.Top + Gap;
			} else {
				this.CurrentSection.SectionOffset = reportSettings.DefaultMargins.Top;
			}

			FitSectionToItems (this.CurrentSection,rpea);
			return new Rectangle (startAfter.Left,
			                      startAfter.Bottom + Gap,
			                      rpea.MarginBounds.Width,
			                      this.CurrentSection.Size.Height + Gap);
		}
		
		
		private Rectangle  MeasurePageFooter (PrintPageEventArgs e) {
			sectionInUse = Convert.ToInt16(GlobalEnums.enmSection.ReportPageFooter,
			                               CultureInfo.InvariantCulture);
			this.CurrentSection.SectionOffset = reportSettings.PageSettings.Bounds.Height - reportSettings.DefaultMargins.Top - reportSettings.DefaultMargins.Bottom;
			FitSectionToItems (this.CurrentSection,e);
			return new Rectangle(reportSettings.DefaultMargins.Left,
			                     this.CurrentSection.SectionOffset,
			                      e.MarginBounds.Width,
			                      this.CurrentSection.Size.Height);
			
			                     
		}
		
		
		private Rectangle MeasureReportFooter (PrintPageEventArgs ppea) {
			sectionInUse = Convert.ToInt16(GlobalEnums.enmSection.ReportFooter,
			                                   CultureInfo.InvariantCulture);
			FitSectionToItems (this.CurrentSection,ppea);
			return new Rectangle (reportSettings.DefaultMargins.Left,
			                      this.CurrentSection.SectionOffset,
			                      ppea.MarginBounds.Width,
			                      this.CurrentSection.Size.Height);                   
		}
		
		protected bool IsRoomForFooter(Point loc) {
			Rectangle r =  new Rectangle( this.page.ReportFooterRectangle.Left,
			                             loc.Y,
			                             this.page.ReportFooterRectangle.Width,
			                             this.page.ReportFooterRectangle.Height);
			
			Rectangle s = new Rectangle (this.page.ReportFooterRectangle.Left,
			                             loc.Y,
			                             
			                             this.page.ReportFooterRectangle.Width,
			                             this.page.PageFooterRectangle.Top - loc.Y -1);
			return s.Contains(r);
		}
		
		protected virtual int RenderSection (ReportPageEventArgs rpea) {

			Point drawPoint	= new Point(0,0);
			if (this.CurrentSection.Visible){
				this.CurrentSection.Render (rpea);
				
				foreach (BaseReportItem item in this.CurrentSection.Items) {
					if (item.Parent == null) {
						item.Parent = this.CurrentSection;
					}
					
					//test for container
					IContainerItem	container = item as IContainerItem;
//					if (container != null) {
//						System.Console.WriteLine("\tContainer {0}",item.Name);
//
//					} else {
//						System.Console.WriteLine("\tStandart Offset for <{0}>",item.Name);
//						
//					}
					
					item.SectionOffset = this.CurrentSection.SectionOffset;
					item.Render(rpea);
					drawPoint.Y = this.CurrentSection.SectionOffset + this.CurrentSection.Size.Height;
					rpea.LocationAfterDraw = new Point (rpea.LocationAfterDraw.X,
					                                     this.CurrentSection.SectionOffset + this.CurrentSection.Size.Height);
					
				}
				
				if ((this.CurrentSection.CanGrow == false)&& (this.CurrentSection.CanShrink == false)) {
					return this.CurrentSection.Size.Height;
				}
				
				return drawPoint.Y;
			}
			return drawPoint.Y;
		}
		
		#region privates
		protected void FitSectionToItems (BaseSection section,PrintPageEventArgs rpea){
			if (section == null) {
				throw new ArgumentNullException("section");
			}
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}
			Rectangle orgRect = new Rectangle (rpea.MarginBounds.Left,
			                                   section.SectionOffset,
			                                   rpea.MarginBounds.Width,
			                                   section.Size.Height);
			
			if ((section.CanGrow == true)||(section.CanShrink == true))  {
				AbstractRenderer.AdjustSection (section,rpea);
			} else {
				AbstractRenderer.AdjustItems (section,rpea);
				
			}
		}
		
		private static void AdjustItems (BaseSection section,PrintPageEventArgs e){

			int toFit = section.Size.Height;
			foreach (BaseReportItem rItem in section.Items) {
				if (!AbstractRenderer.CheckItemInSection (section,rItem,e)){
					
					rItem.Size = new Size (rItem.Size.Width,
					                       toFit - rItem.Location.Y);
					
				}
			}
		}
		
		private static void AdjustSection (BaseSection section,PrintPageEventArgs e){
			
			foreach (BaseReportItem rItem in section.Items) {
				if (!AbstractRenderer.CheckItemInSection (section,rItem,e)){
					
					SizeF size = AbstractRenderer.MeasureReportItem (rItem,e);
					
					section.Size = new Size (section.Size.Width,
					                         Convert.ToInt32(rItem.Location.Y + size.Height));
			
				}
			}
		}
		
		
		private static bool CheckItemInSection (BaseSection section,BaseReportItem item ,PrintPageEventArgs e) {
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
		                                       PrintPageEventArgs e) {
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
				
				sizeF = e.Graphics.MeasureString(str,
				                                 myItem.Font,
				                                 myItem.Size.Width,
				                                 myItem.StringFormat);
			} else {
				sizeF = new SizeF (item.Size.Width,item.Size.Height);
			}
			
			return sizeF;
		}
		#endregion
		
		
		#region PrintDocument Events
		private void ReportPageStart (object sender, PrintPageEventArgs e) {
			if (this.page == null) {
				throw new ArgumentException("page");
			}
			
			Rectangle r1;
			
			if (this.reportDocument.PageNumber == 1) {
				r1 = this.MeasureReportHeader(e);
				page.ReportHeaderRectangle = r1;
			} else {
				r1 = new Rectangle (reportSettings.DefaultMargins.Left,
				                    reportSettings.DefaultMargins.Top,
				                    e.MarginBounds.Width,
				                    0);
			}
			page.ReportHeaderRectangle = r1;
			page.PageHeaderRectangle = this.MeasurePageHeader(r1,e);
			page.PageFooterRectangle = this.MeasurePageFooter (e);			
			page.ReportFooterRectangle = this.MeasureReportFooter(e);
		}
		
		protected virtual void ReportQueryPage (object sender,QueryPageSettingsEventArgs qpea) {
			qpea.PageSettings.Margins = reportSettings.DefaultMargins;
			
			if (this.reportDocument.PageNumber == 1) {
				page = new Page (true);
			} else {
				page = new Page (false);
			}
			
		}
		
		
		protected virtual void  ReportBegin (object sender,PrintEventArgs pea) {
//			System.Console.WriteLine("\tAbstract - ReportBegin");
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
		
		protected Page Page {
			get {
				return page;
			}
			set {
				page = value;
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
