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
using System.Linq;
using System.Windows;
using System.Windows.Controls;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.ContextMenu
{
    public static class MenuContainerBase
    {
        internal static void AddChild(object child, Action onItemVisibleChanged)
        {
            var menuSeparator = child as MenuSeparator;
            if (menuSeparator != null)
            {
                menuSeparator.SetVisibilityBinding();
                return;
            }
            var item = child as FrameworkElement;
            if (item != null)
                item.IsVisibleChanged += delegate { onItemVisibleChanged(); };
        }

        public static MenuItem GetMenuItem(this IMenuContainer menuContainer, params string[] menuNames)
        {
            if (menuContainer == null)
                return null;
            var itemsControl = (ItemsControl)menuContainer;
            foreach (var menuName in menuNames)
            {
                itemsControl = itemsControl.Items.OfType<ItemsControl>().FirstOrDefault(mi => mi.Name == menuName);
                if (itemsControl == null)
                    return null;
            }
            return itemsControl as MenuItem;
        }

        public static Separator GetMenuSeparator(this IMenuContainer menuContainer, params string[] menuNames)
        {
            if (menuContainer == null)
                return null;
            ItemsControl itemsControl;
            if (menuNames.Length == 1)
                itemsControl = (ItemsControl)menuContainer;
            else
                itemsControl = menuContainer.GetMenuItem(menuNames.Take(menuNames.Length - 1).ToArray());
            return itemsControl.Items.OfType<Separator>().FirstOrDefault(s => s.Name == menuNames[menuNames.Length - 1]);
        }
    }
}
