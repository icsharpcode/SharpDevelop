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
	/// Provides methods to describe <see cref="CommandBinding" /> and <see cref="InputBinding" /> 
	/// objects assigned to owners and methods to keep aware of user changes to default gestures
	/// </summary>
	public static class CommandManager
	{
		/// <summary>
		/// Name of the type which should be used as owner if no other 
		/// owner type or element was provided
		/// </summary>
		public static string DefaultOwnerTypeName {
			get; set;
		}
		
		/// <summary>
		/// Suspend <see cref="BindingsChanged" /> and <see cref="GesturesChanged" />
		/// event handlers from being notified about new events
		/// 
		/// This property is usefull when lots of updates hapen. To improve performance
		/// event notifications can be turned off until all updates are finished and after
		/// that all handlers should be notified about the changes by calling method 
		/// <see cref="InvokeBindingsChanged" /> or <see cref="InvokeGesturesChanged" />
		/// with appropriate arguments
		public static bool SuspendEvents {
			get; set;
		}
		
		// Registered binding infos
		private static BindingInfoTemplateDictionary<BindingInfoBase> commandBindings = new BindingInfoTemplateDictionary<BindingInfoBase>();
		private static BindingInfoTemplateDictionary<BindingInfoBase> inputBidnings = new BindingInfoTemplateDictionary<BindingInfoBase>();
		
		// Registered commands
		private static Dictionary<string, RoutedUICommand> routedCommands = new Dictionary<string, RoutedUICommand>();
		internal static Dictionary<string, System.Windows.Input.ICommand> commands = new Dictionary<string, System.Windows.Input.ICommand>();
		
		// Named UI instances and types
		private static RelationshipMap<string, WeakReference> namedUIInstances = new RelationshipMap<string, WeakReference>(null, new WeakReferenceTargetEqualirtyComparer());
		private static RelationshipMap<string, Type> namedUITypes = new RelationshipMap<string, Type>();
		
		// Registered input binding categories
		public static List<InputBindingCategory> InputBindingCategories = new List<InputBindingCategory>();
		
		static CommandManager()
		{
			// Load gesture profile first
			var path = PropertyService.Get("ICSharpCode.Core.Presentation.UserDefinedGesturesManager.UserGestureProfilesDirectory");
			if(path != null && File.Exists(path)) {
				var profile = new UserGestureProfile(path);
				profile.Load();
				
				UserGestureProfileManager.CurrentProfile = profile;
			}
			
			UserGestureProfileManager.CurrentProfileChanged += UserDefinedGesturesManager_CurrentProfileChanged;
		}

		static void UserDefinedGesturesManager_CurrentProfileChanged(object sender, NotifyUserGestureProfileChangedEventArgs args)
		{
			// Collect information about changed gestures and bindings from ProfileChanged event
			var changedGestures = new Dictionary<BindingInfoTemplate, Tuple<InputGestureCollection, InputGestureCollection>>();
			
			if(args.OldProfile != null) {
				args.OldProfile.GesturesChanged -= Profile_GesturesChanged;
				
				foreach(var pair in args.OldProfile) {
					changedGestures.Add(pair.Key, new Tuple<InputGestureCollection, InputGestureCollection>(pair.Value, null));
				}
			}
			
			if(args.NewProfile != null) {
				args.NewProfile.GesturesChanged += Profile_GesturesChanged;
				
				foreach(var pair in args.NewProfile) {
					if(!changedGestures.ContainsKey(pair.Key)) {
						changedGestures.Add(pair.Key, new Tuple<InputGestureCollection, InputGestureCollection>(null, pair.Value));
					} else {
						changedGestures[pair.Key] = new Tuple<InputGestureCollection, InputGestureCollection>(changedGestures[pair.Key].Item1, pair.Value);
					}
				}
			}
			
			var modifiedBindingTemplates = new HashSet<BindingInfoTemplate>();
			var descriptions = new List<GesturesModificationDescription>(changedGestures.Count);
			foreach(var changedGesture in changedGestures) {
				var bindingInfoTemplate = changedGesture.Key;
				InputGestureCollection defaultUserGesture = null;
				if(changedGesture.Value.Item1 == null || changedGesture.Value.Item2 == null) {
					defaultUserGesture = FindInputGestures(bindingInfoTemplate);
				}
				
				var oldGestures = changedGesture.Value.Item1 ?? defaultUserGesture;
				var newGestures = changedGesture.Value.Item2 ?? defaultUserGesture;
				descriptions.Add(new GesturesModificationDescription(changedGesture.Key, oldGestures, newGestures));
						
				modifiedBindingTemplates.Add(bindingInfoTemplate);
			}
			
			// Notify GesturesChanged and BindingsChanged events handlers about changes
			InvokeBindingsChanged(typeof(CommandManager), new NotifyBindingsChangedEventArgs(NotifyBindingsChangedAction.BindingInfoModified, modifiedBindingTemplates));
			InvokeGesturesChanged(typeof(CommandManager), new NotifyGesturesChangedEventArgs(descriptions));
		}

		static void Profile_GesturesChanged(object sender, NotifyGesturesChangedEventArgs args)
		{
			// Collect information about changed gestures and bindings from GesturesChanded event in profile
			var modifiedBindingTemplates = new HashSet<BindingInfoTemplate>();
			foreach(var description in args.ModificationDescriptions) {
				modifiedBindingTemplates.Add(description.InputBindingIdentifier);
			}
			
			// Notify GesturesChanged and BindingsChanged events handlers about changes
			InvokeBindingsChanged(typeof(CommandManager), new NotifyBindingsChangedEventArgs(NotifyBindingsChangedAction.BindingInfoModified, modifiedBindingTemplates));
			InvokeGesturesChanged(typeof(CommandManager), args);
		}
		
		/// <summary>
		/// Get fully qualified assembly name without version and locale
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string GetShortAssemblyQualifiedName(Type type)
		{
			return string.Format("{0}, {1}", type.FullName, type.Assembly.GetName().Name);
		}
		
		/// <summary>
		/// Register <see cref="UIlement" /> instance accessible by unique name
		/// </summary>
		/// <param name="instanceName">Instance name</param>
		/// <param name="instance">Instance</param>
		public static void RegisterNamedUIElement(string instanceName, UIElement instance)
		{	
			if(instanceName == null) {
				throw new ArgumentNullException("instanceName");
			}
			if(instance == null) {
				throw new ArgumentNullException("instance");
			}
			
			var oldInstances = GetNamedUIElementCollection(instanceName).ToArray();
			
			var container = new WeakReference(instance);
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
		
		/// <summary>
		/// Make <see cref="UIlement" /> instance no longer accessible by provided name
		/// </summary>
		/// <param name="instanceName">Instance name</param>
		/// <param name="instance">Instance</param>
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
		/// Get collection of <see cref="UIElement" /> instances associated with provided instance name
		/// </summary>
		/// <param name="instanceName">Instance name</param>
		/// <returns>Instances collection</returns>
		public static ICollection<UIElement> GetNamedUIElementCollection(string instanceName)
		{
			if(instanceName == null) {
				throw new ArgumentNullException("instanceName");
			}
			
			return namedUIInstances.MapForward(instanceName).Where(reference => reference.Target != null).Select(reference => (UIElement)reference.Target).ToList();
		}
		
		/// <summary>
		/// Get collection of names associated with <see cref="UIElement" /> instance
		/// </summary>
		/// <param name="instance">Instance</param>
		/// <returns>Names collection</returns>
		public static ICollection<string> GetUIElementNameCollection(UIElement instance)
		{
			if(instance == null) {
				throw new ArgumentNullException("instance");
			}
			
			return namedUIInstances.MapBackward(new WeakReference(instance));
		}
		
		/// <summary>
		/// Register <see cref="Type" /> accessible by unique name
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
		
		
		/// <summary>
		/// Make <see cref="Type" /> no longer accessible by provided name
		/// </summary>
		/// <param name="typeName">Type name</param>
		/// <param name="type">Type</param>
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
		/// Get collection of <see cref="Type" /> associated with provided type name
		/// </summary>
		/// <param name="typeName">Type name</param>
		/// <returns>Types collection</returns>
		public static ICollection<Type> GetNamedUITypeCollection(string typeName)
		{
			if(typeName == null) {
				throw new ArgumentNullException("typeName");
			}
			
			return namedUITypes.MapForward(typeName);
		}
		
		/// <summary>
		/// Get collection of names associated with <see cref="Type" />
		/// </summary>
		/// <param name="type">Type</param>
		/// <returns>Names collection</returns>
		public static ICollection<string> GetUITypeNameCollection(Type type)
		{
			if(type == null) {
				throw new ArgumentNullException("type");
			}
			
			return namedUITypes.MapBackward(type);
		}

		/// <summary>
		/// Register newly created <see cref="RoutedUICommand" /> with unique name
		/// 
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
		/// Register existing routed command
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
		/// Unregister routed command
		/// </summary>
		/// <param name="routedCommandName">Unregistered routed command name</param>
		public static void UnregisterRoutedUICommand(string routedCommandName) {
			if(routedCommands.ContainsKey(routedCommandName)) {
				var routedCommand = routedCommands[routedCommandName];
				routedCommands.Remove(routedCommandName);
				
				InvokeBindingsChanged(null, new NotifyBindingsChangedEventArgs(NotifyBindingsChangedAction.RoutedUICommandModified, routedCommandName));
			}
		}
		
		/// <summary>
		/// Get instance of <see cref="RoutedUICommand" /> by name
		/// </summary>
		/// <param name="routedCommandName">Routed command name</param>
		/// <returns>Routed UI command instance</returns>
		public static RoutedUICommand GetRoutedUICommand(string routedCommandName) {
			if(routedCommands != null && routedCommands.ContainsKey(routedCommandName)) {
				return routedCommands[routedCommandName];
			}
			
			return null;
		}
		
		/// <summary>
		/// Register <see cref="CommandBindingInfo" /> instance
		/// </summary>
		/// <param name="commandBindingInfo">Binding info</param>
		public static void RegisterCommandBindingInfo(CommandBindingInfo commandBindingInfo) {
			if(commandBindingInfo.OwnerInstanceName == null && commandBindingInfo.OwnerTypeName == null) {
				throw new ArgumentException("Binding owner must be specified");
			}
			
			if(commandBindingInfo.RoutedCommandName == null) {
				throw new ArgumentException("Routed command name must be specified");
			}
			
			commandBindings.Add(BindingInfoTemplate.CreateFromIBindingInfo(commandBindingInfo), commandBindingInfo);
			commandBindingInfo.IsRegistered = true;
				
			CommandManager.BindingsChanged += commandBindingInfo.BindingsChangedHandler;
			InvokeBindingsChanged(null, new NotifyBindingsChangedEventArgs(NotifyBindingsChangedAction.BindingInfoModified, new []{ BindingInfoTemplate.CreateFromIBindingInfo(commandBindingInfo) }));
		}

		/// <summary>
		/// Register <see cref="InputBindingInfo" /> instance
		/// </summary>
		/// <param name="inputBindingInfo">Binding info</param>
		public static void RegisterInputBindingInfo(InputBindingInfo inputBindingInfo)
		{
			if(inputBindingInfo.OwnerInstanceName == null && inputBindingInfo.OwnerTypeName == null) {
				throw new ArgumentException("Binding owner must be specified");
			}
			
			if(inputBindingInfo.RoutedCommandName == null) {
				throw new ArgumentException("Routed command name must be specified");
			}
			
			var similarTemplate = BindingInfoTemplate.CreateFromIBindingInfo(inputBindingInfo);
			var similarInputBinding = FindInputBindingInfos(similarTemplate).FirstOrDefault();
			
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
				inputBidnings.Add(BindingInfoTemplate.CreateFromIBindingInfo(inputBindingInfo), inputBindingInfo);
				inputBindingInfo.IsRegistered = true;
				
				CommandManager.BindingsChanged += inputBindingInfo.BindingsChangedHandler;
				InvokeGesturesChanged(
					typeof(CommandManager), 
					new NotifyGesturesChangedEventArgs(
						new GesturesModificationDescription(
							similarTemplate,
							null,
							inputBindingInfo.ActiveGestures)));
			}
			
			InvokeBindingsChanged(typeof(CommandManager), new NotifyBindingsChangedEventArgs(NotifyBindingsChangedAction.BindingInfoModified, new [] { similarTemplate }));
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
		/// Occurs when information relevant to registered <see cref="IBindingInfo"/> changes
		/// </summary>
		public static event NotifyBindingsChangedEventHandler BindingsChanged;
		
		/// <summary>
		/// Invoke <see cref="BindingsChanged" /> event handlers
		/// </summary>
		/// <param name="sender">Sender object</param>
		/// <param name="args">Event arguments</param>
		public static void InvokeBindingsChanged(object sender, NotifyBindingsChangedEventArgs args)
		{
			if(!SuspendEvents && BindingsChanged != null) {
				BindingsChanged.Invoke(sender, args);
			}
		}
		
		/// <summary>
		/// Occurs when <see cref="InputBindingInfo.ActiveGestures" /> in any registered
		/// <see cref="InputBindingInfo" /> changes
		/// </summary>
		public static event NotifyGesturesChangedEventHandler GesturesChanged;
		
		/// <summary>
		/// Invoke <see cref="GesturesChanged" /> event handlers
		/// </summary>
		/// <param name="sender">Sender object</param>
		/// <param name="args">Event arguments</param>
		public static void InvokeGesturesChanged(object sender, NotifyGesturesChangedEventArgs args)
		{
			if(!SuspendEvents && GesturesChanged != null) {
				GesturesChanged.Invoke(sender, args);
			}
		}
		
		/// <summary>
		/// Load all registered commands in add-in
		/// </summary>
		/// <param name="addIn">Add-in</param>
		public static void LoadAddinCommands(AddIn addIn) {		
			foreach(CommandBindingInfo binding in FindCommandBindingInfos(new BindingInfoTemplate())) {
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
		/// Unregister <see cref="InputBindingInfo" /> instance
		/// </summary>
		/// <param name="inputBindingInfo">Binding info</param>
		public static void UnregisterInputBinding(InputBindingInfo inputBindingInfo)
		{
			UnregisterBindingInfo(inputBidnings, inputBindingInfo);
		}
		
		/// <summary>
		/// Unregister <see cref="CommandBindingInfo" /> instance
		/// </summary>
		/// <param name="commandBindingInfo">Command binding parameters</param>
		public static void UnregisterCommandBinding(CommandBindingInfo commandBindingInfo) 
		{
			UnregisterBindingInfo(commandBindings, commandBindingInfo);
		}
		
		private static void UnregisterBindingInfo(BindingInfoTemplateDictionary<BindingInfoBase> bindingInfoDictionary, BindingInfoBase bindingInfo)
		{
			bindingInfoDictionary.Remove(bindingInfo);
			CommandManager.BindingsChanged -= bindingInfo.BindingsChangedHandler;
			bindingInfo.RemoveActiveBindings();
			bindingInfo.IsRegistered = false;
			
			InvokeBindingsChanged(null, new NotifyBindingsChangedEventArgs(NotifyBindingsChangedAction.BindingInfoModified, new[] { BindingInfoTemplate.CreateFromIBindingInfo(bindingInfo) } ));
		}
		
		#endregion
		
		#region Find binding infos

		/// <summary>
		/// Get list of all <see cref="CommandBindingInfo" />  matched by provided <see cref="BindingInfoTemplate" />
		/// </summary>
		/// <param name="template">Template to match against</param>
		/// <returns>Collection of matched <see cref="CommandBindingInfo" /> instances</returns>
		public static ICollection<CommandBindingInfo> FindCommandBindingInfos(BindingInfoTemplate template)
		{
			return FindBindingInfos(commandBindings, template).Cast<CommandBindingInfo>().ToList();
		}
		
		/// <summary>
		/// Get list of all <see cref="InputBindingInfo" />  matched by provided <see cref="BindingInfoTemplate" />
		/// </summary>
		/// <param name="template">Template to match against</param>
		/// <returns>Collection of matched <see cref="InputBindingInfo" /> instances</returns>
		public static ICollection<InputBindingInfo> FindInputBindingInfos(BindingInfoTemplate template)
		{
			return FindBindingInfos(inputBidnings, template).Cast<InputBindingInfo>().ToList();
		}
		
		private static ICollection<BindingInfoBase> FindBindingInfos(BindingInfoTemplateDictionary<BindingInfoBase> bindingInfos, BindingInfoTemplate template)
		{
			var items = bindingInfos.FindItems(template);
		
			return items ?? new List<BindingInfoBase>();
		}
		
		/// <summary>
		/// Get list of <see cref="System.Windows.Input.InputGesture" /> from <see cref="InputBindingInfo" /> instances matched by
		/// provided <see cref="BindingInfoTemplate" />
		/// </summary>
		/// <param name="template">Template to match against</param>
		/// <returns>Collection of matched <see cref="System.Windows.Input.InputGesture" /> instances</returns>
		public static InputGestureCollection FindInputGestures(BindingInfoTemplate template) {
			var bindings = FindInputBindingInfos(template);
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
		
		/// <summary>
		/// Get registered <see cref="InputBindingCategory" /> by path
		/// </summary>
		/// <param name="categoryPath">Category path</param>
		/// <param name="throwWhenNotFound">Defines whether to throw an exception on category with this path not found or not</param>
		/// <returns></returns>
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
		
		/// <summary>
		/// Get <see cref="InputBindingCategoryCollection" /> of categories matching any of provided paths
		/// </summary>
		/// <param name="categoryPathCollectionString">Category paths separated by comma</param>
		/// <param name="throwWhenNotFound">Defines whether to throw an exception on category with this path not found or not</param>
		/// <returns></returns>
		public static InputBindingCategoryCollection GetInputBindingCategoryCollection(string categoryPathCollectionString, bool throwWhenNotFound)
		{
			var categoryPathCollection = categoryPathCollectionString.Split(',');
			var categories = new InputBindingCategoryCollection();
			foreach(var categoryPath in categoryPathCollection) {
				var category = GetInputBindingCategory(categoryPath, throwWhenNotFound);
				
				if(category != null) {
					categories.Add(category);
				}
			}
			
			return categories;
		}
		
		/// <summary>
		/// Get <see cref="InputBindingInfoCategoryCollection" /> of children categories from root category path
		/// </summary>
		/// <param name="rootCategoryPath">Root category path</param>
		/// <returns>List of child categories</returns>
		public static ICollection<InputBindingCategory> GetInputBindingCategoryChildren(string rootCategoryPath) 
		{
			var children = new InputBindingCategoryCollection();
			var categoryDepth = rootCategoryPath.Count(c => c == '/');
			foreach(var currentCategory in InputBindingCategories) {
				if(currentCategory.Path.StartsWith(rootCategoryPath)) {
					var currentCategoryDepth = currentCategory.Path.Count(c => c == '/');
					
					if(currentCategoryDepth == categoryDepth + 1)
					{
						children.Add(currentCategory);
					}
				}
			}
			
			return children;
		}
		
		/// <summary>
		/// Register <see cref="InputBindingCateogyr" /> with unique path
		/// </summary>
		/// <param name="category">Registered category</param>
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
		/// Reset events handlers and registered objects
		/// 
		/// Use for nit testing only!
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
}