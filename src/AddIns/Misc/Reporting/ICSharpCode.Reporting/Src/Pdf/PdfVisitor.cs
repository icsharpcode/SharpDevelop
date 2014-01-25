// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Drawing;
using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;

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
			foreach (var element in exportContainer.ExportedItems) {
				var container = element as ExportContainer;
				if (container != null) {
					containerLocation = PdfHelper.LocationRelToParent(container);
					var r = new Rectangle(containerLocation,container.DisplayRectangle.Size);
					PdfHelper.FillRectangle(r,container.BackColor,gfx);
					Visit(container);
				}
				containerLocation = PdfHelper.LocationRelToParent(exportContainer);
				var ac = element as IAcceptor;
				ac.Accept(this);
			}
		}
		
		
		public override void Visit(ExportText exportText)
		{
			var columnLocation = containerLocation;
			columnLocation.Offset(exportText.Location);
			if (ShouldSetBackcolor(exportText)) {
				var r = new Rectangle(columnLocation,exportText.DisplayRectangle.Size);
				PdfHelper.FillRectangle(r,exportText.BackColor,gfx);
			}
			
			PdfHelper.WriteText(textFormatter,columnLocation, exportText);
		}

		
		
		public override void Visit(ExportLine exportLine)
		{
			var columnLocation = containerLocation;
			columnLocation.Offset(exportLine.Location);
			var pen = PdfHelper.PdfPen(exportLine);
			pen.DashStyle = PdfHelper.DashStyle(exportLine);
			pen.LineCap = PdfHelper.LineCap(exportLine);
			gfx.DrawLine(pen,columnLocation.ToXPoints(),new Point(exportLine.Size.Width,columnLocation.Y).ToXPoints());
		}
		
		
		public override void Visit (ExportRectangle exportRectangle) {
			var columnLocation = containerLocation;
			columnLocation.Offset(exportRectangle.Location);
			var pen = PdfHelper.PdfPen(exportRectangle);
			pen.DashStyle = PdfHelper.DashStyle(exportRectangle);
			gfx.DrawRectangle(pen,new XRect(columnLocation.ToXPoints(),
			                                exportRectangle.Size.ToXSize()));
		}
		
		
		public PdfPage PdfPage {get; private set;}
	}
}
