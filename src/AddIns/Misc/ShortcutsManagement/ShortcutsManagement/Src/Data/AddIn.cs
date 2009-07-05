using System.Collections.Generic;
using System.ComponentModel;

namespace ICSharpCode.ShortcutsManagement.Data
{
	/// <summary>
	/// Add-in where shortcuts were registered
	/// </summary>
    public class AddIn : INotifyPropertyChanged, IShortcutTreeEntry
    {
        private string name;
        
        /// <summary>
        /// Add-in name
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
                if (name != value) {
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
                if (isVisible != value) {
                    isVisible = value;
                    InvokePropertyChanged("IsVisible");
                }
            }
        }

        /// <summary>
        /// List of add-in categories
        /// </summary>
        public List<ShortcutCategory> Categories
        {
            get; 
            private set;
        }

        /// <summary>
        /// Create new instance of add-in
        /// </summary>
        /// <param name="addInName">Add-in name</param>
        public AddIn(string addInName)
        {
            IsVisible = true;
            Name = addInName;
            Categories = new List<ShortcutCategory>();
        }

        /// <summary>
        /// Find shortcut by shortcutId in add-in categories 
        /// </summary>
        /// <param name="shortcutId"></param>
        /// <returns></returns>
        public Shortcut FindShortcut(string shortcutId)
        {
            foreach (var c in Categories) {
                Shortcut foundShortcut;
                if ((foundShortcut = c.FindShortcut(shortcutId)) != null) {
                    return foundShortcut;
                }
            }

            return null;
        }

        /// <summary>
        /// Clone add-in
        /// </summary>
        /// <returns>Deep copy of add-in</returns>
	    public object Clone()
	    {
	        var clonedAddIn = new AddIn(Name);
	        foreach (var category in Categories) {
	            clonedAddIn.Categories.Add((ShortcutCategory)category.Clone());
	        }

	        return clonedAddIn;
	    }

        /// <summary>
        /// Sort add-in sub categories
        /// </summary>
        public void SortSubEntries() 
        {
            Categories.Sort((a, b) => a.Name.CompareTo(b.Name));
            foreach (var category in Categories)
            {
                category.SortSubEntries();
            }
        }

        /// <summary>
        /// Notify observers about property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Compare add-in to other instances of <see cref="IShortcutTreeEntry"/>
        /// </summary>
        /// <param name="obj">Compared object</param>
        /// <returns>Comparison result</returns>
	    public int CompareTo(object obj) {
            if (obj is ShortcutCategory) return -1;
            if (obj is Shortcut) return -1;

            var addInObj = (AddIn)obj;
	        return Name.CompareTo(addInObj.Name);
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
    }
}
