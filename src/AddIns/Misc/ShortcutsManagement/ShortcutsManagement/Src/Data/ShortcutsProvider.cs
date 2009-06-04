using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Input;
using ICSharpCode.Core.Presentation;

namespace ICSharpCode.ShortcutsManagement
{
    public class ShortcutsProvider
    {
        private readonly ObservableCollection<AddIn> addins = new ObservableCollection<AddIn>();
        
        /// <summary>
        /// Get list of add-ins containing shortcuts
        /// </summary>
        /// <returns>List of add-ins</returns>
        public ObservableCollection<AddIn> GetAddIns()
        {
            return addins;
        }

        /// <summary>
        /// Filter addins containing shortcuts with matching gesture
        /// </summary>
        /// <param name="keyEventArgs">Key event</param>
        public void FilterGesture(InputEventArgs keyEventArgs)
        {
            foreach (var addIn in addins)
            {
                var subCategoryIsVisible = false;
                foreach (var category in addIn.Categories)
                {
                    if (FilterGesture(category, keyEventArgs))
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
        /// <param name="keyEventArgs">Key event</param>
        /// <returns></returns>
        private static bool FilterGesture(ShortcutCategory category, InputEventArgs keyEventArgs)
        {
            var isSubElementVisible = false;
            foreach (var subCategory in category.SubCategories)
            {
                if (FilterGesture(subCategory, keyEventArgs))
                {
                    isSubElementVisible = true;
                }
            }

            foreach (var shortcut in category.Shortcuts)
            {
                var gestureMatched = false;
                foreach (InputGesture gesture in shortcut.Gestures)
                {
                    if(gesture.Matches(null, keyEventArgs))
                    {
                        gestureMatched = true;
                        break;
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
        public void Filter(string filterString)
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

        private static InputGestureCollection GetGestures(string gesturesString)
        {
            var converter = new InputGestureCollectionConverter();
            return (InputGestureCollection)converter.ConvertFromInvariantString(gesturesString);
        }

        public ShortcutsProvider()
        {
        	// Test data
            addins.Add(new AddIn("SharpDevelop"));
                addins[0].Categories.Add(new ShortcutCategory("Editing"));
                    addins[0].Categories[0].Shortcuts.Add(new Shortcut("Copy",  GetGestures("Ctrl + C")));
                    addins[0].Categories[0].Shortcuts.Add(new Shortcut("Paste", GetGestures("Ctrl + V | Ctrl+Insert")));
                    addins[0].Categories[0].Shortcuts.Add(new Shortcut("Cut",   GetGestures("Ctrl + X")));
                    addins[0].Categories[0].Shortcuts.Add(new Shortcut("Undo",  GetGestures("Ctrl + Z")));
                    addins[0].Categories[0].Shortcuts.Add(new Shortcut("Redo",  GetGestures("Ctrl + Y")));
                addins[0].Categories.Add(new ShortcutCategory("Building"));
                    addins[0].Categories[1].Shortcuts.Add(new Shortcut("Build", GetGestures("Ctrl + Shift+B")));
                    addins[0].Categories[1].Shortcuts.Add(new Shortcut("Run",   GetGestures("F5")));
                    addins[0].Categories[1].Shortcuts.Add(new Shortcut("Run without debuger", GetGestures("Ctrl + F5")));
                    addins[0].Categories[1].Shortcuts.Add(new Shortcut("Attach debuger", GetGestures("Ctrl + F8")));
                addins[0].Categories.Add(new ShortcutCategory("Uncategorized"));
                    addins[0].Categories[2].Shortcuts.Add(new Shortcut("Attach debuger", GetGestures("Ctrl + F8")));


            addins.Add(new AddIn("Search & replace"));
                addins[1].Categories.Add(new ShortcutCategory("Uncategorized"));
                    addins[1].Categories[0].Shortcuts.Add(new Shortcut("Quick find", GetGestures("Ctrl + F")));
                    addins[1].Categories[0].Shortcuts.Add(new Shortcut("Quick replace", GetGestures("Ctrl + H")));
                    addins[1].Categories[0].Shortcuts.Add(new Shortcut("Find in files", GetGestures("Ctrl + Shift + F | Ctrl + Shift + H | Ctrl + I")));
                    addins[1].Categories[0].Shortcuts.Add(new Shortcut("Replace in files", GetGestures("Ctrl + Shift + H")));
                    addins[1].Categories[0].Shortcuts.Add(new Shortcut("Find symbol", null));

            addins.Add(new AddIn("Unspecified"));
                addins[2].Categories.Add(new ShortcutCategory("Uncategorized"));
                    addins[2].Categories[0].Shortcuts.Add(new Shortcut("Test regex expression", null));
        }
    }
}
