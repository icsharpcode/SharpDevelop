/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 26.09.2006
 * Time: 14:33
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
	using System.Drawing.Drawing2D;
namespace SharpReportCore.Exporters
{
	/// <summary>
	/// Description of LineStyle.
	/// </summary>
	public class BaseStyleDecorator{
		
		private bool drawBorder;
		private Color backColor;
		private Color foreColor;
		private Point location;
		private Size size;
		private Font font;
		
		private StringFormat stringFormat;
		private StringTrimming stringTrimming;
		private ContentAlignment contentAlignment;
		
		private BaseShape shape;
		private int thickness = 1;
		private DashStyle dashStyle = DashStyle.Solid;
		
		public BaseStyleDecorator():this(Color.White,Color.Black){
		}
		
		public BaseStyleDecorator(Color backColor, Color foreColor)
		{
			this.backColor = backColor;
			this.foreColor = foreColor;
		}
		
		public bool DrawBorder {
			get {
				return drawBorder;
			}
			set {
				drawBorder = value;
			}
		}
		
		public Color BackColor {
			get {
				return backColor;
			}
			set {
				backColor = value;
			}
		}
		
		public Color ForeColor {
			get {
				return foreColor;
			}
			set {
				foreColor = value;
			}
		}
		
		public Point Location {
			get {
				return location;
			}
			set {
				location = value;
			}
		}
		
		public Size Size {
			get {
				return size;
			}
			set {
				size = value;
			}
		}
		
		public Font Font {
			get {
				return font;
			}
			set {
				font = value;
			}
		}
	
		public StringFormat StringFormat {
			get {
				return stringFormat;
			}
			set {
				stringFormat = value;
			}
		}
		
		public StringTrimming StringTrimming {
			get {
				return stringTrimming;
			}
			set {
				stringTrimming = value;
			}
		}
		
		public ContentAlignment ContentAlignment {
			get {
				return contentAlignment;
			}
			set {
				contentAlignment = value;
			}
		}
		
		public Rectangle DisplayRectangle {
			get {
				return new Rectangle(this.location.X,this.location.Y,
				this.size.Width,this.size.Height);
			}
		}
		
		public void DrawGraphic (Graphics graphics) {
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			
			this.shape.DrawShape(graphics,
			                     new BaseLine (this.foreColor,this.dashStyle,this.thickness),
			                 this.DisplayRectangle);
		}
		
		public BaseShape Shape {
			get { return shape; }
			set { shape = value; }
		}
		public int Thickness {
			get { return thickness; }
			set { thickness = value; }
		}
		
		public DashStyle DashStyle {
			get { return dashStyle; }
			set { dashStyle = value; }
		}
		
	}
	
	public class TextDecorator :BaseStyleDecorator
	{
		TextDecorator () :base() {
			
		}
	}
}
