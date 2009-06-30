using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;

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
			NewInputBindings = new InputBindingCollection();
			Gestures = new InputGestureCollection();
		
			Categories = new List<InputBindingCategory>();
		}
		
		public string ownerInstanceName;
		
		/// <summary>
		/// Stores name of named instance to which this binding belongs. When this binding is registered a
		/// <see cref="InputBinding" /> is assigned to owner instance
		/// 
		/// If this attribute is used <see cref="OwnerInstance" />, <see cref="OwnerType" /> and
		/// <see cref="OwnerTypeName" /> can not be set
		/// </summary>
		public string OwnerInstanceName {
			get {
				return ownerInstanceName;
			}
			set {
				if(ownerInstanceName != null || ownerInstance != null || ownerType != null || ownerTypeName != null) {
					throw new ArgumentException("This binding already has an owner");
				}
				
				ownerInstanceName = value;
			}
		}
		
		private UIElement ownerInstance;
		
		/// <summary>
		/// Stores owner instance to which this binding belongs. When this binding is registered a
		/// <see cref="InputBinding" /> is assigned to owner instance
		/// 
		/// If this attribute is used <see cref="OwnerInstanceName" />, <see cref="OwnerType" /> and
		/// <see cref="OwnerTypeName" /> can not be set
		/// </summary>
		public UIElement OwnerInstance{
			get {
				if(OwnerInstanceName != null && ownerInstance == null) {
					ownerInstance = CommandManager.GetNamedUIElementInstance(OwnerInstanceName);
				}
				
				return ownerInstance;
			}
		}
					
		private string ownerTypeName;
		
		/// <summary>
		/// Stores name of owner type. Full name with assembly should be used. When this binding is 
		/// registered <see cref="InputBinding" /> is assigned to all instances of provided class
		/// 
		/// If this attribute is used <see cref="OwnerInstance" />, <see cref="OwnerInstanceName" /> and
		/// <see cref="OwnerType" /> can not be set
		/// </summary>
		public string OwnerTypeName{
			get {
				return ownerTypeName;
			}
			set {
				if(ownerInstanceName != null || ownerInstance != null || ownerType != null || ownerTypeName != null) {
					throw new ArgumentException("This binding already has an owner");
				}
				
				ownerTypeName = value;
			}
		}
		
		private Type ownerType;
					
		/// <summary>
		/// Stores owner type. When this binding is registered <see cref="InputBinding" /> 
		/// is assigned to all instances of provided class
		/// 
		/// If this attribute is used <see cref="OwnerInstance" />, <see cref="OwnerInstanceName" /> and
		/// <see cref="OwnerTypeName" /> can not be set
		/// </summary>
		public Type OwnerType { 
			get {
				if(ownerType == null && OwnerTypeName != null) {
					ownerType = Type.GetType(OwnerTypeName);
					CommandManager.RegisterNamedUIType(OwnerTypeName, ownerType);
				}
				
				return ownerType;
			}
			set {
				if(ownerInstanceName != null || ownerInstance != null || ownerType != null || ownerTypeName != null) {
					throw new ArgumentException("This binding already has an owner");
				}
				
				ownerType = value;
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
	
		private InputGestureCollection _gestures;
		
		/// <summary>
		/// Gestures which triggers this binding
		/// </summary>
		public InputGestureCollection Gestures { 
			get {
				return _gestures;
			}
			set {
				_gestures = value;
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
		public List<InputBindingCategory> Categories {
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
			OldInputBindings = NewInputBindings;
			
			NewInputBindings = new InputBindingCollection();
			if(Gestures != null) {
				foreach(InputGesture gesture in Gestures) {
					var inputBinding = new InputBinding(RoutedCommand, gesture);
					NewInputBindings.Add(inputBinding);
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
				if(defaultInputBindingHandler == null && (OwnerTypeName != null || OwnerType != null)) {
					defaultInputBindingHandler  = delegate {
						if(OwnerType != null && IsModifyed) {
							GenerateInputBindings();
							
							foreach(InputBinding binding in OldInputBindings) {
								CommandManager.RemoveClassInputBinding(OwnerType, binding);
							}
							
							foreach(InputBinding binding in NewInputBindings) {
								System.Windows.Input.CommandManager.RegisterClassInputBinding(OwnerType, binding);
							}
							
							CommandManager.OrderClassInputBindingsByChords(OwnerType);
							
							IsModifyed = false;
						}
					};
				} else if(defaultInputBindingHandler == null && (OwnerInstanceName != null || OwnerInstance != null)) {
					defaultInputBindingHandler = delegate {
						if(OwnerInstance != null && IsModifyed) {
							GenerateInputBindings();
							
							foreach(InputBinding binding in NewInputBindings) {
								OwnerInstance.InputBindings.Remove(binding);
							}
							
							OwnerInstance.InputBindings.AddRange(NewInputBindings);
							
							CommandManager.OrderInstanceInputBindingsByChords(OwnerInstance);
							
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
		internal InputBindingCollection NewInputBindings
		{
			get; set;
		}
	}
}
