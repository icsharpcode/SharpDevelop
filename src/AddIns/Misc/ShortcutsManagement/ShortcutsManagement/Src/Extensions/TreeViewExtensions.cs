using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ICSharpCode.ShortcutsManagement.Extensions
{
	/// <summary>
	/// <see cref="TreeView"/> extention methods
	/// </summary>
    public static class TreeViewExtensions
    {
    	/// <summary>
    	/// Expand TreeView items according to provided path and select element 
    	/// on the lowest level 
    	/// </summary>
    	/// <param name="parentContainer">TreeView instance</param>
    	/// <param name="path">Path to the selected item</param>
        /// <param name="setFocus"></param>
        public static void SelectItem(this ItemsControl parentContainer, List<object> path, bool setFocus)
        {
            if(path == null || path.Count == 0)
            {
                return;
            }

            var head = path.First();
            var tail = path.GetRange(1, path.Count - 1);

            // Get TreeViewItem which wraps first element from path
            var itemContainer = parentContainer.ItemContainerGenerator.ContainerFromItem(head) as TreeViewItem;

            if (itemContainer != null && itemContainer.Items.Count == 0) {
                // If item container doesn't have any sub-elements select it
                itemContainer.IsSelected = true;

                if(setFocus) {
                    Keyboard.Focus(itemContainer);
                }

                var selectMethod = typeof(TreeViewItem).GetMethod("Select", BindingFlags.NonPublic | BindingFlags.Instance);
                selectMethod.Invoke(itemContainer, new object[] { true });
            } else if (itemContainer != null) { 
                // If item container have sub-elements expand it and select item from this container
                itemContainer.IsExpanded = true;

                if (itemContainer.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated) {
                    // If item container is not generated yet register a delegate which would be called when container is generated
                    itemContainer.ItemContainerGenerator.StatusChanged += delegate {
                        SelectItem(itemContainer, tail, setFocus);
                    };
                } else {
                    // If item container already generated select sub-element from this container
                    SelectItem(itemContainer, tail, setFocus);
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
            foreach (Object item in parentContainer.Items) {
                var currentContainer = parentContainer.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                if (currentContainer != null && currentContainer.Items.Count > 0) {
                    currentContainer.IsExpanded = value;
                    if (currentContainer.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated) {
                        currentContainer.ItemContainerGenerator.StatusChanged += delegate {
                            SetExpandAll(currentContainer, value);
                        };
                    } else {
                        SetExpandAll(currentContainer, value);
                    }
                }
            }
        }
    }
}
