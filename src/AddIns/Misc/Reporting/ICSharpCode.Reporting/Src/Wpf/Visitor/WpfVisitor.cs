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
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

using System.Windows.Media.Imaging;
using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;
using ICSharpCode.Reporting.WpfReportViewer.Visitor.Graphics;

namespace ICSharpCode.Reporting.WpfReportViewer.Visitor
{
	/// <summary>
	/// Description of WpfVisitor.
	/// </summary>
	/// 
	class WpfVisitor: AbstractVisitor {
		
		FixedPage fixedPage;
		Canvas sectionCanvas;
		
		
		public override void Visit(ExportPage page){
			fixedPage = FixedDocumentCreator.CreateFixedPage(page);
			FixedPage = fixedPage;
			foreach (var element in page.ExportedItems) {
				AsAcceptor(element).Accept(this);
				fixedPage.Children.Add(sectionCanvas);
			}
		}
		
		
		public override void Visit(ExportContainer exportContainer){
			
			sectionCanvas = FixedDocumentCreator.CreateContainer(exportContainer);
			sectionCanvas = RenderSectionContainer(exportContainer);
		}
	
		
		Canvas RenderSectionContainer (ExportContainer container) {
			var canvas = FixedDocumentCreator.CreateContainer(container);
			foreach (var element in container.ExportedItems) {
				if (IsContainer(element)) {
					RenderRow(canvas, (IExportContainer)element);
				} else {
					AsAcceptor(element).Accept(this);
					canvas.Children.Add(UIElement);
				}
			}
			canvas.Background = FixedDocumentCreator.ConvertBrush(container.BackColor);
			return canvas;
		}

		
		void RenderRow(Canvas canvas, IExportContainer container){
			if (IsGraphicsContainer(container)) {
				canvas.Children.Add(RenderGraphicsContainer(container));
			} else {
				canvas.Children.Add(RenderDataRow((ExportContainer)container));
			}
		}
		
		
		Canvas RenderDataRow (ExportContainer row) {
			var rowCanvas = FixedDocumentCreator.CreateContainer(row);
			var childCanvas = CreateItemsInContainer(row.ExportedItems);
			rowCanvas.Children.Add(childCanvas);
			return rowCanvas;
		}
		
		
		Canvas RenderGraphicsContainer(IExportColumn column){
		
			var graphicsContainer = column as GraphicsContainer;
			var graphCanvas = FixedDocumentCreator.CreateContainer(graphicsContainer);
			CanvasHelper.SetPosition(graphCanvas, column.Location.ToWpf());
			graphCanvas.Background = FixedDocumentCreator.ConvertBrush(column.BackColor);
			
			if (graphicsContainer != null) {
				var rect = column as ExportRectangle;
				if (rect != null) {
					Visit(rect);
				}
				
				var circle = column as ExportCircle;
				if (circle != null) {
					Visit(circle);
				}
				graphCanvas.Children.Add(UIElement);
			}
			return graphCanvas;
		}
		
//	http://stackoverflow.com/questions/25308612/vertical-alignment-with-drawingcontext-drawtext	
		
		public override void Visit(ExportText exportColumn){
			
			var formattedText = FixedDocumentCreator.CreateFormattedText((ExportText)exportColumn);

			var location = new Point(exportColumn.Location.X,exportColumn.Location.Y);
			
			var visual = new DrawingVisual();
			using (var drawingContext = visual.RenderOpen()){
				var bachgroundRect = new Rect(location,new Size(exportColumn.DesiredSize.Width,formattedText.MaxTextHeight));
				if (ShouldSetBackcolor(exportColumn)) {
					drawingContext.DrawRectangle(FixedDocumentCreator.ConvertBrush(exportColumn.BackColor),  null,bachgroundRect);	                             
				}
				
				drawingContext.DrawText(formattedText,location);
				
				if (HasFrame(exportColumn)) {
					var frameRect = new Rect(location,new Size(exportColumn.DesiredSize.Width,formattedText.Height));
					var pen = FixedDocumentCreator.CreateWpfPen(exportColumn);
					pen.Thickness = 1;
					drawingContext.DrawRectangle(null, pen,
					                             frameRect);
				}
			}
			
			var drawingElement = new DrawingElement(visual);
			UIElement = drawingElement;
		}

		
		public override void Visit(ExportImage exportImage)
		{
			var visual = new DrawingVisual();
			using (var dc = visual.RenderOpen()){
				var iss = ToBitmapSource(exportImage.Image);
				dc.DrawImage(iss,new Rect(exportImage.Location.ToWpf(),
				                          new Size(exportImage.Size.Width,exportImage.Size.Height)));
			}
			var drawingElement = new DrawingElement(visual);
			UIElement = drawingElement;
		}
		
		
		static BitmapSource ToBitmapSource(System.Drawing.Image source){
			using (MemoryStream stream = new MemoryStream()) {
				source.Save(stream, ImageFormat.Bmp);

				stream.Position = 0;
				BitmapImage result = new BitmapImage();
				result.BeginInit();
				result.CacheOption = BitmapCacheOption.OnLoad;
				result.StreamSource = stream;
				result.EndInit();
				result.Freeze();
				return result;
			}
		}

		
		public override void Visit(ExportRectangle exportRectangle){
			Canvas  containerCanvas  = FixedDocumentCreator.CreateContainer(exportRectangle);
			Canvas elementCanvas = null;
			var border = CreateBorder(exportRectangle);
			border.CornerRadius = new CornerRadius(Convert.ToDouble(exportRectangle.CornerRadius));
			
			CanvasHelper.SetPosition(border, new Point(0,0));
			
			foreach (var element in exportRectangle.ExportedItems) {
				if (IsGraphicsContainer(element)) {
					elementCanvas = RenderGraphicsContainer(element);
					containerCanvas.Children.Add(elementCanvas);
				} else {
					AsAcceptor(element).Accept(this);
					containerCanvas.Children.Add(UIElement);
				}
			containerCanvas.UpdateLayout();
			}
			
			border.Child = containerCanvas;
			UIElement = border;
		}
		
		
		public override void Visit(ExportLine exportLine){
		
			var pen = FixedDocumentCreator.CreateWpfPen(exportLine);
			var visual = new DrawingVisual();
			using (var dc = visual.RenderOpen()){
				dc.DrawLine(pen,
				            new Point(exportLine.Location.X + exportLine.FromPoint.X, exportLine.Location.Y + exportLine.FromPoint.Y),
				            new Point(exportLine.Location.X + exportLine.ToPoint.X ,
				                      exportLine.Location.Y + exportLine.FromPoint.Y));
			}
			var dragingElement = new DrawingElement(visual);
			UIElement = dragingElement;
		}
		
		
		public override void Visit(ExportCircle exportCircle){
			var drawingElement = CreateCircle(exportCircle);
			var containerCanvas =  CreateItemsInContainer(exportCircle.ExportedItems);
			containerCanvas.Children.Insert(0,drawingElement);
			UIElement = containerCanvas;
		}
		
		
		Canvas CreateItemsInContainer (List<IExportColumn> items) {
			var canvas = new Canvas();
			foreach (var element in items) {
				AsAcceptor(element).Accept(this);
				canvas.Children.Add(UIElement);
			}
			return canvas;
		}
		
		
		static DrawingElement CreateCircle(GraphicsContainer circle){
			var pen = FixedDocumentCreator.CreateWpfPen(circle);
			var rad = CalcRadius(circle.Size);
			var visual = new DrawingVisual();
			using (var dc = visual.RenderOpen()){
				dc.DrawEllipse(FixedDocumentCreator.ConvertBrush(circle.BackColor),
					pen,
					new Point( rad.X,rad.Y),	
					rad.X,
					rad.Y);                 
			}
			return new DrawingElement(visual);
		}
		
		
		static Border CreateBorder(IExportColumn exportColumn){
		
			var border = new Border();
			border.BorderThickness =  Thickness(exportColumn);
			border.BorderBrush = FixedDocumentCreator.ConvertBrush(exportColumn.ForeColor);
			border.Background = FixedDocumentCreator.ConvertBrush(exportColumn.BackColor);
			border.Width = exportColumn.Size.Width + 2;
			border.Height = exportColumn.Size.Height + 2;
			return border;
		}

		
		static Thickness Thickness(IExportColumn exportColumn){
			double bT;
			bT = !IsGraphicsContainer(exportColumn) ? 1 : Convert.ToDouble(((GraphicsContainer)exportColumn).Thickness);
			return new Thickness(bT);
		}
		
		
		static Point CalcRadius(System.Drawing.Size size) {
			return  new Point(size.Width /2,size.Height /2);
		}
		
		
		protected UIElement UIElement {get;private set;}
		
		
		public FixedPage FixedPage {get; private set;}
	}
}
