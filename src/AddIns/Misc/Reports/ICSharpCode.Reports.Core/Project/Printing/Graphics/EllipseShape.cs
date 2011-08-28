// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
