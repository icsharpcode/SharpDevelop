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

namespace ICSharpCode.Reports.Core.old_Exporter
{
	/// <summary>
	/// Description of IExportItemsConverter.
	/// </summary>
	
	public interface IExportItemsConverter
	{
		ExporterCollection ConvertSimpleItems (BaseReportItem parent,Point offset,ReportItemCollection items);
		
		ExportContainer ConvertToContainer (BaseReportItem parent,Point offset,ISimpleContainer item);
		Rectangle ParentRectangle {get;set;}
	}
}
