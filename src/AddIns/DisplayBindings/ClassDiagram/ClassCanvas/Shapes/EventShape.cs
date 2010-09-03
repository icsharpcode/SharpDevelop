// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ClassDiagram
{
	public class EventShape : VectorShape
	{
		static PointF[] lightningPoints1 = new PointF[]{
			new PointF( 5.0f,  1.0f),
			new PointF( 9.0f,  1.0f),
			new PointF( 6.0f,  5.0f),
			new PointF( 8.0f,  5.0f),
			new PointF( 4.0f, 12.0f),
			new PointF( 6.0f,  6.0f),
			new PointF( 4.0f,  6.0f)
		};
		
		static PointF[] lightningPoints2 = new PointF[]{
			new PointF( 9.0f,  1.0f),
			new PointF(10.0f,  2.0f),
			new PointF( 8.0f,  5.0f),
			new PointF( 6.0f,  5.0f)
		};
		
		static PointF[] lightningPoints3 = new PointF[]{
			new PointF( 8.0f,  5.0f),
			new PointF( 9.0f,  6.0f),
			new PointF( 5.0f, 13.0f),
			new PointF( 4.0f, 12.0f)
		};
		
		static PointF[] lightningPoints4 = new PointF[]{
			new PointF( 4.0f,  6.0f),
			new PointF( 6.0f,  6.0f),
			new PointF( 6.0f,  7.0f),
			new PointF( 5.0f,  7.0f)
		};
		
		public override void Draw(Graphics graphics)
		{
			if (graphics == null) return;
			graphics.FillPolygon (Brushes.DarkGoldenrod, lightningPoints2);
			graphics.FillPolygon (Brushes.DarkGoldenrod, lightningPoints3);
			graphics.FillPolygon (Brushes.DarkGoldenrod, lightningPoints4);
			graphics.FillPolygon (Brushes.Gold, lightningPoints1);
		}
		
		public override float ShapeWidth
		{
			get { return 14.0f; }
		}
		
		public override float ShapeHeight
		{
			get { return 14.0f; }
		}
	}
}
