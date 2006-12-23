/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 28/09/2006
 * Time: 19:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

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
