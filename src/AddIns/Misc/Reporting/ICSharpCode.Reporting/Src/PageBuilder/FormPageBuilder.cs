/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 03.04.2013
 * Time: 20:21
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.Exporter;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.PageBuilder.Converter;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.PageBuilder
{
	/// <summary>
	/// Description of FormPageBuilder.
	/// </summary>
	public class FormPageBuilder:BasePageBuilder
	{
		
//		private readonly object addLock = new object();
		
		public FormPageBuilder(IReportModel reportModel):base(reportModel)
		{
		}
		
		
		public override void BuildExportList()
		{
			base.BuildExportList();
			WritePages ();
			BuildReportHeader();
		}
		
		
		void BuildReportHeader()
		{
			if (Pages.Count == 0) {
				var sc = new ContainerConverter(ReportModel.ReportHeader,CurrentLocation);
				var header =sc.Convert();
				CurrentPage.ExportedItems.Add(header);
				var r = new Rectangle(header.Location.X,header.Location.Y,header.Size.Width,header.Size.Height);
				CurrentLocation = new Point (ReportModel.ReportSettings.LeftMargin,r.Bottom + 5);
			}
		}
		
		void BuildPageHeader()
		{
			var sc = new ContainerConverter(ReportModel.PageHeader,CurrentLocation);
			var header =sc.Convert();
			CurrentPage.ExportedItems.Add(header);
		}
		
		void BuilDetail()
		{
			Console.WriteLine("FormPageBuilder - Build DetailSection {0} - {1} - {2}",ReportModel.ReportSettings.PageSize.Width,ReportModel.ReportSettings.LeftMargin,ReportModel.ReportSettings.RightMargin);
		}
		
		
		void BuildPageFooter()
		{
			Console.WriteLine("FormPageBuilder - Build PageFooter {0} - {1}",ReportModel.ReportSettings.PageSize.Height,ReportModel.ReportSettings.BottomMargin);
			CurrentLocation = new Point(ReportModel.ReportSettings.LeftMargin,
			                            ReportModel.ReportSettings.PageSize.Height - ReportModel.ReportSettings.BottomMargin - ReportModel.PageFooter.Size.Height);
				
			var sc = new ContainerConverter(ReportModel.PageFooter,CurrentLocation);
			var header =sc.Convert();
			CurrentPage.ExportedItems.Add(header);
		}
		
		
		void BuildReportFooter()
		{
			Console.WriteLine("FormPageBuilder - Build ReportFooter {0} - {1}",ReportModel.ReportSettings.PageSize.Height,ReportModel.ReportSettings.BottomMargin);
			CurrentLocation = new Point(ReportModel.ReportSettings.LeftMargin,
			                            ReportModel.ReportSettings.PageSize.Height - ReportModel.ReportSettings.BottomMargin - ReportModel.PageFooter.Size.Height);
				
			var sc = new ContainerConverter(ReportModel.ReportFooter,CurrentLocation);
			var header =sc.Convert();
			CurrentPage.ExportedItems.Add(header);
		}
		
		
		void WritePages()
		{
			CurrentPage = base.InitNewPage();
			CurrentLocation = new Point(ReportModel.ReportSettings.LeftMargin,ReportModel.ReportSettings.TopMargin);
			this.BuildReportHeader();
			BuildPageHeader();
			BuilDetail();
			BuildPageFooter();
//			BuildReportFooter();
			base.AddPage(CurrentPage);
		}
		
	
	}
}
