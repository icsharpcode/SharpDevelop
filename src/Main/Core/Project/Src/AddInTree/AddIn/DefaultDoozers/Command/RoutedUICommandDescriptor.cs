using System;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Describes <see cref="System.Windows.Input.RoutedUICommand" /> 
	/// </summary>
	public class RoutedUICommandDescriptor
	{
		private Codon codon;
		
		/// <summary>
		/// Gets text with routed command purpose description
		/// </summary>
		public string Text 
		{
			get {
				return codon.Properties["text"];
			}
		}
		
		/// <summary>
		/// Get routed command name
		/// </summary>
		public string Name 
		{
			get {
				return codon.Properties["name"];
			}
		}
		
		/// <summary>
		/// Creates instance of <see cref="RoutedUICommandDescriptor" />
		/// </summary>
		/// <param name="codon">Reference to codon used to create this descriptor</param>
		public RoutedUICommandDescriptor(Codon codon)
		{
			this.codon = codon;
		}
	}
}
