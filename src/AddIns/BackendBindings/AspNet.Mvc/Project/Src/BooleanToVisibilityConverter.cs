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
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ICSharpCode.AspNet.Mvc
{
	public class BooleanToVisibilityConverter : IValueConverter
	{
		public bool Hidden { get; set; }
		public bool IsReversed { get; set; }
		
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool visible = IsVisible((bool)value);
			if (visible) {
				return Visibility.Visible;
			} else if (Hidden) {
				return Visibility.Hidden;
			}
			return Visibility.Collapsed;
		}
		
		bool IsVisible(bool value)
		{
			if (IsReversed) {
				return !value;
			}
			return value;
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return IsVisible((Visibility)value);
		}
		
		bool IsVisible(Visibility visibility)
		{
			bool visible = visibility == Visibility.Visible;
			return IsVisible(visible);
		}
	}
}
