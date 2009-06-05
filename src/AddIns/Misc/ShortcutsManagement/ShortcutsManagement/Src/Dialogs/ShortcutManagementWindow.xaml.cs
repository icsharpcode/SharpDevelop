using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ICSharpCode.ShortcutsManagement
{
    /// <summary>
    /// Interaction logic for ShortcutManagementWindow.xaml
    /// </summary>
    public partial class ShortcutManagementWindow : Window
    {
        private Shortcut shortcutCopy;

        public Shortcut ModifiedShortcut
        {
            get; private set;
        }

        public bool IsShortcutModified
        {
            get; private set;
        }

        private ICollection<AddIn> AddIns
        {
            get; set;
        }
            
        private InputGesture lastEnteredInputGesture;

        public ShortcutManagementWindow(Shortcut shortcut, ICollection<AddIn> addIns)
        {
            shortcutCopy = (Shortcut)shortcut.Clone();
            shortcutCopy.Gestures.CollectionChanged += Gestures_CollectionChanged;
            DataContext = shortcutCopy;
            
            InitializeComponent();

            AddIns = addIns;
            shortcutsManagementOptionsPanel.DataContext = AddIns;
            FilterSimilarShortcuts();
            shortcutsManagementOptionsPanel.shortcutsTreeView.SetExpandAll(true);
        }

        void Gestures_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            FilterSimilarShortcuts();
        }

        private void FilterSimilarShortcuts()
        {
            var templates = new List<KeyGestureTemplate>();
            foreach (var gesture in shortcutCopy.Gestures)
            {
                if (gesture is KeyGesture)
                {
                    templates.Add(new KeyGestureTemplate((KeyGesture)gesture));
                }
            }

            ShortcutsProvider.FilterGesture(AddIns, templates, true);
        }

        private void shortcutTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            shortcutTextBox.Text = GesturesHelper.GetGestureFromKeyEventArgument(e);

            lastEnteredInputGesture = null;
            try
            {
                lastEnteredInputGesture = new KeyGesture(e.Key, Keyboard.Modifiers);
            }
            catch (NotSupportedException)
            { }
        }
       
        private void removeGestureButton_Click(object sender, RoutedEventArgs e)
        {
            var tag = ((Button) sender).Tag as InputGesture;
            shortcutCopy.Gestures.Remove(tag);
        }

        private void addGestureButton_Click(object sender, RoutedEventArgs e)
        {
            if(lastEnteredInputGesture != null && !shortcutCopy.ContainsGesture(lastEnteredInputGesture))
            {
                shortcutCopy.Gestures.Add(lastEnteredInputGesture);
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            IsShortcutModified = true;
            ModifiedShortcut = shortcutCopy;

            Close();
        }

        private void resetButton_Click(object sender, RoutedEventArgs e)
        {
            IsShortcutModified = false;
            Close();
        }
    }
}
