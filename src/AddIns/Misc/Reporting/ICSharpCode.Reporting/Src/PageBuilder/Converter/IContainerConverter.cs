/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 08.04.2013
 * Time: 19:49
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;

using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Interfaces.Export;

namespace ICSharpCode.Reporting.PageBuilder.Converter
{
	internal interface IContainerConverter
	{
		IExportContainer Convert(IReportContainer reportContainer);
		List<IExportColumn> CreateConvertedList(IReportContainer reportContainer,IExportContainer exportContainer);
		List<IExportColumn> CreateConvertedList(IReportContainer reportContainer,IExportContainer exportContainer,Point position);
		Size Measure(IExportColumn element);
		void ArrangeContainer(IExportContainer exportContainer);
	}
}
