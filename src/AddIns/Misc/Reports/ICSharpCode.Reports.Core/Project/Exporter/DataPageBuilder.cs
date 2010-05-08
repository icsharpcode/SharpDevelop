// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.Events;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core.Exporter
{
	public class DataPageBuilder:BasePager
	{
		IDataManager dataManager;
		IDataNavigator dataNavigator;

		readonly object addLock = new object();
		
		internal delegate ExporterCollection ConverterDelegate (BaseSection s);
		
		#region Constructor
		
		
		public static IReportCreator CreateInstance(IReportModel reportModel, IDataManager dataManager,ILayouter layouter)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			if (dataManager == null) {
				throw new ArgumentNullException("dataManager");
			}
			if (layouter == null) {
				throw new ArgumentNullException("layouter");
			}
			DataPageBuilder instance = new DataPageBuilder(reportModel,dataManager,layouter);
			return instance;
		}
		
		
		private DataPageBuilder (IReportModel reportModel,IDataManager dataManager,ILayouter layouter):base(reportModel,layouter) 
		{
			this.dataManager = dataManager;
		}
		#endregion
		
		
		#region Create and Init new page
	
		
		protected override void BuildNewPage ()
		{
			base.BuildNewPage();
			if (this.dataNavigator != null) {
				this.SinglePage.StartRow = this.dataNavigator.CurrentRow +1;
			}
			
			this.BuildReportHeader();
			this.BuildPageHeader();
		}
		
		#endregion
		
		
		private void SetPageFull()
		{
			if (this.dataNavigator != null) {
				this.SinglePage.EndRow = this.dataNavigator.CurrentRow;
			}
		}
		
		
		private void OnPageFull(object sender, NewPageEventArgs e)
		{
			this.SinglePage.Items.AddRange(e.ItemsList);
			SetPageFull();
			PageBreak();
		}
		
		
		#region Build the Page from Sections
		
		protected override void BuildReportHeader ()
		{
			if ((base.Pages.Count == 0) && (base.ReportModel.ReportHeader.Items.Count > 0)) {
				
				base.ReportModel.ReportHeader.SectionOffset = base.SinglePage.SectionBounds.ReportHeaderRectangle.Top;
				ExporterCollection convertedList =  base.ConvertSection (base.ReportModel.ReportHeader,this.dataNavigator.CurrentRow);
				base.SinglePage.Items.AddRange(convertedList);
			}
		}
		
		
		protected override void BuildPageHeader ()
		{
			base.ReportModel.PageHeader.SectionOffset = base.AdjustPageHeader();
			ExporterCollection convertedList =  base.ConvertSection (base.ReportModel.PageHeader,this.dataNavigator.CurrentRow);
			base.SinglePage.Items.AddRange(convertedList);
			//orginal
			//base.ConvertSection(base.ReportModel.PageHeader,0);
		}
		
		
		protected override void BuildReportFooter (Rectangle footerRectangle)
		{
			base.ReportModel.ReportFooter.SectionOffset = footerRectangle.Top;
			ExporterCollection convertedList = base.ConvertSection (base.ReportModel.ReportFooter,this.dataNavigator.CurrentRow);
			base.SinglePage.Items.AddRange(convertedList);
		}
		
		
		protected override void BuildPageFooter ()
		{
			base.ReportModel.PageFooter.SectionOffset =  base.SinglePage.SectionBounds.PageFooterRectangle.Top;
			ExporterCollection convertedList = convertedList = base.ConvertSection (base.ReportModel.PageFooter,this.dataNavigator.CurrentRow);
			base.SinglePage.Items.AddRange(convertedList);
		}
		
				
		protected  void BuildDetail (BaseSection section,IDataNavigator dataNavigator)
		{
			ExporterCollection convertedList = new ExporterCollection();
			
			
		
			
			foreach (BaseReportItem item in section.Items)
			{
				IBaseConverter baseConverter = ConverterFactory.CreateConverter(item,dataNavigator,
				                                                                this.SinglePage,this.ExportItemsConverter,
				                                                                base.Layouter);
				if (baseConverter != null) {
					
					
					baseConverter.SectionRendering += OnSectionRendering;
					baseConverter.Graphics = base.Graphics;
					baseConverter.PageFull += new EventHandler<NewPageEventArgs>(OnPageFull);
					
					convertedList = baseConverter.Convert(section,item);
				
					base.SinglePage.Items.AddRange(convertedList);
					SetPageFull();
				}
			}
		}
		
		void OnSectionRendering (object sender,SectionRenderEventArgs e)
		{
			base.FireSectionRenderEvent(e.Section,e.RowNumber);
		}
			
			
		#endregion
		
		private void WritePages ()
		{
			this.dataNavigator = this.dataManager.GetNavigator;
			this.BuildNewPage();
			this.dataNavigator.MoveNext();
			BaseSection section = base.ReportModel.DetailSection;

			section.SectionOffset = base.SinglePage.SectionBounds.DetailStart.Y;
			this.BuildDetail (section,dataNavigator);
			
			this.BuildReportFooter(SectionBounds.ReportFooterRectangle);
			this.BuildPageFooter();
			//this is the last Page
			this.AddPage(base.SinglePage);
			base.FinishRendering(this.dataNavigator);
		}
		
		
		private void PageBreak () 
		{
			this.BuildPageFooter();
			this.AddPage(base.SinglePage);
			this.BuildNewPage ();
		}
		
		
		
		#region Public Methodes
		
		protected override void AddPage (ExporterPage page)
		{
			if (page == null) {
				throw new ArgumentNullException("page");
			}
			lock (addLock) {
				base.Pages.Add(page);
				
			}
			base.FirePageCreated(page);
		}
		
		
		public override void BuildExportList () 
		{
			base.BuildExportList();
			WritePages();
		}
		
		#endregion
		
		#region Propertys
		
		public int PageCount
		{
			get {
				lock(addLock) {
					return base.Pages.Count;
				}
			}
		}
		
		
		public ExporterPage FirstPage 
		{
			get {
				if (base.Pages.Count > 0) {
					return base.Pages[0];
				}
				return null;
			}
		}
		
		
		public ExporterPage LastPage 
		{
			get {
				if (base.Pages.Count > 0) {
					return base.Pages[base.Pages.Count -1];
				}
				return null;
			}
		}
	
		#endregion
	}

}
