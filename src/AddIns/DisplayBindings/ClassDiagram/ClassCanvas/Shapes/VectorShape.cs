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
using Tools.Diagrams.Drawables;
using Tools.Diagrams;

namespace ClassDiagram
{
	public abstract class VectorShape : BaseRectangle, IDrawableRectangle
	{
		protected VectorShape ()
		{
			KeepAspectRatio = true;
		}
		
		public abstract void Draw (Graphics graphics);
		
		/// <summary>
		/// Draw the shape to the given graphics object.
		/// </summary>
		/// <param name="graphics"></param>
		public void DrawToGraphics (Graphics graphics)
		{
			float scalex = base.ActualWidth / ShapeWidth;
			float scaley = base.ActualHeight / ShapeHeight;
			
			if (scalex == 0 || scaley == 0) return;
			
			DrawToGraphics (graphics, AbsoluteX, AbsoluteY, scalex, scaley);
		}
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
		public virtual void DrawToGraphics (Graphics graphics, float x, float y)
		{
			DrawToGraphics(graphics, x, y, 1);
		}
	
		public virtual void DrawToGraphics (Graphics graphics, float x, float y, float scale)
		{
			DrawToGraphics(graphics, x, y, scale, scale);			
		}
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
		public virtual void DrawToGraphics (Graphics graphics, float x, float y, float scaleX, float scaleY)
		{
			if (graphics == null) return;

			GraphicsState state = graphics.Save();
			graphics.TranslateTransform (x, y);
			graphics.ScaleTransform(scaleX, scaleY);
			Draw(graphics);
			//graphics.DrawRectangle(Pens.Magenta, 0, 0, ShapeWidth, ShapeHeight);
			graphics.Restore(state);
		}
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
		public virtual bool IsInside (float x, float y)
		{
			return (x >= AbsoluteX && x < AbsoluteX + ActualWidth &&
			        y >= AbsoluteY && y < AbsoluteY + ActualHeight);
		}
		
		public abstract float ShapeWidth { get; }
		public abstract float ShapeHeight { get; }
		
		public override float GetAbsoluteContentWidth() { return ShapeWidth; }
		public override float GetAbsoluteContentHeight() { return ShapeHeight; }
	}
}
