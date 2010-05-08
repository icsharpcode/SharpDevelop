/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 04.01.2009
 * Zeit: 16:40
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
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
		                                             ExporterPage singlePage,IExportItemsConverter exportItemsConverter,ILayouter layouter)
		{

			Type t = itemToConvert.GetType();
			if (t.Equals(typeof(BaseTableItem))) {
				return new TableConverter(dataNavigator,singlePage,exportItemsConverter,layouter);
			}
			
			if (t.Equals(typeof(BaseRowItem))) {
				return new RowConverter (dataNavigator,singlePage,exportItemsConverter,layouter);
			}
			
			return null;
		}
	}
}
