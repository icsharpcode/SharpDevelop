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

using ICSharpCode.Reports.Core.Exporter;
using ICSharpCode.Reports.Core.Globals;

/// <summary>
/// Draw a Rectangle, used by DesingerControls and printing stuff
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 09.10.2005 18:20:51
/// </remarks>
namespace ICSharpCode.Reports.Core {
	
	public class RectangleShape : BaseShape {
		
		public RectangleShape()
		{
		}
		
		
		public override GraphicsPath CreatePath(Point from, Point to)
		{
			throw new NotImplementedException();
		}
		
		
		public override GraphicsPath CreatePath(Rectangle rectangle )
		{
			//http://stackoverflow.com/questions/628261/how-to-draw-rounded-rectangle-with-variable-width-border-inside-of-specific-bound
		
			GraphicsPath gfxPath = new GraphicsPath();
			if (CornerRadius == 0)
			{
				gfxPath.AddRectangle(rectangle);
			}
			else
			{
				gfxPath.AddArc(rectangle.X, rectangle.Y,CornerRadius , CornerRadius, 180, 90);
				gfxPath.AddArc(rectangle.X + rectangle.Width - CornerRadius, rectangle.Y, CornerRadius, CornerRadius, 270, 90);
				gfxPath.AddArc(rectangle.X + rectangle.Width - CornerRadius, rectangle.Y + rectangle.Height - CornerRadius, CornerRadius, CornerRadius, 0, 90);
				gfxPath.AddArc(rectangle.X, rectangle.Y + rectangle.Height - CornerRadius, CornerRadius, CornerRadius, 90, 90);
			}
			gfxPath.CloseAllFigures();
			return gfxPath;
		}
		
		
		public override void DrawShape(Graphics graphics, BaseLine line, Rectangle rectangle)
		{
			base.DrawShape(graphics, line, rectangle);
		}
		
		
		public override void CreatePath(iTextSharp.text.pdf.PdfContentByte contentByte,
		                                BaseLine line,
		                                IBaseStyleDecorator style,		                    
		                                Point from,Point to)
		{
			throw new NotImplementedException();
		}
		
		//	http://www.mikesdotnetting.com/Article/88/iTextSharp-Drawing-shapes-and-Graphics
		
		public override void CreatePath(iTextSharp.text.pdf.PdfContentByte contentByte,
		                                BaseLine line,
		                                IBaseStyleDecorator style,
		                                iTextSharp.text.Rectangle rectangle)
		{
		
			if (contentByte == null) {
				throw new ArgumentNullException("contentByte");
			}
		
			if (style == null) {
				throw new ArgumentNullException("style");
			}
			if (rectangle == null) {
				throw new ArgumentNullException("rectangle");
			}
		
			if (line == null) {
				BaseShape.FillBackGround(contentByte,style,rectangle);
			} 
			else
			{
				BaseShape.SetupShape(contentByte,style);
				contentByte.SetLineWidth(UnitConverter.FromPixel(line.Thickness).Point);
				contentByte.RoundRectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, rectangle.Height, CornerRadius);
				BaseShape.FinishShape(contentByte);
			}
		}
		
		
		public int CornerRadius {get;set;}
	}
}
