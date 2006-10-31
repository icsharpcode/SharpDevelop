/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 20.09.2006
 * Time: 09:36
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;

namespace SharpReportCore.Exporters
{
	/// <summary>
	/// Description of PageBuilder.
	/// </summary>
	public class PageBuilder
	{
		SinglePage singlePage;
		PagesCollection pages;
		ReportModel reportModel;
		DataManager dataManager;
		DataNavigator dataNavigator; 
		ExportItemsConverter lineItemsConverter;
		//
		bool newFull;
		internal delegate ExporterCollection<BaseExportColumn> ConverterDelegate (BaseSection s);
		
		#region Constructor
		
		public PageBuilder () {
			pages = new PagesCollection();
		}
		
		public PageBuilder(ReportModel reportModel,DataManager dataManager):this(){
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			if (dataManager == null) {
				throw new ArgumentNullException("dataManager");
			}
			this.reportModel = reportModel;
			this.dataManager = dataManager;
		}
		
		#endregion
		
		#region Create and Init new page
		
		private SinglePage InitPage () {
			PageSettings ps = this.reportModel.ReportSettings.PageSettings;
			ps.Margins = this.reportModel.ReportSettings.DefaultMargins;
			SectionBounds sb = null;
			if (this.pages.Count == 0) {
				sb = new SectionBounds(ps,true);
			} else {
				sb = new SectionBounds(ps,false);
			}
			
			SinglePage sp = new SinglePage(sb);
			return sp;
		}
		void BuildNewPage () {
			this.singlePage = this.InitPage();
			this.singlePage.CalculatePageBounds(this.reportModel);
			this.newFull = false;
			this.BuildReportHeader();
			this.BuildPageHeader();
		}
		
		#endregion
		
		
		
		
		private void BuildReportHeader () {
			if (this.pages.Count == 0) {
					this.ConvertItemsList (this.reportModel.ReportHeader,
			                       this.singlePage.SectionBounds.ReportHeaderRectangle.Top);
			}
		}
		
		private void BuildPageHeader () {
			this.ConvertItemsList (this.reportModel.PageHeader,
			                this.singlePage.SectionBounds.PageHeaderRectangle.Top);
		}
		
		
	
		private void BuildReportFooter (int locY) {
			this.ConvertItemsList (this.reportModel.ReportFooter,locY);
		}
	
		private void BuildPageFooter () {
			
			this.ConvertItemsList (this.reportModel.PageFooter,this.singlePage.SectionBounds.PageFooterRectangle.Top);
		}
		
		
		/*
		private void display (BaseStyleDecorator li) {
//		private void display (IPerformLine li) {
//			System.Console.WriteLine("\tdisplay {0}",li.ToString());
//			ExportText l = li as ExportText;
			if (li != null) {
				System.Console.WriteLine("\t\t{0} / {1} ",l.StyleDecorator.Location,l.Text);
			}
		}
		*/
		
		#region newCreate
		
	
		void BuildDetail (BaseSection section,Graphics graphics) {
			
			dataNavigator.Fill(section.Items);
			MeasurementService.FitSectionToItems (section,graphics);
			ExporterCollection<BaseExportColumn> convertedList = new ExporterCollection<BaseExportColumn>();
			
			ConverterDelegate converterDelegate;
			
			if (section.Items[0] is IContainerItem) {
				converterDelegate = new ConverterDelegate(ContainerItems);
			} else {
				converterDelegate = new ConverterDelegate(StandartItems);
			}
			
			convertedList = converterDelegate(section);
			BaseExportColumn bec = convertedList[0];

			if ( bec.StyleDecorator.DisplayRectangle.Bottom > this.singlePage.SectionBounds.PageFooterRectangle.Top) {
				this.newFull = true;
				return;
			}
			this.singlePage.Items.AddRange(convertedList);
		}
		
		#region Delegate's and Converter
		
		private void ConvertItemsList (BaseSection section,int offset) {
			this.lineItemsConverter.Offset = offset;
			List <BaseExportColumn>list = section.Items.ConvertAll <BaseExportColumn> (this.lineItemsConverter.ConvertToLineItems);
			this.singlePage.Items.AddRange(list);
		}
		
		private ExporterCollection<BaseExportColumn> ContainerItems (BaseSection section) {
			
			this.lineItemsConverter.Offset = section.SectionOffset;
			IExportColumnBuilder exportLineBuilder = section.Items[0] as IExportColumnBuilder;
			
			if (exportLineBuilder != null) {
				
				Graphics g = reportModel.ReportSettings.PageSettings.PrinterSettings.CreateMeasurementGraphics();

				ExportContainer lineItem = this.lineItemsConverter.ConvertToContainer(section.Items[0]);
				IContainerItem containerItem = section.Items[0] as IContainerItem;
				
				// reread
				this.dataNavigator.Fill(containerItem.Items);
				
				List<BaseExportColumn> list = containerItem.Items.ConvertAll <BaseExportColumn> (this.lineItemsConverter.ConvertToLineItems);

				//Adjust childitems to Location within container
				if (list.Count > 0) {
					list.ForEach(delegate(BaseExportColumn item){
					             	Point p = new Point (lineItem.StyleDecorator.Location.X + item.StyleDecorator.Location.X,
					             	                     item.StyleDecorator.Location.Y);
					             	item.StyleDecorator.Location = p;
					             });
				}

				lineItem.Items.AddRange(list);
				
				ExporterCollection<BaseExportColumn> containerList = new ExporterCollection<BaseExportColumn>();
//				list.ForEach(display);
				containerList.Add (lineItem);
				g.Dispose();
				return containerList;
			}
			return null;
		}
		
		private ExporterCollection<BaseExportColumn> StandartItems (BaseSection section) {

			this.lineItemsConverter.Offset = section.SectionOffset;
			List<BaseExportColumn> list = section.Items.ConvertAll <BaseExportColumn> (this.lineItemsConverter.ConvertToLineItems);
			
			ExporterCollection<BaseExportColumn> childList = new ExporterCollection<BaseExportColumn>();
			childList.AddRange(list);
			return childList;
		}
		
		#endregion
		
		private void WritePages () {
			this.dataNavigator = this.dataManager.GetNavigator;
			
			using (Graphics graphics = reportModel.ReportSettings.PageSettings.PrinterSettings.CreateMeasurementGraphics()){
				this.lineItemsConverter = new ExportItemsConverter(graphics);
				
				this.pages.Clear();
				this.BuildNewPage();
				
				this.newFull = false;
				this.dataNavigator.MoveNext();
				BaseSection section = this.reportModel.DetailSection;

				section.SectionOffset = this.singlePage.SectionBounds.DetailStart.Y;
				do {
					BuildDetail(section,graphics);
					section.SectionOffset += section.Size.Height + 2 * this.singlePage.SectionBounds.Gap;
					if (newFull) {
						PageBreak();
						section.SectionOffset = this.singlePage.SectionBounds.DetailStart.Y;
					} else {
						dataNavigator.MoveNext();
					}
				}
				while (dataNavigator.HasMoreData);
				this.BuildReportFooter(section.SectionOffset + section.Size.Height);
				this.BuildPageFooter();
				//this is the last Page
				this.AddPage(this.singlePage);
			}
		}
		
		
		void PageBreak () {
			this.BuildPageFooter();
			this.AddPage(this.singlePage);
//			System.Console.WriteLine("-------Page Break ---{0} --",this.singlePage.Items.Count);
			System.Console.WriteLine("-------Page Break --- --");
			this.BuildNewPage ();
		}
		
		#endregion
		//TODO did we use this method
		public void AddPage (SinglePage page) {
			if (page == null) {
				throw new ArgumentNullException("page");
			}
			this.pages.Add(page);
		}
		
	
		public void BuildExportList () {
			System.Diagnostics.Stopwatch s = new System.Diagnostics.Stopwatch();
			s.Start();
			System.Console.WriteLine("BuildExportList started");
			WritePages();
			s.Stop();
			TimeSpan ts = s.Elapsed;
			System.Console.WriteLine("BuildExportList finished {0}:{1} Seconds",ts.Seconds,ts.Milliseconds);
			System.Console.WriteLine("");
		}
		
		#region Propertys
		
		public int PageCount{
			get {
				return this.pages.Count;
			}
		}
		
		public SinglePage FirstPage {
			get {
				if (this.pages.Count > 0) {
					return pages[0];
				}
				return null;
			}
		}
		
		public SinglePage LastPage {
			get {
				if (this.pages.Count > 0) {
					return pages[pages.Count -1];
				}
				return null;
			}
		}
		
		public PagesCollection Pages {
			get {
				return pages;
			}
		}
		#endregion
	}

}
