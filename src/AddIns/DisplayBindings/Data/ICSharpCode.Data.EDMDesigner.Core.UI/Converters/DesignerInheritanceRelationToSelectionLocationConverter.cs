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
    public class DesignerInheritanceRelationToSelectionLocationConverter : IValueConverter
    {
        private const double DEC = 2;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var relation = (InheritanceRelation)value;
            switch (parameter.ToString())
            {
                case "1":
                    return new Thickness(relation.X1 - DEC, relation.Y1 - DEC, 0, 0);
                case "2":
                    return new Thickness(relation.X3Arrow - DEC, relation.Y3Arrow - DEC, 0, 0);
            }
            throw new InvalidOperationException();
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
