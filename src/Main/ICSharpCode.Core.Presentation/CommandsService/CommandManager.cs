using System;
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
	public delegate void BindingsUpdatedHandler();
	
	/// <summary>
	/// Global registry to store and access commands, command bindings and input bindings
	/// </summary>
	public static class CommandManager
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
		
		/// <summary>
		/// Path to file where current user defined gestures are set
		/// </summary>
		public static string UserGesturesFilePath
		{
			get {
				return Path.Combine(PropertyService.ConfigDirectory, "UserDefinedGestures.xml");
			}
		}
		
		// Binding infos
		private static Dictionary<string, CommandBindingInfo> commandBindings = new Dictionary<string, CommandBindingInfo>();
		private static Dictionary<string, InputBindingInfo> inputBidnings = new Dictionary<string, InputBindingInfo>();
		
		// Commands
		private static Dictionary<string, RoutedUICommand> routedCommands = new Dictionary<string, RoutedUICommand>();
		internal static Dictionary<string, System.Windows.Input.ICommand> commands = new Dictionary<string, System.Windows.Input.ICommand>();
		
		// Update hanlers
		private static Dictionary<string, List<BindingsUpdatedHandler>> classCommandBindingUpdateHandlers = new Dictionary<string, List<BindingsUpdatedHandler>>();
		private static Dictionary<object, List<BindingsUpdatedHandler>> instanceCommandBindingUpdateHandlers = new Dictionary<object, List<BindingsUpdatedHandler>>();
		private static Dictionary<string, List<BindingsUpdatedHandler>> classInputBindingUpdateHandlers = new Dictionary<string, List<BindingsUpdatedHandler>>();
		private static Dictionary<object, List<BindingsUpdatedHandler>> instanceInputBindingUpdateHandlers = new Dictionary<object, List<BindingsUpdatedHandler>>();
				
		// Named instances and types
		private static Dictionary<string, UIElement> namedUIInstances = new Dictionary<string, UIElement>();
		private static Dictionary<string, Type> namedUITypes = new Dictionary<string, Type>();
		
		// Categories
		private static List<InputBindingCategory> categories = new List<InputBindingCategory>();
		
		static CommandManager()
		{
			UserDefinedGesturesManager.Load(UserGesturesFilePath);
		}
		
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
				// but owner is not loaded then move these to arrays which holds bindings 
				// and update handlers of loaded instances
				if(instanceCommandBindingUpdateHandlers.ContainsKey(instanceName)) {
					foreach(var handler in instanceCommandBindingUpdateHandlers[instanceName]) {
						RegisterInstanceCommandBindingsUpdateHandler(element, handler);
					}
					instanceCommandBindingUpdateHandlers.Remove(instanceName);
					InvokeInstanceCommandBindingUpdateHandlers(element);
					
					foreach(var handler in instanceInputBindingUpdateHandlers[instanceName]) {
						RegisterInstanceInputBindingsUpdateHandler(element, handler);
					}
					instanceInputBindingUpdateHandlers.Remove(instanceName);
					InvokeInstanceInputBindingUpdateHandlers(element);
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
		
		/// <summary>
		/// Register UI type which can be accessible by name
		/// </summary>
		/// <param name="typeName">Type name</param>
		/// <param name="type">Type</param>
		public static void RegisterNamedUIType(string typeName, Type type)
		{
			if(!namedUITypes.ContainsKey(typeName)){
				namedUITypes.Add(typeName, type);
				
				// If any bindings or update handlers where assigned to the type
				// before it was loaded using unique name move these to array 
				// holding type bindings and update handlers
				if(classCommandBindingUpdateHandlers.ContainsKey(typeName)) {
					foreach(var handler in classCommandBindingUpdateHandlers[typeName]) {
						RegisterClassCommandBindingsUpdateHandler(type, handler);
					}
					classCommandBindingUpdateHandlers.Remove(typeName);
					InvokeClassCommandBindingUpdateHandlers(type);
					
					foreach(var handler in classInputBindingUpdateHandlers[typeName]) {
						RegisterClassInputBindingsUpdateHandler(type, handler);
					}
					classInputBindingUpdateHandlers.Remove(typeName);
					InvokeClassInputBindingUpdateHandlers(type);
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
			if(string.IsNullOrEmpty(inputBindingInfo.Name)) {
				throw new ArgumentException("InputBindingInfo instance should have a name assigned");
			} 
			
			if(inputBidnings.ContainsKey(inputBindingInfo.Name)) {
				throw new ArgumentException("InputBindingInfo instance with provided name is already registered");
			} 
			
			// Replace default gestures with user defined gestures
			var userGestures = UserDefinedGesturesManager.GetInputBindingGesture(inputBindingInfo.Name);
			if(userGestures != null) {
				inputBindingInfo.Gestures = userGestures;
			}
			
			if(inputBindingInfo.OwnerTypeName != null || inputBindingInfo.OwnerType != null) {
				RegisterClassDefaultInputBindingHandler(inputBindingInfo);
			} else if (inputBindingInfo.OwnerInstanceName != null || inputBindingInfo.OwnerInstance != null) {
				RegisterInstaceDefaultInputBindingHandler(inputBindingInfo);
			} else {
				throw new ArgumentException("Binding owner must be specified");
			}
			inputBidnings.Add(inputBindingInfo.Name, inputBindingInfo);
		}
		
		/// <summary>
		/// Unregister input binding
		/// </summary>
		/// <param name="inputBindingInfo">Input binding parameters</param>
		public static void UnregisterInputBinding(InputBindingInfo inputBindingInfo)
		{
			inputBidnings.Remove(inputBindingInfo.Name);
		}
		
		/// <summary>
		/// Get instance of <see cref="InputBindingInfo" /> by name
		/// </summary>
		/// <param name="inputBindingName">Input binding info name</param>
		/// <returns>Input binding info matching provided name</returns>
		public static InputBindingInfo GetInputBindingInfo(string inputBindingName) {
			InputBindingInfo bindingInfo;
			inputBidnings.TryGetValue(inputBindingName, out bindingInfo);
			
			return bindingInfo;
		}
		
		
		/// <summary>
		/// Find input input binding infos which satisfy provided arguments
		/// 
		/// Null arguments are ignored
		/// </summary>
		/// <param name="contextName">Context class full name</param>
		/// <param name="contextInstance">Unregister binding assigned to specific context instance</param>
		/// <param name="routedCommandName">Routed UI command name</param>
		public static ICollection<InputBindingInfo> FindInputBindingInfos(string ownerTypeName, Type ownerType, string ownerInstanceName, UIElement ownerInstance, string routedCommandName) {
			var foundBindings = new List<InputBindingInfo>();
			
			foreach(var binding in inputBidnings) {
				if(    (ownerInstanceName == null || binding.Value.OwnerInstanceName == ownerInstanceName)
				    && (ownerInstance == null || binding.Value.OwnerInstance == ownerInstance)
				    && (ownerTypeName == null || binding.Value.OwnerTypeName == ownerTypeName)
				    && (ownerType == null || binding.Value.OwnerType == ownerType)
					&& (routedCommandName == null || binding.Value.RoutedCommandName == routedCommandName)) {
			
					foundBindings.Add(binding.Value);
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
			if(string.IsNullOrEmpty(commandBindingInfo.Name)) {
				throw new ArgumentException("cCommandBindingInfo instance should have a name assigned");
			} 
			
			if(commandBindings.ContainsKey(commandBindingInfo.Name)) {
				throw new ArgumentException("CommandBindingInfo instance with provided name is already registered");
			} 
			
			commandBindingInfo.GenerateCommandBindings();
			
			if(commandBindingInfo.OwnerTypeName != null || commandBindingInfo.OwnerType != null) {
				RegisterClassDefaultCommandBindingHandler(commandBindingInfo);
			} else if (commandBindingInfo.OwnerInstanceName != null || commandBindingInfo.OwnerInstance != null) {
				RegisterInstaceDefaultCommandBindingHandler(commandBindingInfo);
			} else {
				throw new ArgumentException("Binding owner must be specified");
			}
			
			commandBindings.Add(commandBindingInfo.Name, commandBindingInfo);
		}
		
		/// <summary>
		/// Unregister command binding
		/// </summary>
		/// <param name="commandBindingInfo">Command binding parameters</param>
		public static void UnregisterCommandBinding(CommandBindingInfo commandBindingInfo) {
			commandBindings.Remove(commandBindingInfo.Name);
			
			// Remove command bindings
			if(commandBindingInfo.OwnerType != null) {
				foreach(CommandBinding binding in commandBindingInfo.OldCommandBindings) {
					RemoveClassCommandBinding(commandBindingInfo.OwnerType, binding);
				}
				
				foreach(CommandBinding binding in commandBindingInfo.NewCommandBindings) {
					RemoveClassCommandBinding(commandBindingInfo.OwnerType, binding);
				}
			} else if (commandBindingInfo.OwnerInstance != null) {
				foreach(CommandBinding binding in commandBindingInfo.OldCommandBindings) {
					commandBindingInfo.OwnerInstance.CommandBindings.Remove(binding);
				}
			
				foreach(CommandBinding binding in commandBindingInfo.NewCommandBindings) {
					commandBindingInfo.OwnerInstance.CommandBindings.Remove(binding);
				}
			}
		}
		
		#region Register bindings update handler
		
		/// <summary>
		/// Register command binding update handler which is triggered when input bindings associated 
		/// with specified type change
		/// </summary>
		/// <param name="ownerType">Owner type</param>
		/// <param name="handler">Update handler</param>
		public static void RegisterClassCommandBindingsUpdateHandler(Type ownerType, BindingsUpdatedHandler handler) 
		{
			RegisterClassCommandBindingsUpdateHandler(ownerType.AssemblyQualifiedName, handler);
		}
		
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
			var instance = GetNamedUIElementInstance(instanceName);
			if(instance != null) {
				RegisterInstanceCommandBindingsUpdateHandler(instance, handler);
			} else {
				if(!instanceCommandBindingUpdateHandlers.ContainsKey(instanceName)) {
					instanceCommandBindingUpdateHandlers.Add(instanceName, new List<BindingsUpdatedHandler>());
				}
				
				instanceCommandBindingUpdateHandlers[instanceName].Add(handler);
			}
		}
		
		/// <summary>
		/// Register command binding update handler which is triggered when input bindings associated 
		/// with specified instance change
		/// </summary>
		/// <param name="instance">Owner instance</param>
		/// <param name="handler">Update handler</param>
		public static void RegisterInstanceCommandBindingsUpdateHandler(UIElement instance, BindingsUpdatedHandler handler) 
		{
			if(!instanceCommandBindingUpdateHandlers.ContainsKey(instance)) {
				instanceCommandBindingUpdateHandlers.Add(instance, new List<BindingsUpdatedHandler>());
			}
			
			instanceCommandBindingUpdateHandlers[instance].Add(handler);
		}
	
		/// <summary>
		/// Register input binding update handler which is triggered when input bindings associated 
		/// with specified type change
		/// </summary>
		/// <param name="ownerType">Owner type</param>
		/// <param name="handler">Update handler</param>
		public static void RegisterClassInputBindingsUpdateHandler(Type ownerType, BindingsUpdatedHandler handler) 
		{
			RegisterClassInputBindingsUpdateHandler(ownerType.AssemblyQualifiedName, handler);
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
			var instance = GetNamedUIElementInstance(instanceName);
			if(instance != null) {
				RegisterInstanceInputBindingsUpdateHandler(instance, handler);
			} else {
				if(!instanceInputBindingUpdateHandlers.ContainsKey(instanceName)) {
					instanceInputBindingUpdateHandlers.Add(instanceName, new List<BindingsUpdatedHandler>());
				}
				
				instanceInputBindingUpdateHandlers[instanceName].Add(handler);
			}
		}
		
		/// <summary>
		/// Register input binding update handler which is triggered when input bindings associated 
		/// with specified instance change
		/// </summary>
		/// <param name="instance">Owner instance</param>
		/// <param name="handler">Update handler</param>
		public static void RegisterInstanceInputBindingsUpdateHandler(UIElement instance, BindingsUpdatedHandler handler) 
		{
			if(!instanceInputBindingUpdateHandlers.ContainsKey(instance)) {
				instanceInputBindingUpdateHandlers.Add(instance, new List<BindingsUpdatedHandler>());
			}
			
			instanceInputBindingUpdateHandlers[instance].Add(handler);
		}
		#endregion
		
		#region Invoke binding update handlers
		
		
		/// <summary>
		/// Invoke all inbut binding update handlers
		/// </summary>
		public static void InvokeInputBindingUpdateHandlers() 
		{
			foreach(var instanceHandlers in instanceInputBindingUpdateHandlers) {
				foreach(var handler in instanceHandlers.Value) {
					handler.Invoke();
				}
			}
			
			foreach(var classHandlers in classInputBindingUpdateHandlers) {
				foreach(var handler in classHandlers.Value) {
					handler.Invoke();
				}
			}
		}
		
		/// <summary>
		/// Invoke all command binding update handlers
		/// </summary>
		public static void InvokeCommandBindingUpdateHandlers() 
		{
			foreach(var instanceHandlers in instanceCommandBindingUpdateHandlers) {
				foreach(var handler in instanceHandlers.Value) {
					handler.Invoke();
				}
			}
			
			foreach(var classHandlers in classCommandBindingUpdateHandlers) {
				foreach(var handler in classHandlers.Value) {
					handler.Invoke();
				}
			}
		}
		
		/// <summary>
		/// Invoke command binding update handlers associated with UI element instance
		/// </summary>
		/// <param name="owner">Owner instance</param>
		public static void InvokeInstanceCommandBindingUpdateHandlers(UIElement owner) 
		{	
			if(instanceCommandBindingUpdateHandlers.ContainsKey(owner)) {
				foreach(var handler in instanceCommandBindingUpdateHandlers[owner]) {
					handler.Invoke();
				}
			}
		}
		
		/// <summary>
		/// Invoke command binding update handlers associated with UI element instance
		/// </summary>
		/// <param name="owner">Owner instance name</param>
		public static void InvokeInstanceCommandBindingUpdateHandlers(string ownerName) 
		{	
			if(instanceCommandBindingUpdateHandlers.ContainsKey(ownerName)) {
				foreach(var handler in instanceCommandBindingUpdateHandlers[ownerName]) {
					handler.Invoke();
				}
			}
		}
		
		/// <summary>
		/// Invoke command binding update handlers associated with UI element type
		/// </summary>
		/// <param name="owner">Owner type</param>
		public static void InvokeClassCommandBindingUpdateHandlers(Type ownerType) 
		{	
			InvokeClassCommandBindingUpdateHandlers(ownerType.AssemblyQualifiedName);
		}
		
		/// <summary>
		/// Invoke command binding update handlers associated with UI element type
		/// </summary>
		/// <param name="owner">Owner type name</param>
		public static void InvokeClassCommandBindingUpdateHandlers(string ownerTypeName) 
		{	
			if(instanceCommandBindingUpdateHandlers.ContainsKey(ownerTypeName)) {
				foreach(var handler in classCommandBindingUpdateHandlers[ownerTypeName]) {
					handler.Invoke();
				}
			}
		}
					
		/// <summary>
		/// Invoke input binding update handlers associated with UI element instance
		/// </summary>
		/// <param name="owner">Owner instance</param>
		public static void InvokeInstanceInputBindingUpdateHandlers(UIElement owner) 
		{	
			if(instanceInputBindingUpdateHandlers.ContainsKey(owner)) {
				foreach(var handler in instanceInputBindingUpdateHandlers[owner]) {
					handler.Invoke();
				}
			}
		}
		
		/// <summary>
		/// Invoke input binding update handlers associated with UI element instance
		/// </summary>
		/// <param name="ownerName">Owner instance name</param>
		public static void InvokeInstanceInputBindingUpdateHandlers(string ownerName) 
		{	
			if(instanceInputBindingUpdateHandlers.ContainsKey(ownerName)) {
				foreach(var handler in instanceInputBindingUpdateHandlers[ownerName]) {
					handler.Invoke();
				}
			}
		}
		
		/// <summary>
		/// Invoke input binding update handlers associated with UI element type
		/// </summary>
		/// <param name="owner">Owner type</param>
		public static void InvokeClassInputBindingUpdateHandlers(Type ownerType) 
		{	
			InvokeClassInputBindingUpdateHandlers(ownerType.AssemblyQualifiedName);
		}
		
		/// <summary>
		/// Invoke input binding update handlers associated with UI element type
		/// </summary>
		/// <param name="owner">Owner type name</param>
		public static void InvokeClassInputBindingUpdateHandlers(string ownerTypeName) 
		{	
			if(instanceInputBindingUpdateHandlers.ContainsKey(ownerTypeName)) {
				foreach(var handler in instanceInputBindingUpdateHandlers[ownerTypeName]) {
					handler.Invoke();
				}
			}
		}
		#endregion
		
		/// <summary>
		/// Load all registered commands in add-in
		/// </summary>
		/// <param name="addIn">Add-in</param>
		public static void LoadAddinCommands(AddIn addIn) {		
			foreach(var binding in commandBindings) {
				if(binding.Value.AddIn != addIn) continue;
		
				if(binding.Value.CommandTypeName != null && !commands.ContainsKey(binding.Value.CommandTypeName)){
					var command = addIn.CreateObject(binding.Value.CommandTypeName);
					var wpfCommand = command as System.Windows.Input.ICommand;
					if(wpfCommand == null) {
						wpfCommand = new WpfCommandWrapper((ICSharpCode.Core.ICommand)command);
					}
				
					commands.Add(binding.Value.CommandTypeName, wpfCommand);
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
		/// Get registered instance of <see cref="CommandBindingInfo" />
		/// </summary>
		/// <param name="commandBindingName">Command binding info name</param>
		/// <returns>Command binding info matching provided name</returns>
		public static CommandBindingInfo GetCommandBindingInfo(string commandBindingName) {
			CommandBindingInfo bindingInfo;
			commandBindings.TryGetValue(commandBindingName, out bindingInfo);
			
			return bindingInfo;
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
		public static ICollection<CommandBindingInfo> FindCommandBindingInfos(string ownerTypeName, Type ownerType, string ownerInstanceName, UIElement ownerInstance, string routedCommandName, string className) {
			var foundBindings = new List<CommandBindingInfo>();
			
			foreach(var binding in commandBindings) {
				if(    (ownerInstanceName == null || binding.Value.OwnerInstanceName == ownerInstanceName)
				    && (ownerInstance == null || binding.Value.OwnerInstance == ownerInstance)
				    && (ownerTypeName == null || binding.Value.OwnerTypeName == ownerTypeName)
				    && (ownerType == null || binding.Value.OwnerType == ownerType)
					&& (routedCommandName == null || binding.Value.RoutedCommandName == routedCommandName)
					&& (className == null || binding.Value.CommandTypeName == className)) {
				   	
					foundBindings.Add(binding.Value);
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
		public static InputGestureCollection FindInputGestures(string ownerTypeName, Type ownerType, string ownerInstanceName, UIElement ownerInstance, string routedCommandName) {
			var bindings = FindInputBindingInfos(ownerTypeName, ownerType, ownerInstanceName, ownerInstance, routedCommandName);
			var gestures = new InputGestureCollection();
			
			foreach(InputBindingInfo bindingInfo in bindings) {
				gestures.AddRange(bindingInfo.Gestures);
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
		
		/// <summary>
		/// Saves current configuration in specified destination
		/// </summary>
		/// <param name="destinationPath">Destination file path</param>
		public static void SaveGestures(string destinationPath)
		{
			foreach(var inputBindingInfo in inputBidnings) {
				UserDefinedGesturesManager.SetInputBindingGesture(
					inputBindingInfo.Key, 
					inputBindingInfo.Value.Gestures);
			}
			
			UserDefinedGesturesManager.Save(destinationPath);
		}
		
		/// <summary>
		/// Loads current configuration in specified destination
		/// </summary>
		/// <param name="sourcePath">Source file path</param>
		public static void LoadGestures(string sourcePath)
		{
			UserDefinedGesturesManager.Load(sourcePath);
			
			foreach(var inputBindingInfo in inputBidnings) {
				var userGestures = UserDefinedGesturesManager.GetInputBindingGesture(inputBindingInfo.Key);
				
				if(userGestures != null) {
					inputBindingInfo.Value.Gestures = userGestures;
					inputBindingInfo.Value.IsModifyed = true;
				}
			}
		}
		
		/// <summary>
		/// Register default command binding update hander which will keep instance command 
		/// bindings upated
		/// </summary>
		/// <param name="commandBindingInfo">Command binding info</param>
		private static void RegisterInstaceDefaultCommandBindingHandler(CommandBindingInfo commandBindingInfo) 
		{
			if(commandBindingInfo.DefaultCommandBindingHandler != null) {
				commandBindingInfo.DefaultCommandBindingHandler.Invoke();
			}
			
			if(commandBindingInfo.OwnerInstanceName != null && commandBindingInfo.OwnerInstance == null) {
				RegisterInstanceCommandBindingsUpdateHandler(commandBindingInfo.OwnerInstanceName, commandBindingInfo.DefaultCommandBindingHandler);
			} else if(commandBindingInfo.OwnerInstance != null) {
				RegisterInstanceCommandBindingsUpdateHandler(commandBindingInfo.OwnerInstance, commandBindingInfo.DefaultCommandBindingHandler);
			}
		}
		
		/// <summary>
		/// Register default command binding update hander which will keep type command 
		/// bindings upated
		/// </summary>
		/// <param name="commandBindingInfo">Command binding info</param>
		private static void RegisterClassDefaultCommandBindingHandler(CommandBindingInfo commandBindingInfo) 
		{
			if(commandBindingInfo.DefaultCommandBindingHandler != null) {
				commandBindingInfo.DefaultCommandBindingHandler.Invoke();
			}
			
			if(commandBindingInfo.OwnerTypeName != null && commandBindingInfo.OwnerType == null) {
				RegisterClassCommandBindingsUpdateHandler(commandBindingInfo.OwnerTypeName, commandBindingInfo.DefaultCommandBindingHandler);
			} else if(commandBindingInfo.OwnerType != null) {
				RegisterClassCommandBindingsUpdateHandler(commandBindingInfo.OwnerType, commandBindingInfo.DefaultCommandBindingHandler);
			}
		}
		
		/// <summary>
		/// Register default input binding update hander which will keep instance command 
		/// bindings upated
		/// </summary>
		/// <param name="inputBindingInfo">Input binding info</param>
		private static void RegisterInstaceDefaultInputBindingHandler(InputBindingInfo inputBindingInfo) 
		{
			if(inputBindingInfo.DefaultInputBindingHandler != null) {
	 			inputBindingInfo.DefaultInputBindingHandler.Invoke();
			}
			
			if(inputBindingInfo.OwnerInstanceName != null && inputBindingInfo.OwnerInstance == null) {
				RegisterInstanceInputBindingsUpdateHandler(inputBindingInfo.OwnerInstanceName, inputBindingInfo.DefaultInputBindingHandler);
			} else if(inputBindingInfo.OwnerInstance != null) {
				RegisterInstanceInputBindingsUpdateHandler(inputBindingInfo.OwnerInstance, inputBindingInfo.DefaultInputBindingHandler);
			}
		}
		
		/// <summary>
		/// Register default input binding update hander which will keep type command 
		/// bindings upated
		/// </summary>
		/// <param name="inputBindingInfo">Input binding info</param>
		private static void RegisterClassDefaultInputBindingHandler(InputBindingInfo inputBindingInfo) 
		{
			if(inputBindingInfo.DefaultInputBindingHandler != null) {
	 			inputBindingInfo.DefaultInputBindingHandler.Invoke();
			}

			if(inputBindingInfo.OwnerTypeName != null && inputBindingInfo.OwnerType == null) {
				RegisterClassInputBindingsUpdateHandler(inputBindingInfo.OwnerTypeName, inputBindingInfo.DefaultInputBindingHandler);
			} else if(inputBindingInfo.OwnerType != null) {
				RegisterClassInputBindingsUpdateHandler(inputBindingInfo.OwnerType, inputBindingInfo.DefaultInputBindingHandler);
			}
		}
	}	
}
