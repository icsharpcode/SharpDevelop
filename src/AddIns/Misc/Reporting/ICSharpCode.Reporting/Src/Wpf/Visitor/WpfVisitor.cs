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
				var acceptor = element as IAcceptor;
				acceptor.Accept(this);
				fixedPage.Children.Add(sectionCanvas);
			}
		}
		
		
		public override void Visit(ExportContainer exportContainer){
			
			sectionCanvas = FixedDocumentCreator.CreateContainer(exportContainer);
			sectionCanvas.Name = exportContainer.Name;
			CanvasHelper.SetPosition(sectionCanvas,new Point(exportContainer.Location.X,exportContainer.Location.Y));
			PerformList(sectionCanvas,exportContainer.ExportedItems);
		}
		
		
		void PerformList(Canvas myCanvas, System.Collections.Generic.List<IExportColumn> exportedItems)
		{
			foreach (var element in exportedItems) {
				var container = element as ExportContainer;
				if (container != null) {
					var containerCanvas = FixedDocumentCreator.CreateContainer(container);
					CanvasHelper.SetPosition(containerCanvas,new Point(container.Location.X,container.Location.Y));
					myCanvas.Children.Add(containerCanvas);
					PerformList(containerCanvas,container.ExportedItems);
				} else {
					var acceptor = element as IAcceptor;
					acceptor.Accept(this);
					myCanvas.Children.Add(UIElement);
				}
			}
		}
		
		
		public override void Visit(ExportText exportColumn){
			/*			
			var textBlock = FixedDocumentCreator.CreateTextBlock((ExportText)exportColumn,ShouldSetBackcolor(exportColumn));
			CanvasHelper.SetPosition(textBlock,new Point(exportColumn.Location.X,exportColumn.Location.Y));
			UIElement = textBlock;
			*/
			
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
			var pen = FixedDocumentCreator.CreateWpfPen(exportRectangle);
			
			var visual = new DrawingVisual();
			using (var dc = visual.RenderOpen()){
				dc.DrawRectangle(FixedDocumentCreator.ConvertBrush(exportRectangle.BackColor),
				                 pen,
				                 new Rect(exportRectangle.Location.X,exportRectangle.Location.Y,
				                          exportRectangle.Size.Width,exportRectangle.Size.Height));
			}
			var dragingElement = new DrawingElement(visual);
			UIElement = dragingElement;
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
