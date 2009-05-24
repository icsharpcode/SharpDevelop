using System;

namespace ICSharpCode.Core
{
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
