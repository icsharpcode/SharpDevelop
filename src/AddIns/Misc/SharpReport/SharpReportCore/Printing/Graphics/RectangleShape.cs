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
/// Draw a Rectangle, used by DesingerControls and printing stuff
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 09.10.2005 18:20:51
/// </remarks>
namespace SharpReportCore {
	public class RectangleShape : BaseShape {
		
		public RectangleShape() {
		}
		

		public override void DrawShape(Graphics graphics, BaseLine baseLine, RectangleF rectangle) {
			base.DrawShape(graphics,baseLine,rectangle);
			using (Pen p = new Pen(baseLine.Color,baseLine.Thickness)) {
				p.DashStyle = baseLine.DashStyle;
				graphics.DrawRectangle (p,rectangle.Left,
				                 rectangle.Top,
				                 rectangle.Width,
				                 rectangle.Height);
			}
		}
		
		public override void FillShape(Graphics graphics, AbstractFillPattern fillPattern, RectangleF rectangle) {
			graphics.FillRectangle(fillPattern.Brush,
			                rectangle);
		}
		
		public override void FillShape(Graphics graphics, Brush brush, RectangleF rectangle) {
			graphics.FillRectangle(brush, rectangle);
			       
		}
		
	}
}
