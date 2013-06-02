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
using System.Collections.ObjectModel;
using System.Drawing;

using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.Globals;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.PageBuilder.Converter;

namespace ICSharpCode.Reporting.PageBuilder
{
	/// <summary>
	/// Description of BasePageBuilder.
	/// </summary>
	public class BasePageBuilder:IReportCreator
	{
		Graphics graphics;
		
		public BasePageBuilder(IReportModel reportModel)
		{
			if (reportModel == null) {
				 throw new ArgumentNullException("reportModel");
			}
			ReportModel = reportModel;
			Pages = new Collection<IPage>();
			graphics = CreateGraphics.FromSize(reportModel.ReportSettings.PageSize);
		}
		
		
		protected IPage InitNewPage(){
			var pi = CreatePageInfo();
			return new Page(pi,ReportModel.ReportSettings.PageSize);
		}
		
		protected IExportContainer CreateSection(IReportContainer section,Point location)
		{
			var containerConverter = new ContainerConverter(graphics, section, location);
			var header = containerConverter.Convert();
			return header;
		}
		
		
		protected void AddSectionToPage(IExportContainer header)
		{
			header.Parent = CurrentPage;
			CurrentPage.ExportedItems.Add(header);
		}
		
		
		IPageInfo CreatePageInfo()
		{
			var pi = new PageInfo();
			pi.PageNumber = Pages.Count +1;
			pi.ReportName = ReportModel.ReportSettings.ReportName;
			pi.ReportFileName = ReportModel.ReportSettings.FileName;
			return pi;
		}
		

		protected virtual  void AddPage(IPage page) {
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

	    protected IPage CurrentPage {get; set;}
		
		public Collection<IPage> Pages {get; private set;}
		
	}
}
