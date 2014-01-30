// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace ICSharpCode.SharpDevelop.Widgets
{
	public static class ColorHelper
	{
		public static Color ColorFromString(string s)
		{
			if (string.IsNullOrEmpty(s)) {
				return Colors.White;
			}
			if (s[0] != '#') s = "#" + s;
			try {
				return (Color)ColorConverter.ConvertFromString(s);
			}
			catch {
				return Colors.White;
			}
		}

		public static string StringFromColor(Color c)
		{
			return c.ToString().Substring(1);
		}

		public static Color ColorFromHsv(double h, double s, double v)
		{
			double r, g, b;
			RgbFromHsv(h, s, v, out r, out g, out b);
			return Color.FromRgb((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));

		}

		public static void HsvFromColor(Color c, out double h, out double s, out double v)
		{
			HsvFromRgb(c.R / 255.0, c.G / 255.0, c.B / 255.0, out h, out s, out v);
		}

		// http://en.wikipedia.org/wiki/HSV_color_space
		public static void HsvFromRgb(double r, double g, double b, out double h, out double s, out double v)
		{
			var max = Math.Max(r, Math.Max(g, b));
			var min = Math.Min(r, Math.Min(g, b));

			if (max == min) {
				h = 0;
			}
			else if (max == r) {
				h = (60 * (g - b) / (max - min)) % 360;
			}
			else if (max == g) {
				h = 60 * (b - r) / (max - min) + 120;
			}
			else {
				h = 60 * (r - g) / (max - min) + 240;
			}
			
			if (h < 0) h += 360; // C# '%' can return negative values, use real modulus instead

			if (max == 0) {
				s = 0;
			}
			else {
				s = 1 - min / max;
			}

			v = max;
		}

		// http://en.wikipedia.org/wiki/HSV_color_space
		public static void RgbFromHsv(double h, double s, double v, out double r, out double g, out double b)
		{
			h = h % 360;
			if (h < 0) h += 360; // C# '%' can return negative values, use real modulus instead
			int hi = (int)(h / 60) % 6;
			var f = h / 60 - (int)(h / 60);
			var p = v * (1 - s);
			var q = v * (1 - f * s);
			var t = v * (1 - (1 - f) * s);

			switch (hi) {
				case 0: r = v; g = t; b = p; break;
				case 1: r = q; g = v; b = p; break;
				case 2: r = p; g = v; b = t; break;
				case 3: r = p; g = q; b = v; break;
				case 4: r = t; g = p; b = v; break;
				default: r = v; g = p; b = q; break;
			}
		}
	}
}
