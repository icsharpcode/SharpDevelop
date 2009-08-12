using System;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Documents;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using ICSharpCode.Core;
using SDCommandManager=ICSharpCode.Core.Presentation.CommandManager;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Utility class to help manage commands and bindings
	/// </summary>
	public static class CommandsService
	{
		/// <summary>
		/// Register <see cref="InputBindingInfo" /> and <see cref="CommandBindingInfo" /> 
		/// for all MenuItem codons under provided path
		/// </summary>
		/// <param name="caller">Caller object</param>
		/// <param name="menuRootsLocationPath">Path to codons</param>
		public static void RegisterMenuBindings(object caller, string menuRootsLocationPath)
		{
			var menuRoots = AddInTree.BuildItems<MenuLocationDescriptor>(menuRootsLocationPath, caller);
			foreach(var menuRoot in menuRoots) {
				CommandsService.RegisterSingleMenuBindings(menuRoot.Path, caller, menuRoot.Category);
			}
		}
		
		/// <summary>
		/// Register <see cref="InputBindingInfo" /> and <see cref="CommandBindingInfo" /> using data in MenuItem codon
		/// and nested codons
		/// </summary>
		/// <param name="menuPath">Path to MenuItem codon</param>
		/// <param name="caller">Caller object</param>
		/// <param name="categoryPath"><see cref="InputBindingCategory" /> path</param>
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
						if(codon.Properties.Contains("ownerinstance")) {
							commandBindingInfo.OwnerInstanceName = codon.Properties["ownerinstance"];
						} else {
							commandBindingInfo.OwnerTypeName = codon.Properties.Contains("ownertype") ? codon.Properties["ownertype"] : SDCommandManager.DefaultOwnerTypeName;
						}
						commandBindingInfo.CommandInstance = CommandWrapper.GetCommand(codon, null, codon.Properties["loadclasslazy"] == "true");
						commandBindingInfo.RoutedCommandName = routedCommandName;
						commandBindingInfo.IsLazy = true;
						SDCommandManager.RegisterCommandBindingInfo(commandBindingInfo);
					}
					
					// Register input bindings
					var inputBindingInfo = new InputBindingInfo();
					inputBindingInfo.AddIn = codon.AddIn;
					if(codon.Properties.Contains("ownerinstance")) {
						inputBindingInfo.OwnerInstanceName = codon.Properties["ownerinstance"];
					} else {
						inputBindingInfo.OwnerTypeName = codon.Properties.Contains("ownertype") ? codon.Properties["ownertype"] : SDCommandManager.DefaultOwnerTypeName;
					}
					inputBindingInfo.RoutedCommandName = routedCommandName;
					
					var defaultGesture = (InputGestureCollection)new InputGestureCollectionConverter().ConvertFromInvariantString(codon.Properties["shortcut"]);
					inputBindingInfo.DefaultGestures.AddRange(defaultGesture);
					
					// Menu category
					var menuCategory = SDCommandManager.GetInputBindingCategory(categoryPath, true);
					inputBindingInfo.Categories.Add(menuCategory);
					
					// User defined categories
					if(codon.Properties.Contains("inputbindingcategories")) {
						var additionalCategories = SDCommandManager.GetInputBindingCategoryCollection(codon.Properties["inputbindingcategories"], true);
						inputBindingInfo.Categories.AddRange(additionalCategories);
					}
					
					SDCommandManager.RegisterInputBindingInfo(inputBindingInfo);
				}
				
				if(item.SubItems != null) {
					var subMenuCategory = new InputBindingCategory(categoryPath + "/" + item.Codon.Id, codon.Properties["label"]);
					SDCommandManager.RegisterInputBindingCategory(subMenuCategory);
					
					RegisterSingleMenuBindings(item.SubItems, caller, categoryPath + "/" + item.Codon.Id);
				}
			}
		}
		
		/// <summary>
		/// Register all <see cref="RoutedUICommand" /> static propertiess in provided class
		/// </summary>
		/// <param name="type">Class with static properties returning <see cref="RoutedUICommand" /></param>
		public static void RegisterRoutedCommands(Type type) {
			var typeMembers = type.GetMembers(BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty);
			foreach(var member in typeMembers) {
				RoutedUICommand command = null;
				
				var property = member as PropertyInfo;
				var field = member as FieldInfo;
				if(property != null) {
					command = property.GetValue(null, null) as RoutedUICommand;
				} else if(field != null) {
					command = field.GetValue(null) as RoutedUICommand;
				}
				
				if(command != null) {
					SDCommandManager.RegisterRoutedUICommand(command);
				}
			}
		}
		
		/// <summary>
		/// Register default .Net routed commands
		/// </summary>
		public static void RegisterBuiltInRoutedUICommands() {
			RegisterRoutedCommands(typeof(ApplicationCommands));
			RegisterRoutedCommands(typeof(ComponentCommands));
			RegisterRoutedCommands(typeof(MediaCommands));
			RegisterRoutedCommands(typeof(NavigationCommands));
			RegisterRoutedCommands(typeof(EditingCommands));
		}
		
		/// <summary>
		/// Register <see cref="InputBindingCategory">InputBindingCategories</see> described by codons with provided path
		/// </summary>
		/// <param name="caller">Caller object</param>
		/// <param name="path">Path to codons</param>
		public static void RegisterInputBindingCategories(object caller, string path) {
			var descriptors = AddInTree.BuildItems<InputBindingCategoryDescriptor>(path, caller, false);
			
			foreach(var desc in descriptors) 
			{
				RegisterInputBindingCategories(desc, "");
			}
		}
		
		private static void RegisterInputBindingCategories(InputBindingCategoryDescriptor descriptor, string categoryPath)
		{
			categoryPath = categoryPath + "/" + descriptor.Id;
			var category = new InputBindingCategory(categoryPath, descriptor.Text);
			SDCommandManager.RegisterInputBindingCategory(category);
			
			foreach(var desc in descriptor.Children)
			{
				RegisterInputBindingCategories(desc, categoryPath);
			}
		}
		
		/// <summary>
		/// Register <see cref="RoutedUICommand" />s described by codons with provided path
		/// </summary>
		/// <param name="caller">Caller object</param>
		/// <param name="path">Path to codons</param>
		public static void RegisterRoutedUICommands(object caller, string path) 
		{
			var descriptors = AddInTree.BuildItems<RoutedUICommandDescriptor>(path, caller, false);
			foreach(var desc in descriptors) {
				SDCommandManager.RegisterRoutedUICommand(desc.Name, desc.Text);                                                                    	
			}
		}
		
		/// <summary>
		/// Register command and input binding infos described by codons with provided path
		/// </summary>
		/// <param name="caller">Caller object</param>
		/// <param name="path">Path to codons</param>
		public static void RegisterCommandBindings(object caller, string path) 
		{
			var descriptors = AddInTree.BuildItems<CommandBindingInfoDescriptor>(path, caller, false);
			foreach(var desc in descriptors) {
				var commandBindingInfoName = new StringBuilder();
				
				// If routed with such name is not registered register routed command with text same as name
				if(SDCommandManager.GetRoutedUICommand(desc.Command) == null) {
					var commandText = string.IsNullOrEmpty(desc.CommandText) ? desc.Command : desc.CommandText;
					SDCommandManager.RegisterRoutedUICommand(desc.Command, commandText);
				}
				
				var commandBindingInfo = new CommandBindingInfo();
				
				if(!string.IsNullOrEmpty(desc.OwnerInstanceName)) {
					commandBindingInfo.OwnerInstanceName = desc.OwnerInstanceName;
				} else if(!string.IsNullOrEmpty(desc.OwnerTypeName)) {
					commandBindingInfo.OwnerTypeName = desc.OwnerTypeName;
				} else {
					commandBindingInfo.OwnerTypeName = CommandManager.DefaultOwnerTypeName;
				}
				
				commandBindingInfo.RoutedCommandName = desc.Command;
				commandBindingInfo.CommandTypeName = desc.Class;
				commandBindingInfo.AddIn = desc.Codon.AddIn;
				commandBindingInfo.IsLazy = desc.Lazy;
				SDCommandManager.RegisterCommandBindingInfo(commandBindingInfo);
				
				// If gestures are provided register input binding in the same context
				if(!string.IsNullOrEmpty(desc.Gestures)) {
					var gestures = (InputGestureCollection)new InputGestureCollectionConverter().ConvertFromString(desc.Gestures);
					
					var inputBindingInfo = new InputBindingInfo();
					
					if(!string.IsNullOrEmpty(desc.OwnerInstanceName)) {
						inputBindingInfo.OwnerInstanceName = desc.OwnerInstanceName;
					} else if(!string.IsNullOrEmpty(desc.OwnerTypeName)) {
						inputBindingInfo.OwnerTypeName = desc.OwnerTypeName;
					} else {
						inputBindingInfo.OwnerTypeName = CommandManager.DefaultOwnerTypeName;
					}
					
					inputBindingInfo.AddIn = desc.Codon.AddIn;
					inputBindingInfo.RoutedCommandName = desc.Command;
					inputBindingInfo.DefaultGestures.AddRange(gestures);
					
					if(!string.IsNullOrEmpty(desc.CommandText)) {
						inputBindingInfo.RoutedCommandText = desc.CommandText;
					}
					
					if(!string.IsNullOrEmpty(desc.Categories)) {
						var categories = CommandManager.GetInputBindingCategoryCollection(desc.Categories, true);
						inputBindingInfo.Categories.AddRange(categories);
					}
					
					SDCommandManager.RegisterInputBindingInfo(inputBindingInfo);
				}
			}
		}
		
		/// <summary>
		/// Register input binding infos described by codons with provided path
		/// </summary>
		/// <param name="caller">Caller object</param>
		/// <param name="path">Path to codons</param>
		public static void RegisterInputBindings(object caller, string path) 
		{
			var descriptors = AddInTree.BuildItems<InputBindingInfoDescriptor>(path, caller, false);
			foreach(var desc in descriptors) {
				var gestures = (InputGestureCollection)new InputGestureCollectionConverter().ConvertFromString(desc.Gestures);
				
				var inputBindingInfo = new InputBindingInfo();
				
				if(!string.IsNullOrEmpty(desc.OwnerInstanceName)) {
					inputBindingInfo.OwnerInstanceName = desc.OwnerInstanceName;
				} else if(!string.IsNullOrEmpty(desc.OwnerTypeName)) {
					inputBindingInfo.OwnerTypeName = desc.OwnerTypeName;
				} else {
					inputBindingInfo.OwnerTypeName = SDCommandManager.DefaultOwnerTypeName;
				}	
				
				inputBindingInfo.AddIn = desc.Codon.AddIn;
				inputBindingInfo.RoutedCommandName = desc.Command;
				inputBindingInfo.DefaultGestures.AddRange(gestures);
				
				if(!string.IsNullOrEmpty(desc.CommandText)) {
					inputBindingInfo.RoutedCommandText = desc.CommandText;
				}
				
				if(!string.IsNullOrEmpty(desc.Categories)) {
					var categories = SDCommandManager.GetInputBindingCategoryCollection(desc.Categories, true);
					inputBindingInfo.Categories.AddRange(categories);
				}
				
				SDCommandManager.RegisterInputBindingInfo(inputBindingInfo);
			}
		}
	}
}
