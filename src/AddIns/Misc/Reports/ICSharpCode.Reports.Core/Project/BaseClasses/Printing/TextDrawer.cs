// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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

namespace ICSharpCode.Reports.Core.BaseClasses.Printing
{
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
			
			if (decorator.RightToLeft ==System.Windows.Forms.RightToLeft.Yes) {
				stringFormat.FormatFlags = stringFormat.FormatFlags | StringFormatFlags.DirectionRightToLeft;
			}
			
			var formattedString = text;
			if (! String.IsNullOrEmpty(decorator.FormatString)) {
				formattedString = StandardFormatter.FormatOutput(text,decorator.FormatString,decorator.DataType,String.Empty);
			}
			
			graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
			
			graphics.DrawString (formattedString,decorator.Font,
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
