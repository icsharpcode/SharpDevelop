using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace ICSharpCode.ShortcutsManagement
{
    /// <summary>
    /// Shortcut
    /// </summary>
	public class Shortcut : INotifyPropertyChanged
    {
		/// <summary>
		/// List of keyboard gestures which will invoke provided action
		/// </summary>
        public InputGestureCollection Gestures
        {
            get;
            private set;
        }

        private string name;
        
        /// <summary>
        /// Shortcut action name
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if(name != value)
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
                if(isVisible != value)
                {
                    isVisible = value;
                    InvokePropertyChanged("IsVisible");
                }
            }
        }

        /// <summary>
        /// Create new shortcut
        /// </summary>
        /// <param name="shortcutName">Shortcut action name</param>
        /// <param name="gestures">Gestures</param>
        public Shortcut(string shortcutName, InputGestureCollection gestures)
        {
            IsVisible = true;
            Name = shortcutName;
            Gestures = new InputGestureCollection();
            if(gestures != null)
            {
                Gestures.AddRange(gestures);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

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
