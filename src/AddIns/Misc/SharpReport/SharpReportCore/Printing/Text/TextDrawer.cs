
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
		
		
		public void DrawString(Graphics g,string text,Font font,Brush brush,RectangleF rectangle,StringFormat stringFormat) {
			g.DrawString(text,
			             font,
			             brush,
			             rectangle,
			             stringFormat);	
		}
	}
}
