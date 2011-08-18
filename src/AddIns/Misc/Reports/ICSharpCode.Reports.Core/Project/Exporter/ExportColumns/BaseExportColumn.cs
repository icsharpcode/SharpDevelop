// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Globalization;

using ICSharpCode.Reports.Core.BaseClasses.Printing;
using ICSharpCode.Reports.Core.Globals;
using iTextSharp.text.pdf;

namespace ICSharpCode.Reports.Core.Exporter
{
	/// <summary>
	/// Description of BaseLineItem.
	/// </summary>
	public class BaseExportColumn : IBaseExportColumn
	{

		#region Constructors

		public BaseExportColumn()
		{
			this.StyleDecorator = new BaseStyleDecorator(System.Drawing.Color.White, System.Drawing.Color.Black);
		}


		public BaseExportColumn(IBaseStyleDecorator styleDecorator)
		{
			this.StyleDecorator = styleDecorator;
		}

		#endregion

		#region draw item

		public virtual void DrawItem(Graphics graphics)
		{
		}


		public virtual void DrawItem(PdfWriter pdfWriter, ICSharpCode.Reports.Core.Exporter.ExportRenderer.PdfUnitConverter converter)
		{
			if (pdfWriter == null) {
				throw new ArgumentNullException("pdfWriter");
			}
			if (converter == null) {
				throw new ArgumentNullException("converter");
			}
			this.PdfWriter = pdfWriter;
			this.PdfUnitConverter = converter;
		}
		#endregion


		#region Decorate

		/// <summary>
		/// Fill the Background and draw a (Rectangle)Frame around the Control
		/// </summary>
		/// <param name="graphics"></param>

		protected virtual void Decorate(Graphics graphics)
		{
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			var shape = new RectangleShape();
			this.FillShape(graphics, shape);
			StandardPrinter.DrawBorder(graphics, this.StyleDecorator as BaseStyleDecorator);
		}


		protected virtual void Decorate()
		{
			var shape = new RectangleShape();
			shape.DrawShape(this.PdfWriter.DirectContent, null, this.StyleDecorator, ConvertToPdfRectangle());
			this.DrawFrame();
		}

		#endregion


		#region FillShape

		/// <summary>
		/// Draw the Backround <see cref="BaseStyleDecorator"></see>
		/// </summary>
		/// <param name="graphics">a valid graphics object</param>
		/// <param name="shape">the shape to fill</param>
		/// 
		protected virtual void FillShape(Graphics graphics, BaseShape shape)
		{
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			if (shape == null) {
				throw new ArgumentNullException("shape");
			}
			shape.FillShape(graphics, new SolidFillPattern(this.StyleDecorator.BackColor), this.StyleDecorator.DisplayRectangle);
		}

		#endregion


		protected iTextSharp.text.Rectangle ConvertToPdfRectangle()
		{
			ScreenRectToPdfRectConverter rectangleConverter = new ScreenRectToPdfRectConverter(this.PdfUnitConverter);
			iTextSharp.text.Rectangle r = (iTextSharp.text.Rectangle)rectangleConverter.
				ConvertTo(null, CultureInfo.InvariantCulture, 
				          this.StyleDecorator.DisplayRectangle, typeof(iTextSharp.text.Rectangle));

			iTextSharp.text.Rectangle rr = new iTextSharp.text.Rectangle(r.Left, r.Bottom - 2, r.Left + r.Width, r.Bottom + r.Height);
			return rr;
		}


		#region DrawFrame


		private Border CreateDefaultBorder()
		{
			return new Border(new BaseLine(this.StyleDecorator.FrameColor, System.Drawing.Drawing2D.DashStyle.Solid, 1));
		}


		private void DrawFrame()
		{
			if (this.StyleDecorator.DrawBorder) {
				Border b = this.CreateDefaultBorder();
				b.DrawBorder(this.PdfWriter.DirectContent, ConvertToPdfRectangle(), this.StyleDecorator);
			}
		}


		#endregion

		public virtual IBaseStyleDecorator StyleDecorator { get; set; }

		protected ICSharpCode.Reports.Core.Exporter.ExportRenderer.PdfUnitConverter PdfUnitConverter { get; set; }

		protected PdfWriter PdfWriter { get; set; }

	}
}
