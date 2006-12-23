/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 03/11/2006
 * Time: 19:42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using Tools.Diagrams;

namespace ClassDiagram
{
	public abstract class RouteShape : IRouteShape
	{
		private static SizeF baseSize = new SizeF(10, 10);
		
		private static float ConvertDirection (Direction dir)
		{
			switch (dir)
			{
				case Direction.Up: return 0;
				case Direction.Down: return 180;
				case Direction.Right: return 90;
				case Direction.Left: return 270;
			}
			return 0;
		}
		
		public void Draw (Graphics graphics, Route route, bool atEnd)
		{
			if (graphics == null) return;
			if (route == null) return;
			GraphicsState state = graphics.Save();
			float direction = 0;
			PointF pos = default(PointF);
			if (atEnd)
			{
				pos = route.GetEndPoint();
				direction = ConvertDirection(route.GetEndDirection());
			}
			else
			{
				pos = route.GetStartPoint();
				direction = ConvertDirection(route.GetStartDirection());
			}
			
			// In matrix math, the correct way is to put rotation BEFORE
			// translation. However, the simple transformation maethods of
			// GDI+ works in "Prepend" mode, which reverses the order of
			// operations.
			graphics.TranslateTransform(pos.X, pos.Y);
			graphics.RotateTransform(direction);
			
			Paint(graphics);
			graphics.Restore(state);
		}
		
		protected abstract void Paint (Graphics graphics);
		
		protected virtual SizeF Size
		{
			get { return RouteShape.baseSize; }
		}
	}
}
