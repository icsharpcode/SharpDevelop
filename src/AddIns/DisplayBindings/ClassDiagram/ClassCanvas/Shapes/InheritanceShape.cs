// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ClassDiagram
{
	public class InheritanceShape : VectorShape
	{
		static GraphicsPath path = InitializePath();
		
		static GraphicsPath InitializePath ()
		{
			GraphicsPath path = new GraphicsPath();
			path.StartFigure();
			path.AddPolygon(new PointF[]{
				new PointF(0.0f, 1.9f),
				new PointF(2.0f, 1.9f),
				new PointF(2.0f, 1.0f),
				new PointF(3.0f, 2.0f),
				new PointF(2.0f, 3.0f),
				new PointF(2.0f, 2.1f),
				new PointF(0.0f, 2.1f)
			});
			path.CloseFigure();
			path.StartFigure();
			path.AddPolygon(new PointF[]{
				new PointF(2.2f, 1.4f),
				new PointF(2.7f, 2.0f),
				new PointF(2.2f, 2.6f)
			});
			path.FillMode = FillMode.Alternate;
			path.CloseFigure();
			return path;
		}
		
		public override float ShapeWidth
		{
			get { return 3.0f; }
		}
		
		public override float ShapeHeight
		{
			get { return 4.0f; }
		}
		
		public override void Draw(Graphics graphics)
		{
			if (graphics == null) return;
			graphics.FillPath(Brushes.Black, path);
		}
	}
}
