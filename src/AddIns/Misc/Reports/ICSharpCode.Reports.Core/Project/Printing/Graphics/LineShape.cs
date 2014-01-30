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
/// Draw a simple Line
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 10.10.2005 12:01:20
/// </remarks>

namespace ICSharpCode.Reports.Core {	
	
	
	public class LineShape : BaseShape {
		
		/// <summary>
		/// Default constructor - initializes all fields to default values
		/// </summary>
		
		public LineShape()
		{
		}
		
		
		//Linedrawing
		
		#region GDI
		
		public override GraphicsPath CreatePath(Point from, Point to)
		{
			GraphicsPath path = new GraphicsPath();
			path.AddLine(from,to);
			return path;
		}
		
		
		public override GraphicsPath CreatePath(Rectangle rectangle){
			GraphicsPath path;
			int halfRect;
			Rectangle rect = Rectangle.Round(rectangle);
			if (rectangle.Width > rectangle.Height) {
				halfRect = rect.Top + (rect.Height /2);
				path = CreatePath (new Point(rect.Left, halfRect),
				                   new Point(rect.Right, halfRect));
			} else {
				halfRect = rect.Left + (rect.Width /2);
				path = CreatePath (new Point(halfRect,rect.Top),
				                   new Point(halfRect,rect.Bottom));
			}
			return path;
		}
		
		
		#endregion
	
		#region Pdf-iTextSharp
		
		public override void CreatePath(iTextSharp.text.pdf.PdfContentByte contentByte,
		                                BaseLine line,
		                                IBaseStyleDecorator style,
		                                Point from,Point to)
		{
			if (contentByte == null) {
				throw new ArgumentNullException("contentByte");
			}

			BaseShape.SetupShape(contentByte,style);
			contentByte.SetLineWidth(UnitConverter.FromPixel(line.Thickness).Point);
			contentByte.MoveTo(from.X,from.Y );
			contentByte.LineTo(to.X,to.Y);
			BaseShape.FinishShape(contentByte);
		}
		
		public override void CreatePath(iTextSharp.text.pdf.PdfContentByte contentByte,
		                                BaseLine line,
		                                IBaseStyleDecorator style,
		                                iTextSharp.text.Rectangle rectangle)
		{
			throw new NotImplementedException();
		}
		
		
		#endregion
		
	}
}
