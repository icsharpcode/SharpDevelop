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
using ICSharpCode.Reports.Core.Globals;

/// <summary>
/// This Class drwas a Border around an ReportItem
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 12.10.2005 09:12:34
/// </remarks>

namespace ICSharpCode.Reports.Core {
	
	public class Border
	{
		BaseLine baseline;
		BaseLine left;
		BaseLine top;
		BaseLine right;
		BaseLine bottom;
		
		
		public Border(BaseLine baseLine)
		{
			this.baseline = baseLine;
			this.left = baseLine;
			this.top = baseLine;
			this.right = baseLine;
			this.bottom = baseLine;
		}
		
		
		public void DrawBorder (Graphics graphics, Rectangle rectangle ) {
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			using (Pen p = baseline.CreatePen(baseline.Thickness)) {
//				Rectangle r = System.Drawing.Rectangle.Inflate(rectangle,1,1);
				graphics.DrawRectangle (p,rectangle);
			}
		}
		

		public void DrawBorder (iTextSharp.text.pdf.PdfContentByte contentByte,
		                        iTextSharp.text.Rectangle rectangle,
		                        ICSharpCode.Reports.Core.Exporter.IBaseStyleDecorator style)
		{
			if ( contentByte == null) {
				throw new ArgumentNullException("contentByte");
			}

			contentByte.SetColorStroke(style.PdfFrameColor);
			contentByte.SetColorFill(style.PdfBackColor);
			contentByte.SetLineWidth(UnitConverter.FromPixel(baseline.Thickness).Point);
			
			contentByte.MoveTo(rectangle.Left ,rectangle.Top );
			
			contentByte.LineTo(rectangle.Left, rectangle.Top - rectangle.Height);
			
			contentByte.LineTo(rectangle.Left + rectangle.Width, rectangle.Top - rectangle.Height);
			
			contentByte.LineTo(rectangle.Left   + rectangle.Width, rectangle.Top);
			
			contentByte.LineTo(rectangle.Left, rectangle.Top);
			
			contentByte.FillStroke();
			contentByte.ResetRGBColorFill();
			
		}
	}
}
