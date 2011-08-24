// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
