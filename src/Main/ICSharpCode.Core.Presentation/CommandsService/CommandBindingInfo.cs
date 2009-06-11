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
		private UIElement contextInstance;
		
		public CommandBindingInfo()
		{
			ContextName = CommandsRegistry.DefaultContextName;
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
		/// Name of the class which owns this binding
		/// </summary>
		public string ContextName{
			get; set;
		}
					
		/// <summary>
		/// Instance of class which owns this binding
		/// </summary>
		public UIElement Context { 
			set {
				contextInstance = value;
			}
			get {
				if(contextInstance != null) {
					return contextInstance;
				} else {
					UIElement context;
					CommandsRegistry.contexts.TryGetValue(ContextName, out context);
					
					return context;
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
		
		public ExecutedRoutedEventHandler ExecutedEventHandler
		{
			get; set;
		}
		
		public CanExecuteRoutedEventHandler CanExecutedEventHandler
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
