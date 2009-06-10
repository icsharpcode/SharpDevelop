using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ICSharpCode.ShortcutsManagement.Data
{
	/// <summary>
	/// Shortcut category
	/// </summary>
    public class ShortcutCategory : INotifyPropertyChanged, ICloneable
    {
        private string name;
        
        /// <summary>
        /// Category name
        /// 
        /// Dependency property
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                    InvokePropertyChanged("Text");
                }
            }
        }

        private bool isVisible;
        
        /// <summary>
        /// Is category visible in shortcuts tree
        /// 
        /// Dependency property
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return isVisible;
            }
            set
            {
                if (isVisible != value) {
                    isVisible = value;
                    InvokePropertyChanged("IsVisible");
                }
            }
        }

        /// <summary>
        /// Sub cateories
        /// </summary>
        public List<ShortcutCategory> SubCategories
        {
            get;
            private set;
        }

        /// <summary>
        /// Shortcuts assigned to this category
        /// </summary>
        public List<Shortcut> Shortcuts
        {
            get;
            private set;
        }

        /// <summary>
        /// Create new instance of category
        /// </summary>
        /// <param name="categoryName">Category name</param>
        public ShortcutCategory(string categoryName)
        {
            IsVisible = true;
            Shortcuts = new List<Shortcut>(); 
            SubCategories = new List<ShortcutCategory>();
            Name = categoryName;
        }

	    /// <summary>
	    /// Make a deep copy of category object
	    /// </summary>
	    /// <returns>Deep copy of this category</returns>
        public object Clone()
	    {
	        var clonedCategory = new ShortcutCategory(Name);
	        
            foreach (var subCategory in SubCategories) {
	            clonedCategory.SubCategories.Add((ShortcutCategory)subCategory.Clone());
	        }

            foreach (var shortcut in Shortcuts) {
                clonedCategory.Shortcuts.Add((Shortcut)shortcut.Clone());
            }

	        return clonedCategory;
	    }

        /// <summary>
        /// Find shortcut shortcut by ID in this category and subcategories
        /// </summary>
        /// <param name="shortcutId">Shortcut ID</param>
        /// <returns>Shortcut with ID equal to provided one</returns>
        public Shortcut FindShortcut(string shortcutId)
        {
            // Search for shortcut in shortcuts assigned to this category
            foreach (var s in Shortcuts) {
                if(s.Id == shortcutId) {
                    return s;
                }
            }

            // Search for shortcut in sub categories
            foreach (var category in SubCategories) {
                Shortcut foundShortcut;
                if ((foundShortcut = category.FindShortcut(shortcutId)) != null) {
                    return foundShortcut;
                }
            }

            return null;
        }

        /// <summary>
        /// Invoke dependency property changed event
        /// </summary>
        /// <param name="propertyName">Text of dependency property from this classs</param>
        private void InvokePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Notify observers about property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
