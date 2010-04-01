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

namespace ICSharpCode.Reports.Core.Exporter
{
	/// <summary>
	/// Description of ExportImage.
	/// </summary>
	public class ExportImage:BaseExportColumn
	{
		string fileName;
		System.Drawing.Image image;
		bool scaleImageToSize;
		
		#region Constructor
		
		public ExportImage()
		{
		}
		
		
		public ExportImage (BaseStyleDecorator itemStyle):this(itemStyle,false) 
		{
		}
		
		public ExportImage (BaseStyleDecorator itemStyle,bool isContainer):base(itemStyle,isContainer)
		{
		}
		
		#endregion
		
		#region overrides
		
		public override void DrawItem(System.Drawing.Graphics graphics)
		{
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			base.DrawItem(graphics);
			
			if (this.scaleImageToSize) {
				graphics.DrawImageUnscaled(this.Image,this.StyleDecorator.DisplayRectangle);
			} else {

				graphics.DrawImage(this.Image,
				                   this.StyleDecorator.DisplayRectangle);
			}
		}

		public override void DrawItem(iTextSharp.text.pdf.PdfWriter pdfWriter, 
		                              ICSharpCode.Reports.Core.Exporter.ExportRenderer.PdfUnitConverter converter)
		{
			
			base.DrawItem(pdfWriter, converter);
			iTextSharp.text.Image im = iTextSharp.text.Image.GetInstance(image,null,false);
			
			iTextSharp.text.Rectangle r = base.ConvertToPdfRectangle();
			im.ScaleAbsolute (r.Width,r.Height);
			im.SetAbsolutePosition(r.Left,r.Top - r.Height);             
			PdfContentByte cb = base.PdfWriter.DirectContent;
			cb.AddImage(im);
		}
		
		
		/*
		 * 
		  ColumnText ct2 = new ColumnText(t);

>                 ct2.RunDirection = runDirection;

>                 ct2.SetSimpleColumn(signatureRect.X, signatureRect.Y, 

> signatureRect.Width, signatureRect.Height, 0, Element.ALIGN_RIGHT);

> 

>                 Image im = Image.GetInstance(SignatureGraphic);

>                 im.ScaleToFit(signatureRect.Width, signatureRect.Height);

> 

>                 System.Drawing.SizeF imageSize = new System.Drawing.SizeF(im.ScaledWidth, im.ScaledHeight);

>                 Paragraph p = new Paragraph();

>                 // must calculate the point to draw from to make image appear in middle of column

>                 System.Drawing.PointF originPoint = System.Drawing.PointF.Empty;

>                 originPoint.X = 0;

>                 // experimentation found this magic number to counteract Adobe's signature graphic, which

>                 // offsets the y co-ordinate by 15 units

>                 originPoint.Y = -im.ScaledHeight + 15;

> 

>                 System.Drawing.PointF graphicLocation = System.Drawing.PointF.Empty;

>                 graphicLocation.X = originPoint.X + (signatureRect.Width - im.ScaledWidth) / 2;

>                 graphicLocation.Y = originPoint.Y - (signatureRect.Height - im.ScaledHeight) / 2;

>                 p.Add(new Chunk(im, graphicLocation.X, graphicLocation.Y, false));

>                 ct2.AddElement(p);

>                 ct2.Go();


		 */
		#endregion
		
		public override IBaseStyleDecorator StyleDecorator 
		{
			get { return base.StyleDecorator; }
			set { base.StyleDecorator = value; }
		}
		
		// return a copy of image, otherwise pdf is not working
		public System.Drawing.Image Image 
		{
			get { return new Bitmap(image); }
			set { image = value; }
		}
		
		
		public string FileName {
			get { return fileName; }
			set { fileName = value; }
		}
		
		public bool ScaleImageToSize {
			get { return scaleImageToSize; }
			set { scaleImageToSize = value; }
		}
	}
}
