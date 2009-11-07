using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using ICSharpCode.Core.Presentation;
using System.Linq;
using System.Collections.Specialized;

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
		
		/// <summary>
		/// Gets collection of default input suggested assigned by developer
		/// </summary>
		public ReadOnlyCollection<InputGesture> DefaultGestures
		{
			get;
			private set;
		}
		
		private bool _doesUseDefault;
		
		/// <summary>
		/// Gets or sets value which specifies whether default gestures should be used or those assigned by user
		/// </summary>
		public bool DoesUseDefault
		{
			get {
				return _doesUseDefault;
			}
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
		/// Create new instance of <see cref="Shortcut" />
		/// </summary>
		/// <param name="id">Shortcut id. Used to identify shortcut clones</param>
		/// <param name="gestures">Active gestures</param>
		/// <param name="defaultGestures">Default gestures suggest by developer</param>
		public Shortcut(string shortcutText, InputGestureCollection gestures, InputGestureCollection defaultGestures)
			: this(Guid.NewGuid().ToString(), shortcutText, gestures, defaultGestures)
		{
			
		}
		
		/// <summary>
		/// Create new instance of <see cref="Shortcut" />
		/// </summary>
		/// <param name="id">Shortcut id. Used to identify shortcut clones</param>
		/// <param name="shortcutText">Shortcut action name (displayed to user)</param>
		/// <param name="gestures">Active gestures</param>
		/// <param name="defaultGestures">Default gestures suggest by developer</param>
		public Shortcut(string id, string shortcutText, InputGestureCollection gestures, InputGestureCollection defaultGestures)
		{
			IsVisible = true;
			Id = id;
			Name = shortcutText;
			
			if(defaultGestures != null) {
				var defaultGesturesArray = new InputGesture[defaultGestures.Count]; 
				defaultGestures.CopyTo(defaultGesturesArray, 0);
				DefaultGestures = new ReadOnlyCollection<InputGesture>(defaultGesturesArray);
			} else {
				DefaultGestures = new ReadOnlyCollection<InputGesture>(new List<InputGesture>());
			}
			
			Gestures = new ObservableCollection<InputGesture>();
			if(gestures != null) {
				foreach (InputGesture gesture in gestures) {
					Gestures.Add(gesture);
				}
			}
			
			_doesUseDefault = new InputGestureCollection(Gestures).ContainsTemplateForAny(new InputGestureCollection(DefaultGestures), GestureCompareMode.ExactlyMatches);
			Gestures.CollectionChanged += Gestures_CollectionChanged;
		}

		void Gestures_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			var newDoesUseDefaultValue = new InputGestureCollection(Gestures).ContainsTemplateForAny(new InputGestureCollection(DefaultGestures), GestureCompareMode.ExactlyMatches);
			if(_doesUseDefault != newDoesUseDefaultValue) {
				_doesUseDefault = newDoesUseDefaultValue;
				InvokePropertyChanged("DoesUseDefault");
			}
			
			InvokePropertyChanged("Gestures");
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
		
		/// <summary>
		/// Resets active gestures assigned to this shortcut to default <see cref="InputGestureCollection" /> suggested by developer
		/// </summary>
		public void ResetToDefaults() 
		{
			_doesUseDefault = true;
			
			Gestures.Clear();
			foreach(var gesture in DefaultGestures) {
				Gestures.Add(gesture);
			}
		}
	}
}
