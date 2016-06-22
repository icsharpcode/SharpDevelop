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
using System.Windows;
using System.Windows.Media;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Items;

namespace ICSharpCode.Reporting.BaseClasses
{
	/// <summary>
	/// Description of ExtensionMethodes.
	/// Copy from D:\git_Sharpdevelop_Reporting\src\AddIns\Misc\Reports\ICSharpCode.Reports.Core\Project\WPF\ExtensionMethodes.cs
	/// </summary>
	/// <see cref="D:\git_Sharpdevelop_Reporting\src\AddIns\Misc\Reports\ICSharpCode.Reports.Core\Project\WPF\ExtensionMethodes.cs"
	
	public static class ExtensionMethodes
	{
		#region Collections 
		
		public static void ForEach<T>(this IEnumerable<T> input, Action<T> action)
		{
			if (input == null)
				throw new ArgumentNullException("input");
			foreach (T element in input) {
				action(element);
			}
		}
		
		#endregion
		
		#region system.drawing -> Wpf
		
		public static Point ToWpf(this System.Drawing.Point p)
		{
			return new Point(p.X, p.Y);
		}
		
		public static Size ToWpf(this System.Drawing.Size s)
		{
			return new Size(s.Width, s.Height);
		}
		
		public static Rect ToWpf(this System.Drawing.Rectangle rect)
		{
			return new Rect(rect.Location.ToWpf(), rect.Size.ToWpf());
		}
		
		public static Color ToWpf(this System.Drawing.Color c)
		{
			return Color.FromArgb(c.A, c.R, c.G, c.B);
		}
		
		
		public static int FromPoints(double value)
		{
			return (int)Math.Round(value * 72.0 / 96.0);
		}

		public static double ToPoints(int value)
		{
			return Math.Round(value * 96.0 / 72.0);
		}
		
		
		#endregion
		
		#region DPI independence
		public static Rect TransformToDevice(this Rect rect, Visual visual)
		{
			Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
			return Rect.Transform(rect, matrix);
		}
		
		public static Rect TransformFromDevice(this Rect rect, Visual visual)
		{
			Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
			return Rect.Transform(rect, matrix);
		}
		
		public static Size TransformToDevice(this Size size, Visual visual)
		{
			Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
			return new Size(size.Width * matrix.M11, size.Height * matrix.M22);
		}
		
		public static Size TransformFromDevice(this Size size, Visual visual)
		{
			Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
			return new Size(size.Width * matrix.M11, size.Height * matrix.M22);
		}
		
		public static Point TransformToDevice(this Point point, Visual visual)
		{
			Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
			return new Point(point.X * matrix.M11, point.Y * matrix.M22);
		}
		
		public static Point TransformFromDevice(this Point point, Visual visual)
		{
			Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
			return new Point(point.X * matrix.M11, point.Y * matrix.M22);
		}
		#endregion
		
		#region ReportSettings
		
		public static int PrintableWidth (this IReportSettings reportSettings) {
				return reportSettings.PageSize.Width - reportSettings.LeftMargin - reportSettings.RightMargin;
		}
		#endregion
	}
}
