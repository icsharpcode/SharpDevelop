using System;

namespace ICSharpCode.Core
{
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
