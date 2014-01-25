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
using System.Windows.Media;

using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.WpfReportViewer.Visitor.Graphics
{
	class DrawingElement : FrameworkElement{
		
		private VisualCollection children;
	
		public DrawingElement(DrawingVisual visual) {
				children = new VisualCollection(this);
				children.Add(visual);
		}
		
		/*
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
	*/
	
		protected override int VisualChildrenCount{
			get { return children.Count; }
		}
	
		protected override Visual GetVisualChild(int index){
			if (index < 0 || index >= children.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
	
			return children[index];
		}
	}
}
