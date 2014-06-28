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
using System.Drawing.Drawing2D;
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
		int cornerRadius = 1;
		
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
			
			var path = RoundedRectangle.Create(rect,CornerRadius,
				RoundedRectangle.RectangleCorners.All);
			
			using (var pen = new Pen(this.ForeColor,Thickness)) {
				graphics.DrawPath(pen, path);
			}
		}
		
		
		[Category("Appearance")]
		public int CornerRadius {
			get { return cornerRadius; }
			set {
				cornerRadius = value;
				Invalidate();
			}
		}
	}
	
	static class RoundedRectangle
	{
		public enum RectangleCorners
		{
			None = 0, TopLeft = 1, TopRight = 2, BottomLeft = 4, BottomRight = 8,
			All = TopLeft | TopRight | BottomLeft | BottomRight
		}
		
		public static GraphicsPath Create(int x, int y, int width, int height, 
			int radius, RectangleCorners corners)
		{
			int xw = x + width;
			int yh = y + height;
			int xwr = xw - radius;
			int yhr = yh - radius;
			int xr = x + radius;
			int yr = y + radius;
			int r2 = radius * 2;
			int xwr2 = xw - r2;
			int yhr2 = yh - r2;
			
			var p = new GraphicsPath();
			p.StartFigure();
			
			//Top Left Corner
			if ((RectangleCorners.TopLeft & corners) == RectangleCorners.TopLeft)
			{
				p.AddArc(x, y, r2, r2, 180, 90);
			}
			else
			{
				p.AddLine(x, yr, x, y);
				p.AddLine(x, y, xr, y);
			}
			
			//Top Edge
			p.AddLine(xr, y, xwr, y);
			
			//Top Right Corner
			if ((RectangleCorners.TopRight & corners) == RectangleCorners.TopRight)
			{
				p.AddArc(xwr2, y, r2, r2, 270, 90);
			}
			else
			{
				p.AddLine(xwr, y, xw, y);
				p.AddLine(xw, y, xw, yr);
			}
			
			//Right Edge
			p.AddLine(xw, yr, xw, yhr);
			
			//Bottom Right Corner
			if ((RectangleCorners.BottomRight & corners) == RectangleCorners.BottomRight)
			{
				p.AddArc(xwr2, yhr2, r2, r2, 0, 90);
			}
			else
			{
				p.AddLine(xw, yhr, xw, yh);
				p.AddLine(xw, yh, xwr, yh);
			}
			
			//Bottom Edge
			p.AddLine(xwr, yh, xr, yh);
			
			//Bottom Left Corner
			if ((RectangleCorners.BottomLeft & corners) == RectangleCorners.BottomLeft)
			{
				p.AddArc(x, yhr2, r2, r2, 90, 90);
			}
			else
			{
				p.AddLine(xr, yh, x, yh);
				p.AddLine(x, yh, x, yhr);
			}
			
			//Left Edge
			p.AddLine(x, yhr, x, yr);
			
			p.CloseFigure();
			return p;
		}
		
		public static GraphicsPath Create(Rectangle rect, int radius, RectangleCorners c)
		{ return Create(rect.X, rect.Y, rect.Width, rect.Height, radius, c); }
//		
//		public static GraphicsPath Create(int x, int y, int width, int height, int radius)
//		{ return Create(x, y, width, height, radius, RectangleCorners.All); }
		
//		public static GraphicsPath Create(Rectangle rect, int radius)
//		{ return Create(rect.X, rect.Y, rect.Width, rect.Height, radius); }
//		
//		public static GraphicsPath Create(int x, int y, int width, int height)
//		{ return Create(x, y, width, height, 5); }
		
//		public static GraphicsPath Create(Rectangle rect)
//		{ return Create(rect.X, rect.Y, rect.Width, rect.Height); }
	}
}
