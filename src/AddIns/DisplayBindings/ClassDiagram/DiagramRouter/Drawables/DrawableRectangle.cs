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

namespace Tools.Diagrams.Drawables
{
	public class DrawableRectangle : BaseRectangle, IDrawableRectangle
	{
		private Brush fillBrush;
		private Pen strokePen;
		private float tlRad, trRad, brRad, blRad;
		private GraphicsPath path;
		
		public DrawableRectangle (Brush fill, Pen stroke)
			: this (fill, stroke, 1, 1, 1, 1)
		{
		}

		public DrawableRectangle (Brush fill, Pen stroke, float tl, float tr, float br, float bl)
		{
			fillBrush = fill;
			strokePen = stroke;
			tlRad = tl;
			trRad = tr;
			brRad = br;
			blRad = bl;
		}
		
		public Brush FillBrush
		{
			get { return fillBrush; }
			set { fillBrush = value; }
		}
		
		public Pen StrokePen
		{
			get { return strokePen; }
			set { strokePen = value; }
		}
		
		protected override void OnAbsolutePositionChanged()
		{
			base.OnAbsolutePositionChanged();
			path = null;
		}
		
		protected override void OnActualSizeChanged()
		{
			path = null;
		}
		
		protected override void OnSizeChanged()
		{
			path = null;
		}
		
		private void RecreatePath()
		{
			path = new GraphicsPath();
			if (tlRad > 0)
				path.AddArc(AbsoluteX, AbsoluteY, tlRad, tlRad, 180, 90);
			else
				path.AddLine(AbsoluteX, AbsoluteY, AbsoluteX, AbsoluteY);
			
			if (trRad > 0)
				path.AddArc(AbsoluteX + ActualWidth - trRad, AbsoluteY, trRad, trRad, 270, 90);
			else
				path.AddLine(AbsoluteX + ActualWidth, AbsoluteY, AbsoluteX + ActualWidth, AbsoluteY);
			
			if (brRad > 0)
				path.AddArc(AbsoluteX + ActualWidth - brRad, AbsoluteY + ActualHeight - brRad, brRad, brRad, 0, 90);
			else
				path.AddLine(AbsoluteX + ActualWidth, AbsoluteY + ActualHeight, AbsoluteX + ActualWidth, AbsoluteY + ActualHeight);
			
			if (blRad > 0)
				path.AddArc(AbsoluteX, AbsoluteY + ActualHeight - blRad, blRad, blRad, 90, 90);
			else
				path.AddLine(AbsoluteX, AbsoluteY + ActualHeight, AbsoluteX, AbsoluteY + ActualHeight);
			
			path.CloseFigure();
		}
		
		public void DrawToGraphics(Graphics graphics)
		{
			if (graphics == null) return;
			if (path == null) RecreatePath();
			
			if (fillBrush != null)
				graphics.FillPath(fillBrush, path);
			
			if (strokePen != null)
				graphics.DrawPath(strokePen, path);
		}
	}
}
