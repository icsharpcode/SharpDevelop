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
		
		
		public override GraphicsPath CreatePath(RectangleF rect){
			GraphicsPath path1 = new GraphicsPath();
			path1.AddRectangle(rect);
			return path1;
		}


	}
}
