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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using ICSharpCode.Reports.Core.Exporter;


/// <summary>
/// This Class handles Lines
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 28.09.2005 23:46:19
/// </remarks>
namespace ICSharpCode.Reports.Core {
	
	public class BaseLineItem : BaseGraphicItem,IExportColumnBuilder {
		
		private LineShape shape  = new LineShape();
		
		#region Constructor
		
		public BaseLineItem():base() 
		{
		}
		
		#endregion
		
		#region IExportColumnBuilder  implementation
		
		public IBaseExportColumn CreateExportColumn()
		{
			LineDecorator style = this.CreateLineShape();
			ExportGraphic item = new ExportGraphic(style);
			return item as ExportGraphic;
		}
	
		
		private LineDecorator CreateLineShape ()
		{
			LineDecorator decorator = new LineDecorator(this.shape);
			decorator.Size = this.Size;
			decorator.Location = this.Location;
			decorator.BackColor = this.BackColor;
			decorator.ForeColor = this.ForeColor;
			
			decorator.Thickness = base.Thickness;
			decorator.DashStyle = base.DashStyle;
			decorator.From = this.FromPoint;
			decorator.To = this.ToPoint;
			return decorator;
		}
		
		Size CalculateLineSize()
		{
			int dif = 0;
			if (FromPoint.Y < ToPoint.Y) {
				//
				dif = ToPoint.Y - FromPoint.Y;
			} else {
				dif = FromPoint.Y - ToPoint.Y;
			}
			return new Size (Size.Width, dif + ICSharpCode.Reports.Core.Globals.GlobalValues.GapBetweenContainer);
		}
		
		
		#endregion
		
		public override void Render(ReportPageEventArgs rpea)
		{
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}
			base.Render (rpea);
			shape.DrawShape (rpea.PrintPageEventArgs.Graphics,
			                 new BaseLine (this.ForeColor,base.DashStyle,
			                               base.Thickness,
			                               this.StartLineCap,
			                               this.EndLineCap,
			                               this.DashLineCap),
			                 new Point(base.DisplayRectangle.Left + this.FromPoint.X,
			                           this.FromPoint.Y + base.DisplayRectangle.Top),
			                 new Point (base.DisplayRectangle.Left + this.ToPoint.X,
			                            this.ToPoint.Y + base.DisplayRectangle.Top));
			
		}
		
		
		public override string ToString() 
		{
			return "BaseLineItem";
		}
		
		public virtual Point FromPoint {get;set;}
		
		public virtual Point ToPoint {get;set;}
			
		public LineCap StartLineCap {get;set;}
		
		public LineCap EndLineCap {get;set;}
		
		public DashCap DashLineCap {get;set;}
		
	}
}
