/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 03.04.2013
 * Time: 20:21
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.Interfaces;

namespace ICSharpCode.Reporting.PageBuilder
{
	/// <summary>
	/// Description of FormPageBuilder.
	/// </summary>
	public class FormPageBuilder:BasePageBuilder
	{
		
		private readonly object addLock = new object();
		
		public FormPageBuilder(IReportModel reportModel):base(reportModel)
		{
			
		}
		
		
		public override void BuildExportList()
		{
			base.BuildExportList();
			WritePages ();
		}
		
		
		void WritePages()
		{
			CurrentPage = base.InitNewPage();
			this.BuildReportHeader();
			base.AddPage(CurrentPage);
		}
		
		void BuildReportHeader()
		{
			
		}
	
	}
}
