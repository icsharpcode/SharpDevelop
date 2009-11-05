// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;

namespace ICSharpCode.AvalonEdit.Utils
{
	static class PixelSnapHelpers
	{
		public static Size GetPixelSize()
		{
			return new Size(1, 1);
		}
		
		/// <summary>
		/// Aligns val on the next middle of a pixel.
		/// </summary>
		public static double PixelAlign(double val)
		{
			// 0 -> 0.5
			// 0.1 -> 0.5
			// 0.5 -> 0.5
			// 0.9 -> 0.5
			// 1 -> 1.5
			return Math.Round(val + 0.5) - 0.5;
		}
		
		/// <summary>
		/// Aligns the borders of rect on the middles of pixels.
		/// </summary>
		public static Rect PixelAlign(Rect rect)
		{
			rect.X = PixelAlign(rect.X);
			rect.Y = PixelAlign(rect.Y);
			rect.Width = Round(rect.Width);
			rect.Height = Round(rect.Height);
			return rect;
		}
		
		public static Point Round(Point val)
		{
			return new Point(Round(val.X), Round(val.Y));
		}
		
		/// <summary>
		/// Rounds val to a whole number of pixels.
		/// </summary>
		public static double Round(double val)
		{
			return Math.Round(val);
		}
		
		/// <summary>
		/// Rounds val to an even number of pixels.
		/// </summary>
		public static double RoundToEven(double val)
		{
			// 0 -> 0
			// 1 -> 2
			// 2 -> 2
			// 3 -> 4
			return Math.Round(val / 2) * 2;
		}
		
		/// <summary>
		/// Rounds val to an odd number of pixels.
		/// </summary>
		public static double RoundToOdd(double val)
		{
			return RoundToEven(val - 1) + 1;
		}
	}
}
