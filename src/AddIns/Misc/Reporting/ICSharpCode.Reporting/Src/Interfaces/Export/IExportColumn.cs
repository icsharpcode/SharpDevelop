/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 08.04.2013
 * Time: 20:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using ICSharpCode.Reporting.Arrange;
using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.Exporter;

namespace ICSharpCode.Reporting.Interfaces.Export
{
	/// <summary>
	/// Description of IExportColumn.
	/// </summary>
	public interface IExportColumn:IReportObject
	{
		IArrangeStrategy GetArrangeStrategy();
		Size DesiredSize {get;set;}
		IExportColumn Parent {get;set;}
		Rectangle DisplayRectangle {get;}
	}
}
