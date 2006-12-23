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
	public class FieldShape : MethodShape
	{
		static Brush brickBrush1 = new SolidBrush(Color.FromArgb(255, 0, 192, 192));
		static Brush brickBrush2 = new SolidBrush(Color.FromArgb(255, 0, 064, 064));
		static Brush brickBrush3 = new SolidBrush(Color.FromArgb(255, 0, 128, 128));
		
		public override void Draw(Graphics graphics)
		{
			if (graphics == null) return;
			GraphicsState state = graphics.Save();
			graphics.TranslateTransform(17.0f, 0.0f);
			graphics.ScaleTransform(-1.0f, 1.0f);
			MethodShape.DrawBrick(graphics, brickBrush1, brickBrush2, brickBrush3);
			graphics.Restore(state);
			graphics.FillRectangle(Brushes.Gray, 1.0f, 4.5f, 3.5f, 0.5f);
			graphics.FillRectangle(Brushes.Gray, 0.0f, 6.5f, 3.5f, 0.5f);
			graphics.FillRectangle(Brushes.Gray, 2.0f, 8.5f, 3.5f, 0.5f);
		}
	}
}
