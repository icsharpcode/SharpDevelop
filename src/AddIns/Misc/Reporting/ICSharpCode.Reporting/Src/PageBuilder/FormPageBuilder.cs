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
		
		public FormPageBuilder(IReportModel reportModel):base(reportModel)
		{
			
		}
		
		
		public override void BuildExportList()
		{
			base.BuildExportList();
			WritePages ();
		}
		
		
		
		void BuilDetail()
		{
			Console.WriteLine("FormPageBuilder - Build DetailSection {0} - {1} - {2}",ReportModel.ReportSettings.PageSize.Width,ReportModel.ReportSettings.LeftMargin,ReportModel.ReportSettings.RightMargin);
			CurrentLocation = DetailStart;
			
			var detail = CreateSection(ReportModel.DetailSection,CurrentLocation);
			detail.Parent = CurrentPage;
			CurrentPage.ExportedItems.Insert(2,detail);
		}
		
		
		protected override void WritePages()
		{
			base.WritePages();
			BuilDetail();
			base.AddPage(CurrentPage);
			Console.WriteLine("------{0}---------",ReportModel.ReportSettings.PageSize);
		}
		
		
	}
}
