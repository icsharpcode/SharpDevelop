using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.ShortcutsManagement
{
    /// <summary>
    /// This panel is used in SharpDevelop options window to manage shortcuts
    /// </summary>
    public partial class ShortcutsManagementOptionsPanel : UserControl, IOptionPanel
    {
        enum SearchType
        {
            Gesture,
            Word
        }

        private string lastSearchWord;
        private KeyGestureTemplate lastSearchGesture;
        private SearchType lastSearchType;

        public static readonly DependencyProperty IsSearchableProperty = DependencyProperty.Register(
            "IsSearchable",
            typeof(Boolean),
            typeof(ShortcutsManagementOptionsPanel),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, OnIsSearchableChanged));

        public Boolean IsSearchable
        {
            get
            {
                return (Boolean)GetValue(IsSearchableProperty);
            }
            set
            {
                SetValue(IsSearchableProperty, value);
            }
        }

        public static void OnIsSearchableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = (ShortcutsManagementOptionsPanel)d;
            var oldValue = (Boolean)e.OldValue;
            var newValue = (Boolean)e.NewValue;

            if(oldValue != newValue)
            {
                panel.searchSection.Visibility = newValue ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private ICollection<AddIn> AddIns
        {
            get
            {
                return (ICollection<AddIn>)DataContext;
            }
        }

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
                    ShowShortcutManagementWindow((Shortcut)shortcutsTreeView.SelectedItem);
                }
            }
        }

        private void ShowShortcutManagementWindow(Shortcut shortcut)
        {
            var shortcutManagementWindow = new ShortcutManagementWindow(shortcut, AddIns);
            shortcutManagementWindow.ShowDialog();

            shortcut.Gestures.Add(new KeyGesture(Key.L, ModifierKeys.Control));
            if (shortcutManagementWindow.IsShortcutModified)
            {
                shortcut.Gestures.Clear();
                shortcut.Gestures.AddRange(shortcutManagementWindow.ModifiedShortcut.Gestures);
            }

            if (lastSearchType == SearchType.Gesture)
            {
                ShortcutsProvider.FilterGesture(AddIns, lastSearchGesture, false);
            }
            else
            {
                ShortcutsProvider.Filter(AddIns, lastSearchWord);
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

            lastSearchWord = searchTextBox.Text;
            lastSearchType = SearchType.Word;
            ShortcutsProvider.Filter(AddIns, searchTextBox.Text);

            if (!string.IsNullOrEmpty(searchTextBox.Text))
            {
                SelectFirstVisibleShortcut(shortcutsTreeView, AddIns, false);
            }
            else
            {
                shortcutsTreeView.SetExpandAll(false);
            }
        }


        private void searchTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // If Up/Down is pressed switch focus to shortcuts tree
            var keyboardDevice = (KeyboardDevice)e.Device;
            
            if (keyboardDevice.Modifiers == ModifierKeys.None
                && Array.IndexOf(new[] { Key.Up, Key.Down }, e.Key) >= 0)
            {
                SelectFirstVisibleShortcut(shortcutsTreeView, AddIns, true);
                
                return;
            }

            // If enter is pressed open shortcut configuration
            if (keyboardDevice.Modifiers == ModifierKeys.None && e.Key == Key.Enter && shortcutsTreeView.SelectedItem is Shortcut)
            {
                e.Handled = true;
                ShowShortcutManagementWindow((Shortcut)shortcutsTreeView.SelectedItem);

                return;
            }

            if (searchTypeToggleButton.IsChecked.HasValue && !searchTypeToggleButton.IsChecked.Value) return;

            e.Handled = true;

            var keyGestureTemplate = new KeyGestureTemplate(e.Key, Keyboard.Modifiers);

            lastSearchGesture = keyGestureTemplate;
            lastSearchType = SearchType.Gesture;
            ShortcutsProvider.FilterGesture(AddIns, keyGestureTemplate, false);

            SelectFirstVisibleShortcut(shortcutsTreeView, AddIns, false);

            searchTextBox.Text = GesturesHelper.GetGestureFromKeyEventArgument(e);
        }

        private void searchTypeToggleButton_Click(object sender, RoutedEventArgs e)
        {
            searchTextBox.Text = "";

            lastSearchWord = "";
            lastSearchType = SearchType.Word;
            ShortcutsProvider.Filter(AddIns, "");
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
