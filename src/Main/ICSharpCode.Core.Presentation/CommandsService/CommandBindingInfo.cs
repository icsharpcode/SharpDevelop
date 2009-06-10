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
		/// Binded command full name
		/// 
		/// This command is invoke when this binding is triggered
		/// </summary>
		/// <seealso cref="Class"></seealso>
		public string ClassName {
			get; set;
		}
		
		private System.Windows.Input.ICommand classInstance;
		
		/// <summary>
		/// Binded command instance
		/// 
		/// This command is invoke when this binding is triggered. If this value is equal 
		/// to null then add-in is not loaded yet, see <see cref="IsLazy">IsLazy</see> attribute
		/// for details
		/// </summary>
		/// <seealso cref="ClassName"></seealso>
		public System.Windows.Input.ICommand Class { 
			set {
				classInstance = value;
			}
			get {
				if(classInstance != null) {
					return classInstance;
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
		/// Context class full name
		/// 
		/// Described binding will be valid in this context
		/// </summary>
		public string ContextName{
			get; set;
		}
					
		/// <summary>
		/// Context class instance
		/// 
		/// Described binding will be valid in this context
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
		/// to false then this binding can't be triggered until add-in is loaded.
		/// </summary>
		public bool IsLazy{
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
			if(CanExecutedEventHandler != null) {
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
