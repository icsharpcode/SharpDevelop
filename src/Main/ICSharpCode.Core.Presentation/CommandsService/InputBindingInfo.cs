using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Reflection;
using CommandManager = System.Windows.Input.CommandManager;
using SDCommandManager = ICSharpCode.Core.Presentation.CommandManager;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Stores details about input binding
	/// </summary>
	public class InputBindingInfo : BindingInfoBase
	{
		/// <summary>
		/// Creates new instance of <see cref="InputBindingInfo"/>
		/// </summary>
		public InputBindingInfo() 
		{
			OldInputBindings = new InputBindingCollection();
			ActiveInputBindings = new InputBindingCollection();
			DefaultGestures = new ObservableInputGestureCollection();
			Categories = new InputBindingCategoryCollection();
			Groups = new BindingGroupCollection();
		}
		
		private ObservableInputGestureCollection _defaultGestures;
		
		/// <summary>
		/// Gestures which triggers this binding
		/// </summary>
		public ObservableInputGestureCollection DefaultGestures { 
			get {
				return _defaultGestures;
			}
			set {
				if(_defaultGestures != null) {
					_defaultGestures.CollectionChanged -= DefaultGestures_CollectionChanged;
				}
				
				if(value != null) {
					value.CollectionChanged += DefaultGestures_CollectionChanged;
				}
				
				var oldGestures = _defaultGestures;
				_defaultGestures = value;
				
				if(IsRegistered && (UserGestureManager.CurrentProfile == null || UserGestureManager.CurrentProfile[Identifier] == null)) {
					var description = new GesturesModificationDescription(
						Identifier, 
						oldGestures != null ? oldGestures.InputGesturesCollection : new InputGestureCollection(),
						value != null ? value.InputGesturesCollection : new InputGestureCollection());
					
					SDCommandManager.InvokeGesturesChanged(this, new NotifyGesturesChangedEventArgs(description));
				}
			}
		}
		
		private void DefaultGestures_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) 
		{
			if(IsRegistered && (UserGestureManager.CurrentProfile == null || UserGestureManager.CurrentProfile[Identifier] == null)) {
				var newGestures = DefaultGestures.InputGesturesCollection;
				var oldGestures = new InputGestureCollection();
				oldGestures.AddRange(newGestures);
				
				if(e.Action == NotifyCollectionChangedAction.Add) {
					if(e.NewItems != null) {
						foreach(InputGesture ng in e.NewItems) { 
							oldGestures.Remove(ng); 
						}
					}
				} else if(e.Action == NotifyCollectionChangedAction.Remove) {
					if(e.OldItems != null) {
						foreach(InputGesture og in e.OldItems) { 
							oldGestures.Add(og); 
						}
					}
				}
				
				var description = new GesturesModificationDescription(Identifier, oldGestures, newGestures);
				SDCommandManager.InvokeGesturesChanged(this, new NotifyGesturesChangedEventArgs(description));
			}
		}
		
		/// <summary>
		/// Gestures which triggers this binding
		/// </summary>
		public InputGestureCollection ActiveGestures { 
			get {
				if(UserGestureManager.CurrentProfile == null 
				   || UserGestureManager.CurrentProfile[Identifier] == null) {
					return DefaultGestures.GetInputGestureCollection();
				} 
				
				return UserGestureManager.CurrentProfile[Identifier];
			}
		}
		
		InputBindingCategoryCollection _categories;
		
		/// <summary>
		/// List of categories associated with input binding 
		/// </summary>
		public InputBindingCategoryCollection Categories
		{
			get {
				return _categories;
			}
			set {
				if(value == null) {
					throw new ArgumentException("Categories collection can not be null");
				}
				
				var oldValue = _categories;
				_categories = value;
				
				SetCollectionChanged<InputBindingCategory>(oldValue, value, Categories_CollectionChanged);
			}
		}
		
		/// <summary>
		/// New input bindings are assigned to owner when <see cref="CommandBindingInfo" /> is modified
		/// </summary>
		public InputBindingCollection ActiveInputBindings
		{
			get; set;
		}
		
		List<UIElement> oldInstances;

		protected override void SetInstanceBindings(ICollection<UIElement> newInstances)
		{
			if(oldInstances != null) {
				foreach(var ownerInstance in oldInstances) {
					foreach(InputBinding binding in OldInputBindings) {
						ownerInstance.InputBindings.Remove(binding);
					}
				}
			}
			
			oldInstances = new List<UIElement>();
	
			if(newInstances != null) {
				foreach(var ownerInstance in newInstances) {
					ownerInstance.InputBindings.AddRange(ActiveInputBindings);
					oldInstances.Add(ownerInstance);
					
					// Sorting input bindings. This may be slow
					if(ownerInstance.InputBindings != null) {
						ownerInstance.InputBindings.SortByChords();
					}
				}
			}
		}
		
		List<Type> oldTypes;
		
		protected override void SetClassBindings(ICollection<Type> newTypes)
		{
			if(oldTypes != null) {
				foreach(var ownerType in oldTypes) {
					foreach(InputBinding binding in OldInputBindings) {
						SDCommandManager.RemoveClassInputBinding(ownerType, binding);
					}
				}
			}
			
			oldTypes = new List<Type>();
			
			if(newTypes != null) {
				foreach(var ownerType in newTypes) {
					foreach(InputBinding binding in ActiveInputBindings) {
						System.Windows.Input.CommandManager.RegisterClassInputBinding(ownerType, binding);
						oldTypes.Add(ownerType);
					}
					
					var fieldInfo = typeof(System.Windows.Input.CommandManager).GetField("_classInputBindings", BindingFlags.Static | BindingFlags.NonPublic);
					var fieldData = (HybridDictionary)fieldInfo.GetValue(null);
					var classInputBindings = (InputBindingCollection)fieldData[ownerType];
				
					// Sorting input bindings. This may be slow
					if(classInputBindings != null) {
						classInputBindings.SortByChords();
					}
				}
			}
		}
		
		/// <summary>
		/// Old input bindings which where assigned to owner when before <see cref="InputBindingInfo" />
		/// was modified.
		/// 
		/// When new <see cref="InputBinding" />s are generated these bindings are removed from the owner
		/// </summary>
		internal InputBindingCollection OldInputBindings
		{
			get; set;
		}
		
		private void Categories_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) 
		{
			if(e.NewItems != null) {
				foreach(InputBindingCategory addedCategory in e.NewItems) {
					if(!SDCommandManager.InputBindingCategories.Contains(addedCategory)) {
						throw new ArgumentException("InputBindingCategory is not registered in CommandManager");
					}
				}
			}
		}
		
		/// <summary>
		/// Re-generate <see cref="InputBinding" /> from <see cref="InputBindingInfo" />
		/// </summary>
		protected override void GenerateBindings() 
		{			
			OldInputBindings = ActiveInputBindings;
			
			ActiveInputBindings = new InputBindingCollection();
			foreach(InputGesture gesture in ActiveGestures) {
				var inputBinding = new InputBinding(RoutedCommand, gesture);
				ActiveInputBindings.Add(inputBinding);
			}
		}
		
		
		public InputBindingIdentifier Identifier {
			get {
				var identifier = new InputBindingIdentifier();
				identifier.OwnerInstanceName = OwnerInstanceName;
				identifier.OwnerTypeName = OwnerTypeName;
				identifier.RoutedCommandName = RoutedCommandName;
				
				return identifier;
			}
		}
	}

}
