// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
