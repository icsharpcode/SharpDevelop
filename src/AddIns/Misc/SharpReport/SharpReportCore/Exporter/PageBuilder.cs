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
using System.Diagnostics;
using System.Globalization;
using System.Drawing;

using System.Drawing.Printing;

namespace SharpReportCore.Exporters
{
	/// <summary>
	/// Description of PageBuilder.
	/// </summary>
	public class PageBuilder
	{
		List<SinglePage> pages;
		ReportModel reportModel;
		DataManager dataManager;
		DataNavigator dataNavigator; 
		ExportItemsConverter lineItemsConverter;
		
//		public delegate List <IPerformLine> ConverterDelegate (LineItemsConverter c,BaseSection s,SinglePage p);
		public delegate List <BaseExportColumn> ConverterDelegate (BaseSection s,SinglePage p);
		#region Constructor
		public PageBuilder () {
			pages = new List<SinglePage>();
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
		
		#region template
		/*
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
		
		 */
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
	
		private void BuildReportHeader (SinglePage page) {
			BaseSection section = this.reportModel.ReportHeader;
//			System.Console.WriteLine("ReportHeader with {0} items ",section.Items.Count);
			
//			LineItemsConverter conv = new LineItemsConverter(graphics);
			this.lineItemsConverter.Offset = page.SectionBounds.ReportHeaderRectangle.Top;
			System.Console.WriteLine("ReportHeader start at {0} with {1} items",
			                         this.lineItemsConverter.Offset,
			                        section.Items.Count);
			List <BaseExportColumn>l = section.Items.ConvertAll <BaseExportColumn> (this.lineItemsConverter.ConvertToLineItems);
			page.Items.AddRange(l);

		}
		
		private void BuildPageHeader (SinglePage page) {
			BaseSection section = this.reportModel.PageHeader;
//			System.Console.WriteLine("PageHeader with {0} items ",section.Items.Count);

//			LineItemsConverter conv = new LineItemsConverter(graphics);
			this.lineItemsConverter.Offset = page.SectionBounds.PageHeaderRectangle.Top;
			System.Console.WriteLine("Page Header start at {0} with {1} Items",
			                         this.lineItemsConverter.Offset,
			                        section.Items.Count);
			List <BaseExportColumn>l = section.Items.ConvertAll <BaseExportColumn> (this.lineItemsConverter.ConvertToLineItems);
			page.Items.AddRange(l);
			
//			System.Console.WriteLine("\tList<string> {0}",l.Count);
//			System.Console.WriteLine("\tpages.items.coun {0}",page.Items.Count);
		}
		
		
		
		private void BuildDetails (SinglePage page,Graphics graphics) {
			
			bool firstOnPage = true;
			
			if (! dataNavigator.HasMoreData ) {
				return;
			}
			
			
			if (this.pages.Count == 0) {
				dataNavigator.MoveNext();
			}
			
			BaseSection section = this.reportModel.DetailSection;
			section.SectionOffset = page.SectionBounds.DetailStart.Y;
			do {
				
				
				
				dataNavigator.Fill(section.Items);
				
				if (!firstOnPage) {
					section.SectionOffset = section.SectionOffset + section.Size.Height  + 2 * page.SectionBounds.Gap;
				}
				
				MeasurementService.FitSectionToItems (section,graphics);
				List <BaseExportColumn>convertedList = new List<BaseExportColumn>();
				
				ConverterDelegate converterDelegate;
				
				if (section.Items[0] is IContainerItem) {
					converterDelegate = new ConverterDelegate(DoContainerJob);
				} else {
					converterDelegate = new ConverterDelegate(DoJob);
				}
				
				convertedList = converterDelegate(section,page);
				
				page.Items.AddRange(convertedList);
				firstOnPage = false;
			}
			while (dataNavigator.MoveNext());
			
		}
		
		private List <BaseExportColumn> DoContainerJob (BaseSection section,SinglePage page) {
			
			IExportColumnBuilder builder = section.Items[0] as IExportColumnBuilder;
			
			this.lineItemsConverter.Offset = section.SectionOffset;
			
			if (builder != null) {
				System.Console.WriteLine("Create RowList with Location {0}",this.lineItemsConverter.Offset);
				
				Graphics g = reportModel.ReportSettings.PageSettings.PrinterSettings.CreateMeasurementGraphics();
				
				ExportContainer lineItem = (ExportContainer)builder.CreateExportColumn(g);
				
				IContainerItem containerItem = section.Items[0] as IContainerItem;
				
				// reread
				this.dataNavigator.Fill(containerItem.Items);
				List <BaseExportColumn> childList = containerItem.Items.ConvertAll <BaseExportColumn> (this.lineItemsConverter.ConvertToLineItems);

				lineItem.Items.AddRange(childList);
				
				childList.ForEach(display);
				
				List <BaseExportColumn> containerList = new List<BaseExportColumn>();
				containerList.Add (lineItem);
				g.Dispose();
				return containerList;
			}
			return null;
		}
		
		private List <BaseExportColumn> DoJob (BaseSection section,SinglePage page) {

			this.lineItemsConverter.Offset = section.SectionOffset;
			System.Console.WriteLine("Plain Item {0}/{1}",section.SectionOffset,page.SectionBounds.DetailStart);
			List <BaseExportColumn>list = section.Items.ConvertAll <BaseExportColumn> (this.lineItemsConverter.ConvertToLineItems);
			
			if (list.Count > 0) {
				
				bool istrue = list.TrueForAll(delegate(BaseExportColumn item){
				                           	if (item.StyleDecorator.Location.Y + item.StyleDecorator.Size.Height > page.SectionBounds.DetailEnds.Y) {
				                           		return false;
				                           	}
				                           	
				                           	
//				                           	System.Console.WriteLine("{0} / {1}",
//				                           	                         item.ItemStyle.Location.Y + item.ItemStyle.Size.Height,
//				                           	                         page.SectionBounds.DetailEnds.Y);
				                           	return true;
				                           });
			}
//			list.ForEach(display);
			return list;
		}
		
		
		
		private void display (IPerformLine li) {
//			System.Console.WriteLine("\tdisplay {0}",li.ToString());
			ExportText l = li as ExportText;
			if (l != null) {
				System.Console.WriteLine("\t\t{0} / {1} ",l.StyleDecorator.Location,l.Text);
			}
			
		}
		
		
		private void BuildPageFooter (SinglePage page) {
			BaseSection section = this.reportModel.PageFooter;
			                  
			this.lineItemsConverter.Offset = page.SectionBounds.PageFooterRectangle.Top;
//			System.Console.WriteLine("Page Footer start at {0} with {1} Items",
//			                         this.lineItemsConverter.Offset,
//			                        section.Items.Count);
			List <BaseExportColumn>l = section.Items.ConvertAll <BaseExportColumn> (this.lineItemsConverter.ConvertToLineItems);
			page.Items.AddRange(l);
		}
		
		private void Write (SinglePage page) {
			this.dataNavigator = this.dataManager.GetNavigator;
			Graphics graphics = reportModel.ReportSettings.PageSettings.PrinterSettings.CreateMeasurementGraphics();
			this.lineItemsConverter = new ExportItemsConverter(graphics);
			if (this.pages.Count == 0) {
				BuildReportHeader (page);
			}
			
			BuildPageHeader(page) ;
			BuildDetails (page,graphics);
			BuildPageFooter (page);
			graphics.Dispose();
		}
		
		
		public void AddPage (SinglePage page) {
			if (page == null) {
				throw new ArgumentNullException("page");
			}
			this.pages.Add(page);
		}
		
		
		public void CreateReport () {
			SinglePage page = this.CreateNewPage();
			page.CalculatePageBounds(this.reportModel);
			Write(page);
			this.pages.Add(page);
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
	}

}
