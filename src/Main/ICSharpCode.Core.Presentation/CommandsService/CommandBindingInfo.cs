using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using SDCommandManager=ICSharpCode.Core.Presentation.CommandManager;
using CommandManager=System.Windows.Input.CommandManager;
	
namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Stores details about <see cref="CommandBinding" />
	/// </summary>
	public class CommandBindingInfo : BindingInfoBase
	{	
		// Command associated with command binding info
		private string commandTypeName;
		private System.Windows.Input.ICommand commandInstance;
		private ExecutedRoutedEventHandler executedEventHandler;
		private CanExecuteRoutedEventHandler canExecuteEventHandler;
		
		// Owners which where set earlier
		private List<UIElement> oldOwnerInstances;
		private List<Type> oldOwnerTypes;
		
		// Command bindings which where set earlier
		private CommandBindingCollection oldCommandBindingCollection = new CommandBindingCollection();
		
		/// <summary>
		/// Creates new instance of <see cref="CommandBindingInfo" />
		/// </summary>
		public CommandBindingInfo()
		{
			ActiveCommandBindings = new CommandBindingCollection();
		}
		
		/// <summary>
		/// Gets or sets name of named <see cref="System.Windows.Input.ICommand" /> type associated with this <see cref="CommandBindingInfo" /> 
		/// 
		/// Instance of this class is created as soon as user executes the command. See <see cref="IsLazy" /> for details
		/// 
		/// If this attribute is provided <see cref="CommandInstance" />, <see cref="ExecutedEventHandler" />
		/// and <see cref="CanExecuteEventHandler" /> can not be used
		/// </summary>
		public string CommandTypeName {
			get {
				return commandTypeName;
			}
			set {
				if(canExecuteEventHandler != null || executedEventHandler != null || (commandTypeName == null && commandInstance != null)) {
					throw new ArgumentException("Executed, CanExecute or CommandInstance property has already been set");   	
				}
				
				if(IsRegistered) {
					throw new ArgumentException("Can not change command type name while binding info is registered");
				}
				
				commandTypeName = value;
			}
		}
		
		/// <summary>
		/// Gets or sets <see cref="System.Windows.Input.ICommand" /> associated with this <see cref="CommandBindingInfo" />
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
					SDCommandManager.LoadAddinCommands(AddIn);
				}
				
				System.Windows.Input.ICommand command;
				SDCommandManager.commands.TryGetValue(CommandTypeName, out command);
				commandInstance = command;

				return command;
			}
			set {
				if(canExecuteEventHandler != null || executedEventHandler != null || commandTypeName != null) {
					throw new ArgumentException("Executed, CanExecute or CommandTypeName property has already been set");   	
				}
				
				if(IsRegistered) {
					throw new ArgumentException("Can not change command instance while binding info is registered");
				}
				
				commandInstance = value;
			}
		}
		
		/// <summary>
		/// Gets or sets <see cref="CommandBinding" /> "Executed" event handler
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
				if(commandTypeName != null || commandInstance != null) {
					throw new ArgumentException("CommandTypeName or CommandInstance property has already been set");   	
				}
				
				if(IsRegistered) {
					throw new ArgumentException("Can not change \"Executed\" event handler while binding info is registered");
				}
				
				if(commandInstance != null || commandTypeName != null) {
					throw new ArgumentException("Command class is already provided");
				}
				
				executedEventHandler = value;
			}
		}
		
		/// <summary>
		/// Gets or sets <see cref="CommandBinding" /> "CanExecute" event handler
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
				if(commandTypeName != null || commandInstance != null) {
					throw new ArgumentException("CommandTypeName or CommandInstance property has already been set");   	
				}
				
				if(IsRegistered) {
					throw new ArgumentException("Can not change \"CanExecute\" event handler while binding info is registered");
				}
				
				canExecuteEventHandler = value;
			}
		}
		
		/// <summary>
		/// Apply <see cref="ActiveCommandBindings" /> to new <see cref="System.Windows.UIElement" /> collection
		/// </summary>
		/// <param name="newInstances">Collection of modified owner instances</param>
		protected override void PopulateOwnerInstancesWithBindings(ICollection<UIElement> newInstances)
		{				
			if(oldOwnerInstances != null) {
				foreach(var ownerInstance in oldOwnerInstances) {
					foreach(CommandBinding binding in oldCommandBindingCollection) {
						ownerInstance.CommandBindings.Remove(binding);
					}
				}
			}
			
			oldOwnerInstances = new List<UIElement>();
			
			if(newInstances != null) {
				foreach(var ownerInstance in newInstances) {
					ownerInstance.CommandBindings.AddRange(ActiveCommandBindings);
					oldOwnerInstances.Add(ownerInstance);
				}
			}
		}
		
		
		/// <summary>
		/// Apply <see cref="ActiveCommandBindings" /> to new <see cref="System.Type" /> collection
		/// </summary>
		/// <param name="newInstances">Collection of modified owner types</param>
		protected override void PopulateOwnerTypesWithBindings(ICollection<Type> newTypes)
		{
			if(oldOwnerTypes != null) {
				foreach(var ownerType in oldOwnerTypes) {
					foreach(CommandBinding binding in oldCommandBindingCollection) {
						SDCommandManager.RemoveClassCommandBinding(ownerType, binding);
					}
				}
			}
			
			oldOwnerTypes = new List<Type>();
			
			if(newTypes != null) {
				foreach(var ownerType in newTypes) {
					foreach(CommandBinding binding in ActiveCommandBindings) {
						System.Windows.Input.CommandManager.RegisterClassCommandBinding(ownerType, binding);
						oldOwnerTypes.Add(ownerType);
					}
				}
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
		/// Re-generate <see cref="ActiveCommandBindings" /> using <see cref="CommandBindingInfo" /> data
		/// </summary>
		protected override void GenerateBindings() 
		{			
			oldCommandBindingCollection = ActiveCommandBindings;
			
			ActiveCommandBindings = new CommandBindingCollection();
			
			var commandBinding = new CommandBinding(RoutedCommand);
			commandBinding.CanExecute += GenerateCanExecuteEventHandler;					
			commandBinding.Executed += ForwardExecutedEventHandler;
			ActiveCommandBindings.Add(commandBinding);
		}
		
		/// <summary>
		/// Gets <see cref="InputBindingCollection" /> generated by this <see cref="InputBindingInfo" />
		/// </summary>
		public CommandBindingCollection ActiveCommandBindings
		{
			get; private set;
		}

		/// <summary>
		/// Forwards <see cref="CommandBinding.Executed" /> event to event handler provided provided by developer
		/// through one of the available means
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		internal void ForwardExecutedEventHandler(object sender, ExecutedRoutedEventArgs e) {
			if(ExecutedEventHandler != null) {
				ExecutedEventHandler.Invoke(sender, e);
			} else {
				if(IsLazy && CommandInstance == null) {
					AddIn.LoadRuntimeAssemblies();
					
					var command = (ICommand)AddIn.CreateObject(CommandTypeName);
					SDCommandManager.LoadCommand(CommandTypeName, command);
				}
				
				if(CommandInstance != null) {
					CommandInstance.Execute(e.Parameter);
				}
			}
		}
		

		/// <summary>
		/// Forwards <see cref="CommandBinding.CanExecute" /> event to event handler provided provided by developer
		/// through one of the available means
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
