using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace ICSharpCode.ShortcutsManagement.Data
{
    /// <summary>
    /// Shortcut
    /// </summary>
	public class Shortcut : INotifyPropertyChanged, ICloneable
    {
		/// <summary>
		/// List of input gestures which will invoke this action
		/// </summary>
        public ObservableCollection<InputGesture> Gestures
        {
            get;
            private set;
        }


        private string _text;
        
        /// <summary>
        /// Shortcut action name (displayed to user)
        /// </summary>
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if(_text != value)
                {
                    _text = value;
                    InvokePropertyChanged("Text");
                }
            }
        }

        /// <summary>
        /// Shortcut Id.
        /// 
        /// This value is used to identify shortcut clones
        /// </summary>
        public string Id
        {
            get; set;
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
                if(isVisible != value) {
                    isVisible = value;
                    InvokePropertyChanged("IsVisible");
                }
            }
        }

        /// <summary>
        /// Create new instance of shortcut
        /// </summary>
        /// <param name="shortcutText">Shortcut action name (displayed to user)</param>
        /// <param name="gestures">Gestures</param>
        public Shortcut(string shortcutText, InputGestureCollection gestures)
        {
            IsVisible = true;
            Id = Guid.NewGuid().ToString();
            Text = shortcutText;

            Gestures = new ObservableCollection<InputGesture>();
            if(gestures != null) {
                foreach (InputGesture gesture in gestures) {
                    Gestures.Add(gesture);
                }
            }

            // On changes in gestures collection notify that whole property has changed
            Gestures.CollectionChanged += delegate { InvokePropertyChanged("Gestures"); };
        }

        /// <summary>
        /// Determines whether provided gesture is already assigned to this action
        /// </summary>
        /// <param name="gesture">Input gesture</param>
        /// <returns>True if provided gesture is assigned to this action. Otherwise false</returns>
        public bool ContainsGesture(InputGesture gesture)
        {
            foreach (var existingGesture in Gestures) {
                if(existingGesture == gesture) {
                    return true;
                }

                if(existingGesture is KeyGesture && gesture is KeyGesture) {
                    var existingKeyGesture = (KeyGesture) existingGesture;
                    var keyGesture = (KeyGesture) gesture;
                    
                    if(existingKeyGesture.Key == keyGesture.Key && existingKeyGesture.Modifiers == keyGesture.Modifiers) {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Make a deep copy of this object
        /// </summary>
        /// <returns>Deep copy of action</returns>
        public object Clone()
        {
            var clone = new Shortcut(Text, null);
            clone.Id = Id;

            foreach (var gesture in Gestures) {
                clone.Gestures.Add(gesture);
            }

            return clone;
        }

        /// <summary>
        /// Invoke dependency property changed event
        /// </summary>
        /// <param name="propertyName">Text of dependency property from this classs</param>
        private void InvokePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Notify observers about property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
