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
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using System.Collections;
using System.Windows.Media;

namespace ICSharpCode.WpfDesign.Designer.Converters
{
	public class IntFromEnumConverter : IValueConverter
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "converter is immutable")]
		public static readonly IntFromEnumConverter Instance = new IntFromEnumConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (int)value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Enum.ToObject(targetType, (int)value);
		}
	}

	public class HiddenWhenFalse : IValueConverter
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "converter is immutable")]
		public static readonly HiddenWhenFalse Instance = new HiddenWhenFalse();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (bool)value ? Visibility.Visible : Visibility.Hidden;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class CollapsedWhenFalse : IValueConverter
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "converter is immutable")]
		public static readonly CollapsedWhenFalse Instance = new CollapsedWhenFalse();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (bool)value ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class LevelConverter : IValueConverter
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "converter is immutable")]
		public static readonly LevelConverter Instance = new LevelConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return new Thickness(2 + 14 * (int)value, 0, 0, 0);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class CollapsedWhenZero : IValueConverter
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "converter is immutable")]
		public static readonly CollapsedWhenZero Instance = new CollapsedWhenZero();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || (value is int && (int)value == 0)) {
				return Visibility.Collapsed;
			}
			return Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class CollapsedWhenNotNull : IValueConverter
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "converter is immutable")]
		public static readonly CollapsedWhenNotNull Instance = new CollapsedWhenNotNull();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null)
			{
				return Visibility.Collapsed;
			}
			return Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class CollapsedWhenNull : IValueConverter
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "converter is immutable")]
		public static readonly CollapsedWhenNull Instance = new CollapsedWhenNull();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null)
			{
				return Visibility.Visible;
			}
			return Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class FalseWhenNull : IValueConverter
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "converter is immutable")]
		public static readonly FalseWhenNull Instance = new FalseWhenNull();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value != null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class BoldWhenTrue : IValueConverter
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "converter is immutable")]
		public static readonly BoldWhenTrue Instance = new BoldWhenTrue();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (bool)value ? FontWeights.Bold : FontWeights.Normal;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	// Boxed int throw exception without converter (wpf bug?)
	public class DummyConverter : IValueConverter
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "converter is immutable")]
		public static readonly DummyConverter Instance = new DummyConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}
	}

	public class ControlToRealWidthConverter : IMultiValueConverter
	{
		public static readonly ControlToRealWidthConverter Instance = new ControlToRealWidthConverter();

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			return PlacementOperation.GetRealElementSize((UIElement)values[0]).Width;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class ControlToRealHeightConverter : IMultiValueConverter
	{
		public static readonly ControlToRealHeightConverter Instance = new ControlToRealHeightConverter();

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			return PlacementOperation.GetRealElementSize((UIElement)values[0]).Height;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class FormatDoubleConverter : IValueConverter
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "converter is immutable")]
		public static readonly FormatDoubleConverter Instance=new FormatDoubleConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Math.Round((double)value);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class DoubleOffsetConverter : IValueConverter
	{
		public double Offset { get; set; }
		
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (double)value + Offset;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (double)value - Offset;
		}
	}

	public class BlackWhenTrue : IValueConverter
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "converter is immutable")]
		public static readonly BlackWhenTrue Instance = new BlackWhenTrue();

		private Brush black = new SolidColorBrush(Colors.Black);

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (bool)value ? black : null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	
	public class EnumBoolean : IValueConverter
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "converter is immutable")]
		public static readonly EnumBoolean Instance = new EnumBoolean();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string parameterString = parameter as string;
			if (parameterString == null)
				return DependencyProperty.UnsetValue;

			if (Enum.IsDefined(value.GetType(), value) == false)
				return DependencyProperty.UnsetValue;

			object parameterValue = Enum.Parse(value.GetType(), parameterString);

			return parameterValue.Equals(value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string parameterString = parameter as string;
			if (parameterString == null)
				return DependencyProperty.UnsetValue;

			return Enum.Parse(targetType, parameterString);
		}
	}
	
	public class EnumVisibility : IValueConverter
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "converter is immutable")]
		public static readonly EnumVisibility Instance = new EnumVisibility();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string parameterString = parameter as string;
			if (parameterString == null)
				return DependencyProperty.UnsetValue;

			if (Enum.IsDefined(value.GetType(), value) == false)
				return DependencyProperty.UnsetValue;

			object parameterValue = Enum.Parse(value.GetType(), parameterString);

			return parameterValue.Equals(value) ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string parameterString = parameter as string;
			if (parameterString == null)
				return DependencyProperty.UnsetValue;

			return Enum.Parse(targetType, parameterString);
		}
	}

	public class EnumCollapsed : IValueConverter
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "converter is immutable")]
		public static readonly EnumCollapsed Instance = new EnumCollapsed();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string parameterString = parameter as string;
			if (parameterString == null)
				return DependencyProperty.UnsetValue;

			if (Enum.IsDefined(value.GetType(), value) == false)
				return DependencyProperty.UnsetValue;

			object parameterValue = Enum.Parse(value.GetType(), parameterString);

			return parameterValue.Equals(value) ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string parameterString = parameter as string;
			if (parameterString == null)
				return DependencyProperty.UnsetValue;

			return Enum.Parse(targetType, parameterString);
		}
	}

	public class InvertedZoomConverter : IValueConverter
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "converter is immutable")]
		public static readonly InvertedZoomConverter Instance = new InvertedZoomConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return 1.0 / ((double)value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return 1.0 / ((double)value);
		}
	}
}
