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
		
		public static bool SuspendUpdateHandlers {
			get; set;
		}
		
		// Binding infos
		private static BindingInfoTemplateDictionary<BindingInfoBase> commandBindings = new BindingInfoTemplateDictionary<BindingInfoBase>();
		private static BindingInfoTemplateDictionary<BindingInfoBase> inputBidnings = new BindingInfoTemplateDictionary<BindingInfoBase>();
		
		// Commands
		private static Dictionary<string, RoutedUICommand> routedCommands = new Dictionary<string, RoutedUICommand>();
		internal static Dictionary<string, System.Windows.Input.ICommand> commands = new Dictionary<string, System.Windows.Input.ICommand>();
		
		// Named instances and types
		private static RelationshipMap<string, WeakReference> namedUIInstances = new RelationshipMap<string, WeakReference>(null, new WeakReferenceEqualirtyComparer());
		private static RelationshipMap<string, Type> namedUITypes = new RelationshipMap<string, Type>();
		
		// Categories
		public static List<InputBindingCategory> InputBindingCategories = new List<InputBindingCategory>();
		
		static CommandManager()
		{
			// Load gestures profile first
			var path = PropertyService.Get("ICSharpCode.Core.Presentation.UserDefinedGesturesManager.UserGestureProfilesDirectory");
			if(path != null && File.Exists(path)) {
				var profile = new UserGesturesProfile();
				profile.Path = path;
				profile.Load();
				
				UserDefinedGesturesManager.CurrentProfile = profile;
			}
			
			UserDefinedGesturesManager.CurrentProfileChanged += UserDefinedGesturesManager_CurrentProfileChanged;
		}

		static void UserDefinedGesturesManager_CurrentProfileChanged(object sender, NotifyUserGestureProfileChangedEventArgs args)
		{
			var changedGestures = new Dictionary<InputBindingIdentifier, Tuple<InputGestureCollection, InputGestureCollection>>();
			
			if(args.OldProfile != null) {
				args.OldProfile.GesturesChanged -= Profile_GesturesChanged;
				
				foreach(var pair in args.OldProfile) {
					changedGestures.Add(pair.Key, new Tuple<InputGestureCollection, InputGestureCollection>(pair.Value, null));
				}
			}
			
			if(args.NewProfile != null) {
				args.NewProfile.GesturesChanged += Profile_GesturesChanged;
				
				foreach(var pair in args.NewProfile) {
					InputGestureCollection item1 = null;
					if(!changedGestures.ContainsKey(pair.Key)) {
						changedGestures.Add(pair.Key, new Tuple<InputGestureCollection, InputGestureCollection>(null, pair.Value));
					} else {
						changedGestures[pair.Key] = new Tuple<InputGestureCollection, InputGestureCollection>(changedGestures[pair.Key].Item1, pair.Value);
					}
				}
			}
			
			var modifiedBindingTemplates = new HashSet<IBindingInfoTemplate>(new IBindingInfoTemplateEqualityComparer());
			var descriptions = new List<GesturesModificationDescription>(changedGestures.Count);
			foreach(var changedGesture in changedGestures) {
				var bindingInfoTemplate = new BindingInfoTemplate(changedGesture.Key);
				InputGestureCollection defaultUserGesture = null;
				if(changedGesture.Value.Item1 == null || changedGesture.Value.Item2 == null) {
					defaultUserGesture = FindInputGestures(BindingInfoMatchType.Exact, bindingInfoTemplate);
				}
				
				var oldGestures = changedGesture.Value.Item1 ?? defaultUserGesture;
				var newGestures = changedGesture.Value.Item2 ?? defaultUserGesture;
				descriptions.Add(new GesturesModificationDescription(changedGesture.Key, oldGestures, newGestures));
						
				modifiedBindingTemplates.Add(bindingInfoTemplate);
			}
			
			InvokeBindingsChanged(typeof(CommandManager), new NotifyBindingsChangedEventArgs(NotifyBindingsChangedAction.BindingInfoModified, modifiedBindingTemplates));
			InvokeGesturesChanged(typeof(CommandManager), new NotifyGesturesChangedEventArgs(descriptions));
		}

		static void Profile_GesturesChanged(object sender, NotifyGesturesChangedEventArgs args)
		{
			var modifiedBindingTemplates = new HashSet<IBindingInfoTemplate>(new IBindingInfoTemplateEqualityComparer());
			foreach(var description in args.ModificationDescriptions) {
				modifiedBindingTemplates.Add(new BindingInfoTemplate(description.InputBindingIdentifier));
			}
			
			InvokeBindingsChanged(typeof(CommandManager), new NotifyBindingsChangedEventArgs(NotifyBindingsChangedAction.BindingInfoModified, modifiedBindingTemplates));
			InvokeGesturesChanged(typeof(CommandManager), args);
		}
		
		/// <summary>
		/// Register UI element instance accessible by unique name
		/// </summary>
		/// <param name="instanceName">Instance name</param>
		/// <param name="element">Instance</param>
		public static void RegisterNamedUIElement(string instanceName, UIElement element)
		{	
			if(instanceName == null) {
				throw new ArgumentNullException("instanceName");
			}
			if(element == null) {
				throw new ArgumentNullException("element");
			}
			
			var oldInstances = GetNamedUIElementCollection(instanceName).ToArray();
			
			var container = new WeakReference(element);
			if(namedUIInstances.Add(instanceName, container)) {
				InvokeBindingsChanged(
					null, 
					new NotifyBindingsChangedEventArgs(
						NotifyBindingsChangedAction.NamedInstanceModified,
						instanceName,
						oldInstances,
						GetNamedUIElementCollection(instanceName)));
			}	
		}
		
		public static void UnregisterNamedUIElement(string instanceName, UIElement instance)
		{	
			if(instanceName == null) {
				throw new ArgumentNullException("instanceName");
			}
			if(instance == null) {
				throw new ArgumentNullException("element");
			}
			
			var oldInstances = GetNamedUIElementCollection(instanceName).ToArray();
			
			var container = new WeakReference(instance);
			if(namedUIInstances.Remove(instanceName, container)) {
				InvokeBindingsChanged(
					null, 
					new NotifyBindingsChangedEventArgs(
						NotifyBindingsChangedAction.NamedInstanceModified,
						instanceName,
						oldInstances,
						GetNamedUIElementCollection(instanceName)));
			}
		}
		
		/// <summary>
		/// Get instance by unique instance name
		/// </summary>
		/// <param name="instanceName">Instance name</param>
		/// <returns></returns>
		public static ICollection<UIElement> GetNamedUIElementCollection(string instanceName)
		{
			if(instanceName == null) {
				throw new ArgumentNullException("instanceName");
			}
			
			return namedUIInstances.MapForward(instanceName).Where(reference => reference.Target != null).Select(reference => (UIElement)reference.Target).ToList();
		}
		
		public static ICollection<string> GetUIElementNameCollection(UIElement instance)
		{
			if(instance == null) {
				throw new ArgumentNullException("instance");
			}
			
			return namedUIInstances.MapBackward(new WeakReference(instance));
		}
		
		/// <summary>
		/// Register UI type which can be accessible by name
		/// </summary>
		/// <param name="typeName">Type name</param>
		/// <param name="type">Type</param>
		public static void RegisterNamedUIType(string typeName, Type type)
		{
			if(typeName == null) {
				throw new ArgumentNullException("typeName");
			}
			if(type == null) {
				throw new ArgumentNullException("type");
			}
			
			var oldTypes = GetNamedUITypeCollection(typeName).ToArray();
			
			if(namedUITypes.Add(typeName, type)) {
				InvokeBindingsChanged(null, 
					new NotifyBindingsChangedEventArgs(
						NotifyBindingsChangedAction.NamedTypeModified,
						typeName,
						oldTypes,
						GetNamedUITypeCollection(typeName)));
			}
		}
		
		public static void UnregisterNamedUIType(string typeName, Type type)
		{	
			if(typeName == null) {
				throw new ArgumentNullException("typeName");
			}
			if(type == null) {
				throw new ArgumentNullException("type");
			}
			
			var oldTypes = GetNamedUITypeCollection(typeName).ToArray();
			
			if(namedUITypes.Remove(typeName, type)) {
				InvokeBindingsChanged(
					null, 
					new NotifyBindingsChangedEventArgs(
						NotifyBindingsChangedAction.NamedTypeModified,
						typeName,
						oldTypes,
						GetNamedUITypeCollection(typeName)));
			}
		}
		
		/// <summary>
		/// Get type by uniqe type name
		/// </summary>
		/// <param name="typeName">Type name</param>
		/// <returns>Type</returns>
		public static ICollection<Type> GetNamedUITypeCollection(string typeName)
		{
			if(typeName == null) {
				throw new ArgumentNullException("typeName");
			}
			
			return namedUITypes.MapForward(typeName);
		}
		
		public static ICollection<string> GetUITypeNameCollection(Type type)
		{
			if(type == null) {
				throw new ArgumentNullException("type");
			}
			
			return namedUITypes.MapBackward(type);
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
			if(routedCommands.ContainsKey(routedCommandName)) {
				throw new IndexOutOfRangeException("Routed UI command with name " + routedCommandName + " is already registered");
			}
			
			var routedCommand = new RoutedUICommand(text, routedCommandName, typeof(CommandManager));
			
			routedCommands.Add(routedCommandName, routedCommand);
							
			InvokeBindingsChanged(null, new NotifyBindingsChangedEventArgs(NotifyBindingsChangedAction.RoutedUICommandModified, routedCommandName));
					
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
			
			if(routedCommands.ContainsKey(routedCommandName)) {
				throw new IndexOutOfRangeException("Routed UI command with name " + routedCommandName + " is already registered");
			}
			
			routedCommands.Add(routedCommandName, existingRoutedUICommand);
			
			InvokeBindingsChanged(null, new NotifyBindingsChangedEventArgs(NotifyBindingsChangedAction.RoutedUICommandModified, routedCommandName));
		}
	
		/// <summary>
		/// Remove routed command from global registry
		/// </summary>
		/// <param name="routedCommandName">Routed command name</param>
		public static void UnregisterRoutedUICommand(string routedCommandName) {
			if(routedCommands.ContainsKey(routedCommandName)) {
				var routedCommand = routedCommands[routedCommandName];
				routedCommands.Remove(routedCommandName);
				
				InvokeBindingsChanged(null, new NotifyBindingsChangedEventArgs(NotifyBindingsChangedAction.RoutedUICommandModified, routedCommandName));
			}
		}
		
		/// <summary>
		/// Get reference to routed UI command by name
		/// </summary>
		/// <param name="routedCommandName">Routed command name</param>
		/// <returns>Routed command instance</returns>
		public static RoutedUICommand GetRoutedUICommand(string routedCommandName) {
			if(routedCommands != null && routedCommands.ContainsKey(routedCommandName)) {
				return routedCommands[routedCommandName];
			}
			
			return null;
		}

		/// <summary>
		/// Register input binding by specifying this binding parameters
		/// </summary>
		/// <param name="inputBindingInfo">Input binding parameters</param>
		public static void RegisterInputBinding(InputBindingInfo inputBindingInfo)
		{
			if(inputBindingInfo.OwnerInstanceName == null && inputBindingInfo.OwnerTypeName == null) {
				throw new ArgumentException("Binding owner must be specified");
			}
			
			if(inputBindingInfo.RoutedCommandName == null) {
				throw new ArgumentException("Routed command name must be specified");
			}
			
			var similarTemplate = new BindingInfoTemplate(inputBindingInfo, false);
			var similarInputBinding = FindInputBindingInfos(BindingInfoMatchType.SuperSet, similarTemplate).FirstOrDefault();
			
			if(similarInputBinding != null) {
				foreach(InputGesture gesture in inputBindingInfo.DefaultGestures) {
					var existingGesture = new InputGestureCollection(similarInputBinding.DefaultGestures.ToList());
					if(!existingGesture.ContainsTemplateFor(gesture, GestureCompareMode.ExactlyMatches)) {
						similarInputBinding.DefaultGestures.Add(gesture);
					}
				}
				
				similarInputBinding.Categories.AddRange(inputBindingInfo.Categories);
				similarInputBinding.Groups.AddRange(inputBindingInfo.Groups);
			} else {
				inputBidnings.Add(inputBindingInfo, inputBindingInfo);
				inputBindingInfo.IsRegistered = true;
				
				CommandManager.BindingsChanged += inputBindingInfo.BindingsChangedHandler;
			}
			
			InvokeBindingsChanged(null, new NotifyBindingsChangedEventArgs(NotifyBindingsChangedAction.BindingInfoModified, new [] { similarTemplate }));
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
			if(commandBindingInfo.OwnerInstanceName == null && commandBindingInfo.OwnerTypeName == null) {
				throw new ArgumentException("Binding owner must be specified");
			}
			
			if(commandBindingInfo.RoutedCommandName == null) {
				throw new ArgumentException("Routed command name must be specified");
			}
			
			commandBindings.Add(commandBindingInfo, commandBindingInfo);
			commandBindingInfo.IsRegistered = true;
				
			CommandManager.BindingsChanged += commandBindingInfo.BindingsChangedHandler;
			InvokeBindingsChanged(null, new NotifyBindingsChangedEventArgs(NotifyBindingsChangedAction.BindingInfoModified, new []{ new BindingInfoTemplate(commandBindingInfo, false) }));
		}
		
		public static event NotifyBindingsChangedEventHandler BindingsChanged;
		
		public static void InvokeBindingsChanged(object sender, NotifyBindingsChangedEventArgs args)
		{
			if(BindingsChanged != null) {
				BindingsChanged.Invoke(sender, args);
			}
		}
		
		public static event NotifyGesturesChangedEventHandler GesturesChanged;
		
		public static void InvokeGesturesChanged(object sender, NotifyGesturesChangedEventArgs args)
		{
			if(GesturesChanged != null) {
				GesturesChanged.Invoke(sender, args);
			}
		}
		
		/// <summary>
		/// Load all registered commands in add-in
		/// </summary>
		/// <param name="addIn">Add-in</param>
		public static void LoadAddinCommands(AddIn addIn) {		
			foreach(CommandBindingInfo binding in FindCommandBindingInfos(BindingInfoMatchType.SuperSet, new BindingInfoTemplate())) {
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
		
		#region Unregister binding infos
		
		/// <summary>
		/// Unregister input binding
		/// </summary>
		/// <param name="inputBindingInfo">Input binding parameters</param>
		public static void UnregisterInputBinding(BindingInfoMatchType matchType,  params IBindingInfoTemplate[] templates)
		{
			UnregisterBindingInfo(inputBidnings, matchType, templates);
		}
		
		/// <summary>
		/// Unregister command binding
		/// </summary>
		/// <param name="commandBindingInfo">Command binding parameters</param>
		public static void UnregisterCommandBinding(BindingInfoMatchType matchType,  params IBindingInfoTemplate[] templates) 
		{
			UnregisterBindingInfo(commandBindings, matchType, templates);
		}
		
		private static void UnregisterBindingInfo(BindingInfoTemplateDictionary<BindingInfoBase> bindingInfoDictionary, BindingInfoMatchType matchType,  params IBindingInfoTemplate[] templates)
		{
			var foundBindings = FindBindingInfos(bindingInfoDictionary, matchType, templates).ToList();
			
			foreach(var similarBindingInfo in foundBindings) {
				BindingsUpdatedHandler defaultUpdatesHandler;
				if(similarBindingInfo is InputBindingInfo) {
					((InputBindingInfo)similarBindingInfo).IsRegistered = false;
				} else {
					((CommandBindingInfo)similarBindingInfo).IsRegistered = false;
				}
					
				var removedTemplate = new BindingInfoTemplate(similarBindingInfo, false);
				bindingInfoDictionary.Remove(removedTemplate, BindingInfoMatchType.Exact, similarBindingInfo);
				CommandManager.BindingsChanged -= similarBindingInfo.BindingsChangedHandler;
				
				similarBindingInfo.RemoveActiveBindings();
			}
			
			InvokeBindingsChanged(null, new NotifyBindingsChangedEventArgs(NotifyBindingsChangedAction.BindingInfoModified, templates));
		}
		
		#endregion
		
		#region Find binding infos

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
		public static IEnumerable<CommandBindingInfo> FindCommandBindingInfos(BindingInfoMatchType matchType, params IBindingInfoTemplate[] templates)
		{
			var bindings = FindBindingInfos(commandBindings, matchType, templates).ToList();
			
			return bindings.Cast<CommandBindingInfo>();
		}
		
		public static IEnumerable<InputBindingInfo> FindInputBindingInfos(BindingInfoMatchType matchType, params IBindingInfoTemplate[] templates)
		{
			var bindings = FindBindingInfos(inputBidnings, matchType, templates).ToList();
			
			return bindings.Cast<InputBindingInfo>();
		}
		
		private static IEnumerable<BindingInfoBase> FindBindingInfos(BindingInfoTemplateDictionary<BindingInfoBase> bindingInfos, BindingInfoMatchType matchType, params IBindingInfoTemplate[] templates)
		{
        	foreach(var template in templates) {
				foreach(var item in bindingInfos.FindItems(template, matchType)) {
					if(item != null) {
						yield return item;
					}
				}
        	}
		}		

		public static CommandBindingCollection FindCommandBindings(BindingInfoMatchType matchType, params IBindingInfoTemplate[] templates) 
		{
			var commandBindingInfoCollection = FindCommandBindingInfos(matchType, templates);
			var commandBindingCollection = new CommandBindingCollection();
			foreach(var bindingInfo in commandBindingInfoCollection) {
				commandBindingCollection.AddRange(bindingInfo.ActiveCommandBindings);
			}
			
			return commandBindingCollection;
		}
		
		public static InputBindingCollection FindInputBindings(BindingInfoMatchType matchType, params IBindingInfoTemplate[] templates) 
		{
			var inputBindingInfoCollection = FindInputBindingInfos(matchType, templates);
			
			var inputBindingCollection = new InputBindingCollection();
			foreach(var bindingInfo in inputBindingInfoCollection) {
				inputBindingCollection.AddRange(bindingInfo.ActiveInputBindings);
			}
			
			return inputBindingCollection;
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
		public static InputGestureCollection FindInputGestures(BindingInfoMatchType matchType, params IBindingInfoTemplate[] templates) {
			var bindings = FindInputBindingInfos(matchType, templates);
			var gestures = new InputGestureCollection();
			
			foreach(InputBindingInfo bindingInfo in bindings) {
				if(bindingInfo.ActiveGestures != null) {
					foreach(InputGesture gesture in bindingInfo.ActiveGestures) {
						if(!gestures.ContainsTemplateFor(gesture, GestureCompareMode.ExactlyMatches)) {
							gestures.Add(gesture);
						}
					}
				}
			}
			
			return gestures;
		}
		#endregion
		
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
				var category = GetInputBindingCategory(categoryPath, throwWhenNotFound);
				
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
		/// Use for unit tests only
		/// </summary>
		public static void Reset()
		{
			
			commandBindings.Clear();
			inputBidnings.Clear();
		
			routedCommands.Clear();
			commands.Clear();
		
			namedUIInstances.Clear();
			namedUITypes.Clear();
		
			InputBindingCategories.Clear();
			
			BindingsChanged = null;
			GesturesChanged = null;
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