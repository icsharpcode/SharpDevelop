// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Shapes;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Relations;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.Converters
{
    public class AssociationToAlignmentConverter : IValueConverter
    {
        private const double MARGIN = 10;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var association = (Association)value;
            string stringParameter = (string)parameter;
            if (stringParameter.Length != 2)
                throw new InvalidOperationException();
            switch (stringParameter[0])
            {
                case 'X':
                    switch (Compare(association.X1, association.X2))
                    {
                        case -1:
                            if (association.FourPoints)
                                return HorizontalAlignment.Right;
                            switch (stringParameter[1])
                            {
                                case '1':
                                    return HorizontalAlignment.Right;
                                case '2':
                                    return HorizontalAlignment.Left;
                                default:
                                    throw new InvalidCastException();
                            }
                        case 0:
                            return HorizontalAlignment.Left;
                        case 1:
                            if (association.FourPoints)
                                return HorizontalAlignment.Left;
                            switch (stringParameter[1])
                            {
                                case '1':
                                    return HorizontalAlignment.Left;
                                case '2':
                                    //if (association.FourPoints)
                                    //    return HorizontalAlignment.Left;
                                    //else
                                    return HorizontalAlignment.Right;
                                default:
                                    throw new InvalidOperationException();
                            }
                    }
                    break;
                //case 'Y':
                //    switch (Compare(relationContener.Y1, relationContener.Y2))
                //    {
                //        case -1:
                //            switch (stringParameter[1])
                //            {
                //                case '1':
                //                    return VerticalAlignment.Bottom;
                //                case '2':
                //                    if (relationContener.FourPoints)
                //                        return FourPointsVerticalAlignement(relationContener);
                //                    else
                //                        return VerticalAlignment.Top;
                //                default:
                //                    throw new InvalidOperationException();
                //            }
                //        case 0:
                //            return VerticalAlignment.Top;
                //        case 1:
                //            switch (stringParameter[1])
                //            {
                //                case '1':
                //                    return VerticalAlignment.Top;
                //                case '2':
                //                    if (relationContener.FourPoints)
                //                        return FourPointsVerticalAlignement(relationContener);
                //                    else
                //                        return VerticalAlignment.Bottom;
                //                default:
                //                    throw new InvalidOperationException();
                //            }
                //    }
                //    return null;
            }
            throw new NotImplementedException();
        }

        private static HorizontalAlignment FourPointsHorizontalAlignement(Association association)
        {
            switch (Compare(association.X4, association.X3))
            {
                case -1:
                    return HorizontalAlignment.Right;
                case 0:
                case 1:
                    return HorizontalAlignment.Left;
                default:
                    throw new NotImplementedException();
            }
        }
        private static VerticalAlignment FourPointsVerticalAlignement(Association association)
        {
            switch (Compare(association.Y4, association.Y3))
            {
                case -1:
                    return VerticalAlignment.Bottom;
                case 0:
                case 1:
                    return VerticalAlignment.Top;
                default:
                    throw new NotImplementedException();
            }
        }

        private static int Compare(double v1, double v2)
        {
            if (v1 > v2 + MARGIN)
                return -1;
            if (v2 > v1 + MARGIN)
                return 1;
            return 0;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
