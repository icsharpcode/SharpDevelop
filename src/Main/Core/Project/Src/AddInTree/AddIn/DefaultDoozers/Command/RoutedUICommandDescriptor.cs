using System;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Stores information about routed UI command loaded from add-in tree
	/// </summary>
	public class RoutedUICommandDescriptor
	{
		private Codon codon;
		
		/// <summary>
		/// Text presented to user
		/// </summary>
		public string Text {
			get {
				return codon.Properties["text"];
			}
		}
		
		/// <summary>
		/// Routed command name
		/// </summary>
		public string Name {
			get {
				return codon.Properties["name"];
			}
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="codon">Reference to codon used to create this descriptor</param>
		public RoutedUICommandDescriptor(Codon codon)
		{
			this.codon = codon;
		}
	}
}
