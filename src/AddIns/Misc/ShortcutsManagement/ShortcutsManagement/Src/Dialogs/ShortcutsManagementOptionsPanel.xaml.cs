using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.ShortcutsManagement
{
    /// <summary>
    /// This panel is used in SharpDevelop options window to manage shortcuts
    /// </summary>
    public partial class ShortcutsManagementOptionsPanel : UserControl, IOptionPanel
    {
        public ShortcutsManagementOptionsPanel()
        {
            InitializeComponent();
            
        }

        private void shortcutEntry_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                if (shortcutsTreeView.SelectedItem is Shortcut)
                {
                    var receiver = ((ShortcutsProvider)Resources["ShortcutsReceiver"]);
                    var shortcut = (Shortcut)shortcutsTreeView.SelectedItem;
                    new ShortcutManagementWindow(shortcut, receiver).ShowDialog();
                }
            }
        }

        /// <summary>
        /// Filter shortcuts tree view. Display only matching shortcuts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!searchTypeToggleButton.IsChecked.HasValue || searchTypeToggleButton.IsChecked.Value) return;

            var receiver = ((ShortcutsProvider)Resources["ShortcutsReceiver"]);
            receiver.Filter(searchTextBox.Text);

            if (!string.IsNullOrEmpty(searchTextBox.Text))
            {
                SelectFirstVisibleShortcut(shortcutsTreeView, receiver.GetAddIns(), false);
            }
            else
            {
                shortcutsTreeView.SetExpandAll(false);
            }
        }


        private void searchTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var receiver = ((ShortcutsProvider)Resources["ShortcutsReceiver"]);
                
            // If Up/Down is pressed switch focus to shortcuts tree
            var keyboardDevice = (KeyboardDevice)e.Device;
            
            if (keyboardDevice.Modifiers == ModifierKeys.None
                && Array.IndexOf(new[] { Key.Up, Key.Down }, e.Key) >= 0)
            {
                SelectFirstVisibleShortcut(shortcutsTreeView, receiver.GetAddIns(), true);
                
                return;
            }

            // If enter is pressed open shortcut configuration
            if (keyboardDevice.Modifiers == ModifierKeys.None && e.Key == Key.Enter)
            {
                e.Handled = true;
                if (shortcutsTreeView.SelectedItem is Shortcut)
                {
                    var shortcut = (Shortcut) shortcutsTreeView.SelectedItem;
                    new ShortcutManagementWindow(shortcut, receiver).ShowDialog();
                }

                return;
            }

            if (searchTypeToggleButton.IsChecked.HasValue && !searchTypeToggleButton.IsChecked.Value) return;

            e.Handled = true;

            receiver.FilterGesture(e);

            SelectFirstVisibleShortcut(shortcutsTreeView, receiver.GetAddIns(), false);

            searchTextBox.Text = GesturesHelper.GetGestureFromKeyEventArgument(e);
        }

        private void searchTypeToggleButton_Click(object sender, RoutedEventArgs e)
        {
            searchTextBox.Text = "";

            var receiver = ((ShortcutsProvider)Resources["ShortcutsReceiver"]);
            receiver.Filter("");
            shortcutsTreeView.SetExpandAll(false);
        }

        private static void SelectFirstVisibleShortcut(TreeView treeView, IEnumerable<AddIn> addIns, bool setFocus)
        {
            // Select first visible shortcut
            var selectedAddIn = addIns.FirstOrDefault(a => a.IsVisible);
            if (selectedAddIn != null)
            {
                var selectedCategory = selectedAddIn.Categories.FirstOrDefault(c => c.IsVisible);
                if (selectedCategory != null)
                {
                    var selectedShortcut = selectedCategory.Shortcuts.FirstOrDefault(s => s.IsVisible);
                    if (selectedShortcut != null)
                    {
                        treeView.SelectItem(new List<object> { selectedAddIn, selectedCategory, selectedShortcut }, setFocus);
                    }
                }
            }
        }

        #region IOptionPanel
        public void LoadOptions()
        {
        }

        public bool SaveOptions()
        {
            return true;
        }

        public object Owner
        {
            get; set;
        }

        public object Control
        {
            get
            {
                return this;
            }
        }
        #endregion

        private void shortcutsTreeView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // If not navigating tree set focus to search text box
            var keyboardDevice = (KeyboardDevice)e.Device;
            if (keyboardDevice.Modifiers != ModifierKeys.None
                || Array.IndexOf(new[] { Key.Up, Key.Right, Key.Down, Key.Left }, e.Key) < 0)
            {
                searchTextBox.Text = "";
                Keyboard.Focus(searchTextBox);
                return;
            }
        }
    }
}
