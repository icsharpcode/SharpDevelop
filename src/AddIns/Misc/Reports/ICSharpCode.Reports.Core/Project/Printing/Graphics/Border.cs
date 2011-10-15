// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
