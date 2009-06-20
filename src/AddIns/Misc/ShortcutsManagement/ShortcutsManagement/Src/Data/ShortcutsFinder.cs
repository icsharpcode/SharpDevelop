using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using ICSharpCode.Core.Presentation;
using ICSharpCode.ShortcutsManagement.Extensions;

namespace ICSharpCode.ShortcutsManagement.Data
{
    /// <summary>
    /// Filters and hides shortcuts from add-in collection
    /// </summary>
    public class ShortcutsFinder
    {
        /// <summary>
        /// Collection of add-ins containing shortcuts and shortcut categories
        /// </summary>
        public ICollection<IShortcutTreeEntry> RootEntries
        {
            get; set;
        }

        /// <summary>
        /// Create new instance of <see cref="ShortcutsFinder"/>
        /// </summary>
        /// <param name="rootEntries"></param>
        public ShortcutsFinder(ICollection<IShortcutTreeEntry> rootEntries) {
            RootEntries = rootEntries;
        }

        /// <summary>
        /// Hide shortcut by setting <see cref="Shortcut.IsVisible"/> to false
        /// 
        /// Also this function hides parent categories and Add-in if it has no sub
        /// elements left
        /// </summary>
        /// <param name="shortcut">Shortcut to be heden</param>
        public void HideShortcut(Shortcut shortcut)
        {
            foreach (var entry in RootEntries) {
                var subCategoryIsVisible = false;

                var rootAddIn = entry as AddIn;
                if (rootAddIn != null) {
                    foreach (var category in rootAddIn.Categories) {
                        if (HideShortcut(category, shortcut)) {
                            subCategoryIsVisible = true;
                        }
                    }

                    // Hide add-in if it doesn't have any visible sub-elements
                    rootAddIn.IsVisible = subCategoryIsVisible;
                }

                var rootCategory = entry as ShortcutCategory;
                if(rootCategory != null) {
                    HideShortcut(rootCategory, shortcut);
                }

                var rootShortcut = entry as Shortcut;
                if (rootShortcut != null) {
                    shortcut.IsVisible = false;
                }
            }
        }

        private static bool HideShortcut(ShortcutCategory category, Shortcut filteredShortcut)
        {
            // Check if this shortcut is in subcategories
            var isSubElementVisible = false;
            foreach (var subCategory in category.SubCategories) {
                if (HideShortcut(subCategory, filteredShortcut)) {
                    isSubElementVisible = true;
                }
            }

            // Determine whether provided shortcut is in provided category and 
            // hide it if so
            foreach (var shortcut in category.Shortcuts) {
                if (shortcut == filteredShortcut) {
                    shortcut.IsVisible = false;
                }

                if(shortcut.IsVisible) {
                    isSubElementVisible = true;
                }
            }

            // Hide category if it doesn't have any visible sub-elements
            category.IsVisible = isSubElementVisible;

            return category.IsVisible;
        }

        /// <summary>
        /// Filter gestures matching provided key gesture template
        /// 
        /// Hides add-ins and sub-categories if there are no sub-elements left
        /// </summary>
        /// <param name="inputGestureTemplate">Gesture template which should match shortcut gesture partly to make it visible</param>
        /// <param name="mode">Filtering mode</param>
        public void FilterGesture(InputGesture inputGestureTemplate, GestureFilterMode mode)
        {
            FilterGesture(new InputGestureCollection(new[] { inputGestureTemplate }), mode);
        }

        /// <summary>
        /// Filter gestures matching one of provided gesture templates
        /// 
        /// Hides add-ins and sub-categories if there are no sub-elements left
        /// </summary>
        /// <param name="inputGestureTemplateCollection">Collection of gesture templates which (atleast one) should match shortcut gesture partly to make it visible</param>
        /// <param name="mode">Filtering mode</param>
        public void FilterGesture(InputGestureCollection inputGestureTemplateCollection, GestureFilterMode mode)
        {
            Debug.WriteLine("Changed to" + new InputGestureCollectionConverter().ConvertToInvariantString(inputGestureTemplateCollection));
            foreach (var entry in RootEntries)
            {
                var subCategoryIsVisible = false;
                
                // Filter root addin and sub-elements
                var rootAddIn = entry as AddIn;
                if (rootAddIn != null) {
                    foreach (var category in rootAddIn.Categories) {
                        if (FilterGesture(category, inputGestureTemplateCollection, mode)) {
                            subCategoryIsVisible = true;
                        }
                    }

                    // Hide add-in if it doesn't have any visible sub-elements
                    rootAddIn.IsVisible = subCategoryIsVisible;
                }

                // Filter root category and sub-elements
                var rootCategory = entry as ShortcutCategory;
                if (rootCategory != null) {
                    FilterGesture(rootCategory, inputGestureTemplateCollection, mode);
                }

                // Filter root shortcut
                var rootShortcut = entry as Shortcut;
                if (rootShortcut != null) {
                    rootShortcut.IsVisible = false;
                    foreach (InputGesture template in inputGestureTemplateCollection) {
                        if (template.MatchesCollection(new InputGestureCollection(rootShortcut.Gestures), mode)) {
                            rootShortcut.IsVisible = true;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Filter gestures matching one of provided gesture templates from templates collection 
        /// </summary>
        /// <param name="category">Category to filter</param>
        /// <param name="inputGestureTemplateCollection">Collection of gesture templates which (atleast one) should match shortcut gesture partly to make it visible</param>
        /// <param name="mode">Filtering mode</param>
        /// <returns></returns>
        private static bool FilterGesture(ShortcutCategory category, InputGestureCollection inputGestureTemplateCollection, GestureFilterMode mode)
        {
            // Apply filter to sub-categories
            var isSubElementVisible = false;
            foreach (var subCategory in category.SubCategories) {
                if (FilterGesture(subCategory, inputGestureTemplateCollection, mode))
                {
                    isSubElementVisible = true;
                }
            }

            // Apply filter to shortcuts
            foreach (var shortcut in category.Shortcuts) {
                var gestureMatched = false;
                foreach (InputGesture template in inputGestureTemplateCollection)
                {
                    if (shortcut.Gestures.Count > 0 && ((KeyGesture)shortcut.Gestures[0]).Key == Key.F5)
                    {
                        
                    }

                    if (template.MatchesCollection(new InputGestureCollection(shortcut.Gestures), mode))
                    {
                        gestureMatched = true;
                        break;
                    }   
                }

                if (gestureMatched) {
                    shortcut.IsVisible = true;
                    isSubElementVisible = true;
                }
                else {
                    shortcut.IsVisible = false;
                }
            }

            // Hide category if it doesn't have any visible sub-elements
            category.IsVisible = isSubElementVisible;
            return category.IsVisible;
        }

        /// <summary>
        /// Filter addins, child categories and shortcuts where item name
        /// contains filter string
        /// 
        /// Hides add-ins and sub-categories if there are no sub-elements left
        /// </summary>
        /// <param name="filterString">Filter string</param>
        public void Filter(string filterString)
        {
            foreach (var entry in RootEntries) {
                var rootAddIn = entry as AddIn;
                if (rootAddIn != null) {
                    // If add-in name matches filter string show all sub-elements
                    var addInNameContainsFilterString = !string.IsNullOrEmpty(filterString) && rootAddIn.Name.IndexOf(filterString, StringComparison.InvariantCultureIgnoreCase) >= 0;

                    // Apply filter to categories
                    var subCategoryIsVisible = false;

                    foreach (var category in rootAddIn.Categories) {
                        if (Filter(category, filterString, addInNameContainsFilterString ? (bool?) true : null)) {
                            subCategoryIsVisible = true;
                        }
                    }

                    // If last category in add-in was hidden and addin name does not contain 
                    // part of the filter then hide add-in
                    rootAddIn.IsVisible = addInNameContainsFilterString || subCategoryIsVisible;
                }

                var rootCategory = entry as ShortcutCategory;
                if (rootCategory != null) {
                    Filter(rootCategory, filterString, null);
                }

                var rootShortcut = entry as Shortcut;
                if (rootShortcut != null) {
                    rootShortcut.IsVisible = filterString == null || rootShortcut.Name.IndexOf(filterString, StringComparison.InvariantCultureIgnoreCase) >= 0;
                }
            }
        }
	
        /// <summary>
        /// Filter category and child elements where item name contains 
        /// filter string
        /// </summary>
        /// <param name="category">Category to filter</param>
        /// <param name="filterString">Filter string</param>
        /// <param name="forseMatch">If set to true all sub-elements are visible</param>
        /// <returns></returns>
        private static bool Filter(ShortcutCategory category, string filterString, bool? forseMatch)
        {
            // If category name matches filter show all sub-categories and shortcuts
            if (!string.IsNullOrEmpty(filterString) && category.Name.IndexOf(filterString, StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                forseMatch = true;
            }

            // Apply filter to sub-categories
            var isSubElementVisible = false;
            foreach (var subCategory in category.SubCategories) {
                if(Filter(subCategory, filterString, forseMatch)) {
                    isSubElementVisible = true;
                }
            }

            // Filter shortcuts which text match the filter
            foreach (var shortcut in category.Shortcuts) {
                if ((forseMatch.HasValue && forseMatch.Value) || filterString == null || shortcut.Name.IndexOf(filterString, StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    shortcut.IsVisible = true;
                    isSubElementVisible = true;
                }
                else {
                    shortcut.IsVisible = false;
                }
            }

            // Show category if has sub elements, forced or matches search filter
            return category.IsVisible = (forseMatch.HasValue && forseMatch.Value && (category.SubCategories.Count > 0 || category.Shortcuts.Count > 0)) || isSubElementVisible;
        }
    }
}
