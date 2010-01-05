/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 15.04.2009
 * Zeit: 20:51
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Drawing;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core.Exporter
{
	/// <summary>
	/// Description of IExportItemsConverter.
	/// </summary>
	
	public interface IExportItemsConverter
	{
		BaseExportColumn ConvertToLineItem (BaseReportItem item);
		ExportContainer ConvertToContainer (IContainerItem item);
		Point ParentLocation {get;set;}
		int Offset {set;}
	}
}
