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

using System;
using System.Drawing;
using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.Interfaces.Export;
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
		XGraphics xGraphics;
		XTextFormatter textFormatter;
		Point containerLocation;
		
		public PdfVisitor(PdfDocument pdfDocument){
			this.pdfDocument = pdfDocument;
		}
		
		
		public override void Visit(ExportPage page){
			PdfPage = pdfDocument.AddPage();
			xGraphics = XGraphics.FromPdfPage(PdfPage);
			textFormatter  = new XTextFormatter(xGraphics);
			base.Visit(page);
		}
		
		
		public override void Visit(ExportContainer exportContainer){
			foreach (var element in exportContainer.ExportedItems) {
				if (IsContainer(element)) {
					var container = element as ExportContainer;
					containerLocation = PdfHelper.LocationRelToParent(container);
					RenderRow(container);
				} else {
					containerLocation = PdfHelper.LocationRelToParent(exportContainer);
					var acceptor = element as IAcceptor;
					acceptor.Accept(this);
				}
			}
		}
		
		
		void RenderRow(IExportContainer container){
			if (IsGraphicsContainer(container)) {
				RenderGraphicsContainer(container);
			} else {
				RenderDataRow(container);
			}
		}
		
		
		void RenderGraphicsContainer(IExportColumn column){
			var graphicsContainer = column as GraphicsContainer;
			if (graphicsContainer != null) {
				var rect = column as ExportRectangle;
				if (rect != null) {
					Visit(rect);
				}
				
				var circle = column as ExportCircle;
				if (circle != null) {
					Visit(circle);
				}
			}
		}
		
		
		void RenderDataRow (IExportContainer row) {
			var r = new Rectangle(containerLocation,row.DisplayRectangle.Size);
			PdfHelper.FillRectangle(r,row.BackColor,xGraphics);
			foreach (var element in row.ExportedItems) {
				var acceptor = element as IAcceptor;
				acceptor.Accept(this);
			}
		}
		
		
		public override void Visit(ExportText exportText){
			var columnLocation = new Point(containerLocation.X + exportText.Location.X,containerLocation.Y + exportText.Location.Y);
			var columnRect = new Rectangle(columnLocation,exportText.DisplayRectangle.Size).ToXRect();
			if (ShouldSetBackcolor(exportText)) {
				PdfHelper.FillRectangle(columnRect,exportText.BackColor,xGraphics);
			}
			
			if (HasFrame(exportText)) {
				PdfHelper.DrawBorder(columnRect,exportText,xGraphics);
			}
			PdfHelper.WriteText(textFormatter,columnLocation, exportText);
		}

		
		public override void Visit(ExportImage exportImage)
		{
			XImage image = XImage.FromGdiPlusImage(exportImage.Image);
			var location = PdfHelper.LocationRelToParent(exportImage);
			if (exportImage.ScaleImageToSize) {
				xGraphics.DrawImage(image, location.X.ToPoint(), location.Y.ToPoint(), 
				                    exportImage.Size.Width.ToPoint(),exportImage.Size.Height.ToPoint());
			} else {
				xGraphics.DrawImage(image, location.X.ToPoint(), location.Y.ToPoint(), 
				                    exportImage.Image.Size.Width.ToPoint(),exportImage.Image.Size.Height.ToPoint());
			}
		}
		
		
		public override void Visit(ExportLine element){
			var pen = PdfHelper.CreatePen(element);
			var fromPoint = PdfHelper.LocationRelToParent(element);
			var toPoint = new Point(fromPoint.X + element.Size.Width,fromPoint.Y);
			xGraphics.DrawLine(pen,fromPoint.ToXPoints(),toPoint.ToXPoints());
		}
		
		
		public override void Visit (ExportRectangle exportRectangle) {
			var savedLocation = containerLocation;
			xGraphics.DrawRectangle(PdfHelper.CreatePen(exportRectangle),
			                        PdfHelper.CreateBrush(exportRectangle.BackColor),
			                        new XRect(containerLocation.ToXPoints(),
			                                  exportRectangle.Size.ToXSize()));
			RenderContainerInternal(exportRectangle,savedLocation);
		}
			
		
		public override void Visit(ExportCircle exportCircle){
			var savedLocation = containerLocation;
			var pen = PdfHelper.CreatePen(exportCircle);
			xGraphics.DrawEllipse(pen,
			                      PdfHelper.CreateBrush(exportCircle.BackColor) ,
			                      new XRect(containerLocation.ToXPoints(),
			                                exportCircle.Size.ToXSize()));
			RenderContainerInternal (exportCircle,savedLocation);
		}

		
		void RenderContainerInternal(IExportContainer graphicsContainer,Point savedLocation){
			if (graphicsContainer.ExportedItems.Count > 0) {
				foreach (var element in graphicsContainer.ExportedItems) {
					if (IsGraphicsContainer(element)) {
						var loc = new Point(containerLocation.X + element.Location.X,
						                    containerLocation.Y + element.Location.Y);
						containerLocation = loc;
						RenderGraphicsContainer(element);
						containerLocation = savedLocation;
					} else {
						var acceptor = AsAcceptor(element);
						acceptor.Accept(this);
					}
				}
			}
		}
		
		public PdfPage PdfPage {get; private set;}
	}
}
