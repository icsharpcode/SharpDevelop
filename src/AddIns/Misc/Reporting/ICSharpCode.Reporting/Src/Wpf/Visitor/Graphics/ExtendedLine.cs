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
