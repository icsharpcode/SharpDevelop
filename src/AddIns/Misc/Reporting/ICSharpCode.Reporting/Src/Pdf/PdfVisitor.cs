// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
		
		
		public override void Visit(ExportCircle exportCircle){
			var columnLocation = containerLocation;
			columnLocation.Offset(exportCircle.Location);
			var pen = PdfHelper.PdfPen(exportCircle);
			pen.DashStyle = PdfHelper.DashStyle(exportCircle);
			gfx.DrawEllipse(pen,new XRect(columnLocation.ToXPoints(),
			                                exportCircle.Size.ToXSize()));
		}
		
		
		public PdfPage PdfPage {get; private set;}
	}
}
