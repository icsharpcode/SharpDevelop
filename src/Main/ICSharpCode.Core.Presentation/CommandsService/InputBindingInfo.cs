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
		
		public BindingGroupCollection Groups
		{
			get {
				return base.Groups;
			}
			set {
				var oldGroups = base.Groups;
				base.Groups = value;
				
				SetCollectionChanged<BindingGroup>(oldGroups, value, Groups_CollectionChanged);
			}
		}
		
		/// <summary>
		/// Stores name of named instance to which this binding belongs. When this binding is registered a
		/// <see cref="InputBinding" /> is assigned to owner instance
		/// 
		/// If this attribute is used <see cref="OwnerInstance" />, <see cref="OwnerType" /> and
		/// <see cref="OwnerTypeName" /> can not be set
		/// </summary>
		public override string OwnerInstanceName {
			get {
				return base.OwnerInstanceName;
			}
			set {
				base.OwnerInstanceName = value;
				
				SetActiveGesturesChanged(RoutedCommandName, value, OwnerTypeName);
			}
		}
		
		/// <summary>
		/// Stores name of owner type. Full name with assembly should be used. When this binding is 
		/// registered <see cref="InputBinding" /> is assigned to all instances of provided class
		/// 
		/// If this attribute is used <see cref="OwnerInstance" />, <see cref="OwnerInstanceName" /> and
		/// <see cref="OwnerType" /> can not be set
		/// </summary>
		public override string OwnerTypeName 
		{
			get {
				return base.OwnerTypeName;
			}
			set {
				base.OwnerTypeName = value;
				
				SetActiveGesturesChanged(RoutedCommandName, OwnerInstanceName, value);
			}
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
							ActiveGesturesChanged(this, new InputBindingGesturesChangedArgs { InputBindingIdentifier = Identifier });
						}
					};
				}
				
				
				if(UserDefinedGesturesManager.CurrentProfile == null) {
					ActiveGesturesChanged(this, new InputBindingGesturesChangedArgs { InputBindingIdentifier = Identifier });
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
		
		/// <summary>
		/// Name of the routed command which will be invoked when this binding is triggered
		/// </summary>
		public override string RoutedCommandName { 
			get {
				return base.RoutedCommandName;
			}
			set {
				var oldValue = base.RoutedCommandName;
				base.RoutedCommandName = value;
				
				SetActiveGesturesChanged(oldValue, OwnerInstanceName, OwnerTypeName);
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
		
		public event ActiveInputBindingsChangedHandler ActiveInputBindingsChanged;
		
		/// <summary>
		/// New input bindings are assigned to owner when <see cref="CommandBindingInfo" /> is modified
		/// </summary>
		public InputBindingCollection ActiveInputBindings
		{
			get; set;
		}

		protected override void SetInstanceBindings(ICollection<UIElement> oldInstances, ICollection<UIElement> newInstances)
		{
			if(oldInstances != null) {
				foreach(var ownerInstance in oldInstances) {
					foreach(InputBinding binding in OldInputBindings) {
						ownerInstance.InputBindings.Remove(binding);
					}
				}
			}
	
			if(newInstances != null) {
				foreach(var ownerInstance in newInstances) {
					ownerInstance.InputBindings.AddRange(ActiveInputBindings);
					
					// Sorting input bindings. This may be slow
					if(ownerInstance.InputBindings != null) {
						ownerInstance.InputBindings.SortByChords();
					}
				}
			}
		}
		
		protected override void SetClassBindings(ICollection<Type> oldTypes, ICollection<Type> newTypes)
		{
			if(oldTypes != null) {
				foreach(var ownerType in oldTypes) {
					foreach(InputBinding binding in OldInputBindings) {
						SDCommandManager.RemoveClassInputBinding(ownerType, binding);
					}
				}
			}
			
			if(newTypes != null) {
				foreach(var ownerType in newTypes) {
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
			}
			
			if(ActiveInputBindingsChanged != null) {
				ActiveInputBindingsChanged.Invoke(this);
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
		
		private void Groups_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) 
		{
			if(IsRegistered) {
				var modifiedGroups = new BindingGroupCollection();
				if(e.NewItems != null) {						
					modifiedGroups.AddRange(e.NewItems.Cast<BindingGroup>());
				}
				
				if(e.OldItems != null) {
					modifiedGroups.AddRange(e.OldItems.Cast<BindingGroup>());
				}
				
				SDCommandManager.InvokeInputBindingUpdateHandlers(
					this,
					new BindingsUpdatedHandlerArgs(),
					BindingInfoMatchType.SubSet,
					new BindingInfoTemplate(this, false) { Groups = modifiedGroups });
			}
		}
		
		private void ActiveGesturesChanged(object sender, InputBindingGesturesChangedArgs args) 
		{
			var template = new BindingInfoTemplate();
			template.OwnerInstanceName = OwnerInstanceName;
			template.OwnerTypeName = OwnerTypeName;
			template.RoutedCommandName = RoutedCommandName;
			
			SDCommandManager.InvokeInputBindingUpdateHandlers(
				null,
				new BindingsUpdatedHandlerArgs(),
				BindingInfoMatchType.SubSet, 
				template);
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
