using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.ShortcutsManagement.Data;

namespace ICSharpCode.ShortcutsManagement.Dialogs
{
	/// <summary>
	/// This window allows user to modify shortcuts registered in application
	/// </summary>
	public partial class ShortcutManagementWindow : Window
	{
		private HashSet<Shortcut> modifiedShortcuts = new HashSet<Shortcut>();
		
		/// <summary>
		/// Gets modified shortcut copy
		/// </summary>
		public Shortcut Shortcut
		{
			get; private set;
		}
		
		/// <summary>
		/// Gets deep copy of addins list (including copies of categories and shortcuts)
		/// </summary>
		public ICollection<IShortcutTreeEntry> RootEntries
		{
			get; private set;
		}
		
		/// <summary>
		/// Gets list of modified shortcuts. 
		/// 
		/// This list is used to optimize performance. Shortcuts not in this list are not saved.
		/// Allways add modified shortcut to this list.
		/// </summary>
		public ICollection<Shortcut> ModifiedShortcuts
		{
			get {
				return modifiedShortcuts;
			}
		}
		
		/// <summary>
		/// Initializes new <see cref="ShortcutManagementWindow" /> class
		/// </summary>
		/// <param name="shortcut">Shortcut</param>
		/// <param name="rootEntries">List of all other add-ins containing shortcuts and categories. This list is used to find dupliate shortcuts</param>
		public ShortcutManagementWindow(Shortcut shortcut, ICollection<IShortcutTreeEntry> rootEntries)
		{
			Shortcut = shortcut;
			RootEntries = rootEntries;
			
			shortcut.Gestures.CollectionChanged += Gestures_CollectionChanged;
			ModifiedShortcuts.Add(shortcut);
			DataContext = shortcut;
			
			InitializeComponent();
			
			// Display similar shortcuts (Shortcuts with the same input gestures assigned to them)
			shortcutsManagementOptionsPanel.DataContext = rootEntries;
			shortcutsManagementOptionsPanel.Loaded += delegate { FilterSimilarShortcuts(); };
		}
		
		/// <summary>
		/// Filter shortcuts using same gestures as modified shortcut
		/// </summary>
		private void FilterSimilarShortcuts()
		{
			var templates = new InputGestureCollection(Shortcut.Gestures);
			
			// Find shortcuts with same gesture and hide them.
			// Also hide modified shortcut from this list
			var finder = new ShortcutsFinder(RootEntries);
			finder.FilterGesture(templates, GestureCompareMode.Conflicting);
			finder.HideShortcut(Shortcut);
			
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
			Shortcut.Gestures.Remove(tag);
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
			
			// Check whether first chord is unfinished
			var partialKeyGesture = gestureTextBox.Gesture as PartialKeyGesture;
			if (partialKeyGesture != null && !partialKeyGesture.IsFull) {
				DisplayNotification(StringParser.Parse("${res:ShortcutsManagement.ModificationWindow.AdditionFailedFirstChordIsIncomplete}"), NotificationType.Failed);
				return;
			}
			
			// Check whether last chord is finished
			var multiKeyGesture = gestureTextBox.Gesture as MultiKeyGesture;
			if (multiKeyGesture != null && multiKeyGesture.Chords.Last().Key == Key.None) {
				DisplayNotification(StringParser.Parse("${res:ShortcutsManagement.ModificationWindow.AdditionFailedLastChordIsIncompete}"), NotificationType.Failed);
				return;
			}
			
			// Check whether gesture exist in shortcut gestures collection
			foreach (var existingGesture in Shortcut.Gestures) {
				if (gestureTextBox.Gesture.IsTemplateFor(existingGesture, GestureCompareMode.ExactlyMatches)) {
					DisplayNotification(StringParser.Parse("${res:ShortcutsManagement.ModificationWindow.AddingExistingGesture}"), NotificationType.Failed);
					return;
				}
			}
			
			// Add gesture
			if (partialKeyGesture != null) {
				try {
					var keyGesture = new KeyGesture(partialKeyGesture.Key, partialKeyGesture.Modifiers);
					Shortcut.Gestures.Add(keyGesture);
				} catch (NotSupportedException) {
					Shortcut.Gestures.Add(partialKeyGesture);
				}
			} else {
				Shortcut.Gestures.Add(gestureTextBox.Gesture);
			}
			
			DisplayNotification(StringParser.Parse("${res:ShortcutsManagement.ModificationWindow.AdditionIsSuccessfull}"), NotificationType.Added);
		}
		
		private Shortcut prevShortcut;
		
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
				foreach (var modifiedInputGesture in Shortcut.Gestures) {
					if (removedShortcutGestures[i].IsTemplateFor(modifiedInputGesture, GestureCompareMode.StartsWith)) {
						removedShortcutGestures.RemoveAt(i);
						
						modifiedShortcuts.Add(e.RemovedShortcut);
						
						break;
					}
				}
			}
			
			FilterSimilarShortcuts();
		}
		
		/// <summary>
		/// Save changes to shortcuts
		/// </summary>
		/// <param name="sender">Sender object</param>
		/// <param name="e">Event arguments</param>
		private void saveButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			
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
		
		void DefaultButton_Click(object sender, RoutedEventArgs e)
		{
			Shortcut.ResetToDefaults();
		}
	}
}
