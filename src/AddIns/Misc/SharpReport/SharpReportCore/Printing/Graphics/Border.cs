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
/// This Class drwas a Border around an ReportItem
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 12.10.2005 09:12:34
/// </remarks>

namespace SharpReportCore {
	public class Border : object {
		BaseLine baseLine;
		
		
		public Border() {
		}
		
		public Border(BaseLine baseLine)
		{
			this.baseLine = baseLine;
		}
		
		public void DrawBorder (Graphics g, RectangleF rectangle) {
			using (Pen p = new Pen(baseLine.Color,baseLine.Thickness)) {
				p.DashStyle = baseLine.DashStyle;
				g.DrawRectangle (p,rectangle.Left,
				                 rectangle.Top,
				                 rectangle.Width,
				                 rectangle.Height);
			}
		}
		
		public void DrawBorder (Graphics g, Rectangle rectangle ) {
			using (Pen p = new Pen(baseLine.Color,baseLine.Thickness)) {
				p.DashStyle = baseLine.DashStyle;
				g.DrawRectangle (p,rectangle);
			}
		}
	}
}
