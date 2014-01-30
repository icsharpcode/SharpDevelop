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
using System.Windows.Controls;
using System.Windows.Media;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Scrolling related helpers.
	/// </summary>
	public static class ScrollUtils
	{
		/// <summary>
		/// Searches VisualTree of given object for a ScrollViewer.
		/// </summary>
		public static ScrollViewer GetScrollViewer(this DependencyObject o)
		{
			var scrollViewer = o as ScrollViewer;
			if (scrollViewer != null)	{
				return scrollViewer;
			}

			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
			{
				var child = VisualTreeHelper.GetChild(o, i);
				var result = GetScrollViewer(child);
				if (result != null) {
					return result;
				}
			}
			return null;
		}
		
		/// <summary>
		/// Scrolls ScrollViewer up by given offset.
		/// </summary>
		/// <param name="offset">Offset by which to scroll up. Should be positive.</param>
		public static void ScrollUp(this ScrollViewer scrollViewer, double offset)
		{
			ScrollUtils.ScrollByVerticalOffset(scrollViewer, -offset);
		}
		
		/// <summary>
		/// Scrolls ScrollViewer down by given offset.
		/// </summary>
		/// <param name="offset">Offset by which to scroll down. Should be positive.</param>
		public static void ScrollDown(this ScrollViewer scrollViewer, double offset)
		{
			ScrollUtils.ScrollByVerticalOffset(scrollViewer, offset);
		}
		
		/// <summary>
		/// Scrolls ScrollViewer by given vertical offset.
		/// </summary>
		public static void ScrollByVerticalOffset(this ScrollViewer scrollViewer, double offset)
		{
			scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + offset);
		}
		
		public static void SynchronizeScroll(this ScrollViewer target, ScrollViewer source, ScrollSyncOption option, bool proportional = true)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			if (target == null)
				throw new ArgumentNullException("target");
			double newScrollOffset;
			switch (option) {
				case ScrollSyncOption.Vertical:
					if (proportional)
						newScrollOffset = source.VerticalOffset / source.ScrollableHeight * target.ScrollableHeight;
					else
						newScrollOffset = source.VerticalOffset;
					target.ScrollToVerticalOffset(double.IsNaN(newScrollOffset) ? 0 : newScrollOffset);
					break;
				case ScrollSyncOption.Horizontal:
					if (proportional)
						newScrollOffset = source.HorizontalOffset / source.ScrollableWidth * target.ScrollableWidth;
					else
						newScrollOffset = source.HorizontalOffset;
					target.ScrollToHorizontalOffset(double.IsNaN(newScrollOffset) ? 0 : newScrollOffset);
					break;
				case ScrollSyncOption.VerticalToHorizontal:
					if (proportional)
						newScrollOffset = source.VerticalOffset / source.ScrollableHeight * target.ScrollableWidth;
					else
						newScrollOffset = source.VerticalOffset;
					target.ScrollToHorizontalOffset(double.IsNaN(newScrollOffset) ? 0 : newScrollOffset);
					break;
				case ScrollSyncOption.HorizontalToVertical:
					if (proportional)
						newScrollOffset = source.HorizontalOffset / source.ScrollableWidth * target.ScrollableHeight;
					else
						newScrollOffset = source.HorizontalOffset;
					target.ScrollToVerticalOffset(double.IsNaN(newScrollOffset) ? 0 : newScrollOffset);
					break;
				case ScrollSyncOption.Both:
					if (proportional)
						newScrollOffset = source.VerticalOffset / source.ScrollableHeight * target.ScrollableHeight;
					else
						newScrollOffset = source.VerticalOffset;
					target.ScrollToVerticalOffset(double.IsNaN(newScrollOffset) ? 0 : newScrollOffset);
					if (proportional)
						newScrollOffset = source.HorizontalOffset / source.ScrollableWidth * target.ScrollableWidth;
					else
						newScrollOffset = source.HorizontalOffset;
					target.ScrollToHorizontalOffset(double.IsNaN(newScrollOffset) ? 0 : newScrollOffset);
					break;
				case ScrollSyncOption.BothInterchanged:
					if (proportional)
						newScrollOffset = source.VerticalOffset / source.ScrollableHeight * target.ScrollableWidth;
					else
						newScrollOffset = source.VerticalOffset;
					target.ScrollToHorizontalOffset(double.IsNaN(newScrollOffset) ? 0 : newScrollOffset);
					if (proportional)
						newScrollOffset = source.HorizontalOffset / source.ScrollableWidth * target.ScrollableHeight;
					else
						newScrollOffset = source.HorizontalOffset;
					target.ScrollToVerticalOffset(double.IsNaN(newScrollOffset) ? 0 : newScrollOffset);
					break;
				default:
					throw new Exception("Invalid value for ScrollSyncOption");
			}
		}
		
	}
	
	public enum ScrollSyncOption
	{
		Vertical,
		Horizontal,
		VerticalToHorizontal,
		HorizontalToVertical,
		Both,
		BothInterchanged
	}
}
