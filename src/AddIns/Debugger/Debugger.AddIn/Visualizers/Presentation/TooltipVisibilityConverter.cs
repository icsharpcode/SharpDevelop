// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

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
