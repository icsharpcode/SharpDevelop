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
using System.Drawing.Drawing2D;

using Tools.Diagrams;

namespace ClassDiagram
{
	/// <summary>
	/// Description of RouteInheritanceShape.
	/// </summary>
	public class RouteInheritanceShape : RouteShape
	{
		static GraphicsPath path = InitizlizePath();
		
		static GraphicsPath InitizlizePath()
		{
			GraphicsPath path = new GraphicsPath();
			path.AddLines( new PointF[]
			              {
							new PointF(0.0f, 0.0f),
							new PointF(5.0f, 9.0f),
							new PointF(-5.0f, 9.0f)
			              });
			path.CloseFigure();
			return path;
		}
		
		static Pen stroke = Pens.Black;
		static Brush fill = Brushes.White;
		
		protected override void Paint (Graphics graphics)
		{
			graphics.FillPath(fill, path);
			graphics.DrawPath(stroke, path);
		}
	}
}
