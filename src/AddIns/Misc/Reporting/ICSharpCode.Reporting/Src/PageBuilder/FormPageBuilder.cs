/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 03.04.2013
 * Time: 20:21
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using ICSharpCode.Reporting.Exporter;
using ICSharpCode.Reporting.Expressions;
using ICSharpCode.Reporting.Interfaces;

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
			BuildDetail();
			base.AddPage(CurrentPage);
			
			RunDebugVisitor();
			RunExpressions();
		}

		
		void BuildDetail()
		{
			CurrentLocation = DetailStart;
			var detail = CreateSection(ReportModel.DetailSection,CurrentLocation);
			detail.Parent = CurrentPage;
			CurrentPage.ExportedItems.Insert(2,detail);
		}
	}
}
