// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
