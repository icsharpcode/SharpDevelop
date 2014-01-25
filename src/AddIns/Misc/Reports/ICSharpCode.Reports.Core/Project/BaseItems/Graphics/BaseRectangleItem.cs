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
