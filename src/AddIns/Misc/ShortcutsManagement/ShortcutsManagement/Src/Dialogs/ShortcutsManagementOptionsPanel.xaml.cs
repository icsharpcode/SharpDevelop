using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop;
using ICSharpCode.ShortcutsManagement.Data;
using Microsoft.Win32;
using ShortcutManagement=ICSharpCode.ShortcutsManagement.Data;
using CommandManager = ICSharpCode.Core.Presentation.CommandManager;
using MessageBox=System.Windows.MessageBox;
using Shortcut=ICSharpCode.ShortcutsManagement.Data.Shortcut;
using UserControl=System.Windows.Controls.UserControl;

namespace ICSharpCode.ShortcutsManagement.Dialogs
{
    /// <summary>
    /// Interaction logic for ShortcutsManagementOptionsPanel.xaml
    /// </summary>
    public partial class ShortcutsManagementOptionsPanel : UserControl, IOptionPanel
    {
        /// <summary>
        /// Identifies <see cref="SelectedProfile"/> dependency property
        /// </summary>
        public static readonly DependencyProperty SelectedProfileProperty = DependencyProperty.Register(
            "SelectedProfile",
            typeof(UserGesturesProfile),
            typeof(ShortcutsManagementOptionsPanel),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Specifies text box border thickness
        /// </summary>
        public UserGesturesProfile SelectedProfile
        {
            get
            {
                return (UserGesturesProfile)GetValue(SelectedProfileProperty);
            }
            set
            {
                SetValue(SelectedProfileProperty, value);
            }
        }

        /// <summary>
        /// Stores shortcut entry to input binding convertion map
        /// </summary>
        private readonly Dictionary<Shortcut, InputBindingInfo> shortcutsMap = new Dictionary<Shortcut, InputBindingInfo>();

        private List<IShortcutTreeEntry> rootEntries;

        private readonly List<UserGesturesProfile> profiles = new List<UserGesturesProfile>();

        private readonly List<UserGesturesProfile> removedProfiles = new List<UserGesturesProfile>();

        public ShortcutsManagementOptionsPanel()
        {
            ResourceService.RegisterStrings("ICSharpCode.ShortcutsManagement.Resources.StringResources", GetType().Assembly);

            InitializeComponent();
        }

        public void LoadOptions()
        {
            shortcutsManagementOptionsPanel.searchTextBox.Focus();

            if (Directory.Exists(UserDefinedGesturesManager.UserGestureProfilesDirectory))
            {
                var dirInfo = new DirectoryInfo(UserDefinedGesturesManager.UserGestureProfilesDirectory);
                var xmlFiles = dirInfo.GetFiles("*.xml");

                foreach (var fileInfo in xmlFiles)
                {
                    var profile = new UserGesturesProfile();
                    profile.Path = Path.Combine(UserDefinedGesturesManager.UserGestureProfilesDirectory, fileInfo.Name);
                    profile.Load();
                    profiles.Add(profile);

                    if (UserDefinedGesturesManager.CurrentProfile != null && profile.Name == UserDefinedGesturesManager.CurrentProfile.Name)
                    {
                        profilesTextBox.SelectedItem = profile;
                    }
                }
            }
            
            BindProfiles();
            BindShortcuts();
        }

        private void BindProfiles()
        {
            var profilesTextBoxItemsSource = new ArrayList(profiles);

            profilesTextBoxItemsSource.Add(new SeparatorData());

            var deleteItem = new UserGestureProfileAction();
            deleteItem.Name = "Delete";
            deleteItem.Text = StringParser.Parse("${res:ShortcutsManagement.ShortcutsManagementOptionsPanel.ProfileDeleteAction}");
            profilesTextBoxItemsSource.Add(deleteItem);

            var loadItem = new UserGestureProfileAction();
            loadItem.Name = "Load";
            loadItem.Text = StringParser.Parse("${res:ShortcutsManagement.ShortcutsManagementOptionsPanel.ProfileLoadAction}");
            profilesTextBoxItemsSource.Add(loadItem);

            var createItem = new UserGestureProfileAction();
            createItem.Name = "Create";
            createItem.Text = StringParser.Parse("${res:ShortcutsManagement.ShortcutsManagementOptionsPanel.ProfileCreateAction}");
            profilesTextBoxItemsSource.Add(createItem);

            var renameItem = new UserGestureProfileAction();
            renameItem.Name = "Rename";
            renameItem.Text = StringParser.Parse("${res:ShortcutsManagement.ShortcutsManagementOptionsPanel.ProfileRenameAction}");
            profilesTextBoxItemsSource.Add(renameItem);

            var resetItem = new UserGestureProfileAction();
            resetItem.Name = "Reset";
            resetItem.Text = StringParser.Parse("${res:ShortcutsManagement.ShortcutsManagementOptionsPanel.ProfileResetAction}");
            profilesTextBoxItemsSource.Add(resetItem);

            profilesTextBox.DataContext = profilesTextBoxItemsSource;
        }

        private void BindShortcuts()
        {
            // Root shortcut tree entries
            rootEntries = new List<IShortcutTreeEntry>();

            // Stores SD input binding category to category section convertion map
            var categoriesMap = new Dictionary<InputBindingCategory, ShortcutCategory>();

            var unspecifiedCategory = new ShortcutCategory(StringParser.Parse("${res:ShortcutsManagement.UnspecifiedCategoryName}"));
            rootEntries.Add(unspecifiedCategory);

            // Go through all input bindings
            var inputBindingInfos = CommandManager.FindInputBindingInfos(null, null, null);
            foreach (var inputBindingInfo in inputBindingInfos)
            {
                // Find appropriate or create new category sections within add-in section for input binding
                var shortcutCategorySections = new List<ShortcutCategory>();
                if (inputBindingInfo.Categories.Count == 0)
                {
                    // If no category specified assign to "Uncotegorized" category
                    shortcutCategorySections.Add(unspecifiedCategory);
                }
                else
                {
                    // Go throu all categories and find or create appropriate category sections
                    foreach (var bindingCategory in inputBindingInfo.Categories)
                    {
                        ShortcutCategory categorySection;
                        if (categoriesMap.ContainsKey(bindingCategory))
                        {
                            // If found appropriate category assign shortcut to it
                            categorySection = categoriesMap[bindingCategory];
                        }
                        else
                        {
                            // Create appropriate category section and root category sections

                            // Create direct category to which shortcut will be assigned
                            var categoryName = StringParser.Parse(bindingCategory.Name);
                            categoryName = Regex.Replace(categoryName, @"&([^\s])", @"$1");
                            categorySection = new ShortcutCategory(categoryName);
                            categoriesMap.Add(bindingCategory, categorySection);

                            // Go down to root level and create all parent categories
                            var currentBindingCategory = bindingCategory;
                            var currentShortcutCategory = categorySection;
                            while (currentBindingCategory.ParentCategory != null)
                            {
                                ShortcutCategory parentCategorySection;

                                if (!categoriesMap.ContainsKey(currentBindingCategory.ParentCategory))
                                {
                                    // Create parent category section if it's not created yet
                                    var parentCategoryName = StringParser.Parse(currentBindingCategory.ParentCategory.Name);
                                    parentCategoryName = Regex.Replace(parentCategoryName, @"&([^\s])", @"$1");
                                    parentCategorySection = new ShortcutCategory(parentCategoryName);

                                    categoriesMap.Add(currentBindingCategory.ParentCategory, parentCategorySection);
                                }
                                else
                                {
                                    // Use existing category section as parent category section
                                    parentCategorySection = categoriesMap[currentBindingCategory.ParentCategory];
                                }

                                // Add current category section to root category section children
                                if (!parentCategorySection.SubCategories.Contains(currentShortcutCategory))
                                {
                                    parentCategorySection.SubCategories.Add(currentShortcutCategory);
                                }

                                currentShortcutCategory = parentCategorySection;
                                currentBindingCategory = currentBindingCategory.ParentCategory;
                            }

                            // Add root category section to add-in categories list
                            if (!rootEntries.Contains(currentShortcutCategory))
                            {
                                rootEntries.Add(currentShortcutCategory);
                            }
                        }

                        shortcutCategorySections.Add(categorySection);
                    }
                }

                // Get shortcut entry text. Normaly shortcut entry text is equalt to routed command text
                // but this value can be overriden through InputBindingInfo.RoutedCommandText value
                var shortcutText = inputBindingInfo.RoutedCommand.Text;
                if (!string.IsNullOrEmpty(inputBindingInfo.RoutedCommandText))
                {
                    shortcutText = inputBindingInfo.RoutedCommandText;
                }

                shortcutText = StringParser.Parse(shortcutText);

                // Some commands have "&" sign to mark alternative key used to call this command from menu
                // Strip this sign from shortcut entry text
                shortcutText = Regex.Replace(shortcutText, @"&([^\s])", @"$1");

                var gestures = inputBindingInfo.DefaultGestures;
                if(SelectedProfile != null)
                {
                    var userDefinedGestures = SelectedProfile[inputBindingInfo.Identifier];
                    if(userDefinedGestures != null)
                    {
                        gestures = userDefinedGestures;
                    }
                }
                var shortcut = new Shortcut(shortcutText, gestures);

                // Assign shortcut to all categories it is registered in
                foreach (var categorySection in shortcutCategorySections)
                {
                    categorySection.Shortcuts.Add(shortcut);
                }

                shortcutsMap.Add(shortcut, inputBindingInfo);
            }

            rootEntries.Sort();
            foreach (var entry in rootEntries)
            {
                entry.SortSubEntries();
            }

            new ShortcutsFinder(rootEntries).Filter("");
            shortcutsManagementOptionsPanel.DataContext = rootEntries;
        }

        public bool SaveOptions() 
        {
            foreach (var profile in removedProfiles)
            {
                if(File.Exists(profile.Path))
                {
                    File.Delete(profile.Path);
                }
            }

            UserDefinedGesturesManager.CurrentProfile = SelectedProfile;

            foreach (var pair in shortcutsMap)
            {
                pair.Value.IsModifyed = true;
            }

            CommandManager.InvokeInputBindingUpdateHandlers();

            foreach (var profile in profiles)
            {
                profile.Save();
            }

            return true;
        }

        public object Owner {
            get; set;
        }

        public object Control {
            get { return this; }
        }

        private void profilesTextBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems == null || e.AddedItems.Count == 0)
            {
                return;
            }

            if (!(e.AddedItems[0] is UserGesturesProfile))
            {
                if (e.RemovedItems != null && e.RemovedItems.Count > 0)
                {
                    profilesTextBox.SelectedItem = e.RemovedItems[0];
                }
                else if (profilesTextBox.SelectedIndex >= 0)
                {
                    profilesTextBox.SelectedIndex = -1;
                    profilesTextBox.Text = "";
                }
            }

            var userGestureProfileAction = e.AddedItems[0] as UserGestureProfileAction;
            if (userGestureProfileAction != null)
            {
                if (userGestureProfileAction.Name == "Delete" && e.RemovedItems != null && e.RemovedItems.Count > 0)
                {
                    var result = MessageBox.Show(
                        StringParser.Parse("${res:ShortcutsManagement.ShortcutsManagementOptionsPanel.ConfirmDeleteProfileMessage}"),
                        StringParser.Parse("${res:ShortcutsManagement.ShortcutsManagementOptionsPanel.ConfirmDeleteProfileMessageWindowName}"),
                        MessageBoxButton.YesNo);

                    if(MessageBoxResult.Yes == result)
                    {
                        var removedProfile = (UserGesturesProfile)e.RemovedItems[0];
                        profiles.Remove(removedProfile);
                        removedProfiles.Add(removedProfile);
                        SelectedProfile = null;
                    }
                }

                if (userGestureProfileAction.Name == "Rename" && e.RemovedItems != null && e.RemovedItems.Count > 0)
                {
                    var renamedProfile = e.RemovedItems[0] as UserGesturesProfile;

                    if (renamedProfile != null)
                    {
                        var promptWindow = new CreateNewProfilePrompt();
                        promptWindow.BaseProfilesVisibility = Visibility.Collapsed;
                        promptWindow.Text = renamedProfile.Text;
                        promptWindow.ShowDialog();

                        renamedProfile.Text = promptWindow.Text;
                    }
                } 

                if(userGestureProfileAction.Name == "Load")
                {
                    var openDialog = new OpenFileDialog();
                    openDialog.Filter = "Xml files (*.xml)|*.xml";
                    openDialog.FilterIndex = 1;
                    openDialog.RestoreDirectory = false;
                    if(true == openDialog.ShowDialog()) {
                        var loadedProfile = new UserGesturesProfile();
                        loadedProfile.Path = openDialog.FileName;
                        loadedProfile.Load();

                        loadedProfile.Path = Path.Combine(
                            UserDefinedGesturesManager.UserGestureProfilesDirectory,
                            Guid.NewGuid().ToString());

                        profiles.Add(loadedProfile);
                        SelectedProfile = loadedProfile;
                    }
                }

                if (userGestureProfileAction.Name == "Reset")
                {
                    SelectedProfile = null;
                }

                if(userGestureProfileAction.Name == "Create")
                {
                    var promptWindow = new CreateNewProfilePrompt();
                    promptWindow.AvailableBaseProfiles = profiles;
                    promptWindow.ShowDialog();

                    if (promptWindow.DialogResult == true)
                    {
                        UserGesturesProfile newProfile;

                        if (promptWindow.BaseProfile != null)
                        {
                            newProfile = (UserGesturesProfile) promptWindow.BaseProfile.Clone();
                            newProfile.Name = Guid.NewGuid().ToString();
                            newProfile.Text = promptWindow.Text;
                            newProfile.ReadOnly = false;
                        }
                        else
                        {
                            newProfile = new UserGesturesProfile(Guid.NewGuid().ToString(), promptWindow.Text, false);
                        }

                        newProfile.Path = Path.Combine(UserDefinedGesturesManager.UserGestureProfilesDirectory,
                                                       newProfile.Name + ".xml");
                        profiles.Add(newProfile);

                        SelectedProfile = newProfile;
                    }
                }
            }

            var userGestureProfile = e.AddedItems[0] as UserGesturesProfile;
            if (userGestureProfile != null)
            {
                SelectedProfile = userGestureProfile;
            }

            if (SelectedProfile != null)
            {
                profilesTextBox.Text = SelectedProfile.Text;
                profilesTextBox.SelectedItem = SelectedProfile;
            }
            else
            {
                profilesTextBox.SelectedIndex = -1;
                profilesTextBox.Text = "";
            }

            BindShortcuts();
            BindProfiles(); 
        }

        private void shortcutsManagementOptionsPanel_ShortcutModified(object sender, EventArgs e)
        {
            var selectedShortcut = (Shortcut)shortcutsManagementOptionsPanel.shortcutsTreeView.SelectedItem;

            if(SelectedProfile == null)
            {
                MessageBox.Show(StringParser.Parse("${res:ShortcutsManagement.ShortcutsManagementOptionsPanel.NoProfileSelectedMessage}"));
                return;
            }

            if(SelectedProfile.ReadOnly)
            {
                MessageBox.Show(StringParser.Parse("${res:ShortcutsManagement.ShortcutsManagementOptionsPanel.SelectedProfileIsReadOnlyMessage}"));
                return;
            }

            Shortcut shortcutCopy = null;            
            ICollection<IShortcutTreeEntry> rootEntriesCopy = new ObservableCollection<IShortcutTreeEntry>();

            // Make a deep copy of all add-ins, categories and shortcuts
            foreach (var entry in rootEntries)
            {
                var clonedRootEntry = (IShortcutTreeEntry)entry.Clone();
                rootEntriesCopy.Add(clonedRootEntry);

                // Find copy of modified shortcut in copied add-ins collection
                if (shortcutCopy == null)
                {
                    shortcutCopy = clonedRootEntry.FindShortcut(selectedShortcut.Id);
                }
            }

            var shortcutManagementWindow = new ShortcutManagementWindow(shortcutCopy, rootEntriesCopy);
            shortcutManagementWindow.ShowDialog();

            if (SelectedProfile != null && shortcutManagementWindow.DialogResult.HasValue && shortcutManagementWindow.DialogResult.Value)
            {
                // Move modifications from shortcut copies to original shortcut objects
                foreach (var relatedShortcutCopy in shortcutManagementWindow.ModifiedShortcuts)
                {
                    foreach (var rootEntry in rootEntries)
                    {
                        var originalRelatedShortcut = rootEntry.FindShortcut(relatedShortcutCopy.Id);
                        if (originalRelatedShortcut != null)
                        {
                            originalRelatedShortcut.Gestures.Clear();
                            originalRelatedShortcut.Gestures.AddRange(relatedShortcutCopy.Gestures);

                            var id = shortcutsMap[originalRelatedShortcut].Identifier;
                            SelectedProfile[id] = new InputGestureCollection(relatedShortcutCopy.Gestures);
                        }
                    }
                }
            }
        }

    }
}
