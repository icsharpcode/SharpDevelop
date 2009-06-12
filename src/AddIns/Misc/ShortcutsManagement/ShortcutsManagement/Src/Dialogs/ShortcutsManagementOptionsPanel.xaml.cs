using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop;
using ICSharpCode.ShortcutsManagement.Data;
using AddIn=ICSharpCode.Core.AddIn;
using ShortcutManagement=ICSharpCode.ShortcutsManagement.Data;

namespace ICSharpCode.ShortcutsManagement
{
    /// <summary>
    /// Interaction logic for ShortcutsManagementOptionsPanel.xaml
    /// </summary>
    public partial class ShortcutsManagementOptionsPanel : UserControl, IOptionPanel
    {
        /// <summary>
        /// Stores shortcut entry to input binding convertion map
        /// </summary>
        private readonly Dictionary<Shortcut, InputBindingInfo> shortcutsMap = new Dictionary<Shortcut, InputBindingInfo>();

        public ShortcutsManagementOptionsPanel()
        {
            ResourceService.RegisterStrings("ICSharpCode.ShortcutsManagement.Resources.StringResources", GetType().Assembly);

            InitializeComponent();
        }

        public void LoadOptions()
        {
            // Root shortcut tree entries
            var rootEntries = new List<IShortcutTreeEntry>(); 

            // Stores SD add-in to add-in section convertion map
            var addInsMap = new Dictionary<AddIn, ShortcutManagement.AddIn>();

            // Stores SD input binding category to category section convertion map
            var categoriesMap = new Dictionary<ShortcutManagement.AddIn, Dictionary<InputBindingCategory, ShortcutCategory>>();

            // Create default add-in for input bindings which don't specify add-in
            var unspecifiedAddInSection = new ShortcutManagement.AddIn(StringParser.Parse("${res:ShortcutsManagement.UnspecifiedAddInName}"));
            unspecifiedAddInSection.Categories.Add(new ShortcutCategory(StringParser.Parse("${res:ShortcutsManagement.UnspecifiedCategoryName}")));
            rootEntries.Add(unspecifiedAddInSection);

            // Go through all input bindings
            var inputBindingInfos = CommandsRegistry.FindInputBindingInfos(null, null, null);
            foreach(var inputBindingInfo in inputBindingInfos) {
                // Find appropriate or create new add-in section for input binding
                ShortcutManagement.AddIn addinSection;
                if (inputBindingInfo.AddIn == null) {
                    addinSection = unspecifiedAddInSection;
                } else if (addInsMap.ContainsKey(inputBindingInfo.AddIn)) {
                    addinSection = addInsMap[inputBindingInfo.AddIn];
                } else {
                    addinSection = new ShortcutManagement.AddIn(inputBindingInfo.AddIn.Name);
                    addinSection.Categories.Add(new ShortcutCategory(StringParser.Parse("${res:ShortcutsManagement.UnspecifiedCategoryName}")));
                    addInsMap.Add(inputBindingInfo.AddIn, addinSection);
                    categoriesMap.Add(addinSection, new Dictionary<InputBindingCategory, ShortcutCategory>());
                    rootEntries.Add(addinSection);
                }

                // Find appropriate or create new category sections within add-in section for input binding
                var shortcutCategorySections = new List<ShortcutCategory>();
                if (inputBindingInfo.Categories.Count == 0) {
                    // If no category specified assign to "Uncotegorized" category
                    shortcutCategorySections.Add(addinSection.Categories[0]);
                } else {
                    // Go throu all categories and find or create appropriate category sections
                    foreach (var bindingCategory in inputBindingInfo.Categories) {
                        ShortcutCategory categorySection;
                        if (categoriesMap[addinSection].ContainsKey(bindingCategory)) {
                            // If found appropriate category assign shortcut to it
                            categorySection = categoriesMap[addinSection][bindingCategory];
                        } else {
                            // Create appropriate category section and root category sections
                            
                            // Create direct category to which shortcut will be assigned
                            var categoryName = StringParser.Parse(bindingCategory.Name);
                            categorySection = new ShortcutCategory(categoryName);
                            categoriesMap[addinSection].Add(bindingCategory, categorySection);

                            // Go down to root level and create all parent categories
                            var currentBindingCategory = bindingCategory;
                            var currentShortcutCategory = categorySection;
                            while (currentBindingCategory.ParentCategory != null) {
                                ShortcutCategory parentCategorySection;

                                if (!categoriesMap[addinSection].ContainsKey(currentBindingCategory.ParentCategory)) {
                                    // Create parent category section if it's not created yet
                                    var parentCategoryName = StringParser.Parse(currentBindingCategory.ParentCategory.Name);
                                    parentCategorySection = new ShortcutCategory(parentCategoryName);

                                    categoriesMap[addinSection].Add(currentBindingCategory.ParentCategory, parentCategorySection);
                                } else {
                                    // Use existing category section as parent category section
                                    parentCategorySection = categoriesMap[addinSection][currentBindingCategory.ParentCategory];
                                }

                                // Add current category section to root category section children
                                if (!parentCategorySection.SubCategories.Contains(currentShortcutCategory)) {
                                    parentCategorySection.SubCategories.Add(currentShortcutCategory);
                                }

                                currentShortcutCategory = parentCategorySection;
                                currentBindingCategory = currentBindingCategory.ParentCategory;
                            }

                            // Add root category section to add-in categories list
                            if (!addinSection.Categories.Contains(currentShortcutCategory)) {
                                addinSection.Categories.Add(currentShortcutCategory);
                            }
                        }

                        shortcutCategorySections.Add(categorySection);
                    }
                }

                // Get shortcut entry text. Normaly shortcut entry text is equalt to routed command text
                // but this value can be overriden through InputBindingInfo.RoutedCommandText value
                var shortcutText = inputBindingInfo.RoutedCommand.Text;
                if (!string.IsNullOrEmpty(inputBindingInfo.RoutedCommandText)) {
                    shortcutText = inputBindingInfo.RoutedCommandText;
                }

                shortcutText = StringParser.Parse(shortcutText);

                // Some commands have "&" sign to mark alternative key used to call this command from menu
                // Strip this sign from shortcut entry text
                shortcutText = Regex.Replace(shortcutText, @"&([^\s])", @"$1");

                var shortcut = new Shortcut(shortcutText, inputBindingInfo.Gestures);
                
                // Assign shortcut to all categories it is registered in
                foreach (var categorySection in shortcutCategorySections) {
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
            foreach (var pair in shortcutsMap) {
                var shortcut = pair.Key;
                var inputBindingInfo = pair.Value;

                inputBindingInfo.Gestures = new InputGestureCollection(shortcut.Gestures);
            }

            CommandsRegistry.InvokeInputBindingUpdateHandlers(null, null);

            return true;
        }

        public object Owner {
            get; set;
        }

        public object Control {
            get { return this; }
        }
    }
}
