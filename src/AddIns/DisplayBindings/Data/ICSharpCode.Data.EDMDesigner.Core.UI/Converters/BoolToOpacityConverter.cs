// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Globalization;
using System.Windows.Data;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.Converters
{
    public class BoolToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = (bool?)value;
            double parameterValue;
            if (parameter is string)
                parameterValue = double.Parse((string)parameter, CultureInfo.InvariantCulture);
            parameterValue = (double)parameter;
            if (boolValue.HasValue)
            {
                if (boolValue.Value)
                    return 1;
                else
                    return parameterValue;
            }
            else
                return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
