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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Debugger.AddIn.Visualizers
{
	/// <summary>
	/// Used for TextBlock in GridView, to determine TextBlock's tooltip visibility.
	/// </summary>
	public class TooltipVisibilityConverter : IValueConverter
	{
		/// <summary>
		/// Show tooltip if text is too long to fit in the column
		/// </summary>
		/// <param name="value">TextBlock</param>
		/// <param name="targetType"></param>
		/// <param name="parameter">In xaml, specify eg. ConverterParameter=2 to specify column index of column containing the TextBlock</param>
		/// <param name="culture"></param>
		/// <returns>Visibility for Tooltip</returns>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (parameter == null)
				throw new ArgumentNullException("Please specify column index as converter parameter");

			if (value == null)
				return Visibility.Hidden;
			TextBlock textBlock = (TextBlock)value;
			ContentPresenter contentPresenter = (ContentPresenter)VisualTreeHelper.GetParent(textBlock);
			GridViewRowPresenter rowPresenter = (GridViewRowPresenter)contentPresenter.Parent;

			int columnIndex = int.Parse((string)parameter);

			var columnWidth = rowPresenter.Columns[columnIndex].ActualWidth;

			return textBlock.ActualWidth > columnWidth ? Visibility.Visible : Visibility.Hidden;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
