/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 03.04.2013
 * Time: 20:32
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.Globals;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.PageBuilder.Converter;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.PageBuilder
{
	/// <summary>
	/// Description of BasePageBuilder.
	/// </summary>
	public class BasePageBuilder:IReportCreator
	{

		public BasePageBuilder(IReportModel reportModel)
		{
			if (reportModel == null) {
				 throw new ArgumentNullException("reportModel");
			}
			ReportModel = reportModel;
			Pages = new Collection<ExportPage>();
			Graphics = CreateGraphics.FromSize(reportModel.ReportSettings.PageSize);
		}
		
		
		protected ExportPage InitNewPage(){
			var pi = CreatePageInfo();
			return new ExportPage(pi,ReportModel.ReportSettings.PageSize);
		}
		
		#region create Sections
		
		protected void BuildReportHeader()
		{
			if (Pages.Count == 0) {
				var header = CreateSection(ReportModel.ReportHeader,CurrentLocation);
				var r = new Rectangle(header.Location.X,header.Location.Y,header.Size.Width,header.Size.Height);
				CurrentLocation = new Point (ReportModel.ReportSettings.LeftMargin,r.Bottom + 1);
				AddSectionToPage(header);
			}
		}
		
		
		protected void BuildPageHeader()
		{
			var pageHeader = CreateSection(ReportModel.PageHeader,CurrentLocation);
			DetailStart = new Point(ReportModel.ReportSettings.LeftMargin,pageHeader.Location.Y + pageHeader.Size.Height +1);
			AddSectionToPage(pageHeader);
		}
		
		
		protected void BuildPageFooter()
		{
			CurrentLocation = new Point(ReportModel.ReportSettings.LeftMargin,
			                            ReportModel.ReportSettings.PageSize.Height - ReportModel.ReportSettings.BottomMargin - ReportModel.PageFooter.Size.Height);
			
			var pageFooter = CreateSection(ReportModel.PageFooter,CurrentLocation);	
//			DetailEnds = new Point(pageFooter.Location.X,pageFooter.Location.Y -1);
			DetailEnds = new Point(pageFooter.Location.X + pageFooter.Size.Width,pageFooter.Location.Y -1);
			AddSectionToPage(pageFooter);
		}
		
		
		protected void BuildReportFooter()
		{
			var lastSection = CurrentPage.ExportedItems.Last();
			CurrentLocation = new Point(ReportModel.ReportSettings.LeftMargin,
			                            lastSection.Location.Y - lastSection.Size.Height - 1);

			var reportFooter = CreateSection(ReportModel.ReportFooter,CurrentLocation);	
			AddSectionToPage(reportFooter);
		}
		
		#endregion
		
		protected virtual ExportPage CreateNewPage()
		{
			var page = InitNewPage();
			CurrentLocation = new Point(ReportModel.ReportSettings.LeftMargin,ReportModel.ReportSettings.TopMargin);
			return page;
		}
		
		
		protected void  WriteStandardSections() {
			this.BuildReportHeader();
			BuildPageHeader();
			BuildPageFooter();
		}
		
		
		protected bool old_PageFull(System.Collections.Generic.List<IExportColumn> columns)
		{
			var rectToPrint = new Rectangle(columns[0].Location,columns[0].Size);
			Console.WriteLine("{0} - {1}",CurrentLocation,rectToPrint.ToString());
			if (rectToPrint.Bottom > DetailEnds.Y) {
				Console.WriteLine("----------PB---");
				return  true;
			}
			return false;
		}
		
		
		protected bool old_1_PageFull(IExportContainer row) {
			var rectToPrint = new Rectangle(row.Location,row.DesiredSize);
			Console.WriteLine("{0} - {1}",rectToPrint.Bottom.ToString(),DetailEnds.Y);
			if (rectToPrint.Bottom > DetailEnds.Y) {	
				Console.WriteLine("-----------PB----------");
				return  true;
			}
			return false;
		}
		

		
		protected bool PageFull(IExportContainer row) {
			var rectToPrint = new Rectangle(new Point(row.Location.X,row.Location.Y + DetailsRectangle.Location.Y),
			                                row.DesiredSize);
			if (!DetailsRectangle.Contains(rectToPrint)) {
				return  true;
			}
			return false;
		}
		
		
		protected IExportContainer CreateSection(IReportContainer container,Point location)
		{
			var containerConverter = new ContainerConverter(Graphics, location);
			var convertedContainer = containerConverter.Convert(container);
			
			var list = containerConverter.CreateConvertedList(container,convertedContainer);
			convertedContainer.ExportedItems.AddRange(list);
			
			convertedContainer.DesiredSize = MeasureElement(convertedContainer);
			ArrangeContainer(convertedContainer);
			return convertedContainer;
		}
		
		
		protected void AddSectionToPage(IExportContainer header)
		{
			header.Parent = CurrentPage;
			CurrentPage.ExportedItems.Add(header);
		}
		
		
		protected Size MeasureElement (IExportColumn element) {
			var measureStrategy = element.MeasurementStrategy();
			return measureStrategy.Measure(element, Graphics);
		}
		  
		
		protected void ArrangeContainer(IExportContainer exportContainer)
		{
			var exportArrange = exportContainer.GetArrangeStrategy();
			exportArrange.Arrange(exportContainer);
		}

		
		IPageInfo CreatePageInfo()
		{
			var pi = new PageInfo();
			pi.PageNumber = Pages.Count +1;
			pi.ReportName = ReportModel.ReportSettings.ReportName;
			pi.ReportFileName = ReportModel.ReportSettings.FileName;
			return pi;
		}
		

		protected virtual  void AddPage(ExportPage page) {
			if (Pages.Count == 0) {
				page.IsFirstPage = true;
			}
			Pages.Add(page);
		}
		
		
		public virtual void BuildExportList()
		{
			this.Pages.Clear();
		}
		
		protected IReportModel ReportModel {get; private set;}
		
		protected Point CurrentLocation {get; set;}

	    protected ExportPage CurrentPage {get; set;}
		
	    internal Point DetailStart {get;private set;}
	    
	    internal Point DetailEnds {get; private set;}
	    
	    internal Rectangle DetailsRectangle {
	    	get {
	    		var s = new Size(DetailEnds.X - DetailStart.X,DetailEnds.Y - DetailStart.Y);
	    		return new Rectangle(DetailStart,s);
	    	}
	    }
	    
	    protected Graphics Graphics {get;private set;}
	    
		public Collection<ExportPage> Pages {get; private set;}
	}
}
