// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.Globals;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ICSharpCode.Reports.Core.Exporter
{
	/// <summary>
	/// Description of ExportGraphic.
	/// </summary>
	public class ExportGraphic:BaseExportColumn
	{
		
		#region Constructor
		
		public ExportGraphic():base()
		{
		}
		
		public ExportGraphic (IBaseStyleDecorator itemStyle):base(itemStyle)
		{
		}
		
		#endregion
		
		
		public override void DrawItem(Graphics graphics)
		{
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			base.DrawItem(graphics);
			ILineDecorator lineDecorator = base.StyleDecorator as LineDecorator;
			if (lineDecorator != null) {
				GraphicsLineDrawer (graphics);
			}
			else  {
				IGraphicStyleDecorator style = base.StyleDecorator as GraphicStyleDecorator;
				if (style != null) {
					base.FillShape(graphics,style.Shape);
					BaseLine baseLine = null;
					if (style.BackColor == GlobalValues.DefaultBackColor){
						baseLine = new BaseLine (style.ForeColor,style.DashStyle,style.Thickness);
					} else {
						baseLine = new BaseLine (style.BackColor,style.DashStyle,style.Thickness);
					}
					style.Shape.DrawShape(graphics,
					                      baseLine,
					                      style.DisplayRectangle);
				}
			}
		}
		
		
		private void GraphicsLineDrawer (Graphics graphics)
		{
			LineDecorator lineStyle = base.StyleDecorator as LineDecorator;
			
			BaseLine baseLine = null;
			baseLine = new BaseLine (lineStyle.ForeColor,lineStyle.DashStyle,lineStyle.Thickness);
			
			Point from = new Point(lineStyle.DisplayRectangle.Left +  lineStyle.From.X,
			                       lineStyle.DisplayRectangle.Top + lineStyle.From.Y);
			Point to = new Point(lineStyle.DisplayRectangle.Left +  lineStyle.To.X,
			                     lineStyle.DisplayRectangle.Top + lineStyle.To.Y);
			lineStyle.Shape.DrawShape(graphics,
			                          baseLine,
			                          from,
			                          to);
		}
		
		
		
		private void PdfLineDrawer ()                    
		{
			LineDecorator lineStyle = base.StyleDecorator as LineDecorator;
			if (lineStyle != null) {
				iTextSharp.text.Rectangle r = base.ConvertToPdfRectangle();
				int l = lineStyle.DisplayRectangle.Left + lineStyle.From.X;
				
				Point from = new Point ((int)UnitConverter.FromPixel(l),
				                        (int)r.Top - (int)UnitConverter.FromPixel(lineStyle.From.Y));
				                        

				l = lineStyle.DisplayRectangle.Left + lineStyle.To.X;
				
				Point to = new Point ((int)UnitConverter.FromPixel(l),
				                        (int)r.Top - (int)UnitConverter.FromPixel(lineStyle.To.Y));

				lineStyle.Shape.DrawShape(base.PdfWriter.DirectContent,
				                          new BaseLine (lineStyle.ForeColor,lineStyle.DashStyle,lineStyle.Thickness),
				                          lineStyle,
				                          from,to);
			}
		}
		
		
		public override void DrawItem(PdfWriter pdfWriter,
		                              ICSharpCode.Reports.Core.Exporter.ExportRenderer.PdfUnitConverter converter)
		{
			base.DrawItem(pdfWriter, converter);
			ILineDecorator lineDecorator = base.StyleDecorator as LineDecorator;
			if (lineDecorator != null) {
				PdfLineDrawer ();
			}
			else
			{
//				http://www.mikesdotnetting.com/Article/88/iTextSharp-Drawing-shapes-and-Graphics
				IGraphicStyleDecorator style = base.StyleDecorator as GraphicStyleDecorator;
				style.Shape.DrawShape(base.PdfWriter.DirectContent,
				                      new BaseLine (style.ForeColor,style.DashStyle,style.Thickness),
				                      style,
				                      base.ConvertToPdfRectangle());
			}
		}
		
		
		public new IGraphicStyleDecorator StyleDecorator
		{
			get{
				return base.StyleDecorator as IGraphicStyleDecorator;
			}
		}
	}
}
