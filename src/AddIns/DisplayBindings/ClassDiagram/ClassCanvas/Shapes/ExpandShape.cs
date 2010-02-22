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
