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
using ICSharpCode.Reports.Core.BaseClasses.Printing;
using ICSharpCode.Reports.Core.Globals;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ICSharpCode.Reports.Core.Exporter {
	/// <summary>
	/// Description of LineItem.
	/// </summary>
	public class ExportText :BaseExportColumn,IReportExpression
	{
		
		#region Constructors
		
		public ExportText (BaseStyleDecorator itemStyle):base(itemStyle)
		{
		}
		
		#endregion
		
		#region overrides
		
		
		public override void DrawItem(PdfWriter pdfWriter,
		                              ICSharpCode.Reports.Core.Exporter.ExportRenderer.PdfUnitConverter converter)
		{
			if (pdfWriter == null) {
				throw new ArgumentNullException("pdfWriter");
			}
			if (converter == null) {
				throw new ArgumentNullException("converter");
			}
			base.DrawItem(pdfWriter,converter);
			base.Decorate();
	
			PdfContentByte contentByte = base.PdfWriter.DirectContent;
			
			var font = CreateFontFromFactory(this.StyleDecorator);
			
			var columnText = new ColumnText(contentByte);

			if (StyleDecorator.RightToLeft.ToString() == "Yes") {
				columnText.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
			}
			
			var pdfRectangle = base.ConvertToPdfRectangle();

			columnText.SetSimpleColumn(pdfRectangle.Left, pdfRectangle.Top , pdfRectangle.Left + pdfRectangle.Width,pdfRectangle.Top - pdfRectangle.Height);
			
			string formatedText = this.Text;
			
			if (!String.IsNullOrEmpty(StyleDecorator.FormatString)) {
				formatedText = StandardFormatter.FormatOutput(this.Text,this.StyleDecorator.FormatString,
				                                                 this.StyleDecorator.DataType,String.Empty);
			}
			
			Chunk chunk = new Chunk(formatedText,font);

			columnText.AddText(chunk);
			columnText.Alignment = CalculatePdfFormat.PdfAlignment(this.StyleDecorator);
			columnText.Go();
		}
		
	
		private static iTextSharp.text.Font CreateFontFromFactory(TextStyleDecorator styleDecorator)
		{
			iTextSharp.text.Font font = null;
			if (styleDecorator.Font.Unit == GraphicsUnit.Point) {
				
				font = FontFactory.GetFont(styleDecorator.Font.FontFamily.Name,
				                           BaseFont.IDENTITY_H,
				                           styleDecorator.Font.Size,
				                           (int)styleDecorator.Font.Style,
				                           styleDecorator.PdfForeColor);
				
			} else {
				
				font = FontFactory.GetFont(styleDecorator.Font.FontFamily.Name,
				                           BaseFont.IDENTITY_H,
				                           UnitConverter.FromPixel(styleDecorator.Font.Size).Point,
				                           (int)styleDecorator.Font.Style,
				                           styleDecorator.PdfForeColor);
			}
			return font;
		}

		
		
		public override void DrawItem(Graphics graphics)
		{
			
			base.DrawItem(graphics);
			base.Decorate(graphics);
			TextDrawer.DrawString(graphics, this.Text,this.StyleDecorator);
		}
		
		#endregion
		
		
		public string Text {get;set;}
		
		public string Expression {get;set;}
		
		public new TextStyleDecorator StyleDecorator
		{
			get {
				return base.StyleDecorator as TextStyleDecorator;
			}
			set {
				base.StyleDecorator = value;
			}
		}
		
		
		public override string ToString()
		{
			return this.Text;
		}
		
	}

	
	internal class CalculatePdfFormat {
		
		public static int PdfAlignment(TextStyleDecorator textDecorator)
		{
			int retVal = 0;
			
			switch (textDecorator.ContentAlignment) {
					//Top
				case ContentAlignment.TopLeft:
					retVal =  PdfContentByte.ALIGN_LEFT;
					break;
				case ContentAlignment.TopCenter:
					retVal =  PdfContentByte.ALIGN_CENTER;
					break;
				case ContentAlignment.TopRight:
					retVal = PdfContentByte.ALIGN_RIGHT;
					break;
					// Middle
				case ContentAlignment.MiddleLeft:
					retVal = PdfContentByte.ALIGN_LEFT;
					break;
				case ContentAlignment.MiddleCenter:
					retVal = PdfContentByte.ALIGN_CENTER;
					break;
				case ContentAlignment.MiddleRight:
					retVal = PdfContentByte.ALIGN_RIGHT;
					break;
					//Bottom
				case ContentAlignment.BottomLeft:
					retVal = PdfContentByte.ALIGN_LEFT;
					break;
				case ContentAlignment.BottomCenter:
					retVal = PdfContentByte.ALIGN_CENTER;
					break;
				case ContentAlignment.BottomRight:
					retVal = PdfContentByte.ALIGN_RIGHT;
					break;
			}
			return retVal;
			
		}
	}
}
