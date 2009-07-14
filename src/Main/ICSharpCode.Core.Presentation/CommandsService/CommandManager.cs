using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;
using ICSharpCode.Core;

namespace ICSharpCode.Core.Presentation
{	
	/// <summary>
	/// Global registry to store and access commands, command bindings and input bindings
	/// </summary>
	public static class CommandManager
	{
		/// <summary>
		/// Default application context.
		/// 
		/// This should be set to the root UI element
		/// </summary>
		public static string DefaultContextName {
			get; set;
		}
		
		// Binding infos
		private static List<CommandBindingInfo> commandBindings = new List<CommandBindingInfo>();
		private static List<InputBindingInfo> inputBidnings = new List<InputBindingInfo>();
		
		// Commands
		private static Dictionary<string, RoutedUICommand> routedCommands = new Dictionary<string, RoutedUICommand>();
		internal static Dictionary<string, System.Windows.Input.ICommand> commands = new Dictionary<string, System.Windows.Input.ICommand>();
		
		// Command binding update hanlers
		private static Dictionary<string, List<BindingsUpdatedHandler>> classCommandBindingUpdateHandlers = new Dictionary<string, List<BindingsUpdatedHandler>>();
		private static Dictionary<string, List<BindingsUpdatedHandler>> instanceCommandBindingUpdateHandlers = new Dictionary<string, List<BindingsUpdatedHandler>>();
		
		// Input binding update handlers
		private static Dictionary<string, List<BindingsUpdatedHandler>> classInputBindingUpdateHandlers = new Dictionary<string, List<BindingsUpdatedHandler>>();
		private static Dictionary<string, List<BindingsUpdatedHandler>> instanceInputBindingUpdateHandlers = new Dictionary<string, List<BindingsUpdatedHandler>>();
				
		// Named instances and types
		private static Dictionary<string, UIElement> namedUIInstances = new Dictionary<string, UIElement>();
		private static Dictionary<string, Type> namedUITypes = new Dictionary<string, Type>();
		
		// Categories
		public static List<InputBindingCategory> InputBindingCategories = new List<InputBindingCategory>();
		
		/// <summary>
		/// Register UI element instance accessible by unique name
		/// </summary>
		/// <param name="instanceName">Instance name</param>
		/// <param name="element">Instance</param>
		public static void RegisterNamedUIElementInstance(string instanceName, UIElement element)
		{
			if(!namedUIInstances.ContainsKey(instanceName)){
				namedUIInstances.Add(instanceName, element);
				
				// If there are some bindings and update handlers already registered, 
				// but owner is not loaded then invoke those bindings
				if(instanceCommandBindingUpdateHandlers.ContainsKey(instanceName)) {
					InvokeCommandBindingUpdateHandlers(null, instanceName);
				}
				
				if(instanceInputBindingUpdateHandlers.ContainsKey(instanceName)) {
					InvokeInputBindingUpdateHandlers(null, instanceName);
				}
			}
		}
		
		/// <summary>
		/// Get instance by unique instance name
		/// </summary>
		/// <param name="instanceName">Instance name</param>
		/// <returns></returns>
		public static UIElement GetNamedUIElementInstance(string instanceName)
		{
			UIElement instance;
			namedUIInstances.TryGetValue(instanceName, out instance);
			
			return instance;
		}
		
		public static string GetNamedUIElementName(UIElement instance)
		{
			foreach(var currentInstance in namedUIInstances) {
				if(currentInstance.Value == instance) {
					return currentInstance.Key;
				}
			}
			
			return null;
		}
		
		/// <summary>
		/// Register UI type which can be accessible by name
		/// </summary>
		/// <param name="typeName">Type name</param>
		/// <param name="type">Type</param>
		public static void RegisterNamedUIType(string typeName, Type type)
		{
			if(!namedUITypes.ContainsKey(typeName)){
				namedUITypes.Add(typeName, type);
				
				// If any update handlers where assigned to the type and type was not 
				// loaded yet then invoke update handlers
				if(classCommandBindingUpdateHandlers.ContainsKey(typeName)) {
					InvokeCommandBindingUpdateHandlers(typeName, null);
				}
				
				if(classInputBindingUpdateHandlers.ContainsKey(typeName)) {
					InvokeInputBindingUpdateHandlers(typeName, null);
				}
			}
		}
		
		/// <summary>
		/// Get type by uniqe type name
		/// </summary>
		/// <param name="typeName">Type name</param>
		/// <returns>Type</returns>
		public static Type GetNamedUIType(string typeName)
		{
			Type instance;
			namedUITypes.TryGetValue(typeName, out instance);
			
			return instance;
		}
		
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
			var routedCommand = new RoutedUICommand(text, routedCommandName, typeof(CommandManager));
			
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
				routedCommands.Add(routedCommandName, existingRoutedUICommand);
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
			var similarInputBinding = FindInputBindingInfos(inputBindingInfo.OwnerTypeName, inputBindingInfo.OwnerInstanceName, inputBindingInfo.RoutedCommandName).FirstOrDefault();
			
			if(similarInputBinding != null) {
				foreach(InputGesture gesture in inputBindingInfo.DefaultGestures) {
					if(!similarInputBinding.DefaultGestures.ContainsTemplateFor(gesture, GestureCompareMode.ExactlyMatches)) {
						similarInputBinding.DefaultGestures.Add(gesture);
					}
				}
				
				similarInputBinding.Categories.AddRange(inputBindingInfo.Categories);
				similarInputBinding.Groups.AddRange(inputBindingInfo.Groups);
				
				similarInputBinding.IsModifyed = true;
				similarInputBinding.DefaultInputBindingHandler.Invoke();
			} else {
				if(inputBindingInfo.OwnerInstanceName != null || inputBindingInfo.OwnerTypeName != null) {
					RegisterDefaultInputBindingHandler(inputBindingInfo);
				} else {
					throw new ArgumentException("Binding owner must be specified");
				}
				
				inputBidnings.Add(inputBindingInfo);
			}
		}
		
		/// <summary>
		/// Unregister input binding
		/// </summary>
		/// <param name="inputBindingInfo">Input binding parameters</param>
		public static void UnregisterInputBinding(InputBindingInfo inputBindingInfo)
		{
			var similarInputBindingInfos = FindInputBindingInfos(inputBindingInfo.OwnerTypeName, inputBindingInfo.OwnerInstanceName, inputBindingInfo.RoutedCommandName);
			
			foreach(var similarInputBindingInfo in similarInputBindingInfos) {
				inputBidnings.Remove(similarInputBindingInfo);
				
				// Remove command bindings
				if(similarInputBindingInfo.OwnerType != null) {
					foreach(InputBinding binding in similarInputBindingInfo.OldInputBindings) {
						RemoveClassInputBinding(similarInputBindingInfo.OwnerType, binding);
					}
					
					foreach(InputBinding binding in similarInputBindingInfo.NewInputBindings) {
						RemoveClassInputBinding(similarInputBindingInfo.OwnerType, binding);
					}
				} else if (similarInputBindingInfo.OwnerInstance != null) {
					foreach(InputBinding binding in similarInputBindingInfo.OldInputBindings) {
						similarInputBindingInfo.OwnerInstance.InputBindings.Remove(binding);
					}
				
					foreach(InputBinding binding in similarInputBindingInfo.NewInputBindings) {
						similarInputBindingInfo.OwnerInstance.InputBindings.Remove(binding);
					}
				}
			}
		}
		
		/// <summary>
		/// Find input input binding infos which satisfy provided arguments
		/// 
		/// Null arguments are ignored
		/// </summary>
		/// <param name="contextName">Context class full name</param>
		/// <param name="contextInstance">Unregister binding assigned to specific context instance</param>
		/// <param name="routedCommandName">Routed UI command name</param>
		public static ICollection<InputBindingInfo> FindInputBindingInfos(string ownerTypeName, string ownerInstanceName, string routedCommandName) {
			var foundBindings = new List<InputBindingInfo>();
			
			foreach(var binding in inputBidnings) {
				if(    (ownerInstanceName == null || binding.OwnerInstanceName == ownerInstanceName)
				    && (ownerTypeName == null || binding.OwnerTypeName == ownerTypeName)
				    && (routedCommandName == null || binding.RoutedCommandName == routedCommandName)
				   ) {
					foundBindings.Add(binding);
				}
			}
			
			return foundBindings;
		}
		
		/// <summary>
		/// Remove input binding associated with type
		/// </summary>
		/// <param name="ownerType">Owner type</param>
		/// <param name="inputBinding">Input binding</param>
		public static void RemoveClassInputBinding(Type ownerType, InputBinding inputBinding)
		{
			var fieldInfo = typeof(System.Windows.Input.CommandManager).GetField("_classInputBindings", BindingFlags.Static | BindingFlags.NonPublic);
			var fieldData = (HybridDictionary)fieldInfo.GetValue(null);
			var classInputBindings = (InputBindingCollection)fieldData[ownerType];

			if(classInputBindings != null) {
				classInputBindings.Remove(inputBinding);
			}
		}
		
		internal static void OrderClassInputBindingsByChords(Type ownerType) 
		{
			var fieldInfo = typeof(System.Windows.Input.CommandManager).GetField("_classInputBindings", BindingFlags.Static | BindingFlags.NonPublic);
			var fieldData = (HybridDictionary)fieldInfo.GetValue(null);
			var classInputBindings = (InputBindingCollection)fieldData[ownerType];

			if(classInputBindings != null) {
				classInputBindings.SortByChords();
			}
		}
		
		
		internal static void OrderInstanceInputBindingsByChords(UIElement ownerType) 
		{
			if(ownerType.InputBindings != null) {
				ownerType.InputBindings.SortByChords();
			}
		}
		
		/// <summary>
		/// Remove command binding associated with type
		/// </summary>
		/// <param name="ownerType"></param>
		/// <param name="commandBinding"></param>
		public static void RemoveClassCommandBinding(Type ownerType, CommandBinding commandBinding) 
		{
			var fieldInfo = typeof(System.Windows.Input.CommandManager).GetField("_classCommandBindings", BindingFlags.Static | BindingFlags.NonPublic);
			var fieldData = (HybridDictionary)fieldInfo.GetValue(null);
			var classCommandBindings = (CommandBindingCollection)fieldData[ownerType];

			if(classCommandBindings != null) {
				classCommandBindings.Remove(commandBinding);
			}
		}
		
		/// <summary>
		/// Register command binding by specifying command binding parameters
		/// </summary>
		/// <param name="commandBindingInfo">Command binding parameters</param>
		public static void RegisterCommandBinding(CommandBindingInfo commandBindingInfo) {
			commandBindingInfo.GenerateCommandBindings();
			
			if(commandBindingInfo.OwnerInstanceName != null || commandBindingInfo.OwnerTypeName != null) {
				RegisterDefaultCommandBindingHandler(commandBindingInfo);
			} else {
				throw new ArgumentException("Binding owner must be specified");
			}
			
			commandBindings.Add(commandBindingInfo);
		}
		
		/// <summary>
		/// Unregister command binding
		/// </summary>
		/// <param name="commandBindingInfo">Command binding parameters</param>
		public static void UnregisterCommandBinding(CommandBindingInfo commandBindingInfo) {
			var similarCommandBindingInfos = FindCommandBindingInfos(commandBindingInfo.OwnerTypeName, commandBindingInfo.OwnerInstanceName, commandBindingInfo.RoutedCommandName);
			
			foreach(var similarCommandBindingInfo in similarCommandBindingInfos) {
				commandBindings.Remove(similarCommandBindingInfo);
				
				// Remove command bindings
				if(similarCommandBindingInfo.OwnerType != null) {
					foreach(CommandBinding binding in similarCommandBindingInfo.OldCommandBindings) {
						RemoveClassCommandBinding(similarCommandBindingInfo.OwnerType, binding);
					}
					
					foreach(CommandBinding binding in similarCommandBindingInfo.NewCommandBindings) {
						RemoveClassCommandBinding(similarCommandBindingInfo.OwnerType, binding);
					}
				} else if (similarCommandBindingInfo.OwnerInstance != null) {
					foreach(CommandBinding binding in similarCommandBindingInfo.OldCommandBindings) {
						similarCommandBindingInfo.OwnerInstance.CommandBindings.Remove(binding);
					}
				
					foreach(CommandBinding binding in similarCommandBindingInfo.NewCommandBindings) {
						similarCommandBindingInfo.OwnerInstance.CommandBindings.Remove(binding);
					}
				}
			}
		}
		
		#region Register bindings update handler
		
		/// <summary>
		/// Register command binding update handler which is triggered when input bindings associated 
		/// with specified type change
		/// </summary>
		/// <param name="ownerTypeName">Owner type name</param>
		/// <param name="handler">Update handler</param>
		public static void RegisterClassCommandBindingsUpdateHandler(string ownerTypeName, BindingsUpdatedHandler handler) 
		{
			if(!classCommandBindingUpdateHandlers.ContainsKey(ownerTypeName)) {
				classCommandBindingUpdateHandlers.Add(ownerTypeName, new List<BindingsUpdatedHandler>());
			}
			
			if(!classCommandBindingUpdateHandlers[ownerTypeName].Contains(handler)) {
				classCommandBindingUpdateHandlers[ownerTypeName].Add(handler);
			}
		}
		
		/// <summary>
		/// Register command binding update handler which is triggered when input bindings associated 
		/// with specified instance change
		/// </summary>
		/// <param name="instanceName">Owner instance name</param>
		/// <param name="handler">Update handler</param>
		public static void RegisterInstanceCommandBindingsUpdateHandler(string instanceName, BindingsUpdatedHandler handler) 
		{
			if(!instanceCommandBindingUpdateHandlers.ContainsKey(instanceName)) {
				instanceCommandBindingUpdateHandlers.Add(instanceName, new List<BindingsUpdatedHandler>());
			}
			
			instanceCommandBindingUpdateHandlers[instanceName].Add(handler);
		}
		
		/// <summary>
		/// Register input binding update handler which is triggered when input bindings associated 
		/// with specified type change
		/// </summary>
		/// <param name="ownerTypeName">Owner type name</param>
		/// <param name="handler">Update handler</param>
		public static void RegisterClassInputBindingsUpdateHandler(string ownerTypeName, BindingsUpdatedHandler handler) 
		{
			if(!classInputBindingUpdateHandlers.ContainsKey(ownerTypeName)) {
				classInputBindingUpdateHandlers.Add(ownerTypeName, new List<BindingsUpdatedHandler>());
			}
			
			if(!classInputBindingUpdateHandlers[ownerTypeName].Contains(handler)) {
				classInputBindingUpdateHandlers[ownerTypeName].Add(handler);
			}
		}
		
		/// <summary>
		/// Register input binding update handler which is triggered when input bindings associated 
		/// with specified instance change
		/// </summary>
		/// <param name="instanceName">Owner instance name</param>
		/// <param name="handler">Update handler</param>
		public static void RegisterInstanceInputBindingsUpdateHandler(string instanceName, BindingsUpdatedHandler handler) 
		{
			if(!instanceInputBindingUpdateHandlers.ContainsKey(instanceName)) {
				instanceInputBindingUpdateHandlers.Add(instanceName, new List<BindingsUpdatedHandler>());
			}
			
			instanceInputBindingUpdateHandlers[instanceName].Add(handler);
		}
		
		#endregion
		
		#region Invoke binding update handlers

		private static void InvokeBindingUpdateHandlers(Dictionary<string, List<BindingsUpdatedHandler>> updateHandlerDictionary, string ownertName)
		{
			if(ownertName != null && updateHandlerDictionary[ownertName] != null) {
				foreach(var handler in updateHandlerDictionary[ownertName]) {
					if(handler != null) {
						handler.Invoke();
					}
				}
			}
		}
		
		private static void InvokeAllBindingUpdateHandlers(System.Collections.IDictionary updateHandlers)
		{
			foreach(DictionaryEntry updateHandlerPair in updateHandlers) {
				var handlers = (List<BindingsUpdatedHandler>)updateHandlerPair.Value;
				
				if(handlers != null) {
					foreach(var handler in handlers) {
						if(handler != null) {
							handler.Invoke();
						}
					}
				}
			}
		}
		
		public static void InvokeCommandBindingUpdateHandlers() {
			InvokeAllBindingUpdateHandlers(classCommandBindingUpdateHandlers);
			InvokeAllBindingUpdateHandlers(instanceCommandBindingUpdateHandlers);
		}
		
		public static void InvokeInputBindingUpdateHandlers() {
			InvokeAllBindingUpdateHandlers(classInputBindingUpdateHandlers);
			InvokeAllBindingUpdateHandlers(instanceInputBindingUpdateHandlers);
		}
		
		/// <summary>
		/// Invoke all inbut binding update handlers
		/// </summary>
		public static void InvokeInputBindingUpdateHandlers(string ownertTypeName, string ownerInstanceName) 
		{
			InvokeBindingUpdateHandlers(classInputBindingUpdateHandlers, ownertTypeName);
			InvokeBindingUpdateHandlers(instanceInputBindingUpdateHandlers, ownerInstanceName);
		}
		
		/// <summary>
		/// Invoke all command binding update handlers
		/// </summary>
		public static void InvokeCommandBindingUpdateHandlers(string ownertTypeName, string ownerInstanceName) 
		{
			InvokeBindingUpdateHandlers(classCommandBindingUpdateHandlers, ownertTypeName);
			InvokeBindingUpdateHandlers(instanceCommandBindingUpdateHandlers, ownerInstanceName);
		}
		
		#endregion
		
		/// <summary>
		/// Load all registered commands in add-in
		/// </summary>
		/// <param name="addIn">Add-in</param>
		public static void LoadAddinCommands(AddIn addIn) {		
			foreach(var binding in commandBindings) {
				if(binding.AddIn != addIn) continue;
		
				if(binding.CommandTypeName != null && !commands.ContainsKey(binding.CommandTypeName)){
					var command = addIn.CreateObject(binding.CommandTypeName);
					var wpfCommand = command as System.Windows.Input.ICommand;
					if(wpfCommand == null) {
						wpfCommand = new WpfCommandWrapper((ICSharpCode.Core.ICommand)command);
					}
				
					commands.Add(binding.CommandTypeName, wpfCommand);
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
		/// Get list of all command bindings which satisfy provided parameters
		/// 
		/// Null arguments are ignored
		/// </summary>
		/// <param name="contextName">Context class full name</param>
		/// <param name="contextInstance">Get command bindings assigned only to specific context</param>
		/// <param name="routedCommandName">Context class full name</param>
		/// <param name="className">Context class full name</param>
		/// <returns>Collection of managed command bindings</returns>
		public static ICollection<CommandBindingInfo> FindCommandBindingInfos(string ownerTypeName, string ownerInstanceName, string routedCommandName) {
			var foundBindings = new List<CommandBindingInfo>();
			
			foreach(var binding in commandBindings) {
				if(    (ownerInstanceName == null || binding.OwnerInstanceName == ownerInstanceName)
				    && (ownerTypeName == null || binding.OwnerTypeName == ownerTypeName)
				    && (routedCommandName == null || binding.RoutedCommandName == routedCommandName)
				) {	   	
					foundBindings.Add(binding);
				}
			}
			
			return foundBindings;
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
		public static InputGestureCollection FindInputGestures(string ownerTypeName, string ownerInstanceName, string routedCommandName, BindingGroupCollection bindingGroups) {
			var bindings = FindInputBindingInfos(ownerTypeName, ownerInstanceName, routedCommandName);
			var gestures = new InputGestureCollection();
			
			foreach(InputBindingInfo bindingInfo in bindings) {
				if(bindingInfo.ActiveGestures != null && bindingInfo.IsActive) {
					foreach(InputGesture gesture in bindingInfo.ActiveGestures) {
						if(!gestures.ContainsTemplateFor(gesture, GestureCompareMode.ExactlyMatches)) {
							gestures.Add(gesture);
						}
					}
				}
			}
			
			return gestures;
		}
		
		public static InputBindingCategory GetInputBindingCategory(string categoryPath, bool throwWhenNotFound)
		{
			foreach(var category in InputBindingCategories) {
				if(category.Path == categoryPath) {
					return category;
				}
			}
			
			if(throwWhenNotFound) {
				throw new ApplicationException(string.Format("InputBindingCategory with path {0} was not found", categoryPath));
			}
			
			return null;
		}
		
		public static ICollection<InputBindingCategory> GetInputBindingCategoryCollection(string categoryPathCollectionString, bool throwWhenNotFound)
		{
			var categoryPathCollection = categoryPathCollectionString.Split(',');
			var categories = new List<InputBindingCategory>();
			foreach(var categoryPath in categoryPathCollection) {
				var category = CommandManager.GetInputBindingCategory(categoryPath, throwWhenNotFound);
				
				if(category != null) {
					categories.Add(category);
				}
			}
			
			return categories;
		}
		
		public static IEnumerable<InputBindingCategory> GetInputBindingCategoryChildren(string categoryPath) 
		{
			var categoryDepth = categoryPath.Count(c => c == '/');
			foreach(var currentCategory in InputBindingCategories) {
				if(currentCategory.Path.StartsWith(categoryPath)) {
					var currentCategoryDepth = currentCategory.Path.Count(c => c == '/');
					
					if(currentCategoryDepth == categoryDepth + 1)
					{
		 				yield return currentCategory;
					}
				}
			}
		}
		
		public static void RegisterInputBindingCategory(InputBindingCategory category) 
		{
			if(string.IsNullOrEmpty(category.Path)) {
				throw new ArgumentException("InputBindingCategory path can not be empty");
			}
			
			if(string.IsNullOrEmpty(category.Text)) {
				throw new ArgumentException("InputBindingCategory text can not be empty");
			}
			
			InputBindingCategories.Add(category);
		}
		
		/// <summary>
		/// Register default command binding update hander which will keep instance or type command 
		/// bindings upated
		/// </summary>
		/// <param name="commandBindingInfo">Command binding info</param>
		private static void RegisterDefaultCommandBindingHandler(CommandBindingInfo commandBindingInfo) 
		{
			if(commandBindingInfo.DefaultCommandBindingHandler != null) {
				commandBindingInfo.DefaultCommandBindingHandler.Invoke();
			}
			
			if(commandBindingInfo.OwnerInstanceName != null) {
				RegisterInstanceCommandBindingsUpdateHandler(commandBindingInfo.OwnerInstanceName, commandBindingInfo.DefaultCommandBindingHandler);
			} else if(commandBindingInfo.OwnerTypeName != null) {
				RegisterClassCommandBindingsUpdateHandler(commandBindingInfo.OwnerTypeName, commandBindingInfo.DefaultCommandBindingHandler);
			}
		}
		
		
		/// <summary>
		/// Register default input binding update hander which will keep instance or type command 
		/// bindings upated
		/// </summary>
		/// <param name="inputBindingInfo">Input binding info</param>
		private static void RegisterDefaultInputBindingHandler(InputBindingInfo inputBindingInfo) 
		{
			if(inputBindingInfo.DefaultInputBindingHandler != null) {
	 			inputBindingInfo.DefaultInputBindingHandler.Invoke();
			}
			
			if(inputBindingInfo.OwnerInstanceName != null) {
				RegisterInstanceInputBindingsUpdateHandler(inputBindingInfo.OwnerInstanceName, inputBindingInfo.DefaultInputBindingHandler);
			} else if(inputBindingInfo.OwnerTypeName != null) {
				RegisterClassInputBindingsUpdateHandler(inputBindingInfo.OwnerTypeName, inputBindingInfo.DefaultInputBindingHandler);
			}
		}
	}	
		
	public static class TypeExtensions
	{
		public static string GetShortAssemblyQualifiedName(this Type type)
		{
			return string.Format("{0}, {1}", type.FullName, type.Assembly.GetName().Name);
		}
	}
}
