// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			
			iTextSharp.text.Font font = CreateFontFromFactory(this.StyleDecorator);
			CalculatePdfFormat pdfFormat = new CalculatePdfFormat(this.StyleDecorator,font);
			
			ColumnText columnText = new ColumnText(contentByte);

			if (StyleDecorator.RightToLeft.ToString() == "Yes") {
				columnText.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
			}
			
			iTextSharp.text.Rectangle r = base.ConvertToPdfRectangle();
			columnText.SetSimpleColumn(r.Left, r.Top , r.Left + r.Width,r.Top - r.Height,pdfFormat.Leading,pdfFormat.Alignment);
		
			string formatedText = this.Text;
			
			if (!String.IsNullOrEmpty(StyleDecorator.FormatString)) {
				formatedText = StandardFormatter.FormatOutput(this.Text,this.StyleDecorator.FormatString,
				                                                 this.StyleDecorator.DataType,String.Empty);
			}
			
			Chunk chunk = new Chunk(formatedText,font);

			columnText.AddText(chunk);
			
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
		
		TextStyleDecorator textDecorator;
		
		public CalculatePdfFormat (TextStyleDecorator textDecorator,iTextSharp.text.Font font)
		{
			if (textDecorator == null) {
				throw new ArgumentNullException ("textDecorator");
			}
			if (font == null) {
				throw new ArgumentNullException("font");
			}
			this.textDecorator = textDecorator;
			this.Leading = font.Size;
			this.CalculateFormat();
		}
		
		
		private void CalculateFormat()
		{
			
			this.Alignment = PdfContentByte.ALIGN_LEFT;
			
			switch (textDecorator.ContentAlignment) {
					//Top
				case ContentAlignment.TopLeft:
					this.Alignment = PdfContentByte.ALIGN_LEFT;
					break;
				case ContentAlignment.TopCenter:
					this.Alignment = PdfContentByte.ALIGN_CENTER;
					break;
				case ContentAlignment.TopRight:
					this.Alignment = PdfContentByte.ALIGN_RIGHT;
					break;
					// Middle
				case ContentAlignment.MiddleLeft:
					this.Alignment = PdfContentByte.ALIGN_LEFT;
					break;
				case ContentAlignment.MiddleCenter:
					this.Alignment = PdfContentByte.ALIGN_CENTER;
					break;
				case ContentAlignment.MiddleRight:
					this.Alignment = PdfContentByte.ALIGN_RIGHT;
					break;
					//Bottom
				case ContentAlignment.BottomLeft:
					this.Alignment = PdfContentByte.ALIGN_LEFT;
					break;
				case ContentAlignment.BottomCenter:
					this.Alignment = PdfContentByte.ALIGN_CENTER;
					break;
				case ContentAlignment.BottomRight:
					this.Alignment = PdfContentByte.ALIGN_RIGHT;
					break;
			}
			
		}
		
		public float Leading {get;private set;}
		
		public int Alignment {get;private set;}
			
	}
}
