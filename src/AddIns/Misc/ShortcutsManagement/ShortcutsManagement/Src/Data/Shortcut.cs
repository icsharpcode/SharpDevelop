using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace ICSharpCode.ShortcutsManagement
{
    /// <summary>
    /// Shortcut
    /// </summary>
	public class Shortcut : INotifyPropertyChanged, ICloneable
    {
        private ObservableCollection<InputGesture> gestures;

		/// <summary>
		/// List of keyboard gestures which will invoke provided action
		/// </summary>
        public ObservableCollection<InputGesture> Gestures
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
            Gestures = new ObservableCollection<InputGesture>();
            if(gestures != null)
            {
                foreach (InputGesture gesture in gestures)
                {
                    Gestures.Add(gesture);
                }
            }

            Gestures.CollectionChanged += delegate { InvokePropertyChanged("Gestures"); };
        }

        public bool ContainsGesture(InputGesture gesture)
        {
            foreach (var existingGesture in Gestures)
            {
                if(existingGesture == gesture)
                {
                    return true;
                }

                if(existingGesture is KeyGesture && gesture is KeyGesture)
                {
                    var existingKeyGesture = (KeyGesture) existingGesture;
                    var keyGesture = (KeyGesture) gesture;
                    
                    if(existingKeyGesture.Key == keyGesture.Key && existingKeyGesture.Modifiers == keyGesture.Modifiers)
                    {
                        return true;
                    }
                }
            }

            return false;
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

        public object Clone()
        {
            var clone = new Shortcut(Name, null);
            foreach (var gesture in Gestures)
            {
                clone.Gestures.Add(gesture);
            }

            return clone;
        }
    }
}
