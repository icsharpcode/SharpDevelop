using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows;
using ICSharpCode.Core;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

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
		public static string DefaultContextName {
			get; set;
		}
		
		private static List<CommandBindingInfo> commandBindings = new List<CommandBindingInfo>();
		private static List<InputBindingInfo> inputBidnings = new List<InputBindingInfo>();
		
		private static Dictionary<string, RoutedUICommand> routedCommands = new Dictionary<string, RoutedUICommand>();
		internal static Dictionary<string, System.Windows.Input.ICommand> commands = new Dictionary<string, System.Windows.Input.ICommand>();
		internal static Dictionary<string, UIElement> contexts = new Dictionary<string, UIElement>();
		
		private static Dictionary<string, Dictionary<UIElement, List<BindingsUpdatedHandler>>> commandBindingsUpdateHandlers = new Dictionary<string, Dictionary<UIElement, List<BindingsUpdatedHandler>>>();
		private static Dictionary<string, Dictionary<UIElement, List<BindingsUpdatedHandler>>> inputBindingsUpdateHandlers = new Dictionary<string, Dictionary<UIElement, List<BindingsUpdatedHandler>>>();

		private static List<InputBindingCategory> categories = new List<InputBindingCategory>();
		
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
		public static RoutedUICommand RegisterRoutedUICommand(string routedCommandName, string text) {
			var routedCommand = new RoutedUICommand(text, routedCommandName, typeof(CommandsRegistry));
			
			if(!routedCommands.ContainsKey(routedCommandName)) {
				routedCommands.Add(routedCommandName, routedCommand);
			} else {
				var test = routedCommands[routedCommandName];
				throw new IndexOutOfRangeException("Routed UI command with name " + routedCommandName + " is already registered");
			}
			
			return routedCommand;
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
		/// Register input binding by specifying this binding parameters
		/// </summary>
		/// <param name="inputBindingInfo">Input binding parameters</param>
		public static void RegisterInputBinding(InputBindingInfo inputBindingInfo)
		{
			inputBidnings.Add(inputBindingInfo);
			CommandsRegistry.InvokeCommandBindingUpdateHandlers(inputBindingInfo.ContextName, null);
		}
		
		/// <summary>
		/// Unregister input binding
		/// </summary>
		/// <param name="inputBindingInfo">Input binding parameters</param>
		public static void UnregisterInputBinding(InputBindingInfo inputBindingInfo)
		{
			inputBidnings.Remove(inputBindingInfo);
		}
		
		/// <summary>
		/// Find input input bindings which satisfy provided arguments
		/// 
		/// Null arguments are ignored
		/// </summary>
		/// <param name="contextName">Context class full name</param>
		/// <param name="contextInstance">Unregister binding assigned to specific context instance</param>
		/// <param name="routedCommandName">Routed UI command name</param>
		public static ICollection<InputBindingInfo> FindInputBindingInfos(string contextName, UIElement contextInstance, string routedCommandName) {
			var foundBindings = new List<InputBindingInfo>();
			for(int i = inputBidnings.Count - 1; i >= 0; i--) {
				if((contextName == null || inputBidnings[i].ContextName == contextName)
				   && (contextInstance == null || inputBidnings[i].Context == null || inputBidnings[i].Context == contextInstance)
				   && (routedCommandName == null || inputBidnings[i].RoutedCommandName == routedCommandName)) {
					foundBindings.Add(inputBidnings[i]);
				}
			}
			
			return foundBindings;
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
		/// Remove all managed input bindings from <see cref="InputBindingCollection" />
		/// </summary>
		/// <param name="inputBindingCollection">Input binding cllection containing managed input bindings</param>
		public static void RemoveManagedInputBindings(InputBindingCollection inputBindingCollection) {
			for(int i = inputBindingCollection.Count - 1; i >= 0; i--) {
				if(inputBindingCollection[i] is ManagedInputBinding) {
					inputBindingCollection.RemoveAt(i);
				}
			}
		}
		
		/// <summary>
		/// Register command binding by specifying command binding parameters
		/// </summary>
		/// <param name="commandBindingInfo">Command binding parameters</param>
		public static void RegisterCommandBinding(CommandBindingInfo commandBindingInfo) {
			commandBindings.Add(commandBindingInfo);
		}
		
		/// <summary>
		/// Unregister command binding
		/// </summary>
		/// <param name="commandBindingInfo">Command binding parameters</param>
		public static void UnregisterCommandBinding(CommandBindingInfo commandBindingInfo) {
			commandBindings.Remove(commandBindingInfo);
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
		/// Remove all managed command bindings from <see cref="CommandBindingCollection" />
		/// </summary>
		/// <param name="commandBindingsCollection">Command binding cllection containing managed input bindings</param>
		public static void RemoveManagedCommandBindings(CommandBindingCollection commandBindingsCollection) {
			for(int i = commandBindingsCollection.Count - 1; i >= 0; i--) {
				if(commandBindingsCollection[i] is ManagedCommandBinding) {
					commandBindingsCollection.RemoveAt(i);
				}
			}
		}
		
		/// <summary>
		/// Load all registered commands in add-in
		/// </summary>
		/// <param name="addIn">Add-in</param>
		public static void LoadAddinCommands(AddIn addIn) {		
			foreach(var binding in commandBindings) {
				if(binding.AddIn != addIn) continue;
		
				if(binding.ClassName != null && !commands.ContainsKey(binding.ClassName)){
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
		/// Register command object (either instance of <see cref="System.Windows.Input.ICommand" /> or <see cref="ICSharpCode.Core.ICommand" />)
		/// which can be identified by command name
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
		/// Register binding owner instance which can be identified by unique name
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
		public static ICollection<CommandBindingInfo> FindCommandBindingInfos(string contextName, UIElement contextInstance, string routedCommandName, string className) {
			var foundBindings = new List<CommandBindingInfo>();
			
			foreach(var binding in commandBindings) {
				if((contextName == null || binding.ContextName == contextName)
				    && (contextInstance == null || binding.Context == null || binding.Context == contextInstance)
					&& (routedCommandName == null || binding.RoutedCommandName == routedCommandName)
					&& (className == null || binding.ClassName == className)) {
				   	
					foundBindings.Add(binding);
				}
			}
			
			return foundBindings;
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
		public static CommandBindingCollection FindCommandBindings(string contextName, UIElement contextInstance, string routedCommandName, string className) {
			var commandBindingInfos = FindCommandBindingInfos(contextName, contextInstance, routedCommandName, className);
			
			var bindings = new CommandBindingCollection();
			foreach(var binding in commandBindingInfos) {
				var managedCommandBinding = new ManagedCommandBinding(binding.RoutedCommand);
				managedCommandBinding.CanExecute += binding.GeneratedCanExecuteEventHandler;					
				managedCommandBinding.Executed += binding.GeneratedExecutedEventHandler;
				
				bindings.Add(managedCommandBinding);
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
		public static InputBindingCollection FindInputBindings(string contextName, UIElement contextInstance, string routedCommandName) {
			var inputBindingInfos = FindInputBindingInfos(contextName, contextInstance, routedCommandName);
			
			var bindings = new InputBindingCollection();
			foreach(var binding in inputBindingInfos) {
				foreach(InputGesture bindingGesture in binding.Gestures) {
					bindings.Add(new ManagedInputBinding(binding.RoutedCommand, bindingGesture));
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
		public static InputGestureCollection FindInputGestures(string contextName, UIElement contextInstance, string routedCommandName) {
			var bindings = FindInputBindings(contextName, contextInstance, routedCommandName);
			var gestures = new InputGestureCollection();
			
			foreach(InputBinding binding in bindings) {
				gestures.Add(binding.Gesture);
			}
			
			return gestures;
		}
		
		/// <summary>
		/// Register input binding category
		/// 
		/// Format:
		/// , - Separates categories
		/// / - Describes hierarchy meaning category to the left is child of the category to the right
		/// 
		/// <code>parent/child</code>
		/// </summary>
		/// <param name="categoriesString">String representing list of categories.</param>
		/// <returns>Returns list of categories which can be assigned to input binding</returns>
		public static List<InputBindingCategory> RegisterInputBindingCategories(string categoriesString) {			
			var registeredCategories = new List<InputBindingCategory>();

			// Split categories
			var categoryPaths = Regex.Split(categoriesString, @"\s*\,\s*");
			foreach(var categoryPath in categoryPaths) {
				// Split category path
				var pathEntries = Regex.Split(categoryPath, @"\s*\/\s*").ToList();
				
				var accumulatedPath = "";
				InputBindingCategory parentCategory = null;
				
				// In a loop create category hierarchy specified in a path
				foreach(var categoryPathEntry in pathEntries) {
					accumulatedPath += "/" + categoryPathEntry;
					var matchingCategory = categories.FirstOrDefault(c => c.Path == accumulatedPath);
					if(matchingCategory == null) {
						matchingCategory = new InputBindingCategory(categoryPathEntry, parentCategory);
						matchingCategory.Path = accumulatedPath;
						categories.Add(matchingCategory);
					}
					parentCategory = matchingCategory;
				}
				
				registeredCategories.Add(parentCategory);
			}
			
			return registeredCategories;
		}
	}	
}
