// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Windows;
using System.Windows.Data;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.Condition;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.Converters
{
    public class ConditionOperatorToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;
            var conditionOperator = (ConditionOperator)value;
            switch (conditionOperator)
            {
                case ConditionOperator.Equals:
                    return Visibility.Visible;
                case ConditionOperator.IsNull:
                case ConditionOperator.IsNotNull:
                    return Visibility.Collapsed;
                default:
                    throw new NotImplementedException();
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
