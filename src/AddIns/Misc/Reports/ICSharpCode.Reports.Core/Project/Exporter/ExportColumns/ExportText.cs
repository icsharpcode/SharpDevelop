// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ICSharpCode.Reports.Core.Exporter {
	/// <summary>
	/// Description of LineItem.
	/// </summary>
	public class ExportText :BaseExportColumn{
		
		private string text;
		
		#region Constructors
		
//		public ExportText():base()
//		{
//		}
		
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
//			
//			http://www.google.de/search?hl=de&q=itextsharp+%2B+measure+text&start=10&sa=N
//			http://www.mikesdotnetting.com/Article/82/iTextSharp-Adding-Text-with-Chunks-Phrases-and-Paragraphs
//			http://www.mikesdotnetting.com/Category/20
			
//	itextsharp + columntext + textheight		
//itextsharp + columntext + rectangle			

//itextsharp + simulate
//http://www.mail-archive.com/itext-questions@lists.sourceforge.net/msg04747.html
			base.Decorate();
			PdfContentByte contentByte = base.PdfWriter.DirectContent;
			iTextSharp.text.Rectangle r = base.ConvertToPdfRectangle();
			ColumnText columnText = new ColumnText(contentByte);
			PdfFormat pdfFormat = new PdfFormat(this.StyleDecorator,font);
			
			columnText.SetSimpleColumn(r.Left, r.Top , r.Left + r.Width,r.Height,pdfFormat.Leading,pdfFormat.Alignment);
//			int a = Convert.ToInt16((r.Height/font.Size) + 1);
//			columnText.SetSimpleColumn(r.Left, r.Top , r.Left + r.Width,r.Height,a,pdfFormat.Alignment);
			string formated = StandardFormatter.FormatOutput(this.text,this.StyleDecorator.FormatString,
			                                                 this.StyleDecorator.DataType,String.Empty);
			
			Chunk chunk = new Chunk(formated,font);
			
			columnText.AddText(chunk);
			
			columnText.Go();
			int i = columnText.LinesWritten;
			
			if (i > 1) {
				Console.WriteLine("{0} - {1}",i,this.text);
				Console.WriteLine("dif {0}",r.Height/font.Size);
			}
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
	
	internal class PdfFormat {
		
		float leading;
		int alignment;
		iTextSharp.text.Font font;
		TextStyleDecorator textDecorator;
		float height;
		
		public PdfFormat (TextStyleDecorator textDecorator,iTextSharp.text.Font font)
		{
			if (textDecorator == null) {
				throw new ArgumentNullException ("textDecorator");
			}
			if (font == null) {
				throw new ArgumentNullException("font");
			}
			this.font = font;
			this.textDecorator = textDecorator;
			this.height = UnitConverter.FromPixel(this.textDecorator.DisplayRectangle.Height).Point;
			this.CalculateFormat();
		}
		
		
		private void CalculateFormat()
		{
			this.leading = font.Size;
			this.alignment = PdfContentByte.ALIGN_LEFT;
			
			switch (textDecorator.ContentAlignment) {
				//Top
				case ContentAlignment.TopLeft:
					this.alignment = PdfContentByte.ALIGN_LEFT;
					this.leading = UnitConverter.FromPixel(textDecorator.Font.GetHeight()).Point;
					break;
				case ContentAlignment.TopCenter:
					this.alignment = PdfContentByte.ALIGN_CENTER;
//					this.leading = new UnitConverter(this.font.Size,XGraphicsUnit.Pixel).Point +1;
					this.leading = UnitConverter.FromPixel(textDecorator.Font.GetHeight()).Point;
					break;
				case ContentAlignment.TopRight:
					this.alignment = PdfContentByte.ALIGN_RIGHT;
//					this.leading = new UnitConverter(this.font.Size,XGraphicsUnit.Pixel).Point +1;
					this.leading = UnitConverter.FromPixel(textDecorator.Font.GetHeight()).Point;
					break;
				// Middle	
				case ContentAlignment.MiddleLeft:
					this.alignment = PdfContentByte.ALIGN_LEFT;		
					this.leading = (float)Math.Ceiling((this.height / 2) +  ((new UnitConverter(this.font.Size,XGraphicsUnit.Pixel).Point /2) - 0.5));
					break;
				case ContentAlignment.MiddleCenter:
					this.alignment = PdfContentByte.ALIGN_CENTER;
					this.leading = (float)Math.Ceiling((this.height / 2) +  ((new UnitConverter(this.font.Size,XGraphicsUnit.Pixel).Point /2) - 0.5));
					break;
				case ContentAlignment.MiddleRight:
					this.alignment = PdfContentByte.ALIGN_RIGHT;
					this.leading = (float)Math.Ceiling((this.height / 2) +  ((new UnitConverter(this.font.Size,XGraphicsUnit.Pixel).Point /2) - 0.5));
					break;
				//Bottom	
				case ContentAlignment.BottomLeft:
					this.alignment = PdfContentByte.ALIGN_LEFT;
					this.leading = this.height - new UnitConverter(this.font.Size,XGraphicsUnit.Pixel).Point +4;
					break;
				case ContentAlignment.BottomCenter:
					this.alignment = PdfContentByte.ALIGN_CENTER;
					this.leading = this.height - new UnitConverter(this.font.Size,XGraphicsUnit.Pixel).Point +4;
					break;
				case ContentAlignment.BottomRight:
					this.alignment = PdfContentByte.ALIGN_RIGHT;
					this.leading = this.height - new UnitConverter(this.font.Size,XGraphicsUnit.Pixel).Point +4;
					break;
			}
		}
		
		public float Leading {
			get { return leading; }
		}
		
		public int Alignment {
			get { return alignment; }
		}
		
	}
}
