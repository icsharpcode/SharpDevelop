// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Itai Bar-Haim"/>
//     <version>$Revision$</version>
// </file>

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
