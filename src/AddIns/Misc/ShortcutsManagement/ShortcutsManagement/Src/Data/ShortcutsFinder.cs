using System;
using System.Collections.Generic;
using System.Windows.Input;

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
        public ICollection<AddIn> AddIns
        {
            get; set;
        }

        /// <summary>
        /// Create new instance of <see cref="ShortcutsFinder"/>
        /// </summary>
        /// <param name="addIns"></param>
        public ShortcutsFinder(ICollection<AddIn> addIns) {
            AddIns = addIns;
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
            foreach (var addIn in AddIns) {
                var subCategoryIsVisible = false;
                foreach (var category in addIn.Categories) {
                    if (HideShortcut(category, shortcut)) {
                        subCategoryIsVisible = true;
                    }
                }

                // Hide add-in if it doesn't have any visible sub-elements
                addIn.IsVisible = subCategoryIsVisible;
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
        /// <param name="keyGestureTemplate">Gesture template (Uncompleted gesture)</param>
        /// <param name="strictMatch">If false filter gestures which with only partial match</param>
        public void FilterGesture(KeyGestureTemplate keyGestureTemplate, bool strictMatch)
        {
            FilterGesture(new List<KeyGestureTemplate> { keyGestureTemplate }, strictMatch);
        }

        /// <summary>
        /// Filter gestures matching provided key gesture template
        /// 
        /// Hides add-ins and sub-categories if there are no sub-elements left
        /// </summary>
        /// <param name="keyGestureTemplateCollection">Gesture templates collection (Uncompleted gestures)</param>
        /// <param name="strictMatch">If false filter gestures which with only partial match</param>
        public void FilterGesture(ICollection<KeyGestureTemplate> keyGestureTemplateCollection, bool strictMatch)
        {
            foreach (var addIn in AddIns) {
                var subCategoryIsVisible = false;
                foreach (var category in addIn.Categories) {
                    if (FilterGesture(category, keyGestureTemplateCollection, strictMatch)) {
                        subCategoryIsVisible = true;
                    }
                }

                // Hide add-in if it doesn't have any visible sub-elements
                addIn.IsVisible = subCategoryIsVisible;
            }
        }

        /// <summary>
        /// Filter gestures matching one of provided key gesture templates from templates collection 
        /// </summary>
        /// <param name="category">Category to filter</param>
        /// <param name="keyGestureTemplateCollection">Gesture templates collection (Uncompleted gestures)</param>
        /// <returns></returns>
        private static bool FilterGesture(ShortcutCategory category, IEnumerable<KeyGestureTemplate> keyGestureTemplateCollection, bool strictMatch)
        {
            // Apply filter to sub-categories
            var isSubElementVisible = false;
            foreach (var subCategory in category.SubCategories) {
                if (FilterGesture(subCategory, keyGestureTemplateCollection, strictMatch)) {
                    isSubElementVisible = true;
                }
            }

            // Apply filter to shortcuts
            foreach (var shortcut in category.Shortcuts) {
                var gestureMatched = false;
                foreach (InputGesture gesture in shortcut.Gestures) {
                    foreach (var template in keyGestureTemplateCollection) {
                        if (template == null || (gesture is KeyGesture && template.Matches((KeyGesture)gesture, strictMatch))) {
                            gestureMatched = true;
                            break;
                        }   
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
            foreach (var addIn in AddIns) {
                // If add-in name matches filter string show all sub-elements
                var addInNameContainsFilterString = !string.IsNullOrEmpty(filterString) && addIn.Name.IndexOf(filterString, StringComparison.InvariantCultureIgnoreCase) >= 0;

                // Apply filter to categories
                var subCategoryIsVisible = false;
                foreach (var category in addIn.Categories) {
                    if(Filter(category, filterString, addInNameContainsFilterString ? (bool?) true : null)) {
                        subCategoryIsVisible = true;
                    }
                }

                // If last category in add-in was hidden and addin name does not contain 
                // part of the filter then hide add-in
                addIn.IsVisible = addInNameContainsFilterString || subCategoryIsVisible;
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
                if ((forseMatch.HasValue && forseMatch.Value) || shortcut.Text.IndexOf(filterString, StringComparison.InvariantCultureIgnoreCase) >= 0) {
                    shortcut.IsVisible = true;
                    isSubElementVisible = true;
                }
                else {
                    shortcut.IsVisible = false;
                }
            }

            return category.IsVisible = (forseMatch.HasValue && forseMatch.Value) || isSubElementVisible;
        }
    }
}
