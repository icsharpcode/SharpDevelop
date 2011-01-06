// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using ICSharpCode.Reports.Core.Exporter;
using iTextSharp.text.pdf;

/// <summary>
/// This class act's as a baseClass for all Shapes
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 09.10.2005 18:09:35
/// </remarks>
namespace ICSharpCode.Reports.Core {	
	
	public abstract class BaseShape : object {
		
		#region Virual Method's
		
		public abstract GraphicsPath CreatePath (Rectangle rectangle) ;
		public abstract GraphicsPath CreatePath (Point from,Point to);
		
		public abstract void CreatePath (PdfContentByte contentByte,
		                                 BaseLine line,
		                                 IBaseStyleDecorator style,
		                                 iTextSharp.text.Rectangle rectangle);
		
		public abstract void CreatePath (PdfContentByte contentByte,
		                                 BaseLine line,
		                                 IBaseStyleDecorator style,
		                                 Point from,
		                                Point to);
		
		#endregion
		
		
		#region GDI+
		
		private void FillShape (Graphics graphics, Brush brush,Rectangle rectangle)
		{
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			Rectangle r = System.Drawing.Rectangle.Inflate(rectangle,1,1);
			GraphicsPath path1 = this.CreatePath(r);
			graphics.FillPath(brush, path1);
		}
		
		
		public void FillShape (Graphics graphics,AbstractFillPattern fillPattern,Rectangle rectangle) {
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			if (fillPattern == null) {
				throw new ArgumentNullException("fillPattern");
			}
			using (Brush brush = fillPattern.CreateBrush(rectangle)){
				if (brush != null){
					this.FillShape(graphics, brush, rectangle);
				}
			}
		}
		
		#endregion
		
		#region Standard graphics
		
		// Draw a Line
		public void DrawShape(Graphics graphics, BaseLine line, Point from,Point to)
		{
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			if (line == null) {
				throw new ArgumentNullException("line");
			}
			using (Pen pen = line.CreatePen(line.Thickness)){
				if (pen != null){
					GraphicsPath path1 = this.CreatePath(from,to);
					graphics.DrawPath(pen, path1);
				}
			}
		}
		
		
		public virtual void DrawShape(Graphics graphics, BaseLine line, Rectangle rectangle)
		{
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			if (line == null) {
				throw new ArgumentNullException("line");
			}
			
			using (Pen pen = line.CreatePen(line.Thickness)){
				if (pen != null){
					GraphicsPath path1 = this.CreatePath(rectangle);
					graphics.DrawPath(pen, path1);
				}
			}
		}
		
		#endregion
		
		
		#region itextSharp
		
		protected static void SetupShape (PdfContentByte cb,IBaseStyleDecorator style)
		{
			cb.SetColorStroke(style.PdfFrameColor);
			cb.SetColorFill(style.PdfBackColor);
		}
		
		
		protected static void FillBackGround (iTextSharp.text.pdf.PdfContentByte contentByte,
		                             IBaseStyleDecorator style,
		                             iTextSharp.text.Rectangle rectangle)
		{
				contentByte.SetColorFill(style.PdfBackColor);
				contentByte.Rectangle (rectangle.Left, rectangle.Top - rectangle.Height , rectangle.Width,rectangle.Height);
				contentByte.Fill();
		}
		
		
		public void DrawShape(PdfContentByte cb,
		                      BaseLine line,
		                      IBaseStyleDecorator style,
		                      iTextSharp.text.Rectangle r)
		{
			this.CreatePath(cb,line,style,r);
		}
		
		
		public void DrawShape(PdfContentByte cb,
		                      BaseLine line,
		                      IBaseStyleDecorator style,
		                      Point from,Point to)
		{
			this.CreatePath(cb,line,style,from,to);
		}
		
		protected static void FinishShape (PdfContentByte contentByte)
		{
			contentByte.FillStroke();
			contentByte.ResetRGBColorFill();
		}
		#endregion
	}
}
