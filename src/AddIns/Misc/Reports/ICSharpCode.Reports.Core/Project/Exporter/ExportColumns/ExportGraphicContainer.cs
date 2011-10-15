/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 18.02.2011
 * Time: 20:20
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Linq;

using ICSharpCode.Reports.Core.BaseClasses;
using ICSharpCode.Reports.Core.Globals;
using iTextSharp.text.pdf;

namespace ICSharpCode.Reports.Core.Exporter
{
	public class ExportGraphicContainer :ExportContainer
	{
//		public ExportGraphicContainer (IExportContainer itemStyle,bool isContainer):base(itemStyle as BaseStyleDecorator)
		public ExportGraphicContainer (IBaseStyleDecorator itemStyle):base(itemStyle as BaseStyleDecorator)
		{
			
		}
		
		public override void DrawItem(Graphics graphics)
		{
			base.DrawItem(graphics);
			ILineDecorator lineDecorator = base.StyleDecorator as LineDecorator;
			if (lineDecorator != null)
			{
				throw new ArgumentException("Line is not a valid Container");
			}
			else
			{
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
				Items.ForEach(item =>item.DrawItem(graphics));
			}
		}
		
		
		public override void DrawItem(PdfWriter pdfWriter, ICSharpCode.Reports.Core.Exporter.ExportRenderer.PdfUnitConverter converter)
		{
			base.PdfWriter = pdfWriter;
			base.PdfUnitConverter = converter;
			
			IGraphicStyleDecorator style = base.StyleDecorator as GraphicStyleDecorator;
		
			style.Shape.DrawShape(pdfWriter.DirectContent,
			                      new BaseLine (style.ForeColor,style.DashStyle,style.Thickness),
			                      style,
			                      ConvertToPdfRectangle());
			Items.ForEach(item =>item.DrawItem(pdfWriter,converter));
//			foreach (ICSharpCode.Reports.Core.Exporter.BaseExportColumn baseExportColumn in this.Items)
//			{
//				baseExportColumn.DrawItem(pdfWriter,converter);
//			}
		}
		
		
	}
}
