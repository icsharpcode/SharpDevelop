// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Drawing;
using PdfSharp.Drawing;

namespace ICSharpCode.Reporting.Pdf
{
	/// <summary>
	/// Description of PdfExtensions.
	/// </summary>
	public static class PdfExtensions
	{
		
		public static XSize ToXSize(this Size size) {
			return new XSize(Convert(size.Width), Convert(size.Height));               
		}
		
		
		public static XPoint ToXPoints(this Point point) {
			var p = new XPoint(Convert(point.X),Convert(point.Y));
			return p;
		}
		
		
		public static XRect ToXRect( this Rectangle rectangle) {
			return new XRect(rectangle.Location.ToXPoints(),rectangle.Size.ToXSize());
		}
		
		
		public static float ToPoint (this int value) {
			return Convert(value);
		}
		
		
		static float Convert(int toConvert) {
			return toConvert * 72 / 100;
		}
	}
}
