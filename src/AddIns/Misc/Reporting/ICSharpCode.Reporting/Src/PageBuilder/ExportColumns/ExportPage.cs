/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 03.04.2013
 * Time: 20:35
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;

using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.Interfaces.Export;

namespace ICSharpCode.Reporting.PageBuilder.ExportColumns
{
	/// <summary>
	/// Description of Page.
	/// </summary>
	///
	
	public class ExportPage:ExportColumn,IPage,IAcceptor
	{
		public ExportPage(IPageInfo pageInfo,Size pageSize):base()
		{
			if (pageInfo == null) {
				throw new ArgumentNullException("pageInfo");
			}
			PageInfo = pageInfo;
			Name = "Page";
			Size = pageSize;
			exportedItems = new List<IExportColumn>();
		}
		
		
		public bool IsFirstPage {get;set;}
		
		
		public IPageInfo PageInfo {get;private set;}
	
		
		public bool CanShrink {get;set;}
		
		
		public void Accept(IVisitor visitor)
		{
			visitor.Visit(this as ExportPage);
		}
		
		
		List<IExportColumn> exportedItems;
		
		public List<IExportColumn> ExportedItems {
			get { return exportedItems; }
		}
	}
}
