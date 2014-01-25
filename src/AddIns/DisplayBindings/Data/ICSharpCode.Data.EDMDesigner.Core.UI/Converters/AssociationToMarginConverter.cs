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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Relations;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.Converters
{
    public class AssociationToMarginConverter : IValueConverter
    {
        private const double MARGIN_HORIZONTAL = 5;
        private const double MARGIN_TOP = -2;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var association = (Association)value;
            double left, top, right; //, bottom;
            double x1, x2, y1, y2;
            TextBlock tb;
            switch ((string)parameter)
            {
                case "From":
                    tb = association.tb1;
                    x1 = association.X1;
                    x2 = association.X2;
                    y1 = association.Y1;
                    y2 = association.Y2;
                    break;
                case "To":
                    tb = association.tb2;
                    if (association.FourPoints)
                    {
                        x1 = association.X4;
                        x2 = association.X3;
                        y1 = association.Y4;
                        y2 = association.Y3;
                    }
                    else
                    {
                        x1 = association.X2;
                        x2 = association.X1;
                        y1 = association.Y2;
                        y2 = association.Y1;
                    }
                    break;
                default:
                    throw new InvalidOperationException();
            }
            switch (tb.HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    left = x1 + MARGIN_HORIZONTAL;
                    right = 0;
                    break;
                case HorizontalAlignment.Right:
                    if (association.FourPoints)
                        right = Math.Max(association.X1, association.X4) - x1 + MARGIN_HORIZONTAL;
                    else
                        right = x2 + MARGIN_HORIZONTAL;
                    left = 0;
                    break;
                default:
                    left = 0;
                    right = 0;
                    break;
            }
            switch (tb.VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    top = y1 + MARGIN_TOP;
                    //bottom = 0;
                    break;
                case VerticalAlignment.Bottom:
                    top = y2;
                    //bottom = 0;
                    break;
                default:
                    top = 0;
                    //bottom = 0;
                    break;
            }
            return new Thickness(left, top, right, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
