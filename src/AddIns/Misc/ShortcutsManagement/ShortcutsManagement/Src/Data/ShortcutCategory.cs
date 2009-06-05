using System.Collections.Generic;
using System.ComponentModel;

namespace ICSharpCode.ShortcutsManagement
{
	/// <summary>
	/// Shortcut category
	/// </summary>
    public class ShortcutCategory : INotifyPropertyChanged
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
                    InvokePropertyChanged("Name");
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
                if (isVisible != value)
                {
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
        /// Create new category
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
        /// Invoke dependency property changed event
        /// </summary>
        /// <param name="propertyName">Name of dependency property from this classs</param>
        private void InvokePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
