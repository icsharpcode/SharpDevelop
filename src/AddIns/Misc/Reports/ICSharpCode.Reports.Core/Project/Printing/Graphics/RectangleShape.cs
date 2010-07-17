// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>


using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using ICSharpCode.Reports.Core.Exporter;

/// <summary>
/// Draw a Rectangle, used by DesingerControls and printing stuff
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 09.10.2005 18:20:51
/// </remarks>
namespace ICSharpCode.Reports.Core {
	public class RectangleShape : BaseShape {
		
		public RectangleShape() {
		}
		
		
		public override GraphicsPath CreatePath(Point from, Point to)
		{
			throw new NotImplementedException();
		}
		
		
		public override GraphicsPath CreatePath(Rectangle rectangle){
			GraphicsPath path1 = new GraphicsPath();
			path1.AddRectangle(rectangle);
			return path1;
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
			if (style == null) {
				throw new ArgumentNullException("style");
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
				contentByte.MoveTo(rectangle.Left ,rectangle.Top );
				contentByte.LineTo(rectangle.Left, rectangle.Top - rectangle.Height);
				contentByte.LineTo(rectangle.Left + rectangle.Width, rectangle.Top - rectangle.Height);
				contentByte.LineTo(rectangle.Left   + rectangle.Width, rectangle.Top);
				contentByte.LineTo(rectangle.Left, rectangle.Top);
				BaseShape.FinishShape(contentByte);
			} else {
				BaseShape.FillBackGround(contentByte,style,rectangle);
			}
		}
	}
}
