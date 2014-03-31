/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 21.03.2014
 * Time: 20:35
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Drawing.Text;

namespace ICSharpCode.Reporting.BaseClasses
{
	/// <summary>
	/// Description of TextDrawer.
	/// </summary>
	public sealed class TextDrawer 
	{
		
		private TextDrawer() 
		{
		}
		
		
		public static void DrawString(Graphics graphics,string text,
		                              Font font,Brush brush,
		                              RectangleF rectangle,
		                              StringFormat format)
		{
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			
			graphics.DrawString(text,
			                    font,
			                    brush,
			                    rectangle,
			                    format);
		}
		
		
		public static void DrawString (Graphics graphics,string text)
		                       
		{
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
//			if (decorator == null) {
//				throw new ArgumentNullException("decorator");
//			}
			
//			StringFormat stringFormat = BuildStringFormat(decorator.StringTrimming,decorator.ContentAlignment);
//			
//			if (decorator.RightToLeft ==System.Windows.Forms.RightToLeft.Yes) {
//				stringFormat.FormatFlags = stringFormat.FormatFlags | StringFormatFlags.DirectionRightToLeft;
//			}
			
			var formattedString = text;
//			if (! String.IsNullOrEmpty(decorator.FormatString)) {
//				formattedString = StandardFormatter.FormatOutput(text,decorator.FormatString,decorator.DataType,String.Empty);
//			}
			
			graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
			
//			graphics.DrawString (formattedString,decorator.Font,
//			                     new SolidBrush(decorator.ForeColor),
//			                     new Rectangle(decorator.Location.X,
//			                                   decorator.Location.Y,
//			                                   decorator.Size.Width,
//			                                   decorator.Size.Height),
//			                     stringFormat);
		}
		
		
		public static StringFormat BuildStringFormat(StringTrimming stringTrimming,ContentAlignment alignment)
		{
			StringFormat format = StringFormat.GenericTypographic;
			format.Trimming = stringTrimming;
			format.FormatFlags = StringFormatFlags.LineLimit;

			if (alignment <= ContentAlignment.MiddleCenter){
				switch (alignment){
						case ContentAlignment.TopLeft:{
							format.Alignment = StringAlignment.Near;
							format.LineAlignment = StringAlignment.Near;
							return format;
						}
						case ContentAlignment.TopCenter:{
							format.Alignment = StringAlignment.Center;
							format.LineAlignment = StringAlignment.Near;
							return format;
						}
						case (ContentAlignment.TopCenter | ContentAlignment.TopLeft):{
							return format;
						}
						case ContentAlignment.TopRight:{
							format.Alignment = StringAlignment.Far;
							format.LineAlignment = StringAlignment.Near;
							return format;
						}
						case ContentAlignment.MiddleLeft:{
							format.Alignment = StringAlignment.Near;
							format.LineAlignment = StringAlignment.Center;
							return format;
						}
						case ContentAlignment.MiddleCenter:{
							format.Alignment = StringAlignment.Center;
							format.LineAlignment = StringAlignment.Center;
							return format;
						}
				}
				return format;
			}
			if (alignment <= ContentAlignment.BottomLeft){
				if (alignment == ContentAlignment.MiddleRight){
					format.Alignment = StringAlignment.Far;
					format.LineAlignment = StringAlignment.Center;
					return format;
				}
				if (alignment != ContentAlignment.BottomLeft){
					return format;
				}
			}
			else{
				if (alignment != ContentAlignment.BottomCenter){
					if (alignment == ContentAlignment.BottomRight)
					{
						format.Alignment = StringAlignment.Far;
						format.LineAlignment = StringAlignment.Far;
					}
					return format;
				}
				format.Alignment = StringAlignment.Center;
				format.LineAlignment = StringAlignment.Far;
				return format;
			}
			format.Alignment = StringAlignment.Near;
			format.LineAlignment = StringAlignment.Far;
			return format;
		}
	}
}
