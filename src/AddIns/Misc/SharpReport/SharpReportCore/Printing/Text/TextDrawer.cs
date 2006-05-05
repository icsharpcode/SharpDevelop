
//
// SharpDevelop ReportEditor
//
// Copyright (C) 2005 Peter Forstmeier
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Peter Forstmeier (Peter.Forstmeier@t-online.de)

using System;
using System.Drawing;
	
/// <summary>
/// This Class is drawing Strings
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 12.10.2005 10:54:08
/// </remarks>

namespace SharpReportCore {	
	public class TextDrawer : object {
		
		public TextDrawer() {
		}
		
		
		public void DrawString(Graphics graphics,string text,
		                       Font font,Brush brush,
		                       RectangleF rectangle,
		                       StringFormat stringFormat) {
			
			graphics.DrawString(text,
			             font,
			             brush,
			             rectangle,
			             stringFormat);	
		}
		
		
		public void DrawString(Graphics graphics,string text,
		                       Font font,Brush brush,
		                       RectangleF rectangle,
		                       StringTrimming stringTrimming,
		                       ContentAlignment alignment) {
			
			StringFormat s = BuildStringFormat(stringTrimming,alignment);
			this.DrawString(graphics,text,
			                font,brush,
			                rectangle,
			                s);
		}
		
		
		public StringFormat BuildStringFormat(StringTrimming stringTrimming,ContentAlignment alignment){
			StringFormat format = StringFormat.GenericTypographic;
			format.Trimming = stringTrimming;
			format.FormatFlags = StringFormatFlags.LineLimit;

//			if (base.RightToLeft)
//			{
//				format1.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
//			}
			
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
