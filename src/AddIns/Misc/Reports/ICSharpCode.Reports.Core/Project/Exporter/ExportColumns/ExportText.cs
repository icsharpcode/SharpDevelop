// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.BaseClasses.Printing;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ICSharpCode.Reports.Core.Exporter {
	/// <summary>
	/// Description of LineItem.
	/// </summary>
	public class ExportText :BaseExportColumn{
		
		private string text;
		
		#region Constructors
		

		
		public ExportText (BaseStyleDecorator itemStyle,bool isContainer):base(itemStyle,isContainer)
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
			
			iTextSharp.text.Font font = null;
			
			if (this.StyleDecorator.Font.Unit == GraphicsUnit.Point) {
				
				font = FontFactory.GetFont(this.StyleDecorator.Font.FontFamily.Name,
				                           BaseFont.IDENTITY_H,
				                           this.StyleDecorator.Font.Size,
				                           (int)this.StyleDecorator.Font.Style,
				                           this.StyleDecorator.PdfForeColor);
				
			} else {
				
				font = FontFactory.GetFont(this.StyleDecorator.Font.FontFamily.Name,
				                           BaseFont.IDENTITY_H,
				                           UnitConverter.FromPixel(this.StyleDecorator.Font.Size).Point,
				                           (int)this.StyleDecorator.Font.Style,
				                           this.StyleDecorator.PdfForeColor);
			}

			base.Decorate();
			
			PdfContentByte contentByte = base.PdfWriter.DirectContent;
		
			CalculatePdfFormat pdfFormat = new CalculatePdfFormat(this.StyleDecorator,font);
			
			ColumnText columnText = new ColumnText(contentByte);
			iTextSharp.text.Rectangle r = base.ConvertToPdfRectangle();
			columnText.SetSimpleColumn(r.Left, r.Top , r.Left + r.Width,r.Top - r.Height,pdfFormat.Leading,pdfFormat.Alignment);
		
			string formatedText = this.text;
			
			if (!String.IsNullOrEmpty(StyleDecorator.FormatString)) {
				formatedText = StandardFormatter.FormatOutput(this.text,this.StyleDecorator.FormatString,
				                                                 this.StyleDecorator.DataType,String.Empty);
			}
			
			Chunk chunk = new Chunk(formatedText,font);

			columnText.AddText(chunk);
			
			columnText.Go();
		}
		
		
		public override void DrawItem(Graphics graphics)
		{
			
			base.DrawItem(graphics);
			base.Decorate(graphics);
			TextDrawer.DrawString(graphics, this.text,this.StyleDecorator);
		}
		
		#endregion
		
		
		public string Text
		{
			get {
				return text;
			}
			set {
				text = value;
			}
		}
		
		
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
			return this.text;
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
