// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.BaseClasses.Printing;
using ICSharpCode.Reports.Core.Events;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core.Exporter
{
	public class DataPageBuilder:BasePager
	{
		IDataManager dataManager;
		IDataNavigator dataNavigator;
		
		readonly object addLock = new object();
		
		
		#region Constructor
		
		public static IReportCreator CreateInstance(IReportModel reportModel, IDataManager dataManager)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			if (dataManager == null) {
				throw new ArgumentNullException("dataManager");
			}
			DataPageBuilder instance = new DataPageBuilder(reportModel,dataManager);
			return instance;
		}
		
		
		private DataPageBuilder (IReportModel reportModel,IDataManager dataManager):base(reportModel) 
		{
			this.dataManager = dataManager;
		}
		#endregion
		
		
		#region Create and Init new page
	
		
		protected override void BuildNewPage ()
		{
			base.BuildNewPage();
			this.BuildReportHeader();
			this.BuildPageHeader();
		}
		
		#endregion
		
		
		private void OnPageFull(object sender, NewPageEventArgs e)
		{
			this.SinglePage.Items.AddRange(e.ItemsList);
			PageBreak();
			e.SectionBounds = SinglePage.SectionBounds;
		}
		
		
		#region Build the Page from Sections
		
		protected override void BuildReportHeader ()
		{
			SectionBounds.Offset = new Point(base.SectionBounds.MarginBounds.Left,base.SectionBounds.MarginBounds.Top);
			if ((base.Pages.Count == 0) && (base.ReportModel.ReportHeader.Items.Count > 0))
			{
				base.ReportModel.ReportHeader.SectionOffset = base.SinglePage.SectionBounds.ReportHeaderRectangle.Top;
				ConvertSectionInternal (base.ReportModel.ReportHeader);
			} else
			{
				base.ReportModel.ReportHeader.Size = Size.Empty;
			}
			base.SectionBounds.CalculatePageBounds(base.ReportModel);
		}
		
		
		protected override void BuildPageHeader ()
		{
			if (SectionBounds.Offset.Y < base.ReportModel.PageHeader.SectionOffset) {
				SectionBounds.Offset = new Point(SectionBounds.Offset.X,base.ReportModel.PageHeader.SectionOffset);
			}
			base.SectionBounds.CalculatePageBounds(base.ReportModel);
			ConvertSectionInternal (base.ReportModel.PageHeader);
		}
		
		
		protected override void BuildReportFooter (Rectangle footerRectangle)
		{
			bool pageBreak = false;
			
			base.ReportModel.ReportFooter.SectionOffset = footerRectangle.Top + GlobalValues.GapBetweenContainer;
			SectionBounds.Offset = new Point(SectionBounds.Offset.X,footerRectangle.Top + GlobalValues.GapBetweenContainer );
			if (!PrintHelper.IsRoomForFooter(base.SectionBounds,base.ReportModel.ReportFooter.Location)) {
				PageBreak();
				base.ReportModel.ReportFooter.SectionOffset = SectionBounds.DetailArea.Top;
				pageBreak = true;
			}
			
			ExporterCollection convertedList = new ExporterCollection();
			var section = base.ReportModel.DetailSection;
			var table = section.Items[0] as BaseTableItem;
			if (table != null) {
				if (pageBreak) {
					// Print the HeaderRow
					var headerRow = table.Items[0];
					
					var curPos = BaseConverter.ConvertContainer(convertedList,(ISimpleContainer)headerRow,SectionBounds.PageHeaderRectangle.Left,SectionBounds.PageHeaderRectangle.Location);
					base.SinglePage.Items.AddRange(convertedList);
					base.ReportModel.ReportFooter.SectionOffset = curPos.Y + GlobalValues.GapBetweenContainer;
				}
			}
			//allways print the reportFooter
			ConvertSectionInternal(base.ReportModel.ReportFooter);
		}
		
		
		protected override void BuildPageFooter ()
		{
			base.ReportModel.PageFooter.SectionOffset =  base.SinglePage.SectionBounds.PageFooterRectangle.Top;
			SectionBounds.Offset = new Point(SectionBounds.Offset.X, base.SinglePage.SectionBounds.PageFooterRectangle.Top);
			ConvertSectionInternal(base.ReportModel.PageFooter);
		}
		
		
		void ConvertSectionInternal (BaseSection section)
		{
			ExporterCollection convertedList =  base.ConvertSection (section,this.dataNavigator.CurrentRow);
			base.SinglePage.Items.AddRange(convertedList);
		}
		
		
		protected  Point BuildDetail (BaseSection section,IDataNavigator dataNavigator)		
		{
			ExporterCollection convertedList = new ExporterCollection();
			foreach (BaseReportItem item in section.Items)
			{
				IBaseConverter baseConverter = ConverterFactory.CreateConverter(item,base.ReportModel,dataNavigator,
				                                                                this.SinglePage);
				                                                                
				if (baseConverter != null) {
			
					baseConverter.SectionRendering += OnSectionRendering;
					baseConverter.GroupHeaderRendering += OnGroupHeaderRendering;
					baseConverter.GroupFooterRendering += OnGroupFooterRendering;
					baseConverter.RowRendering += OnRowRendering;
					
					baseConverter.Graphics = base.Graphics;
					baseConverter.PageFull += new EventHandler<NewPageEventArgs>(OnPageFull);
					
					convertedList = baseConverter.Convert(section,item);
					
					base.SinglePage.Items.AddRange(convertedList);
					return baseConverter.CurrentPosition;
				}
			}
			return Point.Empty;
		}
		
		
		void OnSectionRendering (object sender,SectionRenderEventArgs e)
		{
			base.FireSectionRenderEvent(e.Section,e.RowNumber);
		}
		
		
		void OnGroupHeaderRendering (object sender, GroupHeaderEventArgs ghea)
		{
			base.FireGroupHeaderEvent(ghea);
		}
		
		
		void OnGroupFooterRendering (object sender, GroupFooterEventArgs gfea)
		{
			base.FireGroupFooterEvent(gfea);
		}
		
		
		void OnRowRendering (object sender,RowRenderEventArgs rrea)
		{
				base.FireRowRenderEvent(rrea);
		}
		#endregion
		
		
		private void WritePages ()
		{
			this.dataNavigator = this.dataManager.GetNavigator;
			this.BuildNewPage();
		    this.SinglePage.IDataNavigator = this.dataNavigator;
			this.dataNavigator.MoveNext();
			BaseSection section = base.ReportModel.DetailSection;

			section.SectionOffset = base.SinglePage.SectionBounds.DetailArea.Top;
			var currentLocation = this.BuildDetail (section,dataNavigator);
			
			var r = new Rectangle (SectionBounds.ReportFooterRectangle.Left,currentLocation.Y,
			                       SectionBounds.ReportFooterRectangle.Size.Width,
			                       SectionBounds.ReportFooterRectangle.Size.Height);
			this.BuildReportFooter(r);
			
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
