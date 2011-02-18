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
using ICSharpCode.Reports.Core.Globals;
using iTextSharp.text.pdf;

namespace ICSharpCode.Reports.Core.Exporter
{
	public class ExportGraphicContainer :ExportContainer
	{
		//ExporterCollection items;
		
		public ExportGraphicContainer (IBaseStyleDecorator itemStyle,bool isContainer):base(itemStyle as BaseStyleDecorator)
		{
			
		}
		
		public override void DrawItem(Graphics graphics)
		{
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
	}
}
