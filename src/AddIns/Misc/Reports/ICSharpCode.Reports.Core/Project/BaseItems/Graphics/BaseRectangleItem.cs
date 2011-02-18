// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using ICSharpCode.Reports.Core.BaseClasses.Printing;
using ICSharpCode.Reports.Core.Exporter;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.Reports.Core.Interfaces;

/// <summary>
/// This class draws a Rectangle
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 29.09.2005 11:57:30
/// </remarks>
namespace ICSharpCode.Reports.Core
{
	
	public class BaseRectangleItem : BaseGraphicItem,IExportColumnBuilder,ISimpleContainer
	{
		private ReportItemCollection items;
		private RectangleShape shape = new RectangleShape();
		
		#region Constructor
		
		public BaseRectangleItem()
		{
		}
		
		#endregion
		
		
		#region IExportColumnBuilder
		
		public BaseExportColumn CreateExportColumn(){
			shape.CornerRadius = CornerRadius;
			IGraphicStyleDecorator style = base.CreateItemStyle(this.shape);
			return  new ExportGraphicContainer(style,true);
		}
		
		
		#endregion
		
		public override void Render(ReportPageEventArgs rpea) {
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}
			base.Render(rpea);
			Rectangle rectangle = base.DisplayRectangle;
			StandardPrinter.FillBackground(rpea.PrintPageEventArgs.Graphics,this.BaseStyleDecorator);
			
			BaseLine line = new BaseLine(base.ForeColor,base.DashStyle,base.Thickness,LineCap.Round,LineCap.Round,DashCap.Round);
			
			using (Pen pen = line.CreatePen(line.Thickness)){
				if (pen != null)
				{
					shape.CornerRadius = this.CornerRadius;

					GraphicsPath gfxPath = shape.CreatePath(rectangle);
					
					rpea.PrintPageEventArgs.Graphics.FillPath(new SolidBrush(BackColor), gfxPath);;
					rpea.PrintPageEventArgs.Graphics.DrawPath(pen, gfxPath);
				}
			}
		}
		
		
		public int CornerRadius {get;set;}
		
		
		public override string ToString() {
			return "BaseRectangleItem";
		}
		
		
		public ReportItemCollection Items {
			get {
				if (this.items == null) {
					this.items = new ReportItemCollection();
				}
				return this.items;
			}
		}
	}
	
	public class ExportGraphicContainer :ExportContainer ,IExportContainer
	{
		ExporterCollection items;
		
		
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
