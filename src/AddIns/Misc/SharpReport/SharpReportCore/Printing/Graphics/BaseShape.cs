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
/// This class act's as a baseClass for all Shapes
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 09.10.2005 18:09:35
/// </remarks>
namespace SharpReportCore {	
	
	public abstract class BaseShape : object {
		
		public void FillShape (Graphics graphics, Brush brush,RectangleF rectangle) {
			
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			GraphicsPath path1 = this.CreatePath(rectangle);
			graphics.FillPath(brush, path1);

		}
		public void FillShape (Graphics graphics,AbstractFillPattern fillPattern,RectangleF rectangle) {
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			if (fillPattern != null){
				using (Brush brush = fillPattern.CreateBrush(rectangle)){
					if (brush != null){
						this.FillShape(graphics, brush, rectangle);
					}
				}
			}
		}
		
		public abstract GraphicsPath CreatePath (RectangleF rectangle) ;
		
		
		
		public void DrawShape(Graphics graphics, BaseLine line, RectangleF rectangle){
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			if (line == null) {
				throw new ArgumentNullException("line");
			}
			using (Pen pen = line.CreatePen()){
				if (pen != null){
					this.new_DrawShape(graphics, pen, rectangle);
				}
			}
		}
	
		public void new_DrawShape(Graphics graphics, Pen pen, RectangleF rectangle){
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			GraphicsPath path1 = this.CreatePath(rectangle);
			graphics.DrawPath(pen, path1);
		}


	}
}
