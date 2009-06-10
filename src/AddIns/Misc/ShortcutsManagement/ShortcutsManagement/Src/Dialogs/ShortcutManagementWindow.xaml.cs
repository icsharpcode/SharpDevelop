using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ICSharpCode.SharpDevelop;
using ICSharpCode.ShortcutsManagement.Data;

namespace ICSharpCode.ShortcutsManagement
{
    /// <summary>
    /// This window allows user to modify shortcuts registered in application
    /// </summary>
    public partial class ShortcutManagementWindow : Window
    {
        /// <summary>
        /// Modified shortcut copy
        /// </summary>
        private readonly Shortcut shortcutCopy;

        /// <summary>
        /// Modified shortcut. 
        /// 
        /// Original shortcut is modified only when "Save" button is pressed
        /// </summary>
        private readonly Shortcut shortcutOriginal;

        /// <summary>
        /// Deep copy of addins list (including copies of categories and shortcuts)
        /// </summary>
        private readonly ICollection<AddIn> addInsCopy = new ObservableCollection<AddIn>();

        /// <summary>
        /// List of all addins
        /// </summary>
        private readonly ICollection<AddIn> addInsOriginal;

        /// <summary>
        /// List of modified shortcuts. 
        /// 
        /// This list is used to optimize performance. Shortcuts not in this list are not saved.
        /// Allways add modified shortcut to this list.
        /// </summary>
        private readonly List<Shortcut> modifiedShortcuts = new List<Shortcut>();
            
        /// <summary>
        /// Last entered input gesture
        /// </summary>
        private InputGesture lastEnteredInputGesture;

        /// <summary>
        /// Initializes new <see cref="ShortcutManagementWindow" /> class
        /// </summary>
        /// <param name="shortcut">Shortcut</param>
        /// <param name="addIns">List of all other add-ins containing shortcuts and categories. This list is used to find dupliate shortcuts</param>
        public ShortcutManagementWindow(Shortcut shortcut, ICollection<AddIn> addIns)
        {
            shortcutOriginal = shortcut;
            addInsOriginal = addIns;

            // Make a deep copy of all add-ins, categories and shortcuts
            var shortcutCopyFound = false;
            foreach (var addIn in addIns)
            {
                var clonedAddIn = (AddIn) addIn.Clone();
                addInsCopy.Add(clonedAddIn);

                // Find copy of modified shortcut in copied add-ins collection
                if (shortcutCopyFound == false && (shortcutCopy = clonedAddIn.FindShortcut(shortcutOriginal.Id)) != null)
                {
                    shortcutCopy.Gestures.CollectionChanged += Gestures_CollectionChanged;
                    modifiedShortcuts.Add(shortcutCopy);
                    DataContext = shortcutCopy;
                    shortcutCopyFound = true;
                } 
            }
            
            InitializeComponent();

            // Display similar shortcuts (Shortcuts with the same input gestures assigned to them)
            shortcutsManagementOptionsPanel.DataContext = addInsCopy;
            shortcutsManagementOptionsPanel.Loaded += delegate { FilterSimilarShortcuts(); };
        }

        /// <summary>
        /// Filter shortcuts using same gestures as modified shortcut
        /// </summary>
        private void FilterSimilarShortcuts()
        {
            var templates = new List<KeyGestureTemplate>();
            foreach (var gesture in shortcutCopy.Gestures) {
                if (gesture is KeyGesture) {
                    templates.Add(new KeyGestureTemplate((KeyGesture)gesture));
                }
            }

            // Find shortcuts with same gesture and hide them.
            // Also hide modified shortcut from this list
            var finder = new ShortcutsFinder(addInsCopy);
            finder.FilterGesture(templates, true);
            finder.HideShortcut(shortcutCopy);

            shortcutsManagementOptionsPanel.ExpandAll();
            shortcutsManagementOptionsPanel.SelectFirstVisibleShortcut(false);
        }

        /// <summary>
        /// Executed when adding or removing gestures used to call modified shortcut
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event arguments</param>
        void Gestures_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            FilterSimilarShortcuts();
        }

        /// <summary>
        /// Executed when user presses key inside gesture text box.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event arguments</param>
        private void shortcutTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            // Get entered key gesture string representation
            shortcutTextBox.Text = new KeyGestureTemplate(e).ToString();
            
            // Accept only valid getures
            lastEnteredInputGesture = null;
            try {
                lastEnteredInputGesture = new KeyGesture(e.Key, Keyboard.Modifiers);
            } catch (NotSupportedException) {
                lastEnteredInputGesture = null;
            }
        }
       
        /// <summary>
        /// Executed when "Remove" button next to gesture is pressed
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">Event arguments</param>
        private void removeGestureButton_Click(object sender, RoutedEventArgs e)
        {
            var tag = ((Button) sender).Tag as InputGesture;
            shortcutCopy.Gestures.Remove(tag);
        }

        /// <summary>
        /// Executed when "Add Gesture" button is clicked
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">Event arguments</param>
        private void addGestureButton_Click(object sender, RoutedEventArgs e)
        {
            // Discard duplicate gestures
            if(lastEnteredInputGesture != null && !shortcutCopy.ContainsGesture(lastEnteredInputGesture))
            {
                shortcutCopy.Gestures.Add(lastEnteredInputGesture);
            }
        }

        /// <summary>
        /// Executed when "Remove" button next to similar shortcut is pressed
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event argument</param>
        private void shortcutsManagementOptionsPanel_RemoveShortcutClick(object sender, RoutedRemoveShortcutEventArgs e)
        {
            e.Handled = true;

            // Remove gestures registered in modified shortcut from deleted shortcut
            var removedShortcutGestures = e.RemovedShortcut.Gestures;
            for (int i = removedShortcutGestures.Count - 1; i >= 0; i--)
            {
                foreach (var modifiedInputGesture in shortcutCopy.Gestures)
                {
                    var modifiedKeyGesture = modifiedInputGesture as KeyGesture;
                    var removedKeyGesture = removedShortcutGestures[i] as KeyGesture;
                    if (modifiedKeyGesture != null
                        && removedKeyGesture != null
                        && modifiedKeyGesture.Key == removedKeyGesture.Key
                        && modifiedKeyGesture.Modifiers == removedKeyGesture.Modifiers)
                    {
                        removedShortcutGestures.RemoveAt(i);
                        FilterSimilarShortcuts();

                        if (!modifiedShortcuts.Contains(e.RemovedShortcut))
                        {
                            modifiedShortcuts.Add(e.RemovedShortcut);
                        }

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Save changes to shortcuts
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event arguments</param>
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            // Move modifications from shortcut copies to original shortcut objects
            foreach (var relatedShortcutCopy in modifiedShortcuts) {
                foreach (var addin in addInsOriginal) {
                    var originalRelatedShortcut = addin.FindShortcut(relatedShortcutCopy.Id);
                    if(originalRelatedShortcut != null) {
                        originalRelatedShortcut.Gestures.Clear();
                        originalRelatedShortcut.Gestures.AddRange(relatedShortcutCopy.Gestures);
                    }
                }
            }

            Close();
        }

        /// <summary>
        /// Execute this method when Reset button is clicked. 
        /// 
        /// Modifications are not saved
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event arguments</param>
        private void resetButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
