using System;
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
	public class InputBindingInfo : IBindingInfo
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
		
		private BindingGroupCollection _groups;
		
		public BindingGroupCollection Groups
		{
			get {
				return _groups;
			}
			set {
				if(value == null) {
					throw new ArgumentException("Groups collection can not be null");
				}

				var oldValue = _groups;
				_groups = value;
				_groups.CollectionChanged += Groups_CollectionChanged;	
				
				if(oldValue != null) {
					var oldItemsList = new System.Collections.ArrayList();
					foreach(var oldItem in oldValue) {
						oldItemsList.Add(oldItem);
					}
					
					var newItemsList = new System.Collections.ArrayList();
					foreach(var newItem in value) {
						newItemsList.Add(newItem);
					}
					
					var args = new NotifyCollectionChangedEventArgs(
						NotifyCollectionChangedAction.Replace,
						newItemsList, 
						oldItemsList,
						0);
					
					Groups_CollectionChanged(this, args);
				}
			}
		}
		
		public string _ownerInstanceName;
		
		/// <summary>
		/// Stores name of named instance to which this binding belongs. When this binding is registered a
		/// <see cref="InputBinding" /> is assigned to owner instance
		/// 
		/// If this attribute is used <see cref="OwnerInstance" />, <see cref="OwnerType" /> and
		/// <see cref="OwnerTypeName" /> can not be set
		/// </summary>
		public string OwnerInstanceName {
			get {
				return _ownerInstanceName;
			}
			set {
				var backup = _ownerInstanceName;
				
				if(_ownerInstanceName != null || _ownerTypeName != null) {
					throw new ArgumentException("This binding already has an owner");
				}
				
				_ownerInstanceName = value;
				
				SetActiveGesturesChanged(RoutedCommandName, _ownerTypeName, OwnerTypeName);
			}
		}
		
		/// <summary>
		/// Stores owner instance to which this binding belongs. When this binding is registered a
		/// <see cref="InputBinding" /> is assigned to owner instance
		/// 
		/// If this attribute is used <see cref="OwnerInstanceName" />, <see cref="OwnerType" /> and
		/// <see cref="OwnerTypeName" /> can not be set
		/// </summary>
		public ICollection<UIElement> OwnerInstances {
			get {
				if(_ownerInstanceName != null) {
					return SDCommandManager.GetNamedUIElementCollection(_ownerInstanceName);
				}
				
				return null;
			}
		}
					
		private string _ownerTypeName;
		
		/// <summary>
		/// Stores name of owner type. Full name with assembly should be used. When this binding is 
		/// registered <see cref="InputBinding" /> is assigned to all instances of provided class
		/// 
		/// If this attribute is used <see cref="OwnerInstance" />, <see cref="OwnerInstanceName" /> and
		/// <see cref="OwnerType" /> can not be set
		/// </summary>
		public string OwnerTypeName 
		{
			get {
				return _ownerTypeName;
			}
			set {
				var backup = _ownerTypeName;
				
				if(_ownerInstanceName != null || _ownerTypeName != null) {
					throw new ArgumentException("This binding already has an owner");
				}
				
				_ownerTypeName = value;
				SetActiveGesturesChanged(RoutedCommandName, OwnerInstanceName, _ownerTypeName);
			}
		}
		
		/// <summary>
		/// Stores owner type. When this binding is registered <see cref="InputBinding" /> 
		/// is assigned to all instances of provided class
		/// 
		/// If this attribute is used <see cref="OwnerInstance" />, <see cref="OwnerInstanceName" /> and
		/// <see cref="OwnerTypeName" /> can not be set
		/// </summary>
		public ICollection<Type> OwnerTypes { 
			get {
				if(_ownerTypeName != null) {
					return SDCommandManager.GetNamedUITypeCollection(_ownerTypeName);
				}
				
				return null;
			}
		}
		
		/// <summary>
		/// Routed command text
		/// 
		/// Override routed command text when displaying to user
		/// </summary>
		/// <seealso cref="RoutedCommand"></seealso>
		public string RoutedCommandText { 
			get; set;
		}
		
		/// <summary>
		/// Add-in to which registered this input binding
		/// </summary>
		public AddIn AddIn {
			get; set;
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
				_defaultGestures = value;
				
				if(value != null)
				{
					_defaultGestures.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e) { 
						// Check for active profile but no custom shortcut
						if(UserDefinedGesturesManager.CurrentProfile == null) {
							var innerArgs = new InputBindingGesturesChangedArgs();
							innerArgs.InputBindingIdentifier = Identifier;
							
							ActiveGesturesChanged(this, innerArgs);
						}
					};
				}
				
				if(routedCommandName != null && (_ownerInstanceName != null || _ownerTypeName != null)) {
					var args = new InputBindingGesturesChangedArgs();
					args.InputBindingIdentifier = Identifier;
					if(UserDefinedGesturesManager.CurrentProfile == null) {	
						ActiveGesturesChanged(this, args);
					}
				}
			}
		}
		
		/// <summary>
		/// Gestures which triggers this binding
		/// </summary>
		public InputGestureCollection ActiveGestures { 
			get {
				if(UserDefinedGesturesManager.CurrentProfile == null 
				   || UserDefinedGesturesManager.CurrentProfile[Identifier] == null) {
					return DefaultGestures.GetInputGestureCollection();
				} 
				
				return UserDefinedGesturesManager.CurrentProfile[Identifier];
			}
		}
		
		private string routedCommandName;
		
		/// <summary>
		/// Name of the routed command which will be invoked when this binding is triggered
		/// </summary>
		public string RoutedCommandName { 
			get {
				return routedCommandName;
			}
			set {
				var backup = routedCommandName;
				routedCommandName = value;
				SetActiveGesturesChanged(backup, OwnerInstanceName, OwnerTypeName);
			}
		}
		
		/// <summary>
		/// Routed command instance which will be invoked when this binding is triggered
		/// </summary>
		public RoutedUICommand RoutedCommand { 
			get {
				return SDCommandManager.GetRoutedUICommand(RoutedCommandName);
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
				_categories.CollectionChanged += Categories_CollectionChanged;	
				
				if(oldValue != null) {
					var oldItemsList = new System.Collections.ArrayList();
					foreach(var oldItem in oldValue) {
						oldItemsList.Add(oldItem);
					}
					
					var newItemsList = new System.Collections.ArrayList();
					foreach(var newItem in value) {
						newItemsList.Add(newItem);
					}
					
					var args = new NotifyCollectionChangedEventArgs(
						NotifyCollectionChangedAction.Replace,
						newItemsList, 
						oldItemsList,
						0);
					
					Categories_CollectionChanged(this, args);
				}
			}
		}
			
		
		public bool IsRegistered
		{
			get; set;
		}
		
		public event ActiveInputBindingsChangedHandler ActiveInputBindingsChanged;
		
		/// <summary>
		/// New input bindings are assigned to owner when <see cref="CommandBindingInfo" /> is modified
		/// </summary>
		public InputBindingCollection ActiveInputBindings
		{
			get; set;
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
		
		private void Groups_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) 
		{
			if(e.OldItems != null) {
				foreach(BindingGroup oldGroup in e.OldItems) {
					oldGroup.InputBindings.Remove(this);
				}
			}
			
			if(e.NewItems != null) {
				foreach(BindingGroup oldGroup in e.NewItems) {
					oldGroup.InputBindings.Add(this);
				}
			}
		}
		
		private void ActiveGesturesChanged(object sender, InputBindingGesturesChangedArgs args) 
		{
			var template = new BindingInfoTemplate();
			template.OwnerInstanceName = OwnerInstanceName;
			template.OwnerTypeName = OwnerTypeName;
			template.RoutedCommandName = RoutedCommandName;
			
			SDCommandManager.InvokeInputBindingUpdateHandlers(BindingInfoMatchType.SubSet, template);
		}
		
		/// <summary>
		/// Re-generate <see cref="InputBinding" /> from <see cref="InputBindingInfo" />
		/// </summary>
		private void GenerateInputBindings() 
		{			
			OldInputBindings = ActiveInputBindings;
			
			ActiveInputBindings = new InputBindingCollection();
			if(ActiveGestures != null && BindingGroup.IsActive(this)) {
				foreach(InputGesture gesture in ActiveGestures) {
					var inputBinding = new InputBinding(RoutedCommand, gesture);
					ActiveInputBindings.Add(inputBinding);
				}
			}
		}
		
		public void RemoveActiveInputBindings()
		{
			if(_ownerTypeName != null) {
				if(OwnerTypes != null) {
					foreach(var ownerType in OwnerTypes) {
						foreach(InputBinding binding in ActiveInputBindings) {
							SDCommandManager.RemoveClassInputBinding(ownerType, binding);
						}
					}
				}
			} else if(_ownerInstanceName != null) {
				if(OwnerInstances != null) {
					foreach(var ownerInstance in OwnerInstances) {
						foreach(InputBinding binding in ActiveInputBindings) {
							ownerInstance.InputBindings.Remove(binding);
						}
					}
				}
			}
		}
		
		private BindingsUpdatedHandler defaultInputBindingHandler;
		
		/// <summary>
		/// Updates owner bindings
		/// </summary>
		internal BindingsUpdatedHandler DefaultInputBindingHandler
		{
			get {
				if(defaultInputBindingHandler == null && OwnerTypeName != null) {
					defaultInputBindingHandler  = delegate {
						if(RoutedCommand != null && OwnerTypes != null && IsRegistered) {
							GenerateInputBindings();
							
							foreach(var ownerType in OwnerTypes) {
								foreach(InputBinding binding in OldInputBindings) {
									SDCommandManager.RemoveClassInputBinding(ownerType, binding);
								}
								
								foreach(InputBinding binding in ActiveInputBindings) {
									System.Windows.Input.CommandManager.RegisterClassInputBinding(ownerType, binding);
								}
								
								var fieldInfo = typeof(System.Windows.Input.CommandManager).GetField("_classInputBindings", BindingFlags.Static | BindingFlags.NonPublic);
								var fieldData = (HybridDictionary)fieldInfo.GetValue(null);
								var classInputBindings = (InputBindingCollection)fieldData[ownerType];
							
								// Sorting input bindings. This may be slow
								if(classInputBindings != null) {
									classInputBindings.SortByChords();
								}
							}
							
							if(ActiveInputBindingsChanged != null) {
								ActiveInputBindingsChanged.Invoke(this);
							}
						}
					};
				} else if(defaultInputBindingHandler == null && OwnerInstanceName != null){
					defaultInputBindingHandler = delegate {
						if(RoutedCommand != null && OwnerInstances != null && IsRegistered) {
							GenerateInputBindings();
							
							foreach(var ownerInstance in OwnerInstances) {
								foreach(InputBinding binding in OldInputBindings) {
									ownerInstance.InputBindings.Remove(binding);
								}
								
								ownerInstance.InputBindings.AddRange(ActiveInputBindings);
								
								// Sorting input bindings. This may be slow
								if(ownerInstance.InputBindings != null) {
									ownerInstance.InputBindings.SortByChords();
								}
							}
						}
					};
				}
				
				return defaultInputBindingHandler;
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
		
		private void SetActiveGesturesChanged(string oldRoutedCommandName, string oldOwnerInstanceName, string oldOwnerTypeName) 
		{
			if(oldRoutedCommandName != null && (oldOwnerInstanceName != null || oldOwnerTypeName != null)) {
				var oldIdentifier = new InputBindingIdentifier();
				oldIdentifier.OwnerInstanceName = oldOwnerInstanceName;
				oldIdentifier.OwnerTypeName = oldOwnerTypeName;
				oldIdentifier.RoutedCommandName = oldRoutedCommandName;
				
				UserDefinedGesturesManager.RemoveActiveGesturesChangedHandler(oldIdentifier, ActiveGesturesChanged);
			}
			
			if(RoutedCommandName != null && (OwnerInstanceName != null || OwnerTypeName != null)) {
				UserDefinedGesturesManager.AddActiveGesturesChangedHandler(Identifier, ActiveGesturesChanged);
			}
		}
	}
	

	public struct InputBindingIdentifier 
	{
		public string OwnerInstanceName {
			get; set;
		}
		
		public string OwnerTypeName {
			get; set;
		}
		
		public string RoutedCommandName {
			get; set;
		}
	}
	
	public delegate void ActiveInputBindingsChangedHandler(InputBindingInfo inputBindingInfo);
}
