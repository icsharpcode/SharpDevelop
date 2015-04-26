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
using System.Windows;

namespace ICSharpCode.Reporting.BaseClasses
{
	/// <summary>
	/// Description of TextDrawer.
	/// </summary>
	public static class TextDrawer 
	{
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
	
//	http://stackoverflow.com/questions/14932063/convert-code-of-a-user-control-from-vb-net-to-c-sharp
//https://mightycoco.wordpress.com/2009/09/22/getstringformatfromcontentallignment-converting-contentallignment-data-into-stringformat-data/		
		public static StringFormat BuildStringFormat(StringTrimming stringTrimming,TextAlignment alignment){
			StringFormat format = StringFormat.GenericTypographic;
			format.Trimming = stringTrimming;
			format.FormatFlags = StringFormatFlags.LineLimit;
			switch (alignment) {
					case TextAlignment.Left:{
						format.Alignment = StringAlignment.Near;
						format.LineAlignment = StringAlignment.Near;
						return format;
					}
					case TextAlignment.Center:{
						format.Alignment = StringAlignment.Center;
						format.LineAlignment = StringAlignment.Near;
						return format;
					}
					
					case TextAlignment.Right:{
						format.Alignment = StringAlignment.Far;
						format.LineAlignment = StringAlignment.Near;
						return format;
					}
					
					case TextAlignment.Justify:{
						format.Alignment = StringAlignment.Center;
						format.LineAlignment = StringAlignment.Near;
						return format;
					}
			}
			return format;
		}
	
		
		/*
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
		*/
	}
}
