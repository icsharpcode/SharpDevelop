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
