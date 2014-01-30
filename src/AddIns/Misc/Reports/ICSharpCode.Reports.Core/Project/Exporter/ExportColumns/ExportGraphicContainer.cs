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
