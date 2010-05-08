// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Drawing.Text;

/// <summary>
/// This Class is drawing Strings
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 12.10.2005 10:54:08
/// </remarks>

namespace ICSharpCode.Reports.Core
{
	public sealed class TextDrawer 
	{
		
		private TextDrawer() 
		{
		}
		
		public static void DrawString(Graphics graphics,string text,
		                       Font font,Brush brush,
		                       RectangleF rectangle,
		                       StringTrimming stringTrimming,
		                       ContentAlignment alignment) 
		{
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			StringFormat stringFormat = BuildStringFormat(stringTrimming,alignment);
			graphics.DrawString(text,
			                    font,
			                    brush,
			                    rectangle,
			                    stringFormat);
		}
		
		
		public static void DrawString (Graphics graphics,string text,
		                               ICSharpCode.Reports.Core.Exporter.TextStyleDecorator decorator)
		{
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			if (decorator == null) {
				throw new ArgumentNullException("decorator");
			}
			StringFormat stringFormat = BuildStringFormat(decorator.StringTrimming,decorator.ContentAlignment);
			graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
			graphics.DrawString (text,decorator.Font,
			                     new SolidBrush(decorator.ForeColor),
			                     new Rectangle(decorator.Location.X,
			                                   decorator.Location.Y,
			                                   decorator.Size.Width,
			                                   decorator.Size.Height),
			                     stringFormat);
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
