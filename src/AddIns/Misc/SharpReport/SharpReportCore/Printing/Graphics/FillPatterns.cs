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

namespace SharpReportCore {	
	/// <summary>
	/// Abstract BaseClass for all AbstractFillPatterns
	/// </summary>
	/// <remarks>
	/// 	created by - Forstmeier Peter
	/// 	created on - 12.10.2005 08:57:05
	/// </remarks>
	public abstract class AbstractFillPattern : object {
		
		Color color;
		Brush brush;
		
		protected AbstractFillPattern(Color color) {
			this.color = color;
			
		}
		
		
		protected Color Color {
			get {
				return color;
			}
			set {
				color = value;
			}
		}
		public Brush Brush {
			get {
				return brush;
			}
			set {
				brush = value;
			}
		}
		
	}
	
	/// <summary>
	/// Solid Fill Pattern
	/// </summary>
	public class SolidFillPattern : AbstractFillPattern {
		public SolidFillPattern (Color color) :base(color){
			base.Brush = new SolidBrush(color);
		}
	}
}
