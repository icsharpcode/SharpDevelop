// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
