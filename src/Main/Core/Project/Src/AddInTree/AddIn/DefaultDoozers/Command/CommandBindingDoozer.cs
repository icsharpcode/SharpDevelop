using System;

namespace ICSharpCode.Core
{
	/// <attribute name="command" use="required">
	/// Name of routed UI command which triggers this binding
	/// </attribute>
	/// <attribute name="class" use="required">
	/// Class implementing System.Window.Input.ICommand or 
	/// ICSharpCode.Core class. CanExecute and Executed methods
	/// are used to handle raised event
	/// </attribute>
	/// <attribute name="lazy" use="optional" enum="1;0;true;false">
	/// Use lazy loading. If addin containing binded command is 
	/// not loaded yet load asseblies referenced in add-in
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
