using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Stores details about input binding
	/// </summary>
	public class InputBindingInfo
	{
		/// <summary>
		/// Creates new instance of <see cref="InputBindingInfo"/>
		/// </summary>
		public InputBindingInfo() 
		{
			IsModifyed = true;
			OldInputBindings = new InputBindingCollection();
			ActiveInputBindings = new InputBindingCollection();
			DefaultGestures = new InputGestureCollection();
			Categories = new InputBindingCategoryCollection();
			Groups = new BindingGroupCollection();
			Groups.CollectionChanged += Groups_CollectionChanged;
		}
		
		private void Groups_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {  
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
		
		public BindingGroupCollection Groups
		{
			get; private set;
		}
		
		public bool IsActive
		{
			get {
				if(OwnerInstanceName != null && Groups != null && Groups.Count > 0) {
					return Groups.IsAttachedTo(OwnerInstanceName);
				}
				
				return true;
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
				if(_ownerInstanceName != null || _ownerTypeName != null) {
					throw new ArgumentException("This binding already has an owner");
				}
				
				_ownerInstanceName = value;
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
					return CommandManager.GetNamedUIElement(_ownerInstanceName);
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
		public string OwnerTypeName{
			get {
				return _ownerTypeName;
			}
			set {
				if(_ownerInstanceName != null || _ownerTypeName != null) {
					throw new ArgumentException("This binding already has an owner");
				}
				
				_ownerTypeName = value;
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
					return CommandManager.GetNamedUIType(_ownerTypeName);
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
	
		private InputGestureCollection _defaultGestures;
		
		/// <summary>
		/// Gestures which triggers this binding
		/// </summary>
		public InputGestureCollection DefaultGestures { 
			get {
				return _defaultGestures;
			}
			set {
				_defaultGestures = value;
			}
		}
		
		/// <summary>
		/// Gestures which triggers this binding
		/// </summary>
		public InputGestureCollection ActiveGestures { 
			get {
				if(UserDefinedGesturesManager.CurrentProfile == null 
				   || UserDefinedGesturesManager.CurrentProfile[Identifier] == null) {
					return DefaultGestures;
				} 
				
				return UserDefinedGesturesManager.CurrentProfile[Identifier];
			}
		}
		
		/// <summary>
		/// Name of the routed command which will be invoked when this binding is triggered
		/// </summary>
		public string RoutedCommandName { 
			get; set;
		}
		
		/// <summary>
		/// Routed command instance which will be invoked when this binding is triggered
		/// </summary>
		public RoutedUICommand RoutedCommand { 
			get {
				return CommandManager.GetRoutedUICommand(RoutedCommandName);
			}
		}
		
		/// <summary>
		/// List of categories associated with input binding 
		/// </summary>
		public InputBindingCategoryCollection Categories {
			get; private set;
		}
			
		/// <summary>
		/// Indicates whether <see cref="InputBindingInfo" /> was modified. When modified
		/// <see cref="InputBinding" />s are re-generated
		/// </summary>
		public bool IsModifyed {
			get; set;
		}
		
		/// <summary>
		/// Re-generate <see cref="InputBinding" /> from <see cref="InputBindingInfo" />
		/// </summary>
		public void GenerateInputBindings() 
		{			
			OldInputBindings = ActiveInputBindings;
			
			ActiveInputBindings = new InputBindingCollection();
			if(ActiveGestures != null && IsActive) {
				foreach(InputGesture gesture in ActiveGestures) {
					var inputBinding = new InputBinding(RoutedCommand, gesture);
					ActiveInputBindings.Add(inputBinding);
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
						if(OwnerTypes != null && IsModifyed) {
							GenerateInputBindings();
							
							foreach(var ownerType in OwnerTypes) {
								foreach(InputBinding binding in OldInputBindings) {
									CommandManager.RemoveClassInputBinding(ownerType, binding);
								}
								
								foreach(InputBinding binding in ActiveInputBindings) {
									System.Windows.Input.CommandManager.RegisterClassInputBinding(ownerType, binding);
								}
								
								CommandManager.OrderClassInputBindingsByChords(ownerType);
							}
							
							IsModifyed = false;
						}
					};
				} else if(defaultInputBindingHandler == null && OwnerInstanceName != null){
					defaultInputBindingHandler = delegate {
						if(OwnerInstances != null && IsModifyed) {
							GenerateInputBindings();
							
							foreach(var ownerInstance in OwnerInstances) {
								foreach(InputBinding binding in OldInputBindings) {
									ownerInstance.InputBindings.Remove(binding);
								}
								
								ownerInstance.InputBindings.AddRange(ActiveInputBindings);
								
								CommandManager.OrderInstanceInputBindingsByChords(ownerInstance);
							}
							
							IsModifyed = false;
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
}
