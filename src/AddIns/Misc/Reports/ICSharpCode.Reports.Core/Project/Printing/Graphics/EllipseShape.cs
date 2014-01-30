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
/// This class draws a Ellipse/Ellipse
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 10.10.2005 09:41:11
/// </remarks>

namespace ICSharpCode.Reports.Core {
	public class EllipseShape : BaseShape
	{
		/// <summary>
		/// Default constructor - initializes all fields to default values
		/// </summary>
		public EllipseShape() {
		}
		
		public override GraphicsPath CreatePath(Point from, Point to)
		{
			throw new NotImplementedException();
		}
		
		
		public override GraphicsPath CreatePath(Rectangle rectangle){
			GraphicsPath path = new GraphicsPath();
			path.AddEllipse(rectangle);
			return path;
		}
		
		
		public override void CreatePath(iTextSharp.text.pdf.PdfContentByte contentByte,
		                                BaseLine line,
		                                IBaseStyleDecorator style,
		                                Point from,Point to)
		{
			throw new NotImplementedException();
		}
		
		
		public override void CreatePath(iTextSharp.text.pdf.PdfContentByte contentByte,
		                                BaseLine line,
		                                IBaseStyleDecorator style,
		                                iTextSharp.text.Rectangle rectangle)
		{
			if (contentByte == null) {
				throw new ArgumentNullException("contentByte");
			}
			if (rectangle == null) {
				throw new ArgumentNullException("rectangle");
			}
		
			if ((line == null)||(line.Thickness < 1)) {
				BaseShape.FillBackGround(contentByte,style,rectangle);
			}
			else if ((style.BackColor == GlobalValues.DefaultBackColor)) {
				BaseShape.SetupShape(contentByte,style);
				contentByte.SetLineWidth(UnitConverter.FromPixel(line.Thickness).Point);
				contentByte.Ellipse(rectangle.Left,
			                    rectangle.Top,
			                    rectangle.Left + rectangle.Width,
			                    rectangle.Top - rectangle.Height);
				BaseShape.FinishShape(contentByte);
			} else {
				BaseShape.FillBackGround(contentByte,style,rectangle);
			}
		}
	}
}
