using System;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Documents;
using System.Text;
using System.Collections;
using ICSharpCode.Core;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Description of CommandsService.
	/// </summary>
	public static class CommandsService
	{
		public static void RegisterMenuBindings(string menuRootsLocationPath, object caller)
		{
			var menuRoots = AddInTree.BuildItems<MenuRootDescriptor>(menuRootsLocationPath, caller);
			foreach(var menuRoot in menuRoots) {
				CommandsService.RegisterSingleMenuBindings(menuRoot.Path, caller, menuRoot.Name);
			}
		}
		
		public static void RegisterSingleMenuBindings(string menuPath, object caller, string categoryPath)
		{
			var menuItemNode = AddInTree.GetTreeNode(menuPath);
			var menuItems = menuItemNode.BuildChildItems<MenuItemDescriptor>(caller);
			
			RegisterSingleMenuBindings(menuItems, caller, categoryPath);
		}
			
		private static void RegisterSingleMenuBindings(IList menuItems, object caller, string categoryPath) {
			foreach(MenuItemDescriptor item in menuItems) {
				var codon = item.Codon;
					
				if(codon.Properties["type"] == "" || codon.Properties["type"] == "command") {
					string routedCommandName = null;
					string routedCommandText = null;
					
					if(codon.Properties.Contains("command")) {
						routedCommandName = codon.Properties["command"];				
						routedCommandText = codon.Properties["command"];
					} else if(codon.Properties.Contains("link") || codon.Properties.Contains("class")) {
						routedCommandName = string.IsNullOrEmpty(codon.Properties["link"]) ? codon.Properties["class"] : codon.Properties["link"];
						routedCommandText = codon.Properties["label"];
					}
					
					var routedCommand = CommandManager.GetRoutedUICommand(routedCommandName);
					if(routedCommand == null) {
						routedCommand = CommandManager.RegisterRoutedUICommand(routedCommandName, routedCommandText);
					}

					if(!codon.Properties.Contains("command") && (codon.Properties.Contains("link") || codon.Properties.Contains("class"))) {
						var commandBindingInfo = new CommandBindingInfo();
						commandBindingInfo.AddIn = codon.AddIn;
						commandBindingInfo.OwnerTypeName = CommandManager.DefaultContextName;
						commandBindingInfo.CommandInstance = CommandWrapper.GetCommand(codon, null, codon.Properties["loadclasslazy"] == "true");
						commandBindingInfo.RoutedCommandName = routedCommandName;
						commandBindingInfo.IsLazy = true;
						CommandManager.RegisterCommandBinding(commandBindingInfo);
					}
					
					// Register input bindings
					var inputBindingInfo = new InputBindingInfo();
					inputBindingInfo.AddIn = codon.AddIn;
					inputBindingInfo.OwnerTypeName = CommandManager.DefaultContextName;
					inputBindingInfo.RoutedCommandName = routedCommandName;
					
					var defaultGesture = (InputGestureCollection)new InputGestureCollectionConverter().ConvertFromInvariantString(codon.Properties["shortcut"]);
					inputBindingInfo.DefaultGestures.AddRange(defaultGesture);
					
					var menuCategories = CommandManager.RegisterInputBindingCategories(categoryPath);
					inputBindingInfo.Categories.AddRange(menuCategories);
					
					if(codon.Properties.Contains("category")) {
						var userDefinedCategories = CommandManager.RegisterInputBindingCategories(codon.Properties["category"]);
						inputBindingInfo.Categories.AddRange(userDefinedCategories);
					}
					
					CommandManager.RegisterInputBinding(inputBindingInfo);
				}
				
				if(item.SubItems != null) {
					RegisterSingleMenuBindings(item.SubItems, caller, categoryPath + "/" + item.Codon.Properties["label"]);
				}
			}
		}
		
		public static void RegisterRoutedCommands(Type type) {
			var typeProperties = type.GetProperties(BindingFlags.Static | BindingFlags.Public);
			foreach(var property in typeProperties) {
				var command = (RoutedUICommand)property.GetValue(null, null);				
				CommandManager.RegisterRoutedUICommand(command);
			}
		}
		
		public static void RegisterBuiltInRoutedUICommands() {
			RegisterRoutedCommands(typeof(ApplicationCommands));
			RegisterRoutedCommands(typeof(ComponentCommands));
			RegisterRoutedCommands(typeof(MediaCommands));
			RegisterRoutedCommands(typeof(NavigationCommands));
			RegisterRoutedCommands(typeof(EditingCommands));
		}
		
		public static void RegisterRoutedUICommands(object caller, string path) 
		{
			var descriptors = AddInTree.BuildItems<RoutedUICommandDescriptor>(path, caller, false);
			foreach(var desc in descriptors) {
				CommandManager.RegisterRoutedUICommand(desc.Name, desc.Text);                                                                    	
			}
		}
		
		public static void RegisterCommandBindings(object caller, string path) 
		{
			var descriptors = AddInTree.BuildItems<CommandBindingDescriptor>(path, caller, false);
			foreach(var desc in descriptors) {
				var commandBindingInfoName = new StringBuilder();

				// If routed with such name is not registered register routed command with text same as name
				if(CommandManager.GetRoutedUICommand(desc.Command) == null) {
					var commandText = string.IsNullOrEmpty(desc.CommandText) ? desc.Command : desc.CommandText;
					CommandManager.RegisterRoutedUICommand(desc.Command, commandText);
				}
				
				var commandBindingInfo = new CommandBindingInfo();
				
				if(!string.IsNullOrEmpty(desc.OwnerInstanceName)) {
					commandBindingInfo.OwnerInstanceName = desc.OwnerInstanceName;
				} else if(!string.IsNullOrEmpty(desc.OwnerTypeName)) {
					commandBindingInfo.OwnerTypeName = desc.OwnerTypeName;
				} else {
					commandBindingInfo.OwnerTypeName = CommandManager.DefaultContextName;
				}
				
				commandBindingInfo.RoutedCommandName = desc.Command;
				commandBindingInfo.CommandTypeName = desc.Class;
				commandBindingInfo.AddIn = desc.Codon.AddIn;
				commandBindingInfo.IsLazy = desc.Lazy;
				CommandManager.RegisterCommandBinding(commandBindingInfo);
				
				// If gestures are provided register input binding in the same context
				if(!string.IsNullOrEmpty(desc.Gestures)) {
					var gestures = (InputGestureCollection)new InputGestureCollectionConverter().ConvertFromString(desc.Gestures);
					
					var inputBindingInfo = new InputBindingInfo();
					
					if(!string.IsNullOrEmpty(desc.OwnerInstanceName)) {
						inputBindingInfo.OwnerInstanceName = desc.OwnerInstanceName;
					} else if(!string.IsNullOrEmpty(desc.OwnerTypeName)) {
						inputBindingInfo.OwnerTypeName = desc.OwnerTypeName;
					} else {
						inputBindingInfo.OwnerTypeName = CommandManager.DefaultContextName;
					}
					
					inputBindingInfo.AddIn = desc.Codon.AddIn;
					inputBindingInfo.RoutedCommandName = desc.Command;
					inputBindingInfo.DefaultGestures = gestures;
					
					if(!string.IsNullOrEmpty(desc.CommandText)) {
						inputBindingInfo.RoutedCommandText = desc.CommandText;
					}
					
					if(!string.IsNullOrEmpty(desc.Category)) {
						inputBindingInfo.Categories.AddRange(CommandManager.RegisterInputBindingCategories(desc.Category));
					}
					
					CommandManager.RegisterInputBinding(inputBindingInfo);
				}
			}
		}
		
		public static void RegisterInputBindings(object caller, string path) 
		{
			var descriptors = AddInTree.BuildItems<InputBindingDescriptor>(path, caller, false);
			foreach(var desc in descriptors) {
				var gestures = (InputGestureCollection)new InputGestureCollectionConverter().ConvertFromString(desc.Gestures);
				
				var inputBindingInfo = new InputBindingInfo();
					
				if(!string.IsNullOrEmpty(desc.OwnerInstanceName)) {
					inputBindingInfo.OwnerInstanceName = desc.OwnerInstanceName;
				} else if(!string.IsNullOrEmpty(desc.OwnerTypeName)) {
					inputBindingInfo.OwnerTypeName = desc.OwnerTypeName;
				} else {
					inputBindingInfo.OwnerTypeName = CommandManager.DefaultContextName;
				}	
				
				inputBindingInfo.AddIn = desc.Codon.AddIn;
				inputBindingInfo.RoutedCommandName = desc.Command;
				inputBindingInfo.DefaultGestures = gestures;
				
				if(!string.IsNullOrEmpty(desc.CommandText)) {
					inputBindingInfo.RoutedCommandText = desc.CommandText;
				}
				
				if(!string.IsNullOrEmpty(desc.Category)) {
					inputBindingInfo.Categories.AddRange(CommandManager.RegisterInputBindingCategories(desc.Category));
				}
				
				CommandManager.RegisterInputBinding(inputBindingInfo);
			}
		}
	}
}
