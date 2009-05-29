using System;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Stores information about input binding loaded from add-in tree
	/// </summary>
	public class InputBindingDescriptor
	{
		/// <summary>
		/// Full name of routed UI command which will be invoked when this binding is triggered
		/// </summary>
		public string Command {
			get; private set;
		}
		
		/// <summary>
		/// Full name of context class.
		/// 
		/// UI element in which this binding will be valid
		/// </summary>
		public string Context {
			get; private set;
		}

		/// <summary>
		/// Description of gesture which will trigger this bindin
		/// </summary>
		public string Gesture {
			get; private set;
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="codon">Reference to codon used to create this descriptor</param>
		public InputBindingDescriptor(Codon codon)
		{
			Command = codon.Properties["command"];
			Context = codon.Properties["context"];
			Gesture = codon.Properties["gesture"];
		}
	}
}
