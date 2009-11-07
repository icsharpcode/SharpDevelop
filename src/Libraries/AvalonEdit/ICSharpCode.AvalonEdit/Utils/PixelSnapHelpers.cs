// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Media;

namespace ICSharpCode.AvalonEdit.Utils
{
	/// <summary>
	/// Contains static helper methods for aligning stuff on a whole number of
	/// </summary>
	public static class PixelSnapHelpers
	{
		/// <summary>
		/// Gets the pixel size on the screen containing visual.
		/// This method does not take transforms on visual into account.
		/// </summary>
		public static Size GetPixelSize(Visual visual)
		{
			Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
			return new Size(matrix.M11, matrix.M22);
		}
		
		/// <summary>
		/// Aligns val on the next middle of a pixel.
		/// </summary>
		/// <param name="val">The value that should be aligned</param>
		/// <param name="pixelSize">The size of one pixel</param>
		public static double PixelAlign(double val, double pixelSize)
		{
			// 0 -> 0.5
			// 0.1 -> 0.5
			// 0.5 -> 0.5
			// 0.9 -> 0.5
			// 1 -> 1.5
			return pixelSize * (Math.Round((val / pixelSize) + 0.5) - 0.5);
		}
		
		/// <summary>
		/// Aligns the borders of rect on the middles of pixels.
		/// </summary>
		public static Rect PixelAlign(Rect rect, Size pixelSize)
		{
			rect.X = PixelAlign(rect.X, pixelSize.Width);
			rect.Y = PixelAlign(rect.Y, pixelSize.Height);
			rect.Width = Round(rect.Width, pixelSize.Width);
			rect.Height = Round(rect.Height, pixelSize.Height);
			return rect;
		}
		
		/// <summary>
		/// Rounds val to whole number of pixels.
		/// </summary>
		public static Point Round(Point val, Size pixelSize)
		{
			return new Point(Round(val.X, pixelSize.Width), Round(val.Y, pixelSize.Height));
		}
		
		/// <summary>
		/// Rounds val to whole number of pixels.
		/// </summary>
		public static Rect Round(Rect rect, Size pixelSize)
		{
			return new Rect(Round(rect.X, pixelSize.Width), Round(rect.Y, pixelSize.Height),
			                Round(rect.Width, pixelSize.Width), Round(rect.Height, pixelSize.Height));
		}
		
		/// <summary>
		/// Rounds val to a whole number of pixels.
		/// </summary>
		public static double Round(double val, double pixelSize)
		{
			return pixelSize * Math.Round(val / pixelSize);
		}
		
		/// <summary>
		/// Rounds val to an whole odd number of pixels.
		/// </summary>
		public static double RoundToOdd(double val, double pixelSize)
		{
			return Round(val - pixelSize, pixelSize * 2) + pixelSize;
		}
	}
}
