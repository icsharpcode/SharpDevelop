/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 05.04.2014
 * Time: 17:23
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Drawing;
using ICSharpCode.Reporting.Addin.DesignableItems;
using ICSharpCode.Reporting.Addin.Designer;
using ICSharpCode.Reporting.Addin.TypeProvider;

namespace ICSharpCode.Reporting.Addin.DesignableItems
{
	/// <summary>
	/// Description of BaseRectangleItem.
	/// </summary>
	/// 
	[Designer(typeof(ContainerDesigner))]
	class BaseRectangleItem:AbstractGraphicItem
	{
		public BaseRectangleItem()
		{
			TypeDescriptor.AddProvider(new RectangleItemTypeProvider(), typeof(BaseRectangleItem));
		}
		
		public override void Draw(Graphics graphics)
		{
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			
			var rect = new Rectangle(ClientRectangle.Left,
				ClientRectangle.Top,
				ClientRectangle.Right -1,
				ClientRectangle.Bottom -1);
			
			using (var pen = new Pen(ForeColor,Thickness)) {
				graphics.DrawRectangle(pen,rect);
			}
			
			
//			backgroundShape.FillShape(graphics,
//			                new SolidFillPattern(this.BackColor),
//			                rect);
			
//			Border b = new Border(new BaseLine (this.ForeColor,System.Drawing.Drawing2D.DashStyle.Solid,1));
//			DrawFrame(graphics,b);
//			BaseLine line = new BaseLine(base.ForeColor,DashStyle,Thickness,LineCap.Round,LineCap.Round,DashCap.Round);
//			using (Pen pen = line.CreatePen(line.Thickness)){
//				shape.CornerRadius = this.CornerRadius;
//				GraphicsPath path1 = shape.CreatePath(rect);
//				graphics.DrawPath(pen, path1);
//				
//			}
			
//			shape.DrawShape (graphics,
//			                 this.Baseline(),
//			                 rect);
			
		}
	}
}
