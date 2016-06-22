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
using System.Windows;
using System.Windows.Controls.Primitives;

namespace ICSharpCode.SharpDevelop.Widgets
{
	/// <summary>
	/// UniformGrid that has spacing between columns/rows.
	/// </summary>
	public class UniformGridWithSpacing : UniformGrid
	{
		public static readonly DependencyProperty SpaceBetweenColumnsProperty =
			DependencyProperty.Register("SpaceBetweenColumns", typeof(double), typeof(UniformGridWithSpacing),
			                            new FrameworkPropertyMetadata(7.0, FrameworkPropertyMetadataOptions.AffectsMeasure));
		
		public double SpaceBetweenColumns {
			get { return (double)GetValue(SpaceBetweenColumnsProperty); }
			set { SetValue(SpaceBetweenColumnsProperty, value); }
		}
		
		public static readonly DependencyProperty SpaceBetweenRowsProperty =
			DependencyProperty.Register("SpaceBetweenRows", typeof(double), typeof(UniformGridWithSpacing),
			                            new FrameworkPropertyMetadata(5.0, FrameworkPropertyMetadataOptions.AffectsMeasure));
		
		public double SpaceBetweenRows {
			get { return (double)GetValue(SpaceBetweenRowsProperty); }
			set { SetValue(SpaceBetweenRowsProperty, value); }
		}
		
		protected override Size MeasureOverride(Size constraint)
		{
			Size s = base.MeasureOverride(constraint);
			return new Size(s.Width + Math.Max(0, this.Columns - 1) * this.SpaceBetweenColumns,
			                s.Height + Math.Max(0, this.Rows - 1) * this.SpaceBetweenRows);
		}
		
		protected override Size ArrangeOverride(Size arrangeSize)
		{
			double spaceBetweenColumns = this.SpaceBetweenColumns;
			double spaceBetweenRows = this.SpaceBetweenRows;
			int rows = Math.Max(1, this.Rows);
			int columns = Math.Max(1, this.Columns);
			Rect rect = new Rect(0, 0,
			                     (arrangeSize.Width - spaceBetweenColumns * (columns - 1)) / columns,
			                     (arrangeSize.Height - spaceBetweenRows * (rows - 1)) / rows);
			int currentColumn = this.FirstColumn;
			rect.X += currentColumn * (rect.Width + spaceBetweenColumns);
			foreach (UIElement element in this.InternalChildren) {
				element.Arrange(rect);
				if (element.Visibility != Visibility.Collapsed) {
					if (++currentColumn >= columns) {
						currentColumn = 0;
						rect.X = 0;
						rect.Y += rect.Height + spaceBetweenRows;
					} else {
						rect.X += rect.Width + spaceBetweenColumns;
					}
				}
			}
			return arrangeSize;
		}
	}
}
