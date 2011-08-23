// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using ICSharpCode.Reports.Core.BaseClasses.Printing;
using ICSharpCode.Reports.Core.Exporter;
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
		
		public IBaseExportColumn CreateExportColumn(){
			shape.CornerRadius = CornerRadius;
			IGraphicStyleDecorator style = base.CreateItemStyle(this.shape);
			return  new ExportGraphicContainer(style);
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
}
