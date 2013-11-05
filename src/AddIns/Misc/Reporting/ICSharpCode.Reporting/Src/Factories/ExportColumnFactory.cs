/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 09.04.2013
 * Time: 19:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Factories
{
	/// <summary>
	/// Description of ExportColumnFactory.
	/// </summary>
	internal class ExportColumnFactory
	{
		public ExportColumnFactory()
		{
		}
		
		public static IExportColumn CreateItem (IPrintableObject item) {
			var export = item.CreateExportColumn();
			return export;
		}
	}
}
