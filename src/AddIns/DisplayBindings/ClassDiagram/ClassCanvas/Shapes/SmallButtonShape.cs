// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ClassDiagram
{
	public class SmallButtonShape : SmallIconShape
	{
		static Rectangle rect = new Rectangle(0, 0, 10, 10);
		static LinearGradientBrush shade = new LinearGradientBrush(
				new PointF(0, 0), new PointF(0, 10),
				Color.White, Color.LightGray);
		static Pen strokePen = Pens.SteelBlue;
		
		public override void Draw(Graphics graphics)
		{
			if (graphics == null) return;
			graphics.FillRectangle(shade, rect);
			graphics.DrawRectangle(strokePen, rect);
		}
	}
}
