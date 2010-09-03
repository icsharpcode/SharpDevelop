// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Type;
using System.Windows;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.Converters
{
    public class IUITypeToBackgroundConverter : IValueConverter
    {
        #region Fields

        private static LinearGradientBrush _entityTypeBrush = null;
        private static LinearGradientBrush _complexTypeBrush = null;

        #endregion

        static IUITypeToBackgroundConverter() 
        {
            _entityTypeBrush = new LinearGradientBrush();
            _entityTypeBrush.StartPoint = new Point(0, 1);
            _entityTypeBrush.EndPoint = new Point(1, 1);
            _entityTypeBrush.GradientStops.Add(new GradientStop(Colors.DarkSeaGreen, 0.0));
            _entityTypeBrush.GradientStops.Add(new GradientStop(Colors.Honeydew, 0.75));

            _complexTypeBrush = new LinearGradientBrush();
            _complexTypeBrush.StartPoint = new Point(0, 1);
            _complexTypeBrush.EndPoint = new Point(1, 1);
            _complexTypeBrush.GradientStops.Add(new GradientStop(Colors.DarkOliveGreen, 0.0));
            _complexTypeBrush.GradientStops.Add(new GradientStop(Colors.Honeydew, 0.75));
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is UIEntityType)
                return _entityTypeBrush;
            else if (value is UIComplexType)
                return _complexTypeBrush;
            throw new NotImplementedException();
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
