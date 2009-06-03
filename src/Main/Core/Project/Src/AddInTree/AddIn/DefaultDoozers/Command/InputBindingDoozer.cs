using System;

namespace ICSharpCode.Core
{
	/// <attribute name="command" use="required">
	/// Name of routed UI command which is triggered by this binding
	/// </attribute>
	/// <attribute name="gesture" use="required">
	/// Gesture which triggers this binding
	/// </attribute>
	/// <usage>Only in /SharpDevelop/Workbench/InputBindings</usage>
	/// <returns>
	/// InputBindingDescriptor object
	/// </returns>
	/// <summary>
	/// Creates descriptor containing information about input binding
	/// </summary>
	public class InputBindingDoozer : IDoozer
	{
		/// <see cref="IDoozer.HandleConditions" />
		public bool HandleConditions {
			get {
				return true;
			}
		}
		
		/// <see cref="IDoozer.BuildItem(object, Codon, System.Collections.ArrayList)">
		/// Builds InputBindingDescriptor
		/// </see>
		public object BuildItem(object caller, Codon codon, System.Collections.ArrayList subItems)
		{
			return new InputBindingDescriptor(codon);
		}
	}
}
