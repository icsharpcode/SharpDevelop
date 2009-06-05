using System.Collections.Generic;
using System.ComponentModel;

namespace ICSharpCode.ShortcutsManagement
{
	/// <summary>
	/// Add-in where shortcuts were registered
	/// </summary>
    public class AddIn : INotifyPropertyChanged
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

        public List<ShortcutCategory> Categories
        {
            get; 
            private set;
        }

        public AddIn(string addInName)
        {
            IsVisible = true;
            Name = addInName;
            Categories = new List<ShortcutCategory>();
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
