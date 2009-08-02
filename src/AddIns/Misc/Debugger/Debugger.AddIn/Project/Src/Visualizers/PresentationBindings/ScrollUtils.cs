// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníèek" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Debugger.AddIn.Visualizers
{
	/// <summary>
	/// Description of ScrollUtils.
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
	}
}
