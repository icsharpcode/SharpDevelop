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
				var contextName = !string.IsNullOrEmpty(desc.Context) ? desc.Context : CommandsRegistry.DefaultContext;
				
				// If routed with such name is not registered register routed command with text same as name
				if(CommandsRegistry.GetRoutedUICommand(desc.Command) == null) {
					CommandsRegistry.RegisterRoutedUICommand(desc.Command, desc.Command);
				}
				
				CommandsRegistry.RegisterCommandBinding(contextName, null, desc.Command, desc.Class, desc.Codon.AddIn, desc.Lazy);
				
				// If gestures are provided register input binding in the same context
				if(!string.IsNullOrEmpty(desc.Gestures)) {
					var gestures = (InputGestureCollection)new InputGestureCollectionConverter().ConvertFromString(desc.Gestures);
					foreach(InputGesture gesture in gestures) {
						CommandsRegistry.RegisterInputBinding(contextName, null, desc.Command, gesture);
					}
				}
			}
		}
		
		public static void RegisterInputBindings(object caller, string path) 
		{
			var descriptors = AddInTree.BuildItems<InputBindingDescriptor>(path, caller, false);
			foreach(var desc in descriptors) {
				var contextName = !string.IsNullOrEmpty(desc.Context) ? desc.Context : CommandsRegistry.DefaultContext;
				
				var gesture = (KeyGesture)new KeyGestureConverter().ConvertFromInvariantString(desc.Gesture);
				CommandsRegistry.RegisterInputBinding(contextName, null, desc.Command, gesture);
			}
		}
	}
}
