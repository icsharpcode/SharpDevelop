using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using CommandManager=ICSharpCode.Core.Presentation.CommandManager;
	
namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Stores details about <see cref="CommandBinding" />
	/// </summary>
	public class CommandBindingInfo : IBindingInfo
	{	
		/// <summary>
		/// Creates new instance of <see cref="CommandBindingInfo" />
		/// </summary>
		public CommandBindingInfo()
		{
			OldCommandBindings = new CommandBindingCollection();
			ActiveCommandBindings = new CommandBindingCollection();
			
			Groups = new BindingGroupCollection();
			Groups.CollectionChanged += Groups_CollectionChanged;
		}
		
		private void Groups_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {  
			if(e.OldItems != null) {
				foreach(BindingGroup oldGroup in e.OldItems) {
					oldGroup.CommandBindings.Remove(this);
				}
			}
			
			if(e.NewItems != null) {
				foreach(BindingGroup newGroup in e.NewItems) {
					newGroup.CommandBindings.Add(this);
				}
			}
		}
		
		public BindingGroupCollection Groups
		{
			get; private set;
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
				routedCommandName = value;
			}
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
		/// Add-in to which binding belongs
		/// </summary>
		public AddIn AddIn {
			get; set;
		}
		
		private string commandTypeName;
		
		/// <summary>
		/// Binded command type full name
		/// 
		/// Instance of this class is created as soon as user executes the command. See
		/// <see cref="IsLazy" /> for details
		/// 
		/// If this attribute is provided <see cref="CommandInstance" />, <see cref="ExecutedEventHandler" />
		/// and <see cref="CanExecuteEventHandler" /> can not be used
		/// </summary>
		public string CommandTypeName {
			get {
				return commandTypeName;
			}
			set {
				if(commandInstance != null || commandTypeName != null || canExecuteEventHandler != null || executedEventHandler != null) {
					throw new ArgumentException("Executed or CanExecute command handlers are already specified");
				}
				
				commandTypeName = value;
			}
		}
		
		private System.Windows.Input.ICommand commandInstance;
		
		/// <summary>
		/// Binded command instance
		/// 
		/// If this attribute is provided <see cref="CommandInstanceName" />, <see cref="ExecutedEventHandler" />
		/// and <see cref="CanExecuteEventHandler" /> can not be used
		/// </summary>
		public System.Windows.Input.ICommand CommandInstance { 
			get {
				if(commandInstance != null) {
					return commandInstance;
				}
				
				if(ExecutedEventHandler != null || CanExecuteEventHandler != null) {
					return null;
				}
				
				if(AddIn != null && (AddIn.DependenciesLoaded || IsLazy)) {
					CommandManager.LoadAddinCommands(AddIn);
				}
				
				System.Windows.Input.ICommand command;
				CommandManager.commands.TryGetValue(CommandTypeName, out command);
				commandInstance = command;

				return command;
			}
			set {
				if(commandInstance != null || commandTypeName != null || canExecuteEventHandler != null || executedEventHandler != null) {
					throw new ArgumentException("Executed or CanExecute command handlers are already specified");
				}
				
				commandInstance = value;
			}
		}
		
		
		private ExecutedRoutedEventHandler executedEventHandler;
		
		/// <summary>
		/// Occurs when invoking "Executed" event
		/// 
		/// If this attribute is provided <see cref="CommandInstanceName" /> and <see cref="CommandInstance" />
		/// can not be used
		/// </summary>
		public ExecutedRoutedEventHandler ExecutedEventHandler
		{
			get {
				return executedEventHandler;
			}
			set {
				if(commandInstance != null || commandTypeName != null) {
					throw new ArgumentException("Command class is already provided");
				}
				
				executedEventHandler = value;
			}
		}
		
		private CanExecuteRoutedEventHandler canExecuteEventHandler;
		
		/// <summary>
		/// Occurs when determining whether "Executed" event can be invoked
		/// 
		/// If this attribute is provided <see cref="CommandInstanceName" /> and <see cref="CommandInstance" />
		/// can not be used
		/// </summary>
		public CanExecuteRoutedEventHandler CanExecuteEventHandler
		{
			get {
				return canExecuteEventHandler;
			}
			set {
				if(commandInstance != null || commandTypeName != null) {
					throw new ArgumentException("Command class is already provided");
				}
				
				canExecuteEventHandler = value;
			}
		}

		public string _ownerInstanceName;
		
		/// <summary>
		/// Stores name of named instance to which this binding belongs. When this binding is registered a
		/// <see cref="CommandBinding" /> is assigned to owner instance
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
		/// <see cref="CommandBinding" /> is assigned to owner instance
		/// 
		/// If this attribute is used <see cref="OwnerInstanceName" />, <see cref="OwnerType" /> and
		/// <see cref="OwnerTypeName" /> can not be set
		/// </summary>
		public ICollection<UIElement> OwnerInstances {
			get {
				if(_ownerInstanceName != null) {
					return CommandManager.GetNamedUIElementCollection(_ownerInstanceName);
				}
				
				return null;
			}
		}
					
		private string _ownerTypeName;
		
		/// <summary>
		/// Stores name of owner type. Full name with assembly should be used. When this binding is 
		/// registered <see cref="CommandBinding" /> is assigned to all instances of provided class
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
		/// Stores owner type. When this binding is registered <see cref="CommandBinding" /> 
		/// is assigned to all instances of provided class
		/// 
		/// If this attribute is used <see cref="OwnerInstance" />, <see cref="OwnerInstanceName" /> and
		/// <see cref="OwnerTypeName" /> can not be set
		/// </summary>
		public ICollection<Type> OwnerTypes { 
			get {
				if(_ownerTypeName != null) {
					return CommandManager.GetNamedUITypeCollection(_ownerTypeName);
				}
				
				return null;
			}
		}
		
		public bool IsRegistered
		{
			get; set;
		}
			
		private BindingsUpdatedHandler defaultCommandBindingHandler;
		
		/// <summary>
		/// Updates owner bindings
		/// </summary>
		internal BindingsUpdatedHandler DefaultCommandBindingHandler
		{
			get {
				if(defaultCommandBindingHandler == null && OwnerTypeName != null) {
	 				defaultCommandBindingHandler = delegate {
						if(RoutedCommand != null && OwnerTypes != null && IsRegistered) {
							GenerateCommandBindings();
							
							foreach(var ownerType in OwnerTypes) {
								foreach(CommandBinding binding in OldCommandBindings) {
									CommandManager.RemoveClassCommandBinding(ownerType, binding);
								}
								
								foreach(CommandBinding binding in ActiveCommandBindings) {
									System.Windows.Input.CommandManager.RegisterClassCommandBinding(ownerType, binding);
								}
							}
						}
					};
				} else if(defaultCommandBindingHandler == null && OwnerInstanceName != null) {
		 			defaultCommandBindingHandler = delegate {
						if(RoutedCommand != null && OwnerInstances != null && IsRegistered) {
							GenerateCommandBindings();
							
							foreach(var ownerInstance in OwnerInstances) {
								foreach(CommandBinding binding in OldCommandBindings) {
									ownerInstance.CommandBindings.Remove(binding);
								}
							
								ownerInstance.CommandBindings.AddRange(ActiveCommandBindings);
							}
						}
					};
				}
				
				return defaultCommandBindingHandler;
			}
		}
		
		/// <summary>
		/// Lazy load
		/// 
		/// If lazy load is enabled then all add-in references are loaded when this 
		/// command is invoked. Otherwice if add-in is not loaded and IsLazy is set
		/// to false then this binding can't be triggered until it is loaded manualy is loaded
		/// using <see cref="CommandsRegistry.LoadCommand" /> or <see cref="CommandsRegistry.LoadAddInCommands" />.
		/// </summary>
		public bool IsLazy {
			get; set;
		}
		
		/// <summary>
		/// Re-generate <see cref="CommandBinding" /> from <see cref="CommandBindingInfo" />
		/// </summary>
		internal void GenerateCommandBindings() 
		{			
			OldCommandBindings = ActiveCommandBindings;
			
			ActiveCommandBindings = new CommandBindingCollection();
			
			if(BindingGroup.IsActive(this)) {
				var commandBinding = new CommandBinding(RoutedCommand);
				commandBinding.CanExecute += GenerateCanExecuteEventHandler;					
				commandBinding.Executed += GenerateExecutedEventHandler;
				ActiveCommandBindings.Add(commandBinding);
			}
		}
		
		/// <summary>
		/// Old command bindings which where assigned to owner when before <see cref="CommandBindingInfo" />
		/// was modified.
		/// 
		/// When new <see cref="CommandBinding" />s are generated these bindings are removed from the owner
		/// </summary>
		internal CommandBindingCollection OldCommandBindings
		{
			get; set;
		}
		
		/// <summary>
		/// New input bindings are assigned to owner when <see cref="CommandBindingInfo" /> is modified
		/// </summary>
		public CommandBindingCollection ActiveCommandBindings
		{
			get; set;
		}

		/// <summary>
		/// Forwards "executed" event to event handler provided to user 
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		internal void GenerateExecutedEventHandler(object sender, ExecutedRoutedEventArgs e) {
			if(ExecutedEventHandler != null) {
				ExecutedEventHandler.Invoke(sender, e);
			} else {
				if(IsLazy && CommandInstance == null) {
					AddIn.LoadRuntimeAssemblies();
					
					var command = (ICommand)AddIn.CreateObject(CommandTypeName);
					CommandManager.LoadCommand(CommandTypeName, command);
				}
				
				if(CommandInstance != null) {
					CommandInstance.Execute(e.Parameter);
				}
			}
		}
		
		/// <summary>
		/// Forwards "can execute" event to event handler provided to user 
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		internal void GenerateCanExecuteEventHandler(object sender, CanExecuteRoutedEventArgs e) {
			if(CanExecuteEventHandler == null && ExecutedEventHandler != null) {
				e.CanExecute = true;
			} else if(CanExecuteEventHandler != null) {
				CanExecuteEventHandler.Invoke(sender, e);
			} else {
				if(IsLazy && CommandInstance == null) {
					e.CanExecute = true;
				} else if(CommandInstance == null) {
					e.CanExecute = false;
				} else {
					e.CanExecute = CommandInstance.CanExecute(e.Parameter); 						
				}				
			}
		}
	}
}
