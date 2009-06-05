using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ICSharpCode.Core.Presentation;

namespace ICSharpCode.ShortcutsManagement
{
    public static class ShortcutsProvider
    {
        /// <summary>
        /// Filter addins containing shortcuts with matching gesture
        /// </summary>
        /// <param name="keyGestureTemplate">Gesture template (Uncompleted gesture)</param>
        public static void FilterGesture(ICollection<AddIn> addins, KeyGestureTemplate keyGestureTemplate, bool strictMatch)
        {
            FilterGesture(addins, new List<KeyGestureTemplate> {keyGestureTemplate}, strictMatch);
        }

        /// <summary>
        /// Filter addins containing shortcuts with matching gesture
        /// </summary>
        /// <param name="keyGestureTemplate">Gesture template (Uncompleted gesture)</param>
        public static void FilterGesture(ICollection<AddIn> addins, ICollection<KeyGestureTemplate> keyGestureTemplateCollection, bool strictMatch)
        {
            foreach (var addIn in addins)
            {
                var subCategoryIsVisible = false;
                foreach (var category in addIn.Categories)
                {
                    if (FilterGesture(category, keyGestureTemplateCollection, strictMatch))
                    {
                        subCategoryIsVisible = true;
                    }
                }

                addIn.IsVisible = subCategoryIsVisible;
            }
        }

        /// <summary>
        /// Filter categories containing shortcuts with matching gesture
        /// </summary>
        /// <param name="category">Category to filter</param>
        /// <param name="keyGestureTemplate">Uncompleted gesture</param>
        /// <returns></returns>
        private static bool FilterGesture(ShortcutCategory category, ICollection<KeyGestureTemplate> keyGestureTemplateCollection, bool strictMatch)
        {
            var isSubElementVisible = false;
            foreach (var subCategory in category.SubCategories)
            {
                if (FilterGesture(subCategory, keyGestureTemplateCollection, strictMatch))
                {
                    isSubElementVisible = true;
                }
            }

            foreach (var shortcut in category.Shortcuts)
            {
                var gestureMatched = false;
                foreach (InputGesture gesture in shortcut.Gestures)
                {
                    foreach (var template in keyGestureTemplateCollection)
                    {
                        if (template == null || (gesture is KeyGesture && template.Matches((KeyGesture)gesture, strictMatch)))
                        {
                            gestureMatched = true;
                            break;
                        }   
                    }
                }

                if (gestureMatched)
                {
                    shortcut.IsVisible = true;
                    isSubElementVisible = true;
                }
                else
                {
                    shortcut.IsVisible = false;
                }
            }

            return category.IsVisible = isSubElementVisible;
        }

        /// <summary>
        /// Filter addins, child categories and shortcuts where item name
        /// contains filter string
        /// </summary>
        /// <param name="filterString">Filter string</param>
        public static void Filter(ICollection<AddIn> addins, string filterString)
        {
            foreach (var addIn in addins)
            {
                var addInNameContainsFilterString = addIn.Name.IndexOf(filterString, StringComparison.InvariantCultureIgnoreCase) >= 0;

                var subCategoryIsVisible = false;
                foreach (var category in addIn.Categories)
                {
                    if(Filter(category, filterString, addInNameContainsFilterString ? (bool?) true : null))
                    {
                        subCategoryIsVisible = true;
                    }
                }

                addIn.IsVisible = addInNameContainsFilterString || subCategoryIsVisible;
            }
        }
	
        /// <summary>
        /// Filter category and child elements where item name contains 
        /// filter string
        /// </summary>
        /// <param name="category">Category to filter</param>
        /// <param name="filterString">Filter string</param>
        /// <param name="forseMatch">If set to true all sub elements are expanded</param>
        /// <returns></returns>
        private static bool Filter(ShortcutCategory category, string filterString, bool? forseMatch)
        {
            if(category.Name.IndexOf(filterString, StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                forseMatch = true;
            }

            var isSubElementVisible = false;
            foreach (var subCategory in category.SubCategories)
            {
                if(Filter(subCategory, filterString, forseMatch))
                {
                    isSubElementVisible = true;
                }
            }

            foreach (var shortcut in category.Shortcuts)
            {
                if (
                     (forseMatch.HasValue && forseMatch.Value) 
                  || shortcut.Name.IndexOf(filterString, StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    shortcut.IsVisible = true;
                    isSubElementVisible = true;
                }
                else
                {
                    shortcut.IsVisible = false;
                }
            }

            return category.IsVisible = (forseMatch.HasValue && forseMatch.Value) || isSubElementVisible;
        }
    }
}
