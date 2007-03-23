/* Created by SharpDevelop.
 * User: itai
 * Date: 28/09/2006
 * Time: 19:03
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

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

