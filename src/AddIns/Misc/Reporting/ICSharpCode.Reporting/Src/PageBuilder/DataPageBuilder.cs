/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 06.06.2013
 * Time: 20:27
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using ICSharpCode.Reporting.Items;

namespace ICSharpCode.Reporting.PageBuilder
{
	/// <summary>
	/// Description of DataPageBuilder.
	/// </summary>
	public class DataPageBuilder:BasePageBuilder
	{
		public DataPageBuilder(ReportModel reportModel, IEnumerable<object> list):base(reportModel)
		{
			List = list;
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
		
		
		
		public IEnumerable<object> List {get; private set;}
	}
}
