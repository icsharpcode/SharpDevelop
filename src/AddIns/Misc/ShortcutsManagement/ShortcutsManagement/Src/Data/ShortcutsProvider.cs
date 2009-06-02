using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ICSharpCode.ShortcutsManagement
{
    internal class ShortcutsProvider
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
                if ((forseMatch.HasValue && forseMatch.Value) || shortcut.Name.IndexOf(filterString, StringComparison.InvariantCultureIgnoreCase) >= 0)
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

        public ShortcutsProvider()
        {
        	// Test data
            addins.Add(new AddIn("SharpDevelop"));
                addins[0].Categories.Add(new ShortcutCategory("Editing"));
                    addins[0].Categories[0].Shortcuts.Add(new Shortcut("Copy", new List<string> { "Ctrl+C" }));
                    addins[0].Categories[0].Shortcuts.Add(new Shortcut("Paste", new List<string> { "Ctrl+V", "Ctrl+Insert" }));
                    addins[0].Categories[0].Shortcuts.Add(new Shortcut("Cut", new List<string> { "Ctrl+X" }));
                    addins[0].Categories[0].Shortcuts.Add(new Shortcut("Undo", new List<string> { "Ctrl+Z" }));
                    addins[0].Categories[0].Shortcuts.Add(new Shortcut("Redo", new List<string> { "Ctrl+Y" }));
                addins[0].Categories.Add(new ShortcutCategory("Building"));
                    addins[0].Categories[1].Shortcuts.Add(new Shortcut("Build", new List<string> { "Ctrl+Shift+B" }));
                    addins[0].Categories[1].Shortcuts.Add(new Shortcut("Run", new List<string> { "F5" }));
                    addins[0].Categories[1].Shortcuts.Add(new Shortcut("Run without debuger", new List<string> { "Ctrl+F5" }));
                    addins[0].Categories[1].Shortcuts.Add(new Shortcut("Attach debuger", new List<string> { "Ctrl+F8" }));
                addins[0].Categories.Add(new ShortcutCategory("Uncategorized"));
                    addins[0].Categories[2].Shortcuts.Add(new Shortcut("Attach debuger", new List<string> { "Ctrl+F8" }));


            addins.Add(new AddIn("Search & replace"));
                addins[1].Categories.Add(new ShortcutCategory("Uncategorized"));
                    addins[1].Categories[0].Shortcuts.Add(new Shortcut("Quick find", new List<string> { "Ctrl+F" }));
                    addins[1].Categories[0].Shortcuts.Add(new Shortcut("Quick replace", new List<string> { "Ctrl+H" }));
                    addins[1].Categories[0].Shortcuts.Add(new Shortcut("Find in files", new List<string> { "Ctrl+Shift+F" }));
                    addins[1].Categories[0].Shortcuts.Add(new Shortcut("Replace in files", new List<string> { "Ctrl+Shift+H" }));
                    addins[1].Categories[0].Shortcuts.Add(new Shortcut("Find symbol", null));

            addins.Add(new AddIn("Unspecified"));
                addins[2].Categories.Add(new ShortcutCategory("Uncategorized"));
                    addins[2].Categories[0].Shortcuts.Add(new Shortcut("Test regex expression", null));
        }
    }
}
