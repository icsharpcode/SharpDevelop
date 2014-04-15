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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;
using ICSharpCode.Reporting.WpfReportViewer.Visitor.Graphics;
using System.Windows.Shapes;

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
				var acceptor = element as IAcceptor;
				acceptor.Accept(this);
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
					if (element is ExportRectangle) {
						var graphContainer = RenderGraphicsContainer((IExportContainer)element);
						canvas.Children.Add(graphContainer);
					}
				} else {
					var acceptor = element as IAcceptor;
					acceptor.Accept(this);
					canvas.Children.Add(UIElement);
				}
			}
			canvas.Background = FixedDocumentCreator.ConvertBrush(container.BackColor);
			return canvas;
		}
	
		
		bool IsContainer (IExportColumn column) {
			return (column is ExportContainer)|| (column is ExportRectangle);
		}
		
		
		Canvas RenderGraphicsContainer(IExportContainer container)
		{
			
			var rect = container as ExportRectangle;
			var graphCanvas = FixedDocumentCreator.CreateContainer(rect);
			CanvasHelper.SetPosition(graphCanvas, container.Location.ToWpf());
			graphCanvas.Background = FixedDocumentCreator.ConvertBrush(container.BackColor);
			if (rect != null) {
				Visit(rect);
				graphCanvas.Children.Add(UIElement);
			}
			return graphCanvas;
		}
		
		
		public override void Visit(ExportText exportColumn){
			
			var ft = FixedDocumentCreator.CreateFormattedText((ExportText)exportColumn);
			var visual = new DrawingVisual();
			var location = new Point(exportColumn.Location.X,exportColumn.Location.Y);
			using (var dc = visual.RenderOpen()){
				if (ShouldSetBackcolor(exportColumn)) {
					dc.DrawRectangle(FixedDocumentCreator.ConvertBrush(exportColumn.BackColor),
						null,
						new Rect(location,new Size(exportColumn.Size.Width,exportColumn.Size.Height)));
				}
				dc.DrawText(ft,location);
			}
			var dragingElement = new DrawingElement(visual);
			UIElement = dragingElement;
			
		}

		
		public override void Visit(ExportLine exportGraphics)
		{
			var pen = FixedDocumentCreator.CreateWpfPen(exportGraphics);
			var visual = new DrawingVisual();
			using (var dc = visual.RenderOpen()){
				dc.DrawLine(pen,
				            new Point(exportGraphics.Location.X, exportGraphics.Location.Y),
				            new Point(exportGraphics.Location.X + exportGraphics.Size.Width,exportGraphics.Location.Y));
			}
			var dragingElement = new DrawingElement(visual);
			UIElement = dragingElement;
		}
		
		
		public override void Visit(ExportRectangle exportRectangle)
		{
			var border = new Border();
			border.BorderThickness = new Thickness(exportRectangle.Thickness);
			border.BorderBrush = FixedDocumentCreator.ConvertBrush(exportRectangle.ForeColor);
			border.Background = FixedDocumentCreator.ConvertBrush(exportRectangle.BackColor);
			border.Width = exportRectangle.Size.Width;
			border.Height = exportRectangle.Size.Height;
			CanvasHelper.SetPosition(border, new Point(0,0));
			var sp = new StackPanel();
			sp.Orientation = Orientation.Horizontal;
			foreach (var element in exportRectangle.ExportedItems) {
				var acceptor = element as IAcceptor;
					acceptor.Accept(this);
				sp.Children.Add(UIElement);
			}
			border.Child = sp;
			UIElement = border;
		}
		
		
		public override void Visit(ExportCircle exportCircle)
		{
			var pen = FixedDocumentCreator.CreateWpfPen(exportCircle);
			var rad = CalcRad(exportCircle.Size);
			
			var visual = new DrawingVisual();
			using (var dc = visual.RenderOpen()){
				
				dc.DrawEllipse(FixedDocumentCreator.ConvertBrush(exportCircle.BackColor),
				                 pen,
				                 new Point(exportCircle.Location.X + rad.X,
				                           exportCircle.Location.Y + rad.Y),
				                 rad.X,
				                 rad.Y);
				                           
				                 
			}
			var dragingElement = new DrawingElement(visual);
			UIElement = dragingElement;
		}
		
		static Point CalcRad(System.Drawing.Size size) {
			return  new Point(size.Width /2,size.Height /2);
		}
		
		protected UIElement UIElement {get;private set;}
		
		
		public FixedPage FixedPage {get; private set;}
	}
	
	

}
