using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop;
using ICSharpCode.ShortcutsManagement.Data;
using ICSharpCode.ShortcutsManagement.Dialogs;
using ICSharpCode.ShortcutsManagement.Extensions;

namespace ICSharpCode.ShortcutsManagement
{
    /// <summary>
    /// This panel is used in SharpDevelop options window to manage shortcuts
    /// </summary>
    public partial class ShortcutsTreeView : UserControl
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
        private ICollection<IShortcutTreeEntry> RootEntries
        {
            get {
                return (ICollection<IShortcutTreeEntry>)DataContext;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ShortcutsTreeView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Display shorcut management window
        /// </summary>
        /// <param name="shortcut">Shortcut to modify</param>
        private void ShowShortcutManagementWindow(Shortcut shortcut)
        {
            var shortcutManagementWindow = new ShortcutManagementWindow(shortcut, RootEntries);
            shortcutManagementWindow.ShowDialog();
        }

        /// <summary>
        /// Select first enabled shortcut in shortcut tree
        /// </summary>
        /// <param name="setFocus">Set focus to this shortcut entry</param>
        public void SelectFirstVisibleShortcut(bool setFocus)
        {
            var path = new List<IShortcutTreeEntry>();
            foreach (var entry in RootEntries) {
                if (entry != null && entry.IsVisible) {
                    path.Add(entry);
                    FindFirstVisibleItemPath(entry, path);
                    shortcutsTreeView.SelectItem(path.Cast<object>().ToList(), setFocus);
                    
                    return;
                }
            }
        }
        /// <summary>
        /// Find path to first <see cref="IShortcutTreeEntry"/> with <see cref="IShortcutTreeEntry.IsVisible"/> true
        /// </summary>
        /// <param name="parent">Starting node</param>
        /// <param name="path">Accumulated path</param>
        private void FindFirstVisibleItemPath(IShortcutTreeEntry parent, List<IShortcutTreeEntry> path) 
        {
            // Find first visible add-in
            var addIn = parent as AddIn;
            if(addIn != null) {
                var selectedCategory = addIn.Categories.FirstOrDefault(a => a.IsVisible);
                if (selectedCategory != null) {
                    path.Add(selectedCategory);
                    FindFirstVisibleItemPath(selectedCategory, path);
                    return;
                }
            }

            // Find first visible category
            var category = parent as ShortcutCategory;
            if (category != null) {
                var selectedCategory = category.SubCategories.FirstOrDefault(a => a.IsVisible);
                if (selectedCategory != null) {
                    path.Add(selectedCategory);
                    FindFirstVisibleItemPath(selectedCategory, path);
                    return;
                }

                // Find first visible shortcut
                var selectedShortcut = category.Shortcuts.FirstOrDefault(a => a.IsVisible);
                if (selectedShortcut != null) {
                    path.Add(selectedShortcut);
                    FindFirstVisibleItemPath(selectedShortcut, path);
                    return;
                }
            }
        }

        /// <summary>
        /// Expand all elements of the tree
        /// </summary>
        public void ExpandAll()
        {
            shortcutsTreeView.SetExpandAll(true);
        }

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

            new ShortcutsFinder(RootEntries).Filter(searchTextBox.Text);

            if (!string.IsNullOrEmpty(searchTextBox.Text)) {
                SelectFirstVisibleShortcut(false);
            } else {
                shortcutsTreeView.SetExpandAll(false);
            }
        }

        /// <summary>
        /// Raised when changing search type
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event arguments</param>
        private void searchTypeToggleButton_Click(object sender, RoutedEventArgs e)
        {
            searchTextBox.Text = "";
            gestureTextBox.shortcutTextBox.Text = "";

            new ShortcutsFinder(RootEntries).Filter("");
            shortcutsTreeView.SetExpandAll(false);

            if(!searchTypeToggleButton.IsChecked.HasValue || !searchTypeToggleButton.IsChecked.Value) {
                Keyboard.Focus(searchTextBox);
            } else {
                Keyboard.Focus(gestureTextBox.shortcutTextBox);
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
        }

        /// <summary>
        /// Raised if gestures entered in gesture text box change
        /// </summary>
        /// <param name="sender">Senrer object</param>
        /// <param name="e">Event arguments</param>
        private void gestureTextBox_GestureChanged(object sender, EventArgs e)
        {
            // Exit if filtering by text (Handled in searchTextBox_TextChanged)
            if (searchTypeToggleButton.IsChecked.HasValue && !searchTypeToggleButton.IsChecked.Value) {
                return;
            }

            if (gestureTextBox.Gesture != null) {
                new ShortcutsFinder(RootEntries).FilterGesture(gestureTextBox.Gesture, GestureFilterMode.PartlyMatches);
                SelectFirstVisibleShortcut(false);
            } else {
                new ShortcutsFinder(RootEntries).Filter("");
            }
        }

        /// <summary>
        /// Raised when user double click on shortcut tree item
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event arguments</param>
        private void shortcutEntry_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2) {
                if (shortcutsTreeView.SelectedItem is Shortcut) {
                    ShowShortcutManagementWindow((Shortcut)shortcutsTreeView.SelectedItem);
                }
            }
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
