// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Relations;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.Converters
{
    public class DesignerAssociationToSelectionLocationConverter : IValueConverter
    {
        private const double DEC = 2;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var association = (Association)value;
            var point = association.FourPoints ? association.XY[4] : association.XY[2];
            switch (parameter.ToString())
            {
                case "1":
                    return new Thickness(association.X1 - DEC, association.Y1 - DEC, 0, 0);
                case "2":
                    return new Thickness(point[0] - DEC, point[1] - DEC, 0, 0);
            }
            throw new InvalidOperationException();
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
