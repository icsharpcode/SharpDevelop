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
		static double PixelAlign(double val)
		{
			return Math.Round(val + 0.5) - 0.5;
		}
		
		public static Rect ToPixels(Rect rect)
		{
			rect.X = PixelAlign(rect.X);
			rect.Y = PixelAlign(rect.Y);
			rect.Width = Math.Round(rect.Width);
			rect.Height = Math.Round(rect.Height);
			return rect;
		}
	}
}
