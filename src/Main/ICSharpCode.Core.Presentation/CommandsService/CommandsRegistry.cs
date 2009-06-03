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
		/// This element is used to represent null key in dictionary
		/// </summary>
		private static UIElement NullUIElement = new UIElement();
		
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
		
		private static Dictionary<string, Dictionary<UIElement, List<BindingsUpdatedHandler>>> commandBindingsUpdateHandlers = new Dictionary<string, Dictionary<UIElement, List<BindingsUpdatedHandler>>>();
		private static Dictionary<string, Dictionary<UIElement, List<BindingsUpdatedHandler>>> inputBindingsUpdateHandlers = new Dictionary<string, Dictionary<UIElement, List<BindingsUpdatedHandler>>>();

		/// <summary>
		/// Get reference to routed UI command by name
		/// </summary>
		/// <param name="routedCommandName">Routed command name</param>
		/// <returns>Routed command instance</returns>
		public static RoutedUICommand GetRoutedUICommand(string routedCommandName) {
			if(routedCommands.ContainsKey(routedCommandName)) {
				return routedCommands[routedCommandName];
			}
			
			return null;
		}
		
		/// <summary>
		/// Checks whether routed UI command registered
		/// </summary>
		/// <param name="routedCommandName">Routed command name</param>
		/// <returns>Returns value specifting whether routed UI command is registered</returns>
		public static bool IsRoutedUICommandRegistered(string routedCommandName) {
			return GetRoutedUICommand(routedCommandName) != null;
		}

		/// <summary>
		/// Register new routed command in the global registry
		/// 
		/// Routed command name should be unique in SharpDevelop scope. 
		/// Use "." to organize commands into groups
		/// </summary>
		/// <param name="routedCommandName">Routed command name</param>
		/// <param name="text">Short text describing command functionality</param>
		public static void RegisterRoutedUICommand(string routedCommandName, string text) {
			var routedCommand = new RoutedUICommand(text, routedCommandName, typeof(CommandsRegistry));
			
			if(!routedCommands.ContainsKey(routedCommandName)) {
				routedCommands.Add(routedCommandName, routedCommand);
			} else {
				var test = routedCommands[routedCommandName];
				throw new IndexOutOfRangeException("Routed UI command with name " + routedCommandName + " is already registered");
			}
		}

		/// <summary>
		/// Register existing routed command in the global registry
		/// 
		/// Routed command then can be accessed 
		/// Routed command name should be uniq in SharpDevelop scope. 
		/// Use "." to organize commands into groups
		/// </summary>
		/// <param name="routedCommandName">Existing routed command</param>
		public static void RegisterRoutedUICommand(RoutedUICommand existingRoutedUICommand) {
			string routedCommandName = existingRoutedUICommand.OwnerType.Name + "." + existingRoutedUICommand.Name;
			
			if(!routedCommands.ContainsKey(routedCommandName)) {
				routedCommands.Add(existingRoutedUICommand.OwnerType.Name + "." + existingRoutedUICommand.Name, existingRoutedUICommand);
			} else {
				throw new IndexOutOfRangeException("Routed UI command with name " + routedCommandName + " is already registered");
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
		/// Register input binding in context
		/// 
		/// Registering input binding means that when specified gesture is met in specified 
		/// context routed command is invoked
		/// </summary>
		/// <param name="contextName">Context class full name</param>
		/// <param name="contextInstance">Context class instance. If instance is provided this input binding only applies to provided UI element</param>
		/// <param name="routedCommandName">Routed UI command invoked on gesture run</param>
		/// <param name="gesture">Gesture</param>
		public static void RegisterInputBinding(string contextName, UIElement contextInstance, string routedCommandName, InputGesture gesture) {			
			var inputBindingInfo = new InputBindingInfo(contextName, contextInstance, routedCommandName, gesture);
			inputBidnings.Add(inputBindingInfo);
		}
		
		/// <summary>
		/// Remove input bindings which satisfy provided arguments
		/// 
		/// Null arguments are ignored
		/// </summary>
		/// <param name="contextName">Context class full name</param>
		/// <param name="contextInstance">Unregister binding assigned to specific context instance</param>
		/// <param name="routedCommandName">Routed UI command name</param>
		/// <param name="gesture">Gesture</param>
		public static void UnregisterInputBindings(string contextName, UIElement contextInstance, string routedCommandName, InputGesture gesture) {
			for(int i = inputBidnings.Count - 1; i >= 0; i--) {
				if((contextName == null || inputBidnings[i].ContextName == contextName)
				   && (contextInstance == null || inputBidnings[i].Context == null || inputBidnings[i].Context == contextInstance)
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
		/// <param name="contextInstance">Register update handler which will trigger only if input bindings registered for this object where triggered</param>
		/// <param name="handler">Update handler delegate</param>
		public static void RegisterInputBindingUpdateHandler(string contextName, UIElement contextInstance, BindingsUpdatedHandler handler) {
			if(contextInstance == null) {
				contextInstance = NullUIElement;
			}
			
			if(!inputBindingsUpdateHandlers.ContainsKey(contextName)) {
				inputBindingsUpdateHandlers.Add(contextName, new Dictionary<UIElement, List<BindingsUpdatedHandler>>());
			}
			
			if(!inputBindingsUpdateHandlers[contextName].ContainsKey(contextInstance)) {
				inputBindingsUpdateHandlers[contextName].Add(contextInstance, new List<BindingsUpdatedHandler>());
			}
			
			inputBindingsUpdateHandlers[contextName][contextInstance].Add(handler);
		}
	
		/// <summary>
		/// Remove input bindings update handler
		/// </summary>
		/// <param name="contextName">Context class full name</param>
		/// <param name="contextInstance">Unregister update handler which was triggered only if input bindings registered for specific instance where updated</param>
		/// <param name="handler">Update handler delegate</param>
		public static void UnregisterInputBindingUpdateHandler(string contextName, UIElement contextInstance, BindingsUpdatedHandler handler) {
			if(contextInstance == null) {
				contextInstance = NullUIElement;
			}
			if(!inputBindingsUpdateHandlers.ContainsKey(contextName)) {
				if(!inputBindingsUpdateHandlers[contextName].ContainsKey(contextInstance)) {
					for(int i = inputBindingsUpdateHandlers[contextName][contextInstance].Count - 1; i >= 0; i++) {
						if(inputBindingsUpdateHandlers[contextName][contextInstance][i] == handler) {
							inputBindingsUpdateHandlers[contextName][contextInstance].RemoveAt(i);
						}
					}
				}
			}
		}
		
		
		/// <summary>
		/// Invoke registered input bindings update handlers registered in specified context
		/// </summary>
		/// <param name="contextName">Context class full name</param>
		public static void InvokeInputBindingUpdateHandlers(string contextName, UIElement contextInstance) {
			if(contextInstance == null) {
				contextInstance = NullUIElement;
			}
			
			if(contextName != null) {
				if(inputBindingsUpdateHandlers.ContainsKey(contextName)) {
					if(contextInstance == NullUIElement) {
						foreach(var instanceHandlers in inputBindingsUpdateHandlers[contextName]) {
							foreach(var handler in instanceHandlers.Value) {
								if(handler != null) {
									((BindingsUpdatedHandler)handler).Invoke();
								}
							}
						}
					}
				} else if(inputBindingsUpdateHandlers[contextName].ContainsKey(contextInstance)) {
		          	foreach(var handler in inputBindingsUpdateHandlers[contextName][contextInstance]) {
						if(handler != null) {
							((BindingsUpdatedHandler)handler).Invoke();
						}
		          	}
				}
			} else {
				foreach(var contextHandlers in inputBindingsUpdateHandlers) {
					foreach(var instanceHandlers in contextHandlers.Value) {
						foreach(var handler in instanceHandlers.Value) {
							if(handler != null) {
								((BindingsUpdatedHandler)handler).Invoke();
							}
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Remove all managed input bindings from input bindings collection
		/// </summary>
		/// <param name="inputBindingCollection"></param>
		public static void RemoveManagedInputBindings(InputBindingCollection inputBindingCollection) {
			for(int i = inputBindingCollection.Count - 1; i >= 0; i--) {
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
		/// <param name="contextInstance">Register update handler which is triggered only if input bindings registered for specific instance are updated</param>
		/// <param name="routedCommandName">Routed UI command name</param>
		/// <param name="className">Command full name to which invokation event is routed</param>
		/// <param name="addIn">Add-in in which hosts the command</param>
		/// <param name="isLazy">Load add-in referenced assemblies on command invocation</param>
		public static void RegisterCommandBinding(string contextName, UIElement contextInstance, string routedCommandName, string className, AddIn addIn, bool isLazy) {
			var commandBindingInfo = new CommandBindingInfo(contextName, contextInstance, routedCommandName, className, addIn, isLazy);
			commandBindings.Add(commandBindingInfo);
		}
		
		/// <summary>
		/// Register command binding which when triggered provided delegates are invoked
		/// </summary>
		/// <param name="contextName">Context class full name</param>
		/// <param name="contextInstance">Register update handler which is triggered only if input bindings registered for specific instance are updated</param>
		/// <param name="routedCommandName">Routed UI command name</param>
		/// <param name="executedEventHandler">Delegate which is called when binding is triggered</param>
		/// <param name="canExecuteEventHandler">Delegate which is called to check whether executedEventHandler can be invoked</param>
		public static void RegisterCommandBinding(string contextName, UIElement contextInstance, string routedCommandName, ExecutedRoutedEventHandler executedEventHandler, CanExecuteRoutedEventHandler canExecuteEventHandler) {
			var commandBindingInfo = new CommandBindingInfo(contextName, contextInstance, routedCommandName, executedEventHandler, canExecuteEventHandler);
			commandBindings.Add(commandBindingInfo);
		}
		
		/// <summary>
		/// Remove all command bindings which satisfy provided parameters
		/// 
		/// Null arguments are ignored
		/// </summary>
		/// <param name="contextName">Context class full name</param>
		/// <param name="contextInstance">Unregister update handler which was triggered only if command bindings registered for specific instance where updated</param>
		/// <param name="routedCommandName">Routed UI command name</param>
		/// <param name="className">Command full name to which invokation event is routed</param>
		public static void UnregisterCommandBindings(string contextName, UIElement contextInstance, string routedCommandName, string className) {	
			for(int i = commandBindings.Count - 1; i >= 0; i--) {
				if((contextName == null || commandBindings[i].ContextName == contextName)
				   && (contextInstance == null || commandBindings[i].Context == null || commandBindings[i].Context == contextInstance)
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
		/// <param name="contextInstance">Register update handler which is triggered only if input bindings registered for specific instance are updated</param>
		/// <param name="handler">Update handler delegate</param>
		public static void RegisterCommandBindingsUpdateHandler(string contextName, UIElement contextInstance, BindingsUpdatedHandler handler) {
			if(contextInstance == null) {
				contextInstance = NullUIElement;
			}
			if(!commandBindingsUpdateHandlers.ContainsKey(contextName)) {
				commandBindingsUpdateHandlers.Add(contextName, new Dictionary<UIElement, List<BindingsUpdatedHandler>>());
			}
			
			if(!commandBindingsUpdateHandlers[contextName].ContainsKey(contextInstance)) {
				commandBindingsUpdateHandlers[contextName].Add(contextInstance, new List<BindingsUpdatedHandler>());
			}
			
			commandBindingsUpdateHandlers[contextName][contextInstance].Add(handler);
		}
	
		/// <summary>
		/// Remove handler command bindings update handler
		/// </summary>
		/// <param name="contextName">Context class full name</param>
		/// <param name="contextInstance">Unregister update handler which was triggered only if input bindings registered for specific instance were updated</param>
		/// <param name="handler">Update handler delegate</param>
		public static void UnregisterCommandBindingsUpdateHandler(string contextName, UIElement contextInstance, BindingsUpdatedHandler handler) {
			if(contextInstance == null) {
				contextInstance = NullUIElement;
			}
			if(commandBindingsUpdateHandlers.ContainsKey(contextName)) {
				if(commandBindingsUpdateHandlers[contextName].ContainsKey(contextInstance)) {
					for(int i = commandBindingsUpdateHandlers[contextName][contextInstance].Count - 1; i >= 0; i--) {
						if(commandBindingsUpdateHandlers[contextName][contextInstance][i] == handler) {
							commandBindingsUpdateHandlers[contextName][contextInstance].RemoveAt(i);
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Invoke registered command bindings update handlers registered in specified context
		/// </summary>
		/// <param name="contextName">Context class full name</param>
		/// <param name="contextInstance">Invoke update handlers which handle update only in specifyc context</param>
		public static void InvokeCommandBindingUpdateHandlers(string contextName, UIElement contextInstance) {
			if(contextInstance == null) {
				contextInstance = NullUIElement;
			}
			
			if(contextName != null) {
				if(commandBindingsUpdateHandlers.ContainsKey(contextName)) {
					if(contextInstance == NullUIElement) {
						foreach(var instanceHandlers in commandBindingsUpdateHandlers[contextName]) {
							foreach(var handler in instanceHandlers.Value) {
								if(handler != null) {
									((BindingsUpdatedHandler)handler).Invoke();
								}
							}
						}
					} else if(commandBindingsUpdateHandlers[contextName].ContainsKey(contextInstance)) {
						foreach(var handler in commandBindingsUpdateHandlers[contextName][contextInstance]) {
							if(handler != null) {
								((BindingsUpdatedHandler)handler).Invoke();
							}
						}
					}
				}
			} else {
				foreach(var contextHandlers in commandBindingsUpdateHandlers) {
					foreach(var instanceHandlers in contextHandlers.Value) {
						foreach(var handler in instanceHandlers.Value) {
							if(handler != null) {
								((BindingsUpdatedHandler)handler).Invoke();
							}
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Remove all managed command bindungs from command bindings collection
		/// </summary>
		/// <param name="commandBindingsCollection"></param>
		public static void RemoveManagedCommandBindings(CommandBindingCollection commandBindingsCollection) {
			for(int i = commandBindingsCollection.Count - 1; i >= 0; i--) {
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
				
				if(!commands.ContainsKey(binding.ClassName)){
					var command = addIn.CreateObject(binding.ClassName);
					var wpfCommand = command as System.Windows.Input.ICommand;
					if(wpfCommand == null) {
						wpfCommand = new WpfCommandWrapper((ICSharpCode.Core.ICommand)command);
					}
				
					commands.Add(binding.ClassName, wpfCommand);
				}
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
			
			if(!commands.ContainsKey(commandName)) {
				commands.Add(commandName, wpfCommand);
			}
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
		/// Get list of all command bindings which satisfy provided parameters
		/// 
		/// Null arguments are ignored
		/// </summary>
		/// <param name="contextName">Context class full name</param>
		/// <param name="contextInstance">Get command bindings assigned only to specific context</param>
		/// <param name="routedCommandName">Context class full name</param>
		/// <param name="className">Context class full name</param>
		/// <returns>Collection of managed command bindings</returns>
		public static CommandBindingCollection GetCommandBindings(string contextName, UIElement contextInstance, string routedCommandName, string className) {
			var bindings = new CommandBindingCollection();
			
			foreach(var binding in commandBindings) {
				if((contextName == null || binding.ContextName == contextName)
				    && (contextInstance == null || binding.Context == null || binding.Context == contextInstance)
					&& (routedCommandName == null || binding.RoutedCommandName == routedCommandName)
					&& (className == null || binding.ClassName == className)) {
				   	
					var managedCommandBinding = new ManagedCommandBinding(binding.RoutedCommand);
					managedCommandBinding.CanExecute += binding.CanExecuteEventHandler;					
					managedCommandBinding.Executed += binding.ExecutedEventHandler;
					
					bindings.Add(managedCommandBinding);
				}
			}
			
			return bindings;
		}
		
		/// <summary>
		/// Get list of all input bindings which satisfy provided parameters
		/// 
		/// Null arguments are ignored
		/// </summary>
		/// <param name="contextName">Context class full name</param>
		/// <param name="contextInstance">Get input bindings assigned only to specific context</param>
		/// <param name="routedCommandName">Routed UI command name</param>
		/// <param name="gesture">Gesture</param>
		public static InputBindingCollection GetInputBindings(string contextName, UIElement contextInstance, string routedCommandName, InputGesture gesture) {
			var bindings = new InputBindingCollection();
			
			foreach(var binding in inputBidnings) {
				if((contextName == null || binding.ContextName == contextName)
				   && (contextInstance == null || binding.Context == null || binding.Context == contextInstance)
					&& (routedCommandName == null || binding.RoutedCommandName == routedCommandName)
					&& (gesture == null || binding.Gesture == gesture)) {
					
					bindings.Add(new ManagedInputBinding(binding.RoutedCommand, binding.Gesture));
				}
			}
			
			return bindings;
		}
		
		/// <summary>
		/// Get list of input gestures from all input bindings which satisfy provided parameters
		/// 
		/// Null arguments are ignored
		/// </summary>
		/// <param name="contextName">Context class full name</param>
		/// <param name="contextInstance">Get gestures assigned only to specific context</param>
		/// <param name="routedCommandName">Routed UI command name</param>
		/// <param name="gesture">Gesture</param>
		public static InputGestureCollection GetInputGestures(string contextName, UIElement contextInstance, string routedCommandName, InputGesture gesture) {
			var bindings = GetInputBindings(contextName, contextInstance, routedCommandName, gesture);
			var gestures = new InputGestureCollection();
			
			foreach(InputBinding binding in bindings) {
				gestures.Add(binding.Gesture);
			}
			
			return gestures;
		}
		
		/// <summary>
		/// Create default BindingUpdateHandler which will update command bindings in specified context
		/// </summary>
		/// <param name="bindingsCollection">Collection which should be updated with latest bindings</param>
		/// <param name="contextName">Context name which was used to register command bindings</param>
		/// <param name="contextInstance">Reference to instance which is used to find command bindings registered in specific context instance</param>
		/// <returns>Bindings updated handler</returns>
		public static BindingsUpdatedHandler CreateCommandBindingUpdateHandler(CommandBindingCollection bindingsCollection, string contextName, UIElement contextInstance) {
			return new CommonCommandBindingUpdateDelegate(bindingsCollection, contextName, contextInstance).UpdateCommandBinding;
		}
		
		/// <summary>
		/// Create default BindingUpdateHandler which will update input bindings in specified context
		/// </summary>
		/// <param name="bindingsCollection">Collection which should be updated with latest bindings</param>
		/// <param name="contextName">Context name which was used to register input bindings</param>
		/// <param name="contextInstance">Reference to instance which is used to find command bindings registered in specific context instance</param>
		/// <returns>Bindings updated handler</returns>
		public static BindingsUpdatedHandler CreateInputBindingUpdateHandler(InputBindingCollection bindingsCollection, string contextName, UIElement contextInstance) {
			return new CommonInputBindingUpdateDelegate(bindingsCollection, contextName, contextInstance).UpdateInputBinding;
		}
		
		class CommonCommandBindingUpdateDelegate
		{
			CommandBindingCollection bindingsCollection;
			string contextName;
			UIElement contextInstance;
			
			public CommonCommandBindingUpdateDelegate(CommandBindingCollection bindingsCollection, string contextName, UIElement contextInstance) {
				this.bindingsCollection = bindingsCollection;
				this.contextName = contextName;
				this.contextInstance = contextInstance;
			}
			
			public void UpdateCommandBinding() {	
            	var newBindings = CommandsRegistry.GetCommandBindings(contextName, contextInstance, null, null);
            	CommandsRegistry.RemoveManagedCommandBindings(bindingsCollection);
            	bindingsCollection.AddRange(newBindings);
			}
		}
		
		
		class CommonInputBindingUpdateDelegate
		{
			InputBindingCollection bindingsCollection;
			string contextName;
			UIElement contextInstance;
			
			public CommonInputBindingUpdateDelegate(InputBindingCollection bindingsCollection, string contextName, UIElement contextInstance) {
				this.bindingsCollection = bindingsCollection;
				this.contextName = contextName;
				this.contextInstance = contextInstance;
			}
			
			public void UpdateInputBinding() {	
            	var newBindings = CommandsRegistry.GetInputBindings(contextName, contextInstance, null, null);
            	CommandsRegistry.RemoveManagedInputBindings(bindingsCollection);
            	bindingsCollection.AddRange(newBindings);
			}
		}	
	}	
}
