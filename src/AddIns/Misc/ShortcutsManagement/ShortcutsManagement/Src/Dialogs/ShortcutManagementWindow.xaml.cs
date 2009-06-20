using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop;
using ICSharpCode.ShortcutsManagement.Data;
using ICSharpCode.ShortcutsManagement.Extensions;

namespace ICSharpCode.ShortcutsManagement.Dialogs
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
        private readonly ICollection<IShortcutTreeEntry> rootEntriesCopy = new ObservableCollection<IShortcutTreeEntry>();

        /// <summary>
        /// List of all addins
        /// </summary>
        private readonly ICollection<IShortcutTreeEntry> rootEntriesOriginal;

        /// <summary>
        /// List of modified shortcuts. 
        /// 
        /// This list is used to optimize performance. Shortcuts not in this list are not saved.
        /// Allways add modified shortcut to this list.
        /// </summary>
        private readonly List<Shortcut> modifiedShortcuts = new List<Shortcut>();

        /// <summary>
        /// Initializes new <see cref="ShortcutManagementWindow" /> class
        /// </summary>
        /// <param name="shortcut">Shortcut</param>
        /// <param name="rootEntries">List of all other add-ins containing shortcuts and categories. This list is used to find dupliate shortcuts</param>
        public ShortcutManagementWindow(Shortcut shortcut, ICollection<IShortcutTreeEntry> rootEntries)
        {
            shortcutOriginal = shortcut;
            rootEntriesOriginal = rootEntries;

            // Make a deep copy of all add-ins, categories and shortcuts
            var shortcutCopyFound = false;
            foreach (var entry in rootEntriesOriginal) {
                var clonedAddIn = (IShortcutTreeEntry)entry.Clone();
                rootEntriesCopy.Add(clonedAddIn);

                // Find copy of modified shortcut in copied add-ins collection
                if (shortcutCopyFound == false && (shortcutCopy = clonedAddIn.FindShortcut(shortcutOriginal.Id)) != null) {
                    shortcutCopy.Gestures.CollectionChanged += Gestures_CollectionChanged;
                    modifiedShortcuts.Add(shortcutCopy);
                    DataContext = shortcutCopy;
                    shortcutCopyFound = true;
                } 
            }
            
            InitializeComponent();

            // Display similar shortcuts (Shortcuts with the same input gestures assigned to them)
            shortcutsManagementOptionsPanel.DataContext = rootEntriesCopy;
            shortcutsManagementOptionsPanel.Loaded += delegate { FilterSimilarShortcuts(); };
        }

        /// <summary>
        /// Filter shortcuts using same gestures as modified shortcut
        /// </summary>
        private void FilterSimilarShortcuts()
        {
            var templates = new InputGestureCollection();
            foreach (var gesture in shortcutCopy.Gestures) {
                var multiKeyGestureTemplate = gesture as MultiKeyGesture;
                if (multiKeyGestureTemplate != null) {
                    if (multiKeyGestureTemplate.Gestures != null && multiKeyGestureTemplate.Gestures.Count > 0) {
                        templates.Add(multiKeyGestureTemplate.Gestures.FirstOrDefault());
                    }
                } else {
                    templates.Add(gesture);
                }
            }

            // Find shortcuts with same gesture and hide them.
            // Also hide modified shortcut from this list
            var finder = new ShortcutsFinder(rootEntriesCopy);
            finder.FilterGesture(templates,  GestureFilterMode.StartsWith);
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
            // Check if any chords are entered
            if (gestureTextBox.Gesture == null) {
                DisplayNotification(StringParser.Parse("${res:ShortcutsManagement.ModificationWindow.AdditionFailedNoChords}"), NotificationType.Failed);
                return;
            }

            // Check whether first chord is finished
            var partialKeyGesture = gestureTextBox.Gesture as PartialKeyGesture;
            if (partialKeyGesture != null && partialKeyGesture.Modifiers == ModifierKeys.None) {
                DisplayNotification(StringParser.Parse("${res:ShortcutsManagement.ModificationWindow.AdditionFailedFirstChordIsIncomplete}"), NotificationType.Failed);
                return;
            }

            // Check whether last chord is finished
            var multiKeyGesture = gestureTextBox.Gesture as MultiKeyGesture;
            if (multiKeyGesture != null && multiKeyGesture.Gestures.Last().Key == Key.None) {
                DisplayNotification(StringParser.Parse("${res:ShortcutsManagement.ModificationWindow.AdditionFailedLastChordIsIncompete}"), NotificationType.Failed);
                return;
            }

            if (partialKeyGesture != null) {
                var keyGesture = new KeyGesture(partialKeyGesture.Key, partialKeyGesture.Modifiers);
                shortcutCopy.Gestures.Add(keyGesture);
            } else {
                shortcutCopy.Gestures.Add(gestureTextBox.Gesture);
            }

            DisplayNotification(StringParser.Parse("${res:ShortcutsManagement.ModificationWindow.AdditionIsSuccessfull}"), NotificationType.Added);
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
            for (int i = removedShortcutGestures.Count - 1; i >= 0; i--) {
                foreach (var modifiedInputGesture in shortcutCopy.Gestures) {
                    var modifiedKeyGesture = modifiedInputGesture as KeyGesture;
                    var removedKeyGesture = removedShortcutGestures[i] as KeyGesture;
                    if (modifiedKeyGesture != null
                                && removedKeyGesture != null
                                && modifiedKeyGesture.Key == removedKeyGesture.Key
                                && modifiedKeyGesture.Modifiers == removedKeyGesture.Modifiers) {
                        removedShortcutGestures.RemoveAt(i);
                        FilterSimilarShortcuts();

                        if (!modifiedShortcuts.Contains(e.RemovedShortcut)) {
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
                foreach (var rootEntry in rootEntriesOriginal) {
                    var originalRelatedShortcut = rootEntry.FindShortcut(relatedShortcutCopy.Id);
                    if(originalRelatedShortcut != null) {
                        originalRelatedShortcut.Gestures.Clear();
                        originalRelatedShortcut.Gestures.AddRange(relatedShortcutCopy.Gestures);
                    }
                }
            }

            Close();
        }

        /// <summary>
        /// Display message describing shortcut addition result
        /// </summary>
        /// <param name="notificationText">Displayed message text</param>
        /// <param name="type">Message type</param>
        public void DisplayNotification(string notificationText, NotificationType type)
        {
            gestureTextBox.NotificationText = notificationText;
            gestureTextBox.NotificationType = type;
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
