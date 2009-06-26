using System;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Documents;
using System.Text;
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
					commandBindingInfoName.AppendFormat("{0}_", desc.OwnerInstanceName);
				} else if(!string.IsNullOrEmpty(desc.OwnerTypeName)) {
					commandBindingInfo.OwnerTypeName = desc.OwnerTypeName;
					commandBindingInfoName.AppendFormat("{0}_", desc.OwnerTypeName);
				} else {
					commandBindingInfo.OwnerTypeName = CommandManager.DefaultContextName;
					commandBindingInfoName.AppendFormat("{0}_", CommandManager.DefaultContextName);
				}
				
				commandBindingInfo.RoutedCommandName = desc.Command;
				commandBindingInfoName.AppendFormat("{0}_", desc.Command);
				
				commandBindingInfo.CommandTypeName = desc.Class;
				commandBindingInfoName.AppendFormat("{0}_", desc.Class);
				
				commandBindingInfo.AddIn = desc.Codon.AddIn;
				commandBindingInfoName.Append(desc.Codon.AddIn.Name);
				
				commandBindingInfo.IsLazy = desc.Lazy;
				
				commandBindingInfo.Name = "CommandBinding_" + commandBindingInfoName.ToString();
				CommandManager.RegisterCommandBinding(commandBindingInfo);
				
				// If gestures are provided register input binding in the same context
				if(!string.IsNullOrEmpty(desc.Gestures)) {
					var gestures = (InputGestureCollection)new InputGestureCollectionConverter().ConvertFromString(desc.Gestures);
					
					var inputBindingInfo = new InputBindingInfo();
					inputBindingInfo.Name = "InputBinding_" + commandBindingInfoName.ToString();
					
					if(!string.IsNullOrEmpty(desc.OwnerInstanceName)) {
						inputBindingInfo.OwnerInstanceName = desc.OwnerInstanceName;
					} else if(!string.IsNullOrEmpty(desc.OwnerTypeName)) {
						inputBindingInfo.OwnerTypeName = desc.OwnerTypeName;
					} else {
						inputBindingInfo.OwnerTypeName = CommandManager.DefaultContextName;
					}
					
					inputBindingInfo.AddIn = desc.Codon.AddIn;
					inputBindingInfo.RoutedCommandName = desc.Command;
					inputBindingInfo.Gestures = gestures;
					
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
				
				StringBuilder inputBindingInfoName = new StringBuilder();
					
				if(!string.IsNullOrEmpty(desc.OwnerInstanceName)) {
					inputBindingInfo.OwnerInstanceName = desc.OwnerInstanceName;
					inputBindingInfoName.AppendFormat("{0}_", desc.OwnerInstanceName);
				} else if(!string.IsNullOrEmpty(desc.OwnerTypeName)) {
					inputBindingInfo.OwnerTypeName = desc.OwnerTypeName;
					inputBindingInfoName.AppendFormat("{0}_", desc.OwnerTypeName);
				} else {
					inputBindingInfo.OwnerTypeName = CommandManager.DefaultContextName;
					inputBindingInfoName.AppendFormat("{0}_", CommandManager.DefaultContextName);
				}	
				
				inputBindingInfo.AddIn = desc.Codon.AddIn;
				inputBindingInfoName.AppendFormat("{0}_", desc.Codon.AddIn.Name);
					
				inputBindingInfo.RoutedCommandName = desc.Command;
				inputBindingInfoName.AppendFormat("{0}_", desc.Command);
				
				inputBindingInfo.Gestures = gestures;
				
				if(!string.IsNullOrEmpty(desc.CommandText)) {
					inputBindingInfo.RoutedCommandText = desc.CommandText;
				}
				
				if(!string.IsNullOrEmpty(desc.Category)) {
					inputBindingInfo.Categories.AddRange(CommandManager.RegisterInputBindingCategories(desc.Category));
				}
				
				inputBindingInfo.Name = inputBindingInfoName.ToString();
				CommandManager.RegisterInputBinding(inputBindingInfo);
			}
		}
	}
}
