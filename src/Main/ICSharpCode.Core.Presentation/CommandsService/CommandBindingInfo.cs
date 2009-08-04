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
		/// <summary>
		/// Creates new instance of <see cref="CommandBindingInfo" />
		/// </summary>
		public CommandBindingInfo()
		{
			OldCommandBindings = new CommandBindingCollection();
			ActiveCommandBindings = new CommandBindingCollection();
			
			Groups = new BindingGroupCollection();
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
					SDCommandManager.LoadAddinCommands(AddIn);
				}
				
				System.Windows.Input.ICommand command;
				SDCommandManager.commands.TryGetValue(CommandTypeName, out command);
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
		
		List<UIElement> oldInstances;
		
		protected override void SetInstanceBindings(ICollection<UIElement> newInstances)
		{				
			if(oldInstances != null) {
				foreach(var ownerInstance in oldInstances) {
					foreach(CommandBinding binding in OldCommandBindings) {
						ownerInstance.CommandBindings.Remove(binding);
					}
				}
			}
			
			oldInstances = new List<UIElement>();
			
			if(newInstances != null) {
				foreach(var ownerInstance in newInstances) {
					ownerInstance.CommandBindings.AddRange(ActiveCommandBindings);
					oldInstances.Add(ownerInstance);
				}
			}
		}
		
		List<Type> oldTypes;
		
		protected override void SetClassBindings(ICollection<Type> newTypes)
		{
			if(oldTypes != null) {
				foreach(var ownerType in oldTypes) {
					foreach(CommandBinding binding in OldCommandBindings) {
						SDCommandManager.RemoveClassCommandBinding(ownerType, binding);
					}
				}
			}
			
			oldTypes = new List<Type>();
			
			if(newTypes != null) {
				foreach(var ownerType in newTypes) {
					foreach(CommandBinding binding in ActiveCommandBindings) {
						System.Windows.Input.CommandManager.RegisterClassCommandBinding(ownerType, binding);
						oldTypes.Add(ownerType);
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
		/// Re-generate <see cref="CommandBinding" /> from <see cref="CommandBindingInfo" />
		/// </summary>
		protected override void GenerateBindings() 
		{			
			OldCommandBindings = ActiveCommandBindings;
			
			ActiveCommandBindings = new CommandBindingCollection();
			
			var commandBinding = new CommandBinding(RoutedCommand);
			commandBinding.CanExecute += GenerateCanExecuteEventHandler;					
			commandBinding.Executed += GenerateExecutedEventHandler;
			ActiveCommandBindings.Add(commandBinding);
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
					SDCommandManager.LoadCommand(CommandTypeName, command);
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
