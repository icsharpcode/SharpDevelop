/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 03.04.2013
 * Time: 20:32
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.Interfaces;

namespace ICSharpCode.Reporting.PageBuilder
{
	/// <summary>
	/// Description of BasePageBuilder.
	/// </summary>
	public class BasePageBuilder:IReportCreator
	{
		private readonly object addLock = new object();
		
		public BasePageBuilder(IReportModel reportModel)
		{
			if (reportModel == null) {
				 throw new ArgumentNullException("reportModel");
			}
			ReportModel = reportModel;
			Pages = new List<IPage>();
		}
		
		
		protected IPage InitNewPage(){
			return new Page();
		}
		

		protected virtual  void AddPage(IPage page) {
			Pages.Add(page);
		}
		
		
		public virtual void BuildExportList()
		{
			this.Pages.Clear();
		}
		
		protected IReportModel ReportModel {get; private set;}
		
		public  IPage CurrentPage {get; protected set;}
		
		public List<IPage> Pages {get; private set;}
		
	}
}
