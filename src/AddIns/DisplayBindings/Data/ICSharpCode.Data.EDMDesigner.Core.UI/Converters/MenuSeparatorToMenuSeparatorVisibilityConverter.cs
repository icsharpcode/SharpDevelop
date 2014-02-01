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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.ContextMenu;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Common;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.Converters
{
    public class MenuSeparatorToMenuSeparatorVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var separator = (MenuSeparator)value;
            var items = VisualTreeHelperUtil.GetControlAscendant<ItemsControl>(separator).Items.OfType<FrameworkElement>();
            bool visibleBefore = false;
            bool visibleAfter = false;
            bool before = true;
            foreach (var item in items)
            {
                if (item is Separator)
                {
                    if (item == separator)
                        before = false;
                    else if (item.IsVisible)
                    {
                        if (before)
                            visibleBefore = false;
                        else
                            break;
                    }
                }
                else if (item.IsVisible)
                {
                    if (before)
                        visibleBefore = true;
                    else
                        visibleAfter = true;
                }
            }
            return visibleBefore && visibleAfter ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
