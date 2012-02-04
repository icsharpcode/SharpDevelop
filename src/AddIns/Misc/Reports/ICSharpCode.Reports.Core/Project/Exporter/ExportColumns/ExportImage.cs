// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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

		
		System.Drawing.Image image;
	
		
		#region Constructor
		
		public ExportImage()
		{
		}
		
		
		public ExportImage (BaseStyleDecorator itemStyle):base(itemStyle) 
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
			
			if (this.ScaleImageToSize) {
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
		
		
	
		#endregion
		
		
		// return a copy of image, otherwise pdf is not working
		public System.Drawing.Image Image 
		{
			get { return new Bitmap(image); }
			set { image = value; }
		}
		
		
		public string FileName {get;set;}
		
		
		public bool ScaleImageToSize {get;set;}
		
	}
}
