using System;

namespace ICSharpCode.Core
{
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
