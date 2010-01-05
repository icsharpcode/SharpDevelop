// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace ICSharpCode.Reports.Core.Exporter
{
	/// <summary>
	/// Description of LineStyle.
	/// </summary>
	public class BaseStyleDecorator : IBaseStyleDecorator
	{

		private bool drawBorder;
		private Color backColor;
		private Color foreColor;
		private Point location;
		private Size size;

		public BaseStyleDecorator() : this(GlobalValues.DefaultBackColor, Color.Black)
		{
		}

		public BaseStyleDecorator(Color backColor, Color foreColor)
		{
			this.backColor = backColor;
			this.foreColor = foreColor;
		}

		
		public bool DrawBorder {
			get { return drawBorder; }
			set { drawBorder = value; }
		}


		public Color BackColor {
			get { return backColor; }
			set { backColor = value; }
		}


		public iTextSharp.text.Color PdfBackColor {
			get { return new iTextSharp.text.Color(this.backColor.R, this.backColor.G, this.backColor.B, this.backColor.A); }
		}


		public Color ForeColor {
			get { return foreColor; }
			set { foreColor = value; }
		}


		public iTextSharp.text.Color PdfForeColor {
			get { return new iTextSharp.text.Color(this.foreColor.R, this.foreColor.G, this.foreColor.B, this.foreColor.A); }
		}


		public Point Location {
			get { return location; }
			set { location = value; }
		}

		public Size Size {
			get { return size; }
			set { size = value; }
		}


		public Rectangle DisplayRectangle {
			get { return new Rectangle(this.location.X, this.location.Y, this.size.Width, this.size.Height); }
		}
	}
}
