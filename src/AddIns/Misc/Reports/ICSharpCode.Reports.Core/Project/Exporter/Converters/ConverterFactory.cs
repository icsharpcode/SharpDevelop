// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core.Exporter
{
	/// <summary>
	/// Description of ConverterFactory.
	/// </summary>
	
	public static class ConverterFactory
	{
		
		public static IBaseConverter CreateConverter (BaseReportItem itemToConvert,IReportModel reportModel,
		                                              IDataNavigator dataNavigator,ExporterPage singlePage)
		                                             
		{
			Type t = itemToConvert.GetType();
			if (t.Equals(typeof(BaseTableItem))) {
				return new GroupedTableConverter(reportModel,dataNavigator,singlePage);
			}
			
			if (t.Equals(typeof(BaseRowItem))) {
				return new GroupedRowConverter (reportModel,dataNavigator,singlePage);
			}
			
			return null;
		}
	}
}
