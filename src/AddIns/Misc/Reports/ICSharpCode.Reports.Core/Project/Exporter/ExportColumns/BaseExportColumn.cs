// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using iTextSharp.text.pdf;

namespace ICSharpCode.Reports.Core.Exporter
{
	/// <summary>
	/// Description of BaseLineItem.
	/// </summary>
	public class BaseExportColumn
	{
		
		private IBaseStyleDecorator styleDecorator;
		private bool isContainer;
		private ICSharpCode.Reports.Core.Exporter.ExportRenderer.PdfUnitConverter converter;
		private PdfWriter pdfWriter;
		
		#region Constructors
		
		public BaseExportColumn()
		{
			this.styleDecorator = new BaseStyleDecorator(System.Drawing.Color.White,
			                                             System.Drawing.Color.Black);
		}
		
		
		public BaseExportColumn(IBaseStyleDecorator itemStyle, bool isContainer)
		{
			this.styleDecorator = itemStyle;
			this.isContainer = isContainer;
		}
		
		#endregion
		
		
		#region draw item
		
		public virtual void DrawItem (Graphics graphics) 
		{
		}
		
		
		public virtual void DrawItem (PdfWriter pdfWriter,
		                              ICSharpCode.Reports.Core.Exporter.ExportRenderer.PdfUnitConverter converter)
		{
			if (pdfWriter == null) {
				throw new ArgumentNullException("pdfWriter");
			}
			if (converter == null) {
				throw new ArgumentNullException("converter");
			}
			this.pdfWriter = pdfWriter;
			this.converter = converter;
		}
		#endregion
		
		
		#region Decorate
		
		/// <summary>
		/// Fill the Background and draw a (Rectangle)Frame around the Control
		/// </summary>
		/// <param name="graphics"></param>
		
		protected virtual void Decorate (Graphics graphics)
		{
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			RectangleShape shape = new RectangleShape();
			this.FillShape(graphics,shape);
			this.DrawFrame(graphics);
		}
		
		
		protected virtual void Decorate ()
		{
			RectangleShape shape = new RectangleShape();
			iTextSharp.text.Rectangle rect = ConvertToPdfRectangle();
			shape.DrawShape(this.pdfWriter.DirectContent,
			            null,
			            this.styleDecorator,rect);
			this.DrawFrame();
		}
		
		#endregion
		
		
		#region FillShape
		
		/// <summary>
		/// Draw the Backround <see cref="BaseStyleDecorator"></see>
		/// </summary>
		/// <param name="graphics">a valid graphics object</param>
		/// <param name="shape">the shape to fill</param>
		protected virtual void FillShape (Graphics graphics,BaseShape shape)
		{
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			if (shape == null) {
				throw new ArgumentNullException("shape");
			}
			
			shape.FillShape(graphics,
			                new SolidFillPattern(this.styleDecorator.BackColor),
			                this.styleDecorator.DisplayRectangle);
			
		}
		
		#endregion
		
		protected iTextSharp.text.Rectangle ConvertToPdfRectangle ()
		{
			ScreenRectToPdfRectConverter rectangleToPage = new ScreenRectToPdfRectConverter(this.converter);
			return (iTextSharp.text.Rectangle)rectangleToPage.ConvertTo(null,System.Globalization.CultureInfo.InvariantCulture,
			                                                            this.styleDecorator.DisplayRectangle,
			                                                            typeof(iTextSharp.text.Rectangle));
		}
		
		#region DrawFrame
		
		
		private Border CreateDefaultBorder()
		{
			return new Border(new BaseLine (this.styleDecorator.FrameColor,
			                                    System.Drawing.Drawing2D.DashStyle.Solid,1));
		}
		
		
		private  void DrawFrame (Graphics graphics)
		{
			if (this.styleDecorator.DrawBorder) {
				Border b = this.CreateDefaultBorder();
				b.DrawBorder(graphics,this.styleDecorator.DisplayRectangle);
			}
		}
		
		
		private  void DrawFrame ()
		{
			if (this.styleDecorator.DrawBorder) {
				Border b = this.CreateDefaultBorder();
				iTextSharp.text.Rectangle rect = ConvertToPdfRectangle();
				b.DrawBorder(this.pdfWriter.DirectContent,rect,this.styleDecorator);
			}
		}

		
		
		#endregion
		

		public virtual IBaseStyleDecorator StyleDecorator {
			get {
				return styleDecorator;
			}
			set {
				this.styleDecorator = value;
			}
		}
		
		public bool IsContainer {
			get {
				return isContainer;
			}
			set {
				isContainer = value;
			}
		}
		
		public ICSharpCode.Reports.Core.Exporter.ExportRenderer.PdfUnitConverter PdfUnitConverter {
			get { return converter; }
		}
		
		public PdfWriter PdfWriter {
			get { return pdfWriter; }
		}
		
	}
}
