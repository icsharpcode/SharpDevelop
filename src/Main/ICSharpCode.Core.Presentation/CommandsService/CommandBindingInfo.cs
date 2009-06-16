using System;
using System.Windows;
using System.Windows.Input;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Stores details about command binding
	/// </summary>
	public class CommandBindingInfo 
	{	
		public CommandBindingInfo()
		{
			IsModifyed = true;
			OldCommandBindings = new CommandBindingCollection();
			NewCommandBindings = new CommandBindingCollection();
		}
		
		/// <summary>
		/// Routed command name
		/// 
		/// Described binding is triggered by this routed command
		/// </summary>
		/// <seealso cref="RoutedCommand"></seealso>
		public string RoutedCommandName { 
			get; set;
		}
		
		/// <summary>
		/// Routed command instance
		/// 
		/// Described binding is triggered by this routed command
		/// </summary>
		/// <seealso cref="RoutedCommandName"></seealso>
		public RoutedUICommand RoutedCommand { 
			get {
				return CommandsRegistry.GetRoutedUICommand(RoutedCommandName);
			}
		}
		
		/// <summary>
		/// Add-in to which binded command belongs
		/// </summary>
		public AddIn AddIn {
			get; set;
		}
		
		/// <summary>
		/// Binded command class full name
		/// 
		/// Instance of this class is created as soon as user executes the command. See
		/// <see cref="IsLazy" /> for details
		/// </summary>
		public string ClassName {
			get; set;
		}
		
		private System.Windows.Input.ICommand classInstance;
		
		/// <summary>
		/// Binded command instance
		/// 
		/// Reference to the command which is invoke when the binding is triggered. If this value is equal 
		/// to null then add-in is not loaded yet, see <see cref="IsLazy" /> attribute
		/// for details
		/// </summary>
		public System.Windows.Input.ICommand Class { 
			set {
				classInstance = value;
			}
			get {
				if(classInstance != null) {
					return classInstance;
				}
				
				if(ExecutedEventHandler != null || CanExecutedEventHandler != null) {
					return null;
				}
				
				if(AddIn != null && (AddIn.DependenciesLoaded || IsLazy)) {
					CommandsRegistry.LoadAddinCommands(AddIn);
				}
				
				System.Windows.Input.ICommand command;
				CommandsRegistry.commands.TryGetValue(ClassName, out command);
				classInstance = command;

				return command;
			}
		}
		
		/// <summary>
		/// Store name of object owning this binding (can only be used with named objects)
		/// 
		/// Named objects can be registered throgut <see cref="CommandsRegistry.RegisterNamedUIElementInstance" />
		/// </summary>
		public string OwnerInstanceName{
			get; set;
		}
		
		private UIElement ownerInstance;
		
		/// <summary>
		/// Stores instance of object which is owning this binding
		/// </summary>
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
	
		/// <summary>
		/// Assembly qualified name of the class owning this instance
		/// 
		/// Named type can be registered throgut <see cref="CommandsRegistry.RegisterNamedUIType" />
		/// </summary>
		public string OwnerTypeName{
			get; set;
		}
		
		private Type ownerType;
					
		/// <summary>
		/// Stores type owning this binding
		/// </summary>
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
		/// Default binding update handler update owner or type bindings (depending on input binding info type)
		/// so they would always contain latest version
		/// </summary>	
		private BindingsUpdatedHandler defaultCommandBindingHandler;
		
		/// <summary>
		/// Default command binding handler. Updates command binding if binding info changes
		/// </summary>
		internal BindingsUpdatedHandler DefaultCommandBindingHandler
		{
			get {
				if(defaultCommandBindingHandler == null && (OwnerTypeName != null || OwnerType != null)) {
	 				defaultCommandBindingHandler = delegate {
						if(OwnerType != null && IsModifyed) {
							GenerateCommandBindings();
							
							foreach(ManagedCommandBinding binding in OldCommandBindings) {
								CommandsRegistry.RemoveClassCommandBinding(OwnerType, binding);
							}
							
							foreach(ManagedCommandBinding binding in NewCommandBindings) {
								CommandManager.RegisterClassCommandBinding(OwnerType, binding);
							}
							
							IsModifyed = false;
						}
					};
				} else if(defaultCommandBindingHandler == null && (OwnerInstanceName != null || OwnerInstance != null)) {
		 			defaultCommandBindingHandler = delegate {
						if(OwnerInstance != null && IsModifyed) {
							GenerateCommandBindings();
							
							foreach(ManagedCommandBinding binding in OldCommandBindings) {
								OwnerInstance.CommandBindings.Remove(binding);
							}
							
							OwnerInstance.CommandBindings.AddRange(NewCommandBindings);
							
							IsModifyed = false;
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
		
		public ExecutedRoutedEventHandler ExecutedEventHandler
		{
			get; set;
		}
		
		public CanExecuteRoutedEventHandler CanExecutedEventHandler
		{
			get; set;
		}
		
		/// <summary>
		/// Indicates that generated command bindings are modified from last access
		/// </summary>
		public bool IsModifyed {
			get; set;
		}
		
		public void GenerateCommandBindings() 
		{			
			OldCommandBindings = NewCommandBindings;
			
			var managedCommandBinding = new ManagedCommandBinding(RoutedCommand);
			managedCommandBinding.CanExecute += GeneratedCanExecuteEventHandler;					
			managedCommandBinding.Executed += GeneratedExecutedEventHandler;
			
			NewCommandBindings = new CommandBindingCollection();
			NewCommandBindings.Add(managedCommandBinding);
		}
		
		internal CommandBindingCollection OldCommandBindings
		{
			get; set;
		}
		
		internal CommandBindingCollection NewCommandBindings
		{
			get; set;
		}

		internal void GeneratedExecutedEventHandler(object sender, ExecutedRoutedEventArgs e) {
			if(ExecutedEventHandler != null) {
				ExecutedEventHandler.Invoke(sender, e);
			} else {
				if(IsLazy && Class == null) {
					AddIn.LoadRuntimeAssemblies();
					
					var command = (ICommand)AddIn.CreateObject(ClassName);
					CommandsRegistry.LoadCommand(ClassName, command);
				}
				
				if(Class != null) {
					Class.Execute(e.Parameter);
				}
			}
		}
		
		internal void GeneratedCanExecuteEventHandler(object sender, CanExecuteRoutedEventArgs e) {
			if(CanExecutedEventHandler == null && ExecutedEventHandler != null) {
				e.CanExecute = true;
			} else if(CanExecutedEventHandler != null) {
				CanExecutedEventHandler.Invoke(sender, e);
			} else {
				if(IsLazy && Class == null) {
					e.CanExecute = true;
				} else if(Class == null) {
					e.CanExecute = false;
				} else {
					e.CanExecute = Class.CanExecute(e.Parameter); 						
				}				
			}
		}
	}
}
