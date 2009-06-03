using System;

namespace ICSharpCode.Core
{
	/// <attribute name="name" use="required">
	/// Routed UI command name
	/// </attribute>
	/// <attribute name="text" use="required">
	/// Routed UI command displayed name
	/// </attribute>
	/// <usage>Only in /SharpDevelop/Workbench/CommandBindings</usage>
	/// <returns>
	/// RoutedUICommandDescriptor object
	/// </returns>
	/// <summary>
	/// Creates descriptor containing information about routed UI command
	/// </summary>
	public class RoutedUICommandDoozer : IDoozer
	{
		/// <see cref="IDoozer.HandleConditions" />
		public bool HandleConditions {
			get {
				return false;
			}
		}
		
		/// <see cref="IDoozer.BuildItem(object, Codon, System.Collections.ArrayList)">
		/// Builds RoutedUICommandDescriptor
		/// </see>
		public object BuildItem(object caller, Codon codon, System.Collections.ArrayList subItems)
		{
			return new RoutedUICommandDescriptor(codon);
		}
	}
}
