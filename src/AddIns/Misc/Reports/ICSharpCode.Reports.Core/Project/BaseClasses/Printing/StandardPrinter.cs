/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 20.06.2010
 * Time: 19:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using ICSharpCode.Reports.Core.old_Exporter;

namespace ICSharpCode.Reports.Core.BaseClasses.Printing
{
	/// <summary>
	/// Description of StandardPrinter.
	/// </summary>
	internal sealed class StandardPrinter
	{
		public StandardPrinter()
		{
		}
		
		public static void FillBackground (Graphics  graphics,BaseStyleDecorator decorator,Rectangle rectangle)
		{
			RectangleShape backgroundShape = new RectangleShape();
			
			backgroundShape.FillShape(graphics,
			                new SolidFillPattern(decorator.BackColor),
			                rectangle);
		}
		
		
		public static void DrawBorder (Graphics graphics,BaseStyleDecorator decorator,Rectangle rectangle)
		{
			if (decorator.DrawBorder)
			{
				if (decorator.FrameColor == Color.Empty)
				{
					decorator.FrameColor = decorator.ForeColor;
				}
				Border border = new Border(new BaseLine (decorator.FrameColor,System.Drawing.Drawing2D.DashStyle.Solid,1));
				border.DrawBorder(graphics,rectangle);
			}
		}
	
	}
}
