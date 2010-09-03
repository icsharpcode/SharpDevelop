// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media.Imaging;

namespace ICSharpCode.CodeQualityAnalysis.Controls
{
	/// <summary>
	/// Description of DependencyVertexConverter.
	/// </summary>
	[ValueConversion(typeof(object), typeof(BitmapSource))]
	public class DependencyIconVertexConverter : IValueConverter
	{
		public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
		{
			var vertex = value as DependencyVertex;
			if (value != null)
				return vertex.Node.Icon;
			else
				return null;
		}
		
		public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}
	}
}
