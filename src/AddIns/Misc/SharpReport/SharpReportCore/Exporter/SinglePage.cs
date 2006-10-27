/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 19.09.2006
 * Time: 22:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Drawing.Printing;

namespace SharpReportCore.Exporters
{
		
	public class SinglePage{
		
		ExporterCollection<BaseExportColumn> items;
		SectionBounds sectionBounds;
		
		
		public SinglePage():this (new SectionBounds(new PageSettings())){
		}
		
		
		public SinglePage(SectionBounds sectionBounds)
		{
			this.sectionBounds = sectionBounds;
			items = new ExporterCollection<BaseExportColumn>();
		}
		
		public void CalculatePageBounds (ReportModel reportModel) {
			
			Graphics graphics = reportModel.ReportSettings.PageSettings.PrinterSettings.CreateMeasurementGraphics();
			Rectangle rectangle;

			if (this.sectionBounds.Firstpage) {
				rectangle = sectionBounds.MeasureReportHeader(reportModel.ReportHeader,graphics);
				
				
			} else {
				rectangle = new Rectangle (reportModel.ReportSettings.DefaultMargins.Left,
				                           reportModel.ReportSettings.DefaultMargins.Top,
				                           this.sectionBounds.MarginBounds.Width,
				                           0);
			}
			
			
			reportModel.ReportHeader.SectionOffset = reportModel.ReportSettings.DefaultMargins.Top;
			sectionBounds.ReportHeaderRectangle = rectangle;
			
			//PageHeader

			this.sectionBounds.MeasurePageHeader(reportModel.PageHeader,
			                                     rectangle,graphics);
			
			//PageFooter
			
			this.sectionBounds.MeasurePageFooter (reportModel.PageFooter,graphics);
			
			//ReportFooter
			
			sectionBounds.ReportFooterRectangle = this.sectionBounds.MeasureReportFooter(reportModel.ReportFooter,
			                                                                             graphics);
			graphics.Dispose();
		}
		
		public void AddLineItem (BaseExportColumn item) {
			if (item == null) {
				throw new ArgumentNullException("item");
			}
			this.items.Add(item);
		}
		
		public ExporterCollection<BaseExportColumn> Items {
			get {
				if (this.items == null) {
					items = new ExporterCollection<BaseExportColumn>();
				}
				return items;
			}
		}
		
		public SectionBounds SectionBounds {
			get {
				return sectionBounds;
			}
		}
		
	}

}
