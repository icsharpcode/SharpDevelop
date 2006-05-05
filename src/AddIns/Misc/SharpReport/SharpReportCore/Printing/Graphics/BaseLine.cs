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
using System.Drawing.Drawing2D;	
/// <summary>
/// This class handle the basic's of drawing all kind of Lines,
/// and we can create a Pen from this properties
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 09.10.2005 18:37:51
/// </remarks>
namespace SharpReportCore {
	public class BaseLine : object {
		
		DashStyle dashStyle;
		Color color;
		float thickness;
		
		
		public BaseLine(Color color, DashStyle dashStyle, float thickness){
			this.color = color;
			this.dashStyle = dashStyle;
			this.thickness = thickness;
		}
		
		
		public Pen CreatePen(){
			return this.CreatePen(72f);
		}


		public Pen CreatePen(float resolution)
		{
			Pen pen;

			if (this.thickness == 0f)
			{
				pen = new Pen(this.color, resolution / 72f);
			}
			else
			{
				pen = new Pen(this.color, (this.thickness * resolution) / 72f);
			}
			
			switch (this.dashStyle){
				case DashStyle.Dot:
					{
						pen.DashStyle = DashStyle.Dot;
						return pen;
					}
				case DashStyle.Dash:
					{
						pen.DashStyle = DashStyle.Dash;
						return pen;
					}
				case DashStyle.DashDot:
					{
						pen.DashStyle = DashStyle.DashDot;
						return pen;
					}
				case DashStyle.DashDotDot:
					{
						pen.DashStyle = DashStyle.DashDotDot;
						return pen;
					}
			}
			return pen;
		}


		public Color Color {
			get {
				return color;
			}
			set {
				color = value;
			}
		}
		public DashStyle DashStyle {
			get {
				return dashStyle;
			}
			set {
				dashStyle = value;
			}
		}
		public float Thickness {
			get {
				return thickness;
			}
			set {
				thickness = value;
			}
		}
		
	}
}
