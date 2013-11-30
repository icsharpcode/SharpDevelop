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
				var con = element as ExportContainer;
				if (con != null) {
					containerLocation = PdfHelper.LocationRelToParent(con);
					var r = new Rectangle(containerLocation,con.DisplayRectangle.Size);
					PdfHelper.FillRectangle(r,con.BackColor,gfx);
					Visit(con);
				}
				containerLocation = PdfHelper.LocationRelToParent(exportContainer);
				var ac = element as IAcceptor;
				ac.Accept(this);
			}
		}
		
		
		public override void Visit(ExportText exportColumn)
		{
			var columnLocation = containerLocation;
			columnLocation.Offset(exportColumn.Location);
			
			if (exportColumn.BackColor != Color.White) {
				var r = new Rectangle(columnLocation,exportColumn.DisplayRectangle.Size);
				PdfHelper.FillRectangle(r,exportColumn.BackColor,gfx);
			}
			PdfHelper.WriteText(textFormatter,columnLocation, exportColumn);
			
		}

		
		public PdfPage PdfPage {get; private set;}
	}
}
