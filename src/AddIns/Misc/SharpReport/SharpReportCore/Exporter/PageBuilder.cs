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
		
		
		
		private SinglePage CreateNewPage () {
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
	
		private void DoConvert (BaseSection section,int offset) {
			this.lineItemsConverter.Offset = offset;
			List <BaseExportColumn>list = section.Items.ConvertAll <BaseExportColumn> (this.lineItemsConverter.ConvertToLineItems);

			list.ForEach(display);
			/*
			if (list.Count > 0) {
				list.ForEach(delegate(BaseExportColumn item){
				             	ExportGraphic gr = item as ExportGraphic;
				             	if (gr != null) {
				             		System.Console.WriteLine("{0} / {1}",gr.ToString(),gr.StyleDecorator.Shape.ToString());
				             	}
				             	
				             });
			}
			*/
			this.singlePage.Items.AddRange(list);
		}
		
		
		private void BuildReportHeader () {
			
			this.DoConvert (this.reportModel.ReportHeader,
			                this.singlePage.SectionBounds.PageHeaderRectangle.Top);
		}
		
		private void BuildPageHeader () {
			this.DoConvert (this.reportModel.PageHeader,
			                this.singlePage.SectionBounds.PageHeaderRectangle.Top);
		}
		
		
		
		private void BuildDetails (Graphics graphics) {
			
			bool firstOnPage = true;
			
			if (! dataNavigator.HasMoreData ) {
				return;
			}
			
			
			if (this.pages.Count == 0) {
				dataNavigator.MoveNext();
			}
			
			BaseSection section = this.reportModel.DetailSection;
			section.SectionOffset = this.singlePage.SectionBounds.DetailStart.Y;
			do {
				dataNavigator.Fill(section.Items);
				
				if (!firstOnPage) {
					section.SectionOffset = section.SectionOffset + section.Size.Height  + 2 * this.singlePage.SectionBounds.Gap;
				}

				MeasurementService.FitSectionToItems (section,graphics);
				ExporterCollection<BaseExportColumn> convertedList = new ExporterCollection<BaseExportColumn>();
				
				ConverterDelegate converterDelegate;
				
				if (section.Items[0] is IContainerItem) {
					converterDelegate = new ConverterDelegate(DoContainerJob);
				} else {
					converterDelegate = new ConverterDelegate(DoJob);
				}
				
				convertedList = converterDelegate(section);
				BaseExportColumn bec = convertedList[0];


				System.Console.WriteLine("SB {0} / DR {1} / Fit {2} ",this.singlePage.SectionBounds.DetailRectangle,
				                        bec.StyleDecorator.DisplayRectangle,
				                       this.singlePage.SectionBounds.DetailRectangle.Contains(bec.StyleDecorator.DisplayRectangle));
				if (this.singlePage.SectionBounds.DetailRectangle.Contains(bec.StyleDecorator.DisplayRectangle)) {
					System.Console.WriteLine("\t fit");
				} else {
					System.Console.WriteLine("\t dont fit");
				}
				this.singlePage.Items.AddRange(convertedList);
				firstOnPage = false;
			}
			while (dataNavigator.MoveNext());
			
		}
		
		private ExporterCollection<BaseExportColumn> DoContainerJob (BaseSection section) {
			
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
		
		
		
		private ExporterCollection<BaseExportColumn> DoJob (BaseSection section) {

			this.lineItemsConverter.Offset = section.SectionOffset;
			List<BaseExportColumn> list = section.Items.ConvertAll <BaseExportColumn> (this.lineItemsConverter.ConvertToLineItems);
			
			ExporterCollection<BaseExportColumn> childList = new ExporterCollection<BaseExportColumn>();
			childList.AddRange(list);
			return childList;
		}
		
		
		
	
		
		
		private void BuildPageFooter () {
			BaseSection section = this.reportModel.PageFooter;
			this.DoConvert (this.reportModel.PageFooter,this.singlePage.SectionBounds.PageFooterRectangle.Top);
		}
		
		private void display (IPerformLine li) {
//			System.Console.WriteLine("\tdisplay {0}",li.ToString());
			ExportText l = li as ExportText;
			if (l != null) {
				System.Console.WriteLine("\t\t{0} / {1} ",l.StyleDecorator.Location,l.Text);
			}
		}
		
		private void Write () {
			this.dataNavigator = this.dataManager.GetNavigator;
			Graphics graphics = reportModel.ReportSettings.PageSettings.PrinterSettings.CreateMeasurementGraphics();
			this.lineItemsConverter = new ExportItemsConverter(graphics);
			if (this.pages.Count == 0) {
				BuildReportHeader ();
			}
			
			BuildPageHeader() ;
			BuildDetails (graphics);
			System.Console.WriteLine("--------");
			System.Console.WriteLine("{0} / {1} / {2}",this.dataNavigator.Count,this.dataNavigator.CurrentRow,this.dataNavigator.HasMoreData);
			BuildPageFooter ();
			graphics.Dispose();
		}
		
		//TODO did we use this method
		public void AddPage (SinglePage page) {
			if (page == null) {
				throw new ArgumentNullException("page");
			}
			this.pages.Add(page);
		}
		
		
		public void BuildExportList () {
			this.singlePage = this.CreateNewPage();
			this.singlePage.CalculatePageBounds(this.reportModel);
			Write();
			this.pages.Add(this.singlePage);
		}
		
		
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
		
	}

}
