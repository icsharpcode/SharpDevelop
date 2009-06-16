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
		public InputBindingInfo() {			
			IsModifyed = true;
			OldInputBindings = new InputBindingCollection();
			NewInputBindings = new InputBindingCollection();
		
			Categories = new List<InputBindingCategory>();
		}
		
		public string OwnerInstanceName{
			get; set;
		}
		
		private UIElement ownerInstance;
		
		public UIElement OwnerInstance{
			get {
				if(OwnerInstanceName != null && ownerInstance == null) {
					ownerInstance = CommandsRegistry.GetNamedUIElementInstance(OwnerInstanceName);
				}
				
				return ownerInstance;
			}
			set {
				ownerInstance = value;
			}
		}
					
		public string OwnerTypeName{
			get; set;
		}
		
		private Type ownerType;
					
		public Type OwnerType { 
			set {
				ownerType = value;
			}
			get {
				if(ownerType == null && OwnerTypeName != null) {
					ownerType = Type.GetType(OwnerTypeName);
					CommandsRegistry.RegisterNamedUIType(OwnerTypeName, ownerType);
				}
				
				return ownerType;
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
	
		/// <summary>
		/// Gestures which triggers this binding
		/// </summary>
		public InputGestureCollection Gestures { 
			get; set; 
		}
		
		/// <summary>
		/// Routed command name
		/// 
		/// Described binding triggers this routed command
		/// </summary>
		/// <seealso cref="RoutedCommand"></seealso>
		public string RoutedCommandName { 
			get; set;
		}
		
		/// <summary>
		/// Routed command instance
		/// 
		/// Described binding triggers this routed command
		/// </summary>
		/// <seealso cref="RoutedCommandName"></seealso>
		public RoutedUICommand RoutedCommand { 
			get {
				return CommandsRegistry.GetRoutedUICommand(RoutedCommandName);
			}
		}
		
		private BindingsUpdatedHandler defaultInputBindingHandler;
		
		/// <summary>
		/// Default binding update handler update owner or type bindings (depending on input binding info type)
		/// so they would always contain latest version
		/// </summary>
		public BindingsUpdatedHandler DefaultInputBindingHandler
		{
			get {
				if(defaultInputBindingHandler == null && (OwnerTypeName != null || OwnerType != null)) {
					defaultInputBindingHandler  = delegate {
						if(OwnerType != null && IsModifyed) {
							GenerateInputBindings();
							
							foreach(ManagedInputBinding binding in OldInputBindings) {
								CommandsRegistry.RemoveClassInputBinding(OwnerType, binding);
							}
							
							foreach(ManagedInputBinding binding in NewInputBindings) {
								CommandManager.RegisterClassInputBinding(OwnerType, binding);
							}
							
							IsModifyed = false;
						}
					};
				} else if(defaultInputBindingHandler == null && (OwnerInstanceName != null || OwnerInstance != null)) {
					defaultInputBindingHandler = delegate {
						if(OwnerInstance != null && IsModifyed) {
							GenerateInputBindings();
							
							foreach(ManagedInputBinding binding in NewInputBindings) {
								OwnerInstance.InputBindings.Remove(binding);
							}
							
							OwnerInstance.InputBindings.AddRange(NewInputBindings);
							
							IsModifyed = false;
						}
					};
				}
				
				return defaultInputBindingHandler;
			}
		}
		
		/// <summary>
		/// List of categories associated with input binding 
		/// </summary>
		public List<InputBindingCategory> Categories {
			get; private set;
		}
			
		/// <summary>
		/// Indicates whether generated input bindings where modified from last access
		/// </summary>
		public bool IsModifyed {
			get; set;
		}
		
		public void GenerateInputBindings() 
		{			
			OldInputBindings = NewInputBindings;
			
			NewInputBindings = new InputBindingCollection();
			foreach(InputGesture gesture in Gestures) {
				var managedInputBinding = new ManagedInputBinding(RoutedCommand, gesture);
				NewInputBindings.Add(managedInputBinding);
			}
		}
		
		internal InputBindingCollection OldInputBindings
		{
			get; set;
		}
		
		internal InputBindingCollection NewInputBindings
		{
			get; set;
		}
	}
}
