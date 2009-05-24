using System;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Stores information about input binding loaded from add-in tree
	/// </summary>
	public class InputBindingDescriptor
	{
		private Codon codon;
		
		/// <summary>
		/// Full name of routed UI command which will be invoked when this binding is triggered
		/// </summary>
		public string Command {
			get {
				return codon.Properties["command"];
			}
		}
		
		/// <summary>
		/// Full name of context class.
		/// 
		/// UI element in which this binding will be valid
		/// </summary>
		public string Context {
			get {
				return codon.Properties["context"];
			}
		}

		/// <summary>
		/// Description of gesture which will trigger this bindin
		/// </summary>
		public string Gesture {
			get {
				return codon.Properties["gesture"];
			}
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="codon">Reference to codon used to create this descriptor</param>
		public InputBindingDescriptor(Codon codon)
		{
			this.codon = codon;
		}
	}
}
