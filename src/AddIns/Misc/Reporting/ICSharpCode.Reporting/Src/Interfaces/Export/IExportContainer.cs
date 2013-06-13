/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 12.04.2013
 * Time: 20:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Interfaces.Export
{
	/// <summary>
	/// Description of IExportContainer.
	/// </summary>
	public interface IExportContainer:IExportColumn
	{
		List<IExportColumn> ExportedItems {get;}
		
	}
}
