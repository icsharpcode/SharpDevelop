using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ICSharpCode.ShortcutsManagement
{
	/// <summary>
	/// TreeView extention methods
	/// </summary>
    public static class TreeViewExtensions
    {
    	/// <summary>
    	/// Expand TreeView items according to provided path and select element 
    	/// on the lowest level 
    	/// </summary>
    	/// <param name="parentContainer">TreeView or TreeViewItem</param>
    	/// <param name="path">Path to the selected item</param>
        public static void SelectItem(this ItemsControl parentContainer, List<object> path)
        {
            var head = path.First();
            var tail = path.GetRange(1, path.Count - 1);
            var itemContainer = parentContainer.ItemContainerGenerator.ContainerFromItem(head) as TreeViewItem;

            if (itemContainer != null && itemContainer.Items.Count == 0)
            {
                itemContainer.IsSelected = true;

                var selectMethod = typeof(TreeViewItem).GetMethod("Select", BindingFlags.NonPublic | BindingFlags.Instance);
                selectMethod.Invoke(itemContainer, new object[] { true });
            }
            else if (itemContainer != null)
            {
                itemContainer.IsExpanded = true;

                if (itemContainer.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                {
                    itemContainer.ItemContainerGenerator.StatusChanged += delegate
                    {
                        SelectItem(itemContainer, tail);
                    };
                }
                else
                {
                    SelectItem(itemContainer, tail);
                }
            }
        }

        /// <summary>
        /// Expand/Collapse all tree view items
        /// </summary>
        /// <param name="parentContainer">TreeView or TreeViewItem</param>
        /// <param name="value">True - expand, False - collapse</param>
        public static void SetExpandAll(this ItemsControl parentContainer, bool value)
        {
            foreach (Object item in parentContainer.Items)
            {
                var currentContainer = parentContainer.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                if (currentContainer != null && currentContainer.Items.Count > 0)
                {
                    currentContainer.IsExpanded = value;
                    if (currentContainer.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                    {
                        currentContainer.ItemContainerGenerator.StatusChanged += delegate
                        {
                            SetExpandAll(currentContainer, value);
                        };
                    }
                    else
                    {
                        SetExpandAll(currentContainer, value);
                    }
                }
            }
        }
    }
}
