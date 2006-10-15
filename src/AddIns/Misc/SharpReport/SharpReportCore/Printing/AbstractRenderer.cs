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
		
		private SectionBounds sectionBounds;
		
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
		
		protected void PrintNoDataMessage(PrintPageEventArgs e){
			if (e == null) {
				throw new ArgumentNullException("e");
			}
			e.Graphics.DrawString(this.reportSettings.NoDataMessage,
			                                            this.ReportSettings.DefaultFont,
			                                            new SolidBrush(Color.Black),
			                                            sectionBounds.DetailArea);
			
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
		/*
		public void old_DebugFooterRectangle (ReportPageEventArgs rpea) {
			
			Rectangle r =  new Rectangle( this.pageBounderys.ReportFooterRectangle.Left,
			                             rpea.LocationAfterDraw.Y,
			                             this.pageBounderys.ReportFooterRectangle.Width,
			                             this.pageBounderys.ReportFooterRectangle.Height);
			
			Rectangle s = new Rectangle (this.pageBounderys.ReportFooterRectangle.Left,
			                             rpea.LocationAfterDraw.Y,
			                             
			                             this.pageBounderys.ReportFooterRectangle.Width,
			                             this.pageBounderys.PageFooterRectangle.Top - rpea.LocationAfterDraw.Y -1);
			
			AbstractRenderer.DebugRectangle(rpea.PrintPageEventArgs,r);
			AbstractRenderer.DebugRectangle(rpea.PrintPageEventArgs,s);
		}
		*/
		#endregion
		
		
		
		///</summary>
		/// <param name="startAt">Section start at this PointF</param>
		/// <param name="e">Graphics</param>

		
		
		protected bool IsRoomForFooter(Point loc) {
			Rectangle r =  new Rectangle( this.sectionBounds.ReportFooterRectangle.Left,
			                             loc.Y,
			                             this.sectionBounds.ReportFooterRectangle.Width,
			                             this.sectionBounds.ReportFooterRectangle.Height);
			
			Rectangle s = new Rectangle (this.sectionBounds.ReportFooterRectangle.Left,
			                             loc.Y,
			                             
			                             this.sectionBounds.ReportFooterRectangle.Width,
			                             this.sectionBounds.PageFooterRectangle.Top - loc.Y -1);
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
		
		
		
		#region PrintDocument Events
		
		private void CalculatePageBounds (Graphics graphics) {
			Rectangle rectangle;
			sectionInUse = Convert.ToInt16(GlobalEnums.enmSection.ReportHeader,
			                               CultureInfo.InvariantCulture);
			if (this.reportDocument.PageNumber == 1) {
				
				rectangle = sectionBounds.MeasureReportHeader(this.CurrentSection,graphics);
				
				
			} else {
				rectangle = new Rectangle (reportSettings.DefaultMargins.Left,
				                    reportSettings.DefaultMargins.Top,
				                    this.sectionBounds.MarginBounds.Width,
				                    0);
			}
			
			this.CurrentSection.SectionOffset = this.reportSettings.DefaultMargins.Top;
			sectionBounds.ReportHeaderRectangle = rectangle;
			
			//PageHeader
			sectionInUse = Convert.ToInt16(GlobalEnums.enmSection.ReportPageHeader,
			                               CultureInfo.InvariantCulture);
			
			this.sectionBounds.MeasurePageHeader(this.CurrentSection,rectangle,graphics);
			                                     
			
			//PageFooter
			sectionInUse = Convert.ToInt16(GlobalEnums.enmSection.ReportPageFooter,
			                               CultureInfo.InvariantCulture);
	
 			this.sectionBounds.MeasurePageFooter (this.CurrentSection,graphics);
 			
 			//ReportFooter
 			sectionInUse = Convert.ToInt16(GlobalEnums.enmSection.ReportFooter,
			                               CultureInfo.InvariantCulture);
 			sectionBounds.ReportFooterRectangle = this.sectionBounds.MeasureReportFooter(this.CurrentSection,
 			                                                                             graphics);
		}
		
		private void ReportPageStart (object sender, PrintPageEventArgs ppea) {
			if (this.sectionBounds == null) {
				throw new ArgumentException("page");
			}
			this.CalculatePageBounds (ppea.Graphics);
		}
		
		protected virtual void ReportQueryPage (object sender,QueryPageSettingsEventArgs qpea) {
			qpea.PageSettings.Margins = reportSettings.DefaultMargins;
			
			if (this.reportDocument.PageNumber == 0) {
				sectionBounds = new SectionBounds (qpea.PageSettings,true,AbstractRenderer.Gap);
			} else {
				sectionBounds = new SectionBounds (qpea.PageSettings,false,AbstractRenderer.Gap);
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
		
		protected SectionBounds SectionBounds {
			get {
				return sectionBounds;
			}
			set {
				sectionBounds = value;
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
