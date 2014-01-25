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
