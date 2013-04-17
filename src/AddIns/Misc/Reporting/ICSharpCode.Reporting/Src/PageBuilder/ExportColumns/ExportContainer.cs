/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 12.04.2013
 * Time: 20:27
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using ICSharpCode.Reporting.Interfaces.Export;

namespace ICSharpCode.Reporting.PageBuilder.ExportColumns
{
	/// <summary>
	/// Description of BaseExportContainer.
	/// </summary>
	public class ExportContainer:ExportColumn,IExportContainer
	{
		public ExportContainer()
		{
			ExportedItems = new List<IExportColumn>();
		}
		
		public  List<IExportColumn> ExportedItems {get;set;}
		
	}
}
