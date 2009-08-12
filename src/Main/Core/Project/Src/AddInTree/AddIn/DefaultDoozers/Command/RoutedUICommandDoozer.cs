using System;

namespace ICSharpCode.Core
{
	/// <attribute name="name" use="required">
	/// System.Window.Input.RoutedUICommand unique name
	/// </attribute>
	/// <attribute name="text" use="required">
	/// Displayed text associated with created System.Window.Input.RoutedUICommand 
	/// </attribute>
	/// <usage>Only in /SharpDevelop/CommandManager/RoutedUICommands</usage>
	/// <returns>
	/// RoutedUICommandDescriptor object
	/// </returns>
	/// <summary>
	/// Creates new System.Window.Input.RoutedUICommand and associates it with a unique name which can be used later to reference routed command
	/// </summary>
	public class RoutedUICommandDoozer : IDoozer
	{
		/// <inheritdoc />
		public bool HandleConditions {
			get {
				return false;
			}
		}
		
		/// <summary>
		/// Builds instance of <see cref="RoutedUICommandDescriptor" /> from codon
		/// </summary>
		/// <param name="caller">Caller object</param>
		/// <param name="codon">Codon</param>
		/// <param name="subItems">Codon sub-items</param>
		/// <returns>Instance of <see cref="RoutedUICommandDescriptor" /></returns>
		public object BuildItem(object caller, Codon codon, System.Collections.ArrayList subItems)
		{
			return new RoutedUICommandDescriptor(codon);
		}
	}
}
