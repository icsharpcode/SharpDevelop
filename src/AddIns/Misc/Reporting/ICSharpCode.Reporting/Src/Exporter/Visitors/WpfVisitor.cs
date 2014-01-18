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
using System.Windows.Media.Imaging;

using ICSharpCode.Reporting.ExportRenderer;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.Exporter.Visitors
{
	/// <summary>
	/// Description of WpfVisitor.
	/// </summary>
	/// 
	class WpfVisitor: AbstractVisitor {
		
		readonly FixedDocumentCreator documentCreator;
		FixedPage fixedPage;
		Canvas sectionCanvas;
		
		public WpfVisitor()
		{
			documentCreator = new FixedDocumentCreator();
		}
		
		
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
			
			sectionCanvas = documentCreator.CreateContainer(exportContainer);
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
					var containerCanvas = documentCreator.CreateContainer(container);
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
			var textBlock = documentCreator.CreateTextBlock((ExportText)exportColumn,ShouldSetBackcolor(exportColumn));
			CanvasHelper.SetPosition(textBlock,new Point(exportColumn.Location.X,exportColumn.Location.Y));
			UIElement = textBlock;
		}
		
		
		
		public override void Visit(ExportLine exportGraphics)
		{
			var pen = documentCreator.CreateWpfPen(exportGraphics);
			pen.Thickness = exportGraphics.Thickness;
			pen.DashStyle = FixedDocumentCreator.DashStyle(exportGraphics);
			pen.StartLineCap = FixedDocumentCreator.LineCap(exportGraphics.StartLineCap);
			pen.EndLineCap = FixedDocumentCreator.LineCap(exportGraphics.EndLineCap);
			ExtendedLine m = new ExtendedLine(exportGraphics,pen);
			UIElement = m;
		}
		
		
		protected UIElement UIElement {get;private set;}
		
		
		public FixedPage FixedPage {get; private set;}
	}
	
	
	class ExtendedLine : FrameworkElement{
		private VisualCollection children;

		public ExtendedLine(ExportLine exportGraphics,Pen pen){
			children = new VisualCollection(this);
			var visual = new DrawingVisual();
			children.Add(visual);
			using (var dc = visual.RenderOpen())
			{
				dc.DrawLine(pen,
				            new Point(exportGraphics.Location.X, exportGraphics.Location.Y),
				            new Point(exportGraphics.Location.X + exportGraphics.Size.Width,exportGraphics.Location.Y));
			}
		}


		protected override int VisualChildrenCount{
			get { return children.Count; }
		}

		protected override Visual GetVisualChild(int index){
			if (index < 0 || index >= children.Count)
			{
				throw new ArgumentOutOfRangeException();
			}

			return children[index];
		}
	}
}
