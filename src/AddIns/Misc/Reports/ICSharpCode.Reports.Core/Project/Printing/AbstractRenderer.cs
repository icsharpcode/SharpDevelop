// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>


using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;

using ICSharpCode.Reports.Core.Interfaces;
using ICSharpCode.Reports.Expressions.ReportingLanguage;

	/// <summary>
	/// Base Class for Rendering Reports
	/// 
	/// </summary>
	/// <remarks>
	/// 	created by - Forstmeier Peter
	/// 	created on - 13.12.2004 09:55:16
	/// </remarks>
	/// 
namespace ICSharpCode.Reports.Core 
{
	public abstract class AbstractRenderer : IDisposable
	{
		private const int gap = 1;
		
		private ReportDocument reportDocument;
		private ReportSectionCollection sections;
		private ReportSettings reportSettings;
		
		private int currentPageNumber;		
		
		private IExpressionEvaluatorFacade expressionFassade;
		private ILayouter layout;
		private IReportModel reportModel;
		
		public event EventHandler<SectionRenderEventArgs> Rendering;
		public event EventHandler<SectionRenderEventArgs> SectionRendered;
		
		
		protected AbstractRenderer(IReportModel reportModel,ReportDocument reportDocument,ILayouter layout)
		{
			if (reportModel == null) {
				throw new MissingModelException();
			}
			if (reportDocument == null) {
				throw new ArgumentNullException("reportDocument");
			}
			if (layout == null) {
				throw new ArgumentNullException("layout");
			}
			this.reportModel = reportModel;
			this.reportSettings = reportModel.ReportSettings;
			this.reportDocument = reportDocument;
			this.layout = layout;
			this.sections = reportModel.SectionCollection;
			Init();
		}
		
		
		private void Init()
		{
			this.reportDocument.DocumentName = reportSettings.ReportName;
			
			// Events from ReportDocument
			this.reportDocument.QueryPageSettings += new QueryPageSettingsEventHandler (ReportQueryPage);
			this.reportDocument.BeginPrint += new PrintEventHandler(ReportBegin);
			
			this.reportDocument.PrintPage += delegate(object sender,PrintPageEventArgs e){
				this.CalculatePageBounds ();
			};
			
			this.reportDocument.EndPrint += new PrintEventHandler(ReportEnd);

			// homemade events
			this.reportDocument.BodyStart += new EventHandler<ReportPageEventArgs> (BodyStart);
			
			this.reportDocument.BodyEnd += new EventHandler<ReportPageEventArgs> (BodyEnd);

			//
			this.reportDocument.RenderReportHeader += new EventHandler<ReportPageEventArgs> (PrintReportHeader);
			this.reportDocument.RenderPageHeader += new EventHandler<ReportPageEventArgs> (PrintPageHeader);
			this.reportDocument.RenderDetails += new EventHandler<ReportPageEventArgs> (PrintDetail);
			this.reportDocument.RenderPageEnd += new EventHandler<ReportPageEventArgs> (PrintPageEnd);
			this.reportDocument.RenderReportEnd += new EventHandler<ReportPageEventArgs> (PrintReportFooter);
			this.expressionFassade = new ExpressionEvaluatorFacade();
		}
		
		
		#region Event handling for SectionRendering
		
		protected void AddSectionEvents () 
		{
			this.CurrentSection.SectionPrinting += new EventHandler<SectionEventArgs>(OnSectionPrinting);
			this.CurrentSection.SectionPrinted += new EventHandler<SectionEventArgs>(OnSectionPrinted);
		}
		
		
		protected void RemoveSectionEvents ()
		{
			this.CurrentSection.SectionPrinting -= new EventHandler<SectionEventArgs>(OnSectionPrinting);
			this.CurrentSection.SectionPrinted -= new EventHandler<SectionEventArgs>(OnSectionPrinted);
		}
		
		
		private  void OnSectionPrinting (object sender,SectionEventArgs e)
		{
			if (this.Rendering != null) {
				SectionRenderEventArgs ea = new SectionRenderEventArgs (e.Section,
				                                                        this.currentPageNumber,
				                                                        this.CurrentRow,
				                                                        this.CurrentSection);
				this.Rendering(this,ea);
			}
		}
		
		
		private void OnSectionPrinted (object sender,SectionEventArgs e)
		{
			if (this.SectionRendered != null) {
				SectionRenderEventArgs ea = new SectionRenderEventArgs (e.Section,
				                                                        this.currentPageNumber,
				                                                        this.CurrentRow,
				                                                        this.CurrentSection);
				this.SectionRendered(this,ea);
			}
		}
		
		#endregion
		
		
		#region SharpReport Events
		
		internal virtual void PrintReportHeader (object sender, ReportPageEventArgs rpea)
		{
			this.CurrentSection = this.reportModel.ReportHeader;
			this.AddSectionEvents();
		}
		
		internal virtual void PrintPageHeader (object sender, ReportPageEventArgs rpea) 
		{
			this.CurrentSection = this.reportModel.PageHeader;
			this.AddSectionEvents();
		}
		

		internal virtual void  BodyStart (object sender,ReportPageEventArgs rpea) 
		{
			this.CurrentSection = this.reportModel.DetailSection;
		}
	
		
		internal virtual void  PrintDetail (object sender,ReportPageEventArgs rpea) 
		{
			this.CurrentSection = this.reportModel.DetailSection;
			this.AddSectionEvents();
		}
		
		
		internal virtual void  BodyEnd (object sender,ReportPageEventArgs rpea) 
		{
			this.CurrentSection = this.reportModel.ReportFooter;
		}
		
		
		internal virtual void  PrintPageEnd (object sender,ReportPageEventArgs rpea) 
		{
			this.CurrentSection = this.reportModel.ReportFooter;
			this.AddSectionEvents();
		}
		
		
		internal virtual void  PrintReportFooter (object sender,ReportPageEventArgs rpea) 
		{
			this.CurrentSection = this.reportModel.ReportFooter;
			this.AddSectionEvents();
		}
		
		
		#endregion
		
		public  static void PageBreak(ReportPageEventArgs pea) 
		{
			if (pea == null) {
				throw new ArgumentNullException("pea");
			}
			pea.PrintPageEventArgs.HasMorePages = true;
			pea.ForceNewPage = true;
		}
		
		
		protected void PrintNoDataMessage(PrintPageEventArgs e)
		{
			if (e == null) {
				throw new ArgumentNullException("e");
			}
			e.Graphics.DrawString(this.reportSettings.NoDataMessage,
			                      this.ReportSettings.DefaultFont,
			                      new SolidBrush(Color.Black),
			                      this.SectionBounds.DetailArea);
		}
		
	
		protected virtual Point RenderSection (ReportPageEventArgs rpea)
		{
			Point drawPoint	= Point.Empty;
			
			if (this.CurrentSection.Visible){
				this.CurrentSection.Render (rpea);
				expressionFassade.SinglePage = this.reportDocument.SinglePage;
				
				if (this.CurrentSection.Items.Count > 0) {

					Rectangle desiredRectangle = Layout.Layout(rpea.PrintPageEventArgs.Graphics,this.CurrentSection);
					
					Rectangle sectionRectangle = new Rectangle(this.CurrentSection.Location.X,
					                                           this.CurrentSection.Location.Y,
					                                           this.CurrentSection.Size.Width,
					                                           this.CurrentSection.Size.Height);
					if (desiredRectangle.Height >= sectionRectangle.Height) {
						this.CurrentSection.Size = new Size(this.CurrentSection.Size.Width,desiredRectangle.Height + 10);
					}
					
//					PrintHelper.DebugRectangle(rpea.PrintPageEventArgs.Graphics,Pens.Blue,new Rectangle(CurrentSection.Location,CurrentSection.Size));
				}
				
				foreach (BaseReportItem item in this.CurrentSection.Items) {
					if (item.Parent == null) {
						item.Parent = this.CurrentSection;
					}
					
					item.SectionOffset = this.CurrentSection.SectionOffset;
					BaseTextItem bti = item as BaseTextItem;
					
					if (bti != null) {
						bti.Text = expressionFassade.Evaluate(bti.Text);
					}
					item.Render(rpea);
					drawPoint = new Point(this.CurrentSection.Location.X,
					                      this.CurrentSection.SectionOffset + this.CurrentSection.Size.Height);
					rpea.LocationAfterDraw = new Point (rpea.LocationAfterDraw.X,
					                                    this.CurrentSection.SectionOffset + this.CurrentSection.Size.Height);
					
				}
				
				if ((this.CurrentSection.CanGrow == false)&& (this.CurrentSection.CanShrink == false)) {
					return new Point(this.CurrentSection.Location.X,
					                 this.CurrentSection.Size.Height);
				}
				return drawPoint;
			}
			return drawPoint;
		}
		
		
		#region PrintDocument Events
		
		private void CalculatePageBounds () 
		{
			this.SinglePage.CalculatePageBounds(this.reportModel);
		}
		
		
		internal virtual void ReportQueryPage (object sender,QueryPageSettingsEventArgs qpea) 
		{
			qpea.PageSettings.Margins = new Margins(reportSettings.LeftMargin,reportSettings.RightMargin,reportSettings.TopMargin,reportSettings.BottomMargin);
			bool firstPage;
			if (this.currentPageNumber == 0) {
				firstPage = true;
			} else {
				firstPage = false;
			}
			this.currentPageNumber ++;
			ISinglePage sp  = new SinglePage(new SectionBounds (reportSettings,firstPage),0);	
			PrintHelper.InitPage(sp,this.reportSettings);
			sp.PageNumber = this.currentPageNumber;
			reportDocument.SinglePage = sp;
		}
		
		
		internal virtual void  ReportBegin (object sender,PrintEventArgs pea)
		{
		}
		
		
		internal virtual void  ReportEnd (object sender,PrintEventArgs e) 
		{
		}
	
		#endregion
		
		
		#region property's
		
		public ReportDocument ReportDocument {get {return this.reportDocument;}}
		
		
		public ReportSettings ReportSettings {get {return reportSettings;}}
		
		
		public ReportSectionCollection Sections {get {return this.sections;}}
		
		
		public bool Cancel {get;set;}
		
		
		public ISinglePage SinglePage {get { return this.reportDocument.SinglePage; }}
	
		
		protected int CurrentRow {get;set;}
		
		
		protected BaseSection CurrentSection {get;set;}
		
		
		protected SectionBounds SectionBounds { get {return this.SinglePage.SectionBounds;}}
		
		
		protected IExpressionEvaluatorFacade ExpressionFassade {get { return expressionFassade; }}
	
	
		protected ILayouter Layout {get {return this.layout;}}
		
		
		#endregion
	
		#region IDispoable
		public  void Dispose()
		{
			this.Dispose(true);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.reportDocument != null) {
				this.reportDocument.Dispose();
			}
			}
		}
		#endregion
	}
}


