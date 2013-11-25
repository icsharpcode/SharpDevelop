// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Drawing;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Pdf
{
	/// <summary>
	/// Description of PdfVisitor.
	/// </summary>
	class PdfVisitor: AbstractVisitor
	{
		readonly PdfDocument pdfDocument;
		XGraphics gfx;
		XTextFormatter textFormatter;
		Point containerLocation;
		
		public PdfVisitor(PdfDocument pdfDocument)
		{
			this.pdfDocument = pdfDocument;
		}
		
		public override void Visit(ExportPage page)
		{
			var pageSize = page.Size.ToXSize();
			PdfPage = pdfDocument.AddPage();
			gfx = XGraphics.FromPdfPage(PdfPage);
			textFormatter  = new XTextFormatter(gfx);
			base.Visit(page);
		}
		
		public override void Visit(ExportContainer exportContainer)
		{
			containerLocation = exportContainer.Location;
			PdfHelper.DrawRectangle(exportContainer,gfx);
			foreach (var element in exportContainer.ExportedItems) {
				var ac = element as IAcceptor;
				ac.Accept(this);
			}
		}
		
		
		public override void Visit(ExportText exportColumn)
		{
			var columnLocation = containerLocation;
			columnLocation.Offset(exportColumn.Location);
			PdfHelper.WriteText(textFormatter,columnLocation, exportColumn);
			if (exportColumn.DrawBorder) {
				PdfHelper.DrawRectangle(new Rectangle(columnLocation,exportColumn.DesiredSize),exportColumn.FrameColor,gfx);
			}
		}

		public PdfPage PdfPage {get; private set;}
	}
}
