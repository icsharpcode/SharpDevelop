using System;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Documents;
using ICSharpCode.Core;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Description of CommandsService.
	/// </summary>
	public static class CommandsService
	{
		public static void RegisterRoutedCommands(Type type) {
			var typeProperties = type.GetProperties(BindingFlags.Static | BindingFlags.Public);
			foreach(var property in typeProperties) {
				var command = (RoutedUICommand)property.GetValue(null, null);				
				CommandsRegistry.RegisterRoutedUICommand(command);
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
				CommandsRegistry.RegisterRoutedUICommand(desc.Name, desc.Text);                                                                    	
			}
		}
		
		public static void RegisterCommandBindings(object caller, string path) 
		{
			var descriptors = AddInTree.BuildItems<CommandBindingDescriptor>(path, caller, false);
			foreach(var desc in descriptors) {
				// If routed with such name is not registered register routed command with text same as name
				if(CommandsRegistry.GetRoutedUICommand(desc.Command) == null) {
					var commandText = string.IsNullOrEmpty(desc.CommandText) ? desc.Command : desc.CommandText;
					CommandsRegistry.RegisterRoutedUICommand(desc.Command, commandText);
				}
				
				var commandBindingInfo = new CommandBindingInfo();
				
				if(!string.IsNullOrEmpty(desc.OwnerInstanceName)) {
					commandBindingInfo.OwnerInstanceName = desc.OwnerInstanceName;
				} else if(!string.IsNullOrEmpty(desc.OwnerTypeName)) {
					commandBindingInfo.OwnerTypeName = desc.OwnerTypeName;
				} else {
					commandBindingInfo.OwnerTypeName = CommandsRegistry.DefaultContextName;
				}
				
				commandBindingInfo.RoutedCommandName = desc.Command;
				commandBindingInfo.ClassName = desc.Class;
				commandBindingInfo.AddIn = desc.Codon.AddIn;
				commandBindingInfo.IsLazy = desc.Lazy;
				CommandsRegistry.RegisterCommandBinding(commandBindingInfo);
				
				// If gestures are provided register input binding in the same context
				if(!string.IsNullOrEmpty(desc.Gestures)) {
					var gestures = (InputGestureCollection)new InputGestureCollectionConverter().ConvertFromString(desc.Gestures);
					
					var inputBindingInfo = new InputBindingInfo();
					
					if(!string.IsNullOrEmpty(desc.OwnerInstanceName)) {
						inputBindingInfo.OwnerInstanceName = desc.OwnerInstanceName;
					} else if(!string.IsNullOrEmpty(desc.OwnerTypeName)) {
						inputBindingInfo.OwnerTypeName = desc.OwnerTypeName;
					} else {
						inputBindingInfo.OwnerTypeName = CommandsRegistry.DefaultContextName;
					}
					
					inputBindingInfo.AddIn = desc.Codon.AddIn;
					inputBindingInfo.RoutedCommandName = desc.Command;
					inputBindingInfo.Gestures = gestures;
					
					if(!string.IsNullOrEmpty(desc.CommandText)) {
						inputBindingInfo.RoutedCommandText = desc.CommandText;
					}
					
					if(!string.IsNullOrEmpty(desc.Category)) {
						inputBindingInfo.Categories.AddRange(CommandsRegistry.RegisterInputBindingCategories(desc.Category));
					}
					
					CommandsRegistry.RegisterInputBinding(inputBindingInfo);
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
					inputBindingInfo.OwnerTypeName = CommandsRegistry.DefaultContextName;
				}	
				
				inputBindingInfo.AddIn = desc.Codon.AddIn;
				inputBindingInfo.RoutedCommandName = desc.Command;
				inputBindingInfo.Gestures = gestures;
				
				if(!string.IsNullOrEmpty(desc.CommandText)) {
					inputBindingInfo.RoutedCommandText = desc.CommandText;
				}
				
				if(!string.IsNullOrEmpty(desc.Category)) {
					inputBindingInfo.Categories.AddRange(CommandsRegistry.RegisterInputBindingCategories(desc.Category));
				}
				
				CommandsRegistry.RegisterInputBinding(inputBindingInfo);
			}
		}
	}
}
