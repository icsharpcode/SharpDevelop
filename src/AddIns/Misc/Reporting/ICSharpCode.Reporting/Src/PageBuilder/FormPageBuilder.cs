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
using System.Linq;

using ICSharpCode.Reporting.Globals;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Interfaces.Export;

namespace ICSharpCode.Reporting.PageBuilder
{
	/// <summary>
	/// Description of FormPageBuilder.
	/// </summary>
	public class FormPageBuilder:BasePageBuilder
	{
		
		Point detailStart;
		
		public FormPageBuilder(IReportModel reportModel):base(reportModel)
		{
			
		}
		
		
		public override void BuildExportList()
		{
			base.BuildExportList();
			WritePages ();
		}
		
		
		void BuildReportHeader()
		{
			if (Pages.Count == 0) {
				IExportContainer header = CreateSection(ReportModel.ReportHeader,CurrentLocation);
				var r = new Rectangle(header.Location.X,header.Location.Y,header.Size.Width,header.Size.Height);
				CurrentLocation = new Point (ReportModel.ReportSettings.LeftMargin,r.Bottom + 1);
				AddSectionToPage(header);
			}
		}
		
		void BuildPageHeader()
		{
			IExportContainer header = CreateSection(ReportModel.PageHeader,CurrentLocation);
			detailStart = new Point(ReportModel.ReportSettings.LeftMargin,header.Location.Y + header.Size.Height +1);
			AddSectionToPage(header);
		}
		
		void BuilDetail()
		{
			Console.WriteLine("FormPageBuilder - Build DetailSection {0} - {1} - {2}",ReportModel.ReportSettings.PageSize.Width,ReportModel.ReportSettings.LeftMargin,ReportModel.ReportSettings.RightMargin);
			CurrentLocation = detailStart;
			IExportContainer header = CreateSection(ReportModel.DetailSection,CurrentLocation);
			header.Parent = CurrentPage;
			CurrentPage.ExportedItems.Insert(2,header);
		}
		
		
		void BuildPageFooter()
		{
			Console.WriteLine("FormPageBuilder - Build PageFooter {0} - {1}",ReportModel.ReportSettings.PageSize.Height,ReportModel.ReportSettings.BottomMargin);
			CurrentLocation = new Point(ReportModel.ReportSettings.LeftMargin,
			                            ReportModel.ReportSettings.PageSize.Height - ReportModel.ReportSettings.BottomMargin - ReportModel.PageFooter.Size.Height);
			
			IExportContainer header = CreateSection(ReportModel.PageFooter,CurrentLocation);	
			AddSectionToPage(header);
		}

		
		void BuildReportFooter()
		{
			Console.WriteLine("FormPageBuilder - Build ReportFooter {0} - {1}",ReportModel.ReportSettings.PageSize.Height,ReportModel.ReportSettings.BottomMargin);
			var lastSection = CurrentPage.ExportedItems.Last();
			CurrentLocation = new Point(ReportModel.ReportSettings.LeftMargin,
			                            lastSection.Location.Y - lastSection.Size.Height - 1);

			IExportContainer header = CreateSection(ReportModel.ReportFooter,CurrentLocation);	
			AddSectionToPage(header);
		}
		
		
		void WritePages()
		{
			CurrentPage = base.InitNewPage();
			CurrentLocation = new Point(ReportModel.ReportSettings.LeftMargin,ReportModel.ReportSettings.TopMargin);
			this.BuildReportHeader();
			BuildPageHeader();
			BuildPageFooter();
			BuilDetail();
			BuildReportFooter();
			base.AddPage(CurrentPage);
			Console.WriteLine("------{0}---------",ReportModel.ReportSettings.PageSize);
		}
		
		
	}
}
