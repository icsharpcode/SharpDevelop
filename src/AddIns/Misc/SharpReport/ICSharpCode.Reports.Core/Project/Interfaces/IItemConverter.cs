/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 14.08.2009
 * Zeit: 20:00
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using ICSharpCode.Reports.Core.Exporter;
using System;
using System.Drawing;
using ICSharpCode.Reports.Core.Events;

namespace ICSharpCode.Reports.Core.Interfaces
{
	/// <summary>
	/// Description of IItemConverter.
	/// </summary>
	public interface IBaseConverter
	{		
		ExporterCollection Convert (BaseReportItem parent,BaseReportItem item);	
		event EventHandler <NewPageEventArgs> PageFull;
		SectionBounds SectionBounds {get;}
		IDataNavigator DataNavigator {get;}
		Rectangle ParentRectangle {get;}
		ExporterPage SinglePage {get;}
		IExportItemsConverter ExportItemsConverter{get;}
		ILayouter Layouter {get;}
		Graphics Graphics {get;set;}
	}
}
