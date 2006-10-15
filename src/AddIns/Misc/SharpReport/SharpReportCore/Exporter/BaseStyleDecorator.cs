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
		
	}
	
	public class TextDecorator :BaseStyleDecorator
	{
		TextDecorator () :base() {
			
		}
	}
}
