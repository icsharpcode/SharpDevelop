using System;

namespace ICSharpCode.Core
{
	/// <attribute name="command" use="required">
	/// Name of routed UI command which triggers this binding. 
	/// 
	/// Routed UI command details are specified in path '/SharpDevelop/Workbench/RoutedUICommands'
	/// </attribute>
	/// <attribute name="class" use="required">
	/// Class implementing System.Window.Input.ICommand or 
	/// ICSharpCode.Core class. CanExecute and Executed methods
	/// are used to handle raised event
	/// </attribute>
	/// <attribute name="context" use="optional">
	/// Specified binding owner
	/// 
	/// If context is not specified binding is applied to default context
	/// </attribute>
	/// <attribute name="lazy" use="optional" enum="1;0;true;false">
	/// Use lazy loading. If addin containing binded command is 
	/// not loaded yet, load asseblies referenced in add-in and then 
	/// invoke command
	/// </attribute>
	/// <attribute name="gestures" use="optional">
	/// Create input bindings in the same context which will trigger specified routed UI command
	/// </attribute>
	/// <usage>Only in /SharpDevelop/Workbench/CommandBindings</usage>
	/// <returns>
	/// CommandBindingDescriptor object
	/// </returns>
	/// <summary>
	/// Creates descriptor containing information about command binding
	/// </summary>
	public class CommandBindingDoozer : IDoozer
	{
		/// <see cref="IDoozer.HandleConditions" />
		public bool HandleConditions {
			get {
				return false;
			}
		}
		
		/// <see cref="IDoozer.BuildItem(object, Codon, System.Collections.ArrayList)">
		/// Builds CommandBindingDescriptor
		/// </see>
		public object BuildItem(object caller, Codon codon, System.Collections.ArrayList subItems)
		{
			return new CommandBindingDescriptor(codon);
		}
	}
}
