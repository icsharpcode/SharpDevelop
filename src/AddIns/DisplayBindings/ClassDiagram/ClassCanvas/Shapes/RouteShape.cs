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
