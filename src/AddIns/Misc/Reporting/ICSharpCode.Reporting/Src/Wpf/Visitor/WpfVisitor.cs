/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 06.05.2013
 * Time: 20:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
			Console.WriteLine(myCanvas.Name);
			foreach (var element in exportedItems) {
				var container = element as ExportContainer;
				if (container != null) {
//					Console.WriteLine("recursive");
					var containerCanvas = FixedDocumentCreator.CreateContainer(container);
					CanvasHelper.SetPosition(containerCanvas,new Point(container.Location.X,container.Location.Y));
					myCanvas.Children.Add(containerCanvas);
//					Console.WriteLine("call recursive");
					PerformList(containerCanvas,container.ExportedItems);
				} else {
					var acceptor = element as IAcceptor;
					acceptor.Accept(this);
					myCanvas.Children.Add(UIElement);
				}
			}
		}
		
		
		public override void Visit(ExportText exportColumn){
			var textBlock = FixedDocumentCreator.CreateTextBlock((ExportText)exportColumn,ShouldSetBackcolor(exportColumn));
			CanvasHelper.SetPosition(textBlock,new Point(exportColumn.Location.X,exportColumn.Location.Y));
			UIElement = textBlock;
		}
		
		
		
		public override void Visit(ExportLine exportGraphics)
		{
			var pen = FixedDocumentCreator.CreateWpfPen(exportGraphics);
			var visual = new DrawingVisual();
			using (var dc = visual.RenderOpen())
			{
				dc.DrawLine(pen,
				            new Point(exportGraphics.Location.X, exportGraphics.Location.Y),
				            new Point(exportGraphics.Location.X + exportGraphics.Size.Width,exportGraphics.Location.Y));
			}
			DrawingElement m = new DrawingElement(visual);
			UIElement = m;
		}
		
		
		public override void Visit(ExportRectangle exportRectangle)
		{
			var pen = FixedDocumentCreator.CreateWpfPen(exportRectangle);
			var visual = new DrawingVisual();
		
			using (var dc = visual.RenderOpen())
			{dc.DrawRectangle(FixedDocumentCreator.ConvertBrush(exportRectangle.BackColor),
				                  pen,
				                  new Rect(exportRectangle.Location.X,exportRectangle.Location.Y,
				                           exportRectangle.Size.Width,exportRectangle.Size.Height));
			}
			DrawingElement m = new DrawingElement(visual);
			UIElement = m;
		}
		
		
		
		protected UIElement UIElement {get;private set;}
		
		
		public FixedPage FixedPage {get; private set;}
	}
	
	

}
