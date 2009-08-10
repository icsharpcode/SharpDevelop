using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;


namespace ICSharpCode.ShortcutsManagement.Data
{
	/// <summary>
	/// Shortcut
	/// </summary>
	public class Shortcut : INotifyPropertyChanged, IShortcutTreeEntry
	{
		/// <summary>
		/// Gets collection of input gestures assigned to this <see cref="Shortcut" /> instance
		/// </summary>
		public ObservableCollection<InputGesture> Gestures
		{
			get;
			private set;
		}
		
		private string _name;
		
		/// <summary>
		/// Gets or sets shortcut name (displayed to user)
		/// </summary>
		public string Name
		{
			get {
				return _name;
			}
			set {
				if(_name != value) {
				    _name = value;
				    InvokePropertyChanged("Name");
				}
			}
		}
		
		/// <summary>
		/// Sorts shortcut sub-elements
		/// </summary>
		public void SortSubEntries() 
		{
		
		}
		
		/// <summary>
		/// Gets or sets shortcut id.
		/// 
		/// This value is used to identify shortcut clones
		/// </summary>
		public string Id
		{
			get; set;
		}
		
		private bool isVisible;
		
		/// <summary>
		/// Gets or sets whether category is visible in shortcuts tree
		/// 
		/// Dependency property
		/// </summary>
		public bool IsVisible
		{
			get {
				return isVisible;
			}
			set {
				if(isVisible != value) {
				    isVisible = value;
				    InvokePropertyChanged("IsVisible");
				}
			}
		}
		
		/// <summary>
		/// Notify observers about property changes
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
		
		/// <summary>
		/// Create new instance of shortcut
		/// </summary>
		/// <param name="shortcutText">Shortcut action name (displayed to user)</param>
		/// <param name="gestures">Gestures</param>
		public Shortcut(string shortcutText, InputGestureCollection gestures)
		{
			IsVisible = true;
			Id = Guid.NewGuid().ToString();
			Name = shortcutText;
			
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
		/// Compare shortcut to other instances of <see cref="IShortcutTreeEntry"/>
		/// </summary>
		/// <param name="obj">Compared object</param>
		/// <returns>Comparison result</returns>
		public int CompareTo(object obj)
		{
			if (obj is AddIn) return 1;
			if (obj is ShortcutCategory) return 1;
			
			var shortcutObj = (Shortcut)obj;
			return Name.CompareTo(shortcutObj.Name);
		}
		
		/// <summary>
		/// Returns this <see cref="Shortcut"/> instance if searched id matches
		/// </summary>
		/// <param name="shortcutId">Searched shortcut id</param>
		/// <returns>Found shortcut instance or null</returns>
		public Shortcut FindShortcut(string shortcutId) {
			return Id == shortcutId ? this : null;
		}
		
		/// <summary>
		/// Invoke dependency property changed event
		/// </summary>
		/// <param name="propertyName">Name of dependency property from this classs</param>
		private void InvokePropertyChanged(string propertyName)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
