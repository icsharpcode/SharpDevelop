// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision: 5369 $</version>
// </file>

using System;
using iTextSharp.text.pdf;

namespace ICSharpCode.Reports.Core.Exporter
{
	public interface IExportContainer
	{
		void DrawItem(System.Drawing.Graphics graphics);
		void DrawItem(PdfWriter pdfWriter, ICSharpCode.Reports.Core.Exporter.ExportRenderer.PdfUnitConverter converter);
//		void AddLineItem(BaseExportColumn item);
		ExporterCollection Items { get; }
	}
}
