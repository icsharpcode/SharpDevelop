// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Debugger.AddIn.Visualizers
{
	/// <summary>
	/// Used for TextBlocks in PositionedGraphNodeControl, to make property name bold when the property is expanded. 
	/// </summary>
	public class BoolToBoldConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (!(value is bool))
				return FontWeights.Normal;
			
			return (bool)value ? FontWeights.Bold : FontWeights.Normal;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
