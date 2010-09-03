// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
