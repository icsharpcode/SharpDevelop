using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows;
using ICSharpCode.Core;
using System.Threading;

namespace ICSharpCode.Core.Presentation
{
	public delegate void BindingsUpdatedHandler();
	
	/// <summary>
	/// Global registry to store and access commands, command bindings and input bindings
	/// </summary>
	public static class CommandsRegistry
	{
		/// <summary>
		/// Default application context.
		/// 
		/// This should be set to the root UI element
		/// </summary>
		public static string DefaultContext {
			get; set;
		}
		
		private static List<CommandBindingInfo> commandBindings = new List<CommandBindingInfo>();
		private static List<InputBindingInfo> inputBidnings = new List<InputBindingInfo>();
		
		private static Dictionary<string, RoutedUICommand> routedCommands = new Dictionary<string, RoutedUICommand>();
		internal static Dictionary<string, System.Windows.Input.ICommand> commands = new Dictionary<string, System.Windows.Input.ICommand>();
		internal static Dictionary<string, UIElement> contexts = new Dictionary<string, UIElement>();
		
		private static Dictionary<string, List<BindingsUpdatedHandler>> commandBindingsUpdateHandlers = new Dictionary<string, List<BindingsUpdatedHandler>>();
		private static Dictionary<string, List<BindingsUpdatedHandler>> inputBindingsUpdateHandlers = new Dictionary<string, List<BindingsUpdatedHandler>>();

		/// <summary>
		/// Get reference to routed UI command by name
		/// </summary>
		/// <param name="routedCommandName">Routed command name</param>
		/// <returns>Routed command instance</returns>
		public static RoutedUICommand GetRoutedUICommand(string routedCommandName) {
			RoutedUICommand routedUICommand;
			routedCommands.TryGetValue(routedCommandName, out routedUICommand);
			
			return routedUICommand;
		}

		/// <summary>
		/// Register new routed command in the global registry
		/// 
		/// Routed command name should be uniq in SharpDevelop scope. 
		/// Use "." to emulate namespaces
		/// </summary>
		/// <param name="routedCommandName">Routed command name</param>
		/// <param name="text">Short text describing command functionality</param>
		public static void RegisterRoutedUICommand(string routedCommandName, string text) {
			var routedCommand = new RoutedUICommand(text, routedCommandName, typeof(UIElement));
			
			if(!routedCommands.ContainsKey(routedCommandName)) {
				routedCommands.Add(routedCommandName, routedCommand);
			} else {
				throw new IndexOutOfRangeException("Routed command with name " + routedCommandName + " is already registered");
			}
		}
	
		/// <summary>
		/// Remove routed command from global registry
		/// </summary>
		/// <param name="routedCommandName">Routed command name</param>
		public static void UnregisterRoutedUICommand(string routedCommandName) {
			if(routedCommands.ContainsKey(routedCommandName)) {
				routedCommands.Remove(routedCommandName);
			}
		}
		
		
		/// <summary>
		/// Register input binding
		/// 
		/// Registering input binding means that when provided gesture is met in specified 
		/// context routed command will be invoked
		/// </summary>
		/// <param name="contextName">Context class full name</param>
		/// <param name="routedCommandName">Routed UI command invoked on gesture run</param>
		/// <param name="gesture">Gesture</param>
		public static void RegisterInputBinding(string contextName, string routedCommandName, KeyGesture gesture) {			
			var inputBindingInfo = new InputBindingInfo(contextName, routedCommandName, gesture);
			inputBidnings.Add(inputBindingInfo);
		}
		
		/// <summary>
		/// Remove input binding from global registry
		/// 
		/// Null attributes are ignored and oly bindings which satisfy not null arguments are removed
		/// </summary>
		/// <param name="contextName">Context class full name</param>
		/// <param name="routedCommandName">Routed UI command name</param>
		/// <param name="gesture">Gesture</param>
		public static void UnregisterInputBindings(string contextName, string routedCommandName, KeyGesture gesture) {
			for(var i = inputBidnings.Count - 1; i >= 0; i--) {
				if((contextName == null || inputBidnings[i].ContextName == contextName)
				   && (routedCommandName == null || inputBidnings[i].RoutedCommandName == routedCommandName)
				   && (gesture == null || inputBidnings[i].Gesture == gesture)) {
					inputBidnings.RemoveAt(i);
				}
			}
		}
	
		/// <summary>
		/// Register delegate which will be invoked on change in input bindings in specified context
		/// </summary>
		/// <param name="contextName">Context class full name</param>
		/// <param name="handler">Update handler delegate</param>
		public static void RegisterInputBindingUpdateHandler(string contextName, BindingsUpdatedHandler handler) {
			if(!inputBindingsUpdateHandlers.ContainsKey(contextName)) {
				inputBindingsUpdateHandlers.Add(contextName, new List<BindingsUpdatedHandler>());
			}
			
			inputBindingsUpdateHandlers[contextName].Add(handler);
		}
		
		/// <summary>
		/// Invoke registered input bindings update handlers registered in specified context
		/// </summary>
		/// <param name="contextName">Context class full name</param>
		public static void InvokeInputBindingUpdateHandlers(string contextName) {
			if(contextName != null) {
				if(inputBindingsUpdateHandlers.ContainsKey(contextName)) {
					foreach(var handler in inputBindingsUpdateHandlers[contextName]) {
						handler.Invoke();
					}
				}
			} else {
				foreach(var contextHandlers in inputBindingsUpdateHandlers) {
					foreach(var handler in contextHandlers.Value) {
						handler.Invoke();
					}
				}
			}
		}
		
		/// <summary>
		/// Remove all managed input bindings from input bindings collection
		/// </summary>
		/// <param name="inputBindingCollection"></param>
		public static void RemoveManagedInputBindings(InputBindingCollection inputBindingCollection) {
			for(var i = inputBindingCollection.Count - 1; i >= 0; i--) {
				if(inputBindingCollection[i] is ManagedInputBinding) {
					inputBindingCollection.RemoveAt(i);
				}
			}
		}
		
		/// <summary>
		/// Register command binding
		/// 
		/// Registering command binding means that when provided routed command is invoked 
		/// in specified context event is routed to specified command (implementing ICommand class)
		/// </summary>
		/// <param name="contextName">Context class full name</param>
		/// <param name="routedCommandName">Routed UI command name</param>
		/// <param name="className">Command full name to which invokation event is routed</param>
		/// <param name="addIn">Add-in in which hosts the command</param>
		/// <param name="isLazy">Load add-in referenced assemblies on command invocation</param>
		public static void RegisterCommandBinding(string contextName, string routedCommandName, string className, AddIn addIn, bool isLazy) {
			var commandBindingInfo = new CommandBindingInfo(contextName, routedCommandName, className, addIn, isLazy);
			commandBindings.Add(commandBindingInfo);
		}
		
		/// <summary>
		/// Remove registered command bindnig from global registry
		/// </summary>
		/// <param name="contextName"></param>
		/// <param name="routedCommandName"></param>
		/// <param name="className"></param>
		public static void UnregisterCommandBindings(string contextName, string routedCommandName, string className) {	
			for(var i = commandBindings.Count - 1; i >= 0; i--) {
				if((contextName == null || commandBindings[i].ContextName == contextName)
				   && (routedCommandName == null || commandBindings[i].RoutedCommandName == routedCommandName)
				   && (className == null || commandBindings[i].ClassName == className)) {
					inputBidnings.RemoveAt(i);
				}
			}
		}
	
		/// <summary>
		/// Register delegate which will be invoked on any chage in command bindings of specified context
		/// </summary>
		/// <param name="contextName">Context class full name</param>
		/// <param name="handler">Update handler delegate</param>
		public static void RegisterCommandBindingsUpdateHandler(string contextName, BindingsUpdatedHandler handler) {
			if(!commandBindingsUpdateHandlers.ContainsKey(contextName)) {
				commandBindingsUpdateHandlers.Add(contextName, new List<BindingsUpdatedHandler>());
			}
			
			commandBindingsUpdateHandlers[contextName].Add(handler);
		}
		
		/// <summary>
		/// Invoke registered command bindings update handlers registered in specified context
		/// </summary>
		/// <param name="contextName">Context class full name</param>
		public static void InvokeCommandBindingUpdateHandlers(string contextName) {
			if(contextName != null) {
				if(commandBindingsUpdateHandlers.ContainsKey(contextName)) {
					foreach(var handler in commandBindingsUpdateHandlers[contextName]) {
						handler.Invoke();
					}
				}
			} else {
				foreach(var contextHandlers in commandBindingsUpdateHandlers) {
					foreach(var handler in contextHandlers.Value) {
						handler.Invoke();
					}
				}
			}
		}
		
		/// <summary>
		/// Remove all managed command bindungs from command bindings collection
		/// </summary>
		/// <param name="commandBindingsCollection"></param>
		public static void RemoveManagedCommandBindings(CommandBindingCollection commandBindingsCollection) {
			for(var i = commandBindingsCollection.Count - 1; i >= 0; i--) {
				if(commandBindingsCollection[i] is ManagedCommandBinding) {
					commandBindingsCollection.RemoveAt(i);
				}
			}
		}
		
		/// <summary>
		/// Load all registered commands in addin
		/// </summary>
		/// <param name="addIn">Addin</param>
		public static void LoadAddinCommands(AddIn addIn) {		
			foreach(var binding in commandBindings) {
				if(binding.AddIn != addIn) continue;
				
				LoadCommand(binding.ClassName, addIn.CreateObject(binding.ClassName));
			}
		}
		
		/// <summary>
		/// Load command
		/// </summary>
		/// <param name="commandName">Command name</param>
		/// <param name="command">Command instance</param>
		public static void LoadCommand(string commandName, object command) {
			var wpfCommand = command as System.Windows.Input.ICommand;
			if(wpfCommand == null) {
				wpfCommand = new WpfCommandWrapper((ICSharpCode.Core.ICommand)command);
			}
			
			commands.Add(commandName, wpfCommand);
		}
		
		/// <summary>
		/// Load context
		/// </summary>
		/// <param name="contextName">Context class full name</param>
		/// <param name="context">Context class instance</param>
		public static void LoadContext(string contextName, UIElement context) {
			contexts[contextName] = context;
		}
		
		
		/// <summary>
		/// Get all commands bindings registered in provided context
		/// </summary>
		/// <param name="contextName">Context class full name</param>
		/// <returns>Collection of managed command bindings</returns>
		public static CommandBindingCollection GetCommandBindings(string contextName) {
			var bindings = new CommandBindingCollection();
			
			foreach(var binding in commandBindings) {
				if(binding.ContextName != contextName) continue;
				
				var managedCommandBinding = new ManagedCommandBinding(binding.RoutedCommand);
				
				managedCommandBinding.CanExecute += delegate(Object sender, CanExecuteRoutedEventArgs e) {
						if(binding.IsLazy && binding.Class == null) {
							e.CanExecute = true;
						} else if(binding.Class == null) {
							e.CanExecute = false;
						} else {
							e.CanExecute = binding.Class.CanExecute(e.Parameter); 						
						}
					};
				
				managedCommandBinding.Executed += delegate(Object sender, ExecutedRoutedEventArgs e) {
						if(binding.IsLazy && binding.Class == null) {
							binding.AddIn.LoadRuntimeAssemblies();
							
							var command = (ICommand)binding.AddIn.CreateObject(binding.ClassName);
							CommandsRegistry.LoadCommand(binding.ClassName, command);
						}
						
						if(binding.Class != null) {
							binding.Class.Execute(e.Parameter);
						}
					};
				
				bindings.Add(managedCommandBinding);
			}
			
			return bindings;
		}
		
		/// <summary>
		/// Get list of all input bindings registered in provided context
		/// </summary>
		/// <param name="contextName">Context class full name</param>
		/// <returns>Collection of managed command bindings</returns>
		public static InputBindingCollection GetInputBindings(string contextName) {
			var bindings = new InputBindingCollection();
			
			foreach(var binding in inputBidnings) {
				if(binding.ContextName != contextName) continue;
				
				bindings.Add(new ManagedInputBinding(binding.RoutedCommand, binding.Gesture));
			}
			
			return bindings;
		}
	}	
}
