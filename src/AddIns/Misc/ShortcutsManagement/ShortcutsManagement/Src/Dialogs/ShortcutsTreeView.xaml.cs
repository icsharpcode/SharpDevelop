using System;
using System.Collections.Generic;
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
    public partial class ShortcutsTreeView : UserControl, IOptionPanel
    {
        /// <summary>
        /// Identifies <see cref="IsSearchable"/> dependency property
        /// </summary>
        public static readonly DependencyProperty IsSearchableProperty = DependencyProperty.Register(
            "IsSearchable",
            typeof(Boolean),
            typeof(ShortcutsTreeView),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Specifies whether shortcuts tree can be searched using part of the shortcut's name or
        /// gesture
        /// </summary>
        public Boolean IsSearchable
        {
            get {
                return (Boolean)GetValue(IsSearchableProperty);
            }
            set {
                SetValue(IsSearchableProperty, value);
            }
        }

        /// <summary>
        /// Identifies <see cref="IsRemovableShortcutsEnabled" /> dependency property
        /// </summary>
        public static readonly DependencyProperty IsRemovableShortcutsEnabledProperty = DependencyProperty.Register(
            "IsRemovableShortcutsEnabled",
            typeof(Boolean),
            typeof(ShortcutsTreeView),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Specify whether shortcuts can be removed from the tree.
        /// 
        /// If this value set to true <see cref="RemoveShortcutClick"/> will be raised when
        /// shortcut is deleted
        /// </summary>
        public Boolean IsRemovableShortcutsEnabled
        {
            get {
                return (Boolean)GetValue(IsSearchableProperty);
            }
            set {
                SetValue(IsSearchableProperty, value);
            }
        }

        /// <summary>
        /// Occurs when user tries to remove shortcut from shortcuts tree
        /// </summary>
        public event RemoveShortcutRoutedHandler RemoveShortcutClick;

        /// <summary>
        /// List of add-ins containing shortcut categories and shortcuts
        /// </summary>
        private ICollection<AddIn> AddIns
        {
            get {
                return (ICollection<AddIn>)DataContext;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ShortcutsTreeView()
        {
            InitializeComponent(); 
        }

        private void shortcutEntry_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2) {
                if (shortcutsTreeView.SelectedItem is Shortcut) {
                    ShowShortcutManagementWindow((Shortcut)shortcutsTreeView.SelectedItem);
                }
            }
        }

        private void ShowShortcutManagementWindow(Shortcut shortcut)
        {
            var shortcutManagementWindow = new ShortcutManagementWindow(shortcut, AddIns);
            shortcutManagementWindow.ShowDialog();
        }

        public void SelectFirstVisibleShortcut(bool setFocus)
        {
            // Select first visible shortcut
            var selectedAddIn = AddIns.FirstOrDefault(a => a.IsVisible);
            if (selectedAddIn != null) {
                var selectedCategory = selectedAddIn.Categories.FirstOrDefault(c => c.IsVisible);
                if (selectedCategory != null) {
                    var selectedShortcut = selectedCategory.Shortcuts.FirstOrDefault(s => s.IsVisible);
                    if (selectedShortcut != null) {
                        shortcutsTreeView.SelectItem(new List<object> { selectedAddIn, selectedCategory, selectedShortcut }, setFocus);
                    }
                }
            }
        }

        public void ExpandAll()
        {
            shortcutsTreeView.SetExpandAll(true);
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
            get {
                return this;
            }
        }
        #endregion

        /// <summary>
        /// Raised when user starts to type inside shortcuts tree
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event arguments</param>
        private void shortcutsTreeView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // If not navigating tree set focus to search text box
            var keyboardDevice = (KeyboardDevice)e.Device;
            if (keyboardDevice.Modifiers != ModifierKeys.None || Array.IndexOf(new[] { Key.Up, Key.Right, Key.Down, Key.Left }, e.Key) < 0) {
                searchTextBox.Text = "";
                Keyboard.Focus(searchTextBox);
                return;
            }
        }

        /// <summary>
        /// Raised when user tries to remove a shortcut from shortcut tree
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event arguments</param>
        private void removeShortcutButton_Click(object sender, RoutedEventArgs e)
        {
            if(RemoveShortcutClick != null) {
                var removeButton = (Button) sender;
                var routedRemoveShortcutEventArgs = new RoutedRemoveShortcutEventArgs(e.RoutedEvent, e.OriginalSource, (Shortcut)removeButton.Tag);

                // Forward event
                RemoveShortcutClick.Invoke(sender, routedRemoveShortcutEventArgs);
            }
        }


        /// <summary>
        /// Raised when user changes text in search textbox
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">Event arguments</param>
        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!searchTypeToggleButton.IsChecked.HasValue || searchTypeToggleButton.IsChecked.Value) return;

            new ShortcutsFinder(AddIns).Filter(searchTextBox.Text);

            if (!string.IsNullOrEmpty(searchTextBox.Text)) {
                SelectFirstVisibleShortcut(false);
            } else {
                shortcutsTreeView.SetExpandAll(false);
            }
        }

        /// <summary>
        /// Raised when user presses a key inside search box
        /// </summary>
        /// <param name="sender">Sender object </param>
        /// <param name="e">Event arguments</param>
        private void searchTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var keyboardDevice = (KeyboardDevice)e.Device;

            // If Up/Down is pressed switch focus to shortcuts tree
            if (keyboardDevice.Modifiers == ModifierKeys.None && Array.IndexOf(new[] { Key.Up, Key.Down }, e.Key) >= 0) {
                SelectFirstVisibleShortcut(true);
                return;
            }

            // If enter is pressed open shortcut configuration
            if (keyboardDevice.Modifiers == ModifierKeys.None && e.Key == Key.Enter && shortcutsTreeView.SelectedItem is Shortcut) {
                e.Handled = true;
                ShowShortcutManagementWindow((Shortcut)shortcutsTreeView.SelectedItem);
                return;
            }

            // Exit if filtering by text (Handled in searchTextBox_TextChanged)
            if (searchTypeToggleButton.IsChecked.HasValue && !searchTypeToggleButton.IsChecked.Value) {
                return;
            }

            // Filter shortcuts with similar gestures assigned and display entered gesture inside search textbox
            var keyGestureTemplate = new KeyGestureTemplate(e.Key, Keyboard.Modifiers);
            new ShortcutsFinder(AddIns).FilterGesture(keyGestureTemplate, false);
            SelectFirstVisibleShortcut(false);
            searchTextBox.Text = new KeyGestureTemplate(e).ToString();

            e.Handled = true;
        }

        /// <summary>
        /// Raised when changing search type
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event arguments</param>
        private void searchTypeToggleButton_Click(object sender, RoutedEventArgs e)
        {
            searchTextBox.Text = "";

            new ShortcutsFinder(AddIns).Filter("");
            shortcutsTreeView.SetExpandAll(false);
        }
    }

    public delegate void RemoveShortcutRoutedHandler(object sender, RoutedRemoveShortcutEventArgs args);

    /// <summary>
    /// Contains state information and event data associated with <see cref="RemoveShortcutRoutedHandler"/>
    /// </summary>
    public class RoutedRemoveShortcutEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Reference to shortcut being removed
        /// </summary>
        public Shortcut RemovedShortcut
        {
            get; 
            private set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="routedEvent">The routed event identifier for this instance of RoutedRemoveShortcutEventArgs</param>
        /// <param name="source">Source which will be reported when event is handled</param>
        /// <param name="removedShortcut">Shortcut being removed</param>
        public RoutedRemoveShortcutEventArgs(RoutedEvent routedEvent, object source, Shortcut removedShortcut)
            : base(routedEvent, source)
        {
            RemovedShortcut = removedShortcut;
        }
    }
}
