// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// This class adds the AutoWidth property to the WPF ListView.
	/// It supports a semi-colon-separated list of values, for each defined cell.
	/// Each value can either be a fixed size double, or a percentage.
	/// The sizes of columns with a percentage will be calculated from the
	/// remaining width (after assigning the fixed sizes).
	/// Examples: 50%;25%;25% or 30;100%;50
	/// </summary>
	public class GridViewColumnAutoSize
	{
		public static readonly DependencyProperty AutoWidthProperty =
			DependencyProperty.RegisterAttached("AutoWidth", typeof(string), typeof(GridViewColumnAutoSize),
			                                    new FrameworkPropertyMetadata(null, AutoWidthPropertyChanged));
		
		public static string GetAutoWidth(DependencyObject obj)
		{
			return (string)obj.GetValue(AutoWidthProperty);
		}
		
		public static void SetAutoWidth(DependencyObject obj, string value)
		{
			obj.SetValue(AutoWidthProperty, value);
		}
		
		static void AutoWidthPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			ListView grid = sender as ListView;
			if (grid == null) return;
			grid.SizeChanged += delegate(object listView, SizeChangedEventArgs e) {
				ListView lv = listView as ListView;
				if (lv == null) return;
				GridView v = lv.View as GridView;
				if (v == null) return;
				CalculateSizes(v, GetAutoWidth(lv), e.NewSize.Width);
			};
			GridView view = grid.View as GridView;
			if (view == null) return;
			CalculateSizes(view, args.NewValue as string, grid.ActualWidth);
		}
		
		static void CalculateSizes(GridView view, string sizeValue, double fullWidth)
		{
			string[] sizes = (sizeValue ?? "").Split(';');
			
			Debug.Assert(sizes.Length == view.Columns.Count);
			Dictionary<int, Func<double, double>> percentages = new Dictionary<int, Func<double, double>>();
			double remainingWidth = fullWidth - 30; // 30 is a good offset for the scrollbar
			
			for (int i = 0; i < view.Columns.Count; i++) {
				var column = view.Columns[i];
				double size;
				bool isPercentage = !double.TryParse(sizes[i], out size);
				if (isPercentage) {
					size = double.Parse(sizes[i].TrimEnd('%', ' '));
					percentages.Add(i, w => w * size / 100.0);
				} else {
					column.Width = size;
					remainingWidth -= size;
				}
			}
			
			if (remainingWidth < 0) return;
			foreach (var p in percentages) {
				var column = view.Columns[p.Key];
				column.Width = p.Value(remainingWidth);
			}
		}
	}
}
