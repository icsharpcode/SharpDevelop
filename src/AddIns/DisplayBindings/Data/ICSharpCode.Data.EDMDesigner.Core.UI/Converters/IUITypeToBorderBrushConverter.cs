// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Type;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.Converters
{
    public class IUITypeToBorderBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var uiEntityType = value as UIEntityType;
            if (uiEntityType != null)
            {
                if (uiEntityType.BusinessInstance.Abstract)
                    return new DrawingBrush(new GeometryDrawing(Brushes.Blue, null, Geometry.Parse("M0,0 0,1 1,1 1,0Z M 1,1 2,1 2,2 1,2Z"))) { ViewportUnits = BrushMappingMode.Absolute, Viewport = new System.Windows.Rect(0, 0, 4, 4), TileMode = TileMode.FlipXY };
                return Brushes.Olive;
            }
            else if (value is UIComplexType)
                return Brushes.DarkOliveGreen;
            throw new NotImplementedException();
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
