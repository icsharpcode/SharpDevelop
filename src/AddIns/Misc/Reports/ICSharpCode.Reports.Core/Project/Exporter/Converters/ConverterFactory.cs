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
		
		
		public static IBaseConverter CreateConverter (BaseReportItem itemToConvert,IDataNavigator dataNavigator,
		                                             ExporterPage singlePage,ILayouter layouter)
		{

			Type t = itemToConvert.GetType();
			if (t.Equals(typeof(BaseTableItem))) {
				return new TableConverter(dataNavigator,singlePage,layouter);
			}
			
			if (t.Equals(typeof(BaseRowItem))) {
				return new GroupedRowConverter (dataNavigator,singlePage,layouter);
			}
			
			return null;
		}
	}
}
