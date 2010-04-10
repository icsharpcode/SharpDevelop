/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Forstmeier
 * Datum: 01.05.2007
 * Zeit: 15:38
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.Interfaces;
using ICSharpCode.Reports.Expressions.ReportingLanguage;

namespace ICSharpCode.Reports.Core.Exporter
{
	/// <summary>
	/// Description of BasePager.
	/// </summary>
	public class BasePager:IReportCreator
	{
		private ExporterPage singlePage;
		private PagesCollection pages;
		private IReportModel reportModel;
		private Graphics graphics;
		private bool pageFull;
		private readonly object pageLock = new object();
		private IExportItemsConverter exportItemsConverter;
		private ILayouter layouter;
		
		public event EventHandler<PageCreatedEventArgs> PageCreated;
		public event EventHandler<SectionRenderEventArgs> SectionRendering;
		
		#region Constructor
		
		public BasePager(IReportModel reportModel,ILayouter layouter)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			if (layouter == null) {
				throw new ArgumentNullException ("layouter");
			}
			this.reportModel = reportModel;
			this.layouter = layouter;
			this.graphics = CreateGraphicObject.FromSize(this.reportModel.ReportSettings.PageSize);
			this.exportItemsConverter = new ExportItemsConverter();
		}
		
		#endregion
		
		#region Create and Init new page
		
		protected int AdjustPageHeader ()
		{
			int offset = 0;
			if (this.SinglePage.Items.Count > 0) {
				offset = this.SinglePage.SectionBounds.PageHeaderRectangle.Top;
			} else {
				offset = this.SinglePage.SectionBounds.ReportHeaderRectangle.Top;
			}
			return offset;
		}
		
		
		protected ExporterPage InitNewPage ()
		{
			bool firstPage;
			this.ReportModel.ReportSettings.LeftMargin = this.reportModel.ReportSettings.LeftMargin;
			if (this.Pages.Count == 0) {
				firstPage = true;
			} else {
				firstPage = false;
			}
			SectionBounds sectionBounds  = new SectionBounds (this.reportModel.ReportSettings,firstPage);
			ExporterPage sp = ExporterPage.CreateInstance(sectionBounds,this.pages.Count + 1);
			return sp;
		}
		
		
		protected virtual void BuildNewPage ()
		{
			this.singlePage = this.InitNewPage();
			PrintHelper.InitPage(this.singlePage,this.reportModel.ReportSettings);			
			this.singlePage.CalculatePageBounds(this.ReportModel);
			this.pageFull = false;
		}
		
		#endregion
		
		#region Converters
		
		protected void ConvertSection (BaseSection section,int currentRow)
		{
			this.FireSectionRenderEvent (section ,currentRow);
			this.Convert(section);
		}
		
		
		private void Convert (BaseSection section)
		{
			this.exportItemsConverter.Offset = section.SectionOffset;
			PrintHelper.AdjustParent((BaseSection)section,section.Items);
			ExporterCollection list = null;
			
			if (section.Items.Count > 0) {
				
				ISimpleContainer container = section.Items[0] as ISimpleContainer;
				if (container != null) {

					ExportContainer exportContainer = this.exportItemsConverter.ConvertToContainer(container);
					this.exportItemsConverter.ParentLocation = exportContainer.StyleDecorator.Location;
					
					AdjustBackColor (container);
					
					ExporterCollection clist = container.Items.ConvertAll <BaseExportColumn> (this.exportItemsConverter.ConvertToLineItem) as ExporterCollection;
					
					
					exportContainer.Items.AddRange(clist);
					list = new ExporterCollection();
					list.Add(exportContainer);
					
				} else {
					this.exportItemsConverter.ParentLocation = section.Location;
					
					Rectangle desiredRectangle = layouter.Layout(this.graphics,section);
					Rectangle sectionRectangle = new Rectangle(0,0,section.Size.Width,section.Size.Height);
					
					if (!sectionRectangle.Contains(desiredRectangle)) {
						section.Size = new Size(section.Size.Width,desiredRectangle.Size.Height);
					}
					list = section.Items.ConvertAll <BaseExportColumn> (this.exportItemsConverter.ConvertToLineItem);
				}
				if ((list != null) && (list.Count) > 0) {
					this.singlePage.Items.AddRange(list);
				}
			}
		}
		
		private static void AdjustBackColor (ISimpleContainer container)
		{
			BaseReportItem parent = container as BaseReportItem;
			foreach (BaseReportItem item in container.Items)
			{
				item.BackColor = parent.BackColor;
			}
		}
		#endregion
		
		
		#region Convertion
		
		protected virtual void BuildReportHeader ()
		{
		}
		
		protected virtual void BuildPageHeader ()
		{
		}
		
		protected virtual void BuildDetailInternal (BaseSection section)
		{
		}
		
		protected virtual void BuildPageFooter ()
		{
		}
		
		protected virtual void BuildReportFooter (Rectangle footerRectangle)
		{
		}
		
		
		public virtual void BuildExportList ()
		{
			this.Pages.Clear();
		}
		
		protected virtual void AddPage (ExporterPage page)
		{
		}
		
		#endregion
		
		
		protected void FinishRendering (IDataNavigator navigator)
		{
			if (this.Pages.Count == 0) {
				return;
			}
			
			// set values known only end of reportcreation
			foreach (ExporterPage p in this.pages)
			{
				p.TotalPages = this.Pages.Count;
			}
			
			IExpressionEvaluatorFacade evaluatorFacade = new ExpressionEvaluatorFacade();
			
			foreach (ExporterPage p in this.pages)
			{
				this.singlePage = p;
				evaluatorFacade.SinglePage = this.singlePage;
				evaluatorFacade.SinglePage.IDataNavigator = navigator;
				EvaluateRecursive(evaluatorFacade,p.Items);
			}
		}
		 
		
		private void EvaluateRecursive (IExpressionEvaluatorFacade evaluatorFassade,ExporterCollection items)
		{
			foreach (BaseExportColumn be in items) {
				ExportContainer ec = be as ExportContainer;
				if (ec != null)
				{
					if (ec.Items.Count > 0) {
						EvaluateRecursive(evaluatorFassade,ec.Items);
					}
				}
				ExportText et = be as ExportText;
				if (et != null) {
					et.Text = evaluatorFassade.Evaluate(et.Text);
				}
			}
		}
		
		
		#region Event's
		
		protected void FireSectionRenderEvent (BaseSection section,int currentRow)
		{
			SectionRenderEventArgs ea =
				new SectionRenderEventArgs(section,
				                           pages.Count,
				                           currentRow,
				                           section);
			
			EventHelper.Raise<SectionRenderEventArgs>(SectionRendering,this,ea);
		}
		
		
		
		protected void FirePageCreated(ExporterPage page)
		{
			EventHelper.Raise<PageCreatedEventArgs>(PageCreated,this,
			                                        new PageCreatedEventArgs(page));
		}
		#endregion
		
		
		#region Property's
		
		protected Graphics Graphics {
			get { return graphics; }
		}
		
		
		public ILayouter Layouter {
			get { return layouter; }
		}
		
		public IReportModel ReportModel
		{
			get { return reportModel; }
			set { reportModel = value; }
		}
		
		
		protected ExporterPage SinglePage
		{
			get { return singlePage; }
			set { singlePage = value; }
		}
		
		
		public PagesCollection Pages
		{
			get {
				lock(pageLock) {
					if (this.pages == null) {
						this.pages = new PagesCollection();
					}
					return pages;
				}
			}
		}
		
		
		protected SectionBounds SectionBounds
		{
			get { return singlePage.SectionBounds; }
		}
		
		
		protected bool PageFull
		{
			get { return pageFull; }
			set { pageFull = value; }
		}
		
		
		protected IExportItemsConverter ExportItemsConverter
		{
			get { return exportItemsConverter; }
			set { exportItemsConverter = value; }
		}
		
		#endregion
	}
}

