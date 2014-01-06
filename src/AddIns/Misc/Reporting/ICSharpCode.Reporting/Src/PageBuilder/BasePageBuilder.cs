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
using ICSharpCode.Reporting.DataManager.Listhandling;
using ICSharpCode.Reporting.Exporter;
using ICSharpCode.Reporting.Expressions;
using ICSharpCode.Reporting.Globals;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.Items;
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
		
		#region create Report Sections
		
		void BuildReportHeader()
		{
			if (Pages.Count == 0) {
				var header = CreateSection(ReportModel.ReportHeader,CurrentLocation);
				var r = new Rectangle(header.Location.X, header.Location.Y, header.Size.Width, header.Size.Height);
				CurrentLocation = new Point (ReportModel.ReportSettings.LeftMargin,r.Bottom + 1);
				AddSectionToPage(header);
			}
		}
		
		
		void BuildPageHeader()
		{
			var pageHeader = CreateSection(ReportModel.PageHeader,CurrentLocation);
			DetailStart = new Point(ReportModel.ReportSettings.LeftMargin,pageHeader.Location.Y + pageHeader.DesiredSize.Height +1);
			AddSectionToPage(pageHeader);
		}
		
		
		void BuildPageFooter()
		{
			CurrentLocation = new Point(ReportModel.ReportSettings.LeftMargin,
			                            ReportModel.ReportSettings.PageSize.Height - ReportModel.ReportSettings.BottomMargin - ReportModel.PageFooter.Size.Height);
			
			var pageFooter = CreateSection(ReportModel.PageFooter,CurrentLocation);	
			DetailEnds = new Point(pageFooter.Location.X + pageFooter.Size.Width,pageFooter.Location.Y -1);
			AddSectionToPage(pageFooter);
		}
		
		
		void AddSectionToPage(IExportContainer header)
		{
			header.Parent = CurrentPage;
			CurrentPage.ExportedItems.Add(header);
		}
		
		
		protected void BuildReportFooter()
		{
			var lastSection = CurrentPage.ExportedItems.Last();
			CurrentLocation = new Point(ReportModel.ReportSettings.LeftMargin,
			                            lastSection.Location.Y - lastSection.Size.Height - 1);

			var reportFooter = CreateSection(ReportModel.ReportFooter,CurrentLocation);	
			AddSectionToPage(reportFooter);
		}
		
		
		protected void  WriteStandardSections() {
			this.BuildReportHeader();
			BuildPageHeader();
			BuildPageFooter();
		}
		
		
		protected bool PageFull(IExportContainer container) {
			if (container.DisplayRectangle.Bottom > DetailEnds.Y) {
				return true;
			}
			return false;
		}

		#endregion
		

		protected IExportContainer CreateSection(IReportContainer container,Point location)
		{
			var containerConverter = new ContainerConverter(location);
			var convertedContainer = containerConverter.ConvertToExportContainer(container);
			
			var list = containerConverter.CreateConvertedList(container.Items);
			
			containerConverter.SetParent(convertedContainer,list);
			convertedContainer.ExportedItems.AddRange(list);
			
			convertedContainer.DesiredSize = MeasureElement(convertedContainer);
			ArrangeContainer(convertedContainer);
			return convertedContainer;
		}
		
		#region Arrange and Measure
		
		protected Size MeasureElement (IExportColumn element) {
			var measureStrategy = element.MeasurementStrategy();
			return measureStrategy.Measure(element, Graphics);
		}
		  
		
		protected static void ArrangeContainer(IExportContainer exportContainer)
		{
			var exportArrange = exportContainer.GetArrangeStrategy();
			exportArrange.Arrange(exportContainer);
		}

		#endregion
		
		#region Pagehandling
		
		IPageInfo CreatePageInfo()
		{
			var pi = new PageInfo();
			pi.PageNumber = Pages.Count +1;
			pi.ReportName = ReportModel.ReportSettings.ReportName;
			pi.ReportFileName = ReportModel.ReportSettings.FileName;
			return pi;
		}
		
		ExportPage InitNewPage(){
			var pi = CreatePageInfo();
			return new ExportPage(pi,ReportModel.ReportSettings.PageSize);
		}
		
		
		protected virtual ExportPage CreateNewPage()
		{
			var page = InitNewPage();
			CurrentLocation = new Point(ReportModel.ReportSettings.LeftMargin,ReportModel.ReportSettings.TopMargin);
			return page;
		}
		
		protected virtual  void AddPage(ExportPage page) {
			if (Pages.Count == 0) {
				page.IsFirstPage = true;
			}
			Pages.Add(page);
		}
		
		
		public virtual void BuildExportList()
		{
			Pages.Clear();
			CurrentPage = CreateNewPage ();
			WriteStandardSections();
			CurrentLocation = DetailStart;
		}
		
		
		protected void UpdatePageInfo() {
			foreach (var page in Pages) {
				page.PageInfo.TotalPages = Pages.Count;
			}
		}
		
		#endregion
		
		#region Visitors
		
		protected void SetupExpressionRunner (ReportSettings reportsettings,CollectionDataSource dataSource){
			ExpressionRunner = new ExpressionRunner(Pages,reportsettings,dataSource);
		}
		
//		protected void RunExpressions(ReportSettings reportsettings,CollectionDataSource dataSource)
//		{
////			var expressionRunner = new ExpressionRunner(Pages,reportsettings,dataSource);
//			ExpressionRunner.Run();
//		}

		
		protected void RunDebugVisitor()
		{
			var debugExporter = new DebugExporter(Pages);
			debugExporter.Run();
		}
		
		#endregion
		
		protected IReportModel ReportModel {get; private set;}
		
		protected Point CurrentLocation {get; set;}

	    protected ExportPage CurrentPage {get; set;}
		
	    protected Graphics Graphics {get;private set;}
	    
	    internal Point DetailStart {get;private set;}
	    
	    internal Point DetailEnds {get; private set;}
	    
	    internal ExpressionRunner ExpressionRunner {get;private set;}
	    
	    
	    internal Rectangle DetailsRectangle {
	    	get {
	    		var s = new Size(DetailEnds.X - DetailStart.X,DetailEnds.Y - DetailStart.Y);
	    		return new Rectangle(DetailStart,s);
	    	}
	    }
	    
	    
		public Collection<ExportPage> Pages {get; private set;}
		
	}
}
