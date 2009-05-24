using System;
using System.Reflection;
using System.Windows.Input;
using ICSharpCode.Core;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Description of CommandsService.
	/// </summary>
	public static class CommandsService
	{
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
				
				CommandsRegistry.RegisterCommandBinding(contextName, desc.Command, desc.Class, desc.Codon.AddIn, desc.Lazy);
			}
		}
		
		public static void RegisterInputBindings(object caller, string path) 
		{
			var descriptors = AddInTree.BuildItems<InputBindingDescriptor>(path, caller, false);
			foreach(var desc in descriptors) {
				var contextName = !string.IsNullOrEmpty(desc.Context) ? desc.Context : CommandsRegistry.DefaultContext;
				
				var gesture = (KeyGesture)new KeyGestureConverter().ConvertFromInvariantString(desc.Gesture);
				CommandsRegistry.RegisterInputBinding(contextName, desc.Command, gesture);
			}
		}
	}
}
