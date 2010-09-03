// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ClassDiagram
{
	public class ExpandShape : CollapseShape
	{
		public override void Draw(Graphics graphics)
		{
			if (graphics == null) return;
			CollapseShape.DrawButton(graphics);
			
			graphics.TranslateTransform(1, 23);
			graphics.ScaleTransform(1, -1);
			CollapseShape.DrawArrow(graphics);
			graphics.TranslateTransform(0, 6);
			CollapseShape.DrawArrow(graphics);
		}	
	}
}
